#region Copyright & License
/*
	Copyright (c) 2005-2010 nJupiter

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.
*/
#endregion

using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Globalization;

using nJupiter.DataAccess;
using nJupiter.DataAccess.Sql.Util;

namespace nJupiter.Services.Forum {

	public class ForumSqlDao : ForumDao {
		#region Constants
		private enum IdParameterType {
			None,
			Category,
			Post,
			ImmediatePostDescendant,
			RootPostDescendant
		}
		private enum AttributeType {
			PostAttribute,
			CategoryAttribute
		}
		private enum SavePostErrorCode {
			CategoryNotFound = 1,
			ParentNotFound,
			CannotMoveChildPostToCategory,
			CannotMovePostToOtherPost,
			PostNeedsParentOrCategory,
			PostDeleted,
			PostConcurrentlyUpdated
		}
		private enum SaveCategoryErrorCode {
			NameAlreadyExists = 1,
			CategoryNeedsDomain,
			CategoryDeleted,
			CategoryConcurrentlyUpdated
		}

		private const string SpNameFilterposts								= "dbo.FORUM_FilterPosts";
		private const string SpNameFiltercategories							= "dbo.FORUM_FilterCategories";
		private const string SpNameGetnumberofposts							= "dbo.FORUM_GetNumberOfPosts";
		private const string SpNameGetdomains								= "dbo.FORUM_GetDomains";
		private const string SpNameGetpostattributes						= "dbo.FORUM_GetPostAttributes";
		private const string SpNameGetcategoryattributes					= "dbo.FORUM_GetCategoryAttributes";
		private const string SpNameSavepost									= "dbo.FORUM_SavePost";
		private const string SpNameSavecategory								= "dbo.FORUM_SaveCategory";
		private const string SpNameSavepostattribute						= "dbo.FORUM_SavePostAttribute";
		private const string SpNameSavecategoryattribute					= "dbo.FORUM_SaveCategoryAttribute";
		private const string SpNameMoveposts								= "dbo.FORUM_MovePosts";
		private const string SpNameDeleteposts								= "dbo.FORUM_DeletePosts";
		private const string SpNameDeletecategories							= "dbo.FORUM_DeleteCategories";
		private const string SpNameDeletepostattribute						= "dbo.FORUM_DeletePostAttribute";
		private const string SpNameDeletecategoryattribute					= "dbo.FORUM_DeleteCategoryAttribute";

		private const string SpParamNameReturnvalue							= "@intReturnValue";
		private const string SpParamNameId									= "@guidID";
		private const string SpParamNamePostid								= "@guidPostID";
		private const string SpParamNameCategoryid							= "@guidCategoryID";
		private const string SpParamNameParentid							= "@guidParentID";
		private const string SpParamNameImmediatedescendantid				= "@guidImmediateDescendantID";
		private const string SpParamNameRootdescendantid					= "@guidRootDescendantID";
		private const string SpParamNameValue								= "@chvnValue";
		private const string SpParamNameExtendedvalue						= "@txtnExtendedValue";
		private const string SpParamNameName								= "@chvnName";
		private const string SpParamNameCategoryname						= "@chvnCategoryName";
		private const string SpParamNameAttributename						= "@chvAttributeName";
		private const string SpParamNameVisible								= "@bitVisible";
		private const string SpParamNameDomain								= "@chvnDomain";
		private const string SpParamNameGetonlychildren						= "@bitGetOnlyChildren";
		private const string SpParamNameDeleteonlychildren					= "@bitDeleteOnlyChildren";
		private const string SpParamNameUntil								= "@dteUntil";
		private const string SpParamNameIncludehidden						= "@bitIncludeHidden";
		private const string SpParamNamePostincludehidden					= "@bitPostIncludeHidden";
		private const string SpParamNameDatefilterfrom						= "@dteDateFilterFrom";
		private const string SpParamNamePostdatefilterfrom					= "@dtePostDateFilterFrom";
		private const string SpParamNameDatefilterto						= "@dteDateFilterTo";
		private const string SpParamNamePostdatefilterto					= "@dtePostDateFilterTo";
		private const string SpParamNameDatefiltercolumn					= "@intDateFilterColumn";
		private const string SpParamNamePostdatefiltercolumn				= "@bitPostDateFilterColumn";
		private const string SpParamNameLoadattributes						= "@bitGetAttributes";
		private const string SpParamNamePostloadattributes					= "@bitPostGetAttributes";
		private const string SpParamNameLevels								= "@intLevels";
		private const string SpParamNamePostlevels							= "@intPostLevels";
		private const string SpParamNameLimitsize							= "@intLimitSize";
		private const string SpParamNamePostlimitsize						= "@intPostLimitSize";
		private const string SpParamNameLimitpage							= "@intLimitPage";
		private const string SpParamNamePostlimitpage						= "@intPostLimitPage";
		private const string SpParamNameLimitsortcolumn						= "@intLimitSortColumn";
		private const string SpParamNamePostlimitsortcolumn					= "@intPostLimitSortColumn";
		private const string SpParamNameLimitsortattributename				= "@chvLimitSortAttributeName";
		private const string SpParamNamePostlimitsortattributename			= "@chvPostLimitSortAttributeName";
		private const string SpParamNameLimitsortattributedefaultvalue		= "@chvnLimitSortAttributeDefaultValue";
		private const string SpParamNamePostlimitsortattributedefaultvalue	= "@chvnPostLimitSortAttributeDefaultValue";
		private const string SpParamNameLimitsortdirectionasc				= "@bitLimitSortDirectionAsc";
		private const string SpParamNamePostlimitsortdirectionasc			= "@bitPostLimitSortDirectionAsc";
		private const string SpParamNameSearchtext							= "@chvnSearchText";
		private const string SpParamNameUseridentity						= "@chvUserIdentity";
		private const string SpParamNameAuthor								= "@chvnAuthor";
		private const string SpParamNameTitle								= "@chvnTitle";
		private const string SpParamNameBody								= "@txtnBody";
		private const string SpParamNamePostssearchquery					= "@txtnPostsSearchQuery";
		private const string SpParamNameFromcategoryid						= "@guidFromCategoryID";
		private const string SpParamNameTocategoryid						= "@guidToCategoryID";
		private const string SpParamNameFromdomain							= "@chvnFromDomain";
		private const string SpParamNameTodomain							= "@chvnToDomain";
		private const string SpParamNameFromcategoryname					= "@chvnFromCategoryName";
		private const string SpParamNameTocategoryname						= "@chvnToCategoryName";
		private const string SpParamNameTimestamp							= "@tsTimestamp";

		private const string ColNameId										= "ID";
		private const string ColNameCategoryid								= "CategoryID";
		private const string ColNameRootpostid								= "RootPostID";
		private const string ColNameRootposttitle							= "RootPostTitle";
		private const string ColNameEffectivecategoryid						= "EffectiveCategoryID";
		private const string ColNameEffectivecategoryname					= "EffectiveCategoryName";
		private const string ColNameParentid								= "ParentID";
		private const string ColNameDomain									= "Domain";
		private const string ColNameValue									= "Value";
		private const string ColNameExtendedvalue							= "ExtendedValue";
		private const string ColNameTypename								= "TypeName";
		private const string ColNameAssemblyname							= "AssemblyName";
		private const string ColNameAssemblypath							= "AssemblyPath";
		private const string ColNameName									= "Name";
		private const string ColNameUseridentity							= "UserIdentity";
		private const string ColNameAuthor									= "Author";
		private const string ColNameTitle									= "Title";
		private const string ColNameBody									= "Body";
		private const string ColNameTimeposted								= "TimePosted";
		private const string ColNameVisible									= "Visible";
		private const string ColNamePostcount								= "PostCount";
		private const string ColNameTimelastpost							= "TimeLastPost";
		private const string ColNameRank									= "Rank";
		private const string ColNameEffectivelyvisible						= "EffectivelyVisible";
		private const string ColNamePagingtotalnumberofposts				= "PagingTotalNumberOfPosts";
		private const string ColNameRootpostcount							= "RootPostCount";
		private const string ColNameTimestamp								= "Timestamp";

		private const string ParamNameId									= "id";
		private const string ParamNameDomain								= "domain";
		private const string ParamNameName									= "name";
		private const string ParamNameCategory								= "category";
		private const string ParamNameCategoryname							= "categoryName";
		private const string ParamNameSearchcriteria						= "searchCriteria";
		private const string ParamNamePost									= "post";
		private const string ParamNameCategoryid							= "categoryId";
		private const string ParamNameParentid								= "parentId";
		private const string ParamNameFromdomain							= "fromDomain";
		private const string ParamNameFromcategoryname						= "fromCategoryName";
		private const string ParamNameTodomain								= "toDomain";
		private const string ParamNameTocategoryname						= "toCategoryName";
		private const string ParamNameFromcategoryid						= "fromCategoryId";
		private const string ParamNameTocategoryid							= "toCategoryId";
		#endregion
		
		#region Variables
		private DataSource currentDataSource;
		#endregion

		#region Constructors
		public ForumSqlDao(string forumDaoName) : base(forumDaoName) {
			base.PostAttributes		= GetAttributes(AttributeType.PostAttribute);
			base.CategoryAttributes	= GetAttributes(AttributeType.CategoryAttribute);
		}
		#endregion

		#region Properties
		private DataSource CurrentDataSource {
			get {
				const string settingDataSourceKey = "dataSource";

				if(this.currentDataSource == null) {
					string settingDataSource	= GetSetting(settingDataSourceKey);
					this.currentDataSource		= settingDataSource == null ? DataSource.GetInstance() : DataSource.GetInstance(settingDataSource);
				}
				return this.currentDataSource;
			}
		}
		#endregion

		#region Domain Methods
		public override string[] GetDomains() {
			DataSet dsDomains	= this.CurrentDataSource.ExecuteDataSet(SpNameGetdomains);
			string[] domains	= new string[dsDomains.Tables[0].Rows.Count];
			int i				= 0;
			foreach(DataRow row in dsDomains.Tables[0].Rows) {
				domains[i++] = (string)row[ColNameDomain];
			}
			return domains;
		}
		#endregion

		#region Category Methods
		public override CategoryWithPagedPostsResult GetCategory(CategoryId id, CategoryResultConfiguration resultConfiguration, ThreadedPostsResultConfiguration postsResultConfiguration, PagingConfiguration pagingConfiguration) {
			if(id == null) {
				throw new ArgumentNullException(ParamNameId);
			}
			Hashtable totalCounts;
			CategoryCollection categories = GetCategories(null, id, null, GetCategoriesResultConfiguration(resultConfiguration), postsResultConfiguration, pagingConfiguration, out totalCounts);
			return categories.Count.Equals(0) ? null : ForumDao.CreateCategoryWithPagedPostsResultInstance(categories[0], totalCounts == null ? 0 : (int)totalCounts[id]);
		}
		public override CategoryWithPagedPostsResult GetCategory(string domain, string name, CategoryResultConfiguration resultConfiguration, ThreadedPostsResultConfiguration postsResultConfiguration, PagingConfiguration pagingConfiguration) {
			if(domain == null) {
				throw new ArgumentNullException(ParamNameDomain);
			}
			if(name == null) {
				throw new ArgumentNullException(ParamNameName);
			}
			Hashtable totalCounts;
			CategoryCollection categories = GetCategories(domain, null, name, GetCategoriesResultConfiguration(resultConfiguration), postsResultConfiguration, pagingConfiguration, out totalCounts);
			return categories.Count.Equals(0) ? null : ForumDao.CreateCategoryWithPagedPostsResultInstance(categories[0], totalCounts == null ? 0 : (int)totalCounts[categories[0].Id]);
		}
		public override CategoryWithPagedPostsResult GetCategory(PostId id, CategoryResultConfiguration resultConfiguration, ThreadedPostsResultConfiguration postsResultConfiguration, PagingConfiguration pagingConfiguration) {
			if(id == null) {
				throw new ArgumentNullException(ParamNameId);
			}
			Hashtable totalCounts;
			CategoryCollection categories = GetCategories(null, id, null, GetCategoriesResultConfiguration(resultConfiguration), postsResultConfiguration, pagingConfiguration, out totalCounts);
			return categories.Count.Equals(0) ? null : ForumDao.CreateCategoryWithPagedPostsResultInstance(categories[0], totalCounts == null ? 0 : (int)totalCounts[id]);
		}

		public override CategoryCollection GetCategories(CategoriesResultConfiguration resultConfiguration, ThreadedPostsResultConfiguration postsResultConfiguration, int postsLimitSize) {
			Hashtable totalCounts;
			return GetCategories(null, null, null, resultConfiguration, postsResultConfiguration, new PagingConfiguration(postsLimitSize), out totalCounts);
		}
		public override CategoryCollection GetCategories(string domain, CategoriesResultConfiguration resultConfiguration, ThreadedPostsResultConfiguration postsResultConfiguration, int postsLimitSize) {
			if(domain == null) {
				throw new ArgumentNullException(ParamNameDomain);
			}
			Hashtable totalCounts;
			return GetCategories(domain, null, null, resultConfiguration, postsResultConfiguration, new PagingConfiguration(postsLimitSize), out totalCounts);
		}

		public override void SaveCategory(Category category) {
			if(category == null) {
				throw new ArgumentNullException(ParamNameCategory);
			}
			using(Transaction transaction = Transaction.BeginTransaction(this.CurrentDataSource)) {
				SaveCategory(category, category.Domain, transaction);
				transaction.Commit();
			}
		}
		public override bool DeleteCategory(CategoryId id) {
			if(id == null) {
				throw new ArgumentNullException(ParamNameId);
			}
			using(Transaction transaction = Transaction.BeginTransaction(this.CurrentDataSource)) {
				bool categoryWasAffected = DeleteCategories(id, null, null, transaction).Equals(1);
				transaction.Commit();
				return categoryWasAffected;
			}
		}
		public override bool DeleteCategory(string domain, string name) {
			if(domain == null) {
				throw new ArgumentNullException(ParamNameDomain);
			}
			if(name == null) {
				throw new ArgumentNullException(ParamNameName);
			}
			using(Transaction transaction = Transaction.BeginTransaction(this.CurrentDataSource)) {
				bool categoryWasAffected = DeleteCategories(null, domain, name, transaction).Equals(1);
				transaction.Commit();
				return categoryWasAffected;
			}
		}

		public override int DeleteCategories() {
			using(Transaction transaction = Transaction.BeginTransaction(this.CurrentDataSource)) {
				int categoriesAffected = DeleteCategories(null, null, null, transaction);
				transaction.Commit();
				return categoriesAffected;
			}
		}
		public override int DeleteCategories(string domain) {
			if(domain == null) {
				throw new ArgumentNullException(ParamNameDomain);
			}
			using(Transaction transaction = Transaction.BeginTransaction(this.CurrentDataSource)) {
				int categoriesAffected = DeleteCategories(null, domain, null, transaction);
				transaction.Commit();
				return categoriesAffected;
			}
		}
		#endregion

		#region Post Methods
		public override PostWithPagedPostsResult GetPost(PostId id, PostType postType, ThreadedPostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration) {
			if(id == null) {
				throw new ArgumentNullException(ParamNameId);
			}
			PagedPostsResult pagedPostsResult = GetPosts(id, GetIdParameterType(postType), false, null, null, resultConfiguration, pagingConfiguration, null);
			return pagedPostsResult.Posts.Count.Equals(0) ? null : ForumDao.CreatePostWithPagedPostsResultsInstance(pagedPostsResult.Posts[0], pagedPostsResult.TotalCount);
		}

		public override PagedPostsResult GetPosts(ThreadedPostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration) {
			return GetPosts(null, IdParameterType.None, false, null, null, resultConfiguration, pagingConfiguration, null);
		}
		public override PagedPostsResult GetPosts(string domain, ThreadedPostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration) {
			if(domain == null) {
				throw new ArgumentNullException(ParamNameDomain);
			}
			return GetPosts(null, IdParameterType.None, false, domain, null, resultConfiguration, pagingConfiguration, null);
		}
		public override PagedPostsResult GetPosts(CategoryId id, ThreadedPostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration) {
			if(id == null) {
				throw new ArgumentNullException(ParamNameId);
			}
			return GetPosts(id, IdParameterType.Category, false, null, null, resultConfiguration, pagingConfiguration, null);
		}
		public override PagedPostsResult GetPosts(string domain, string categoryName, ThreadedPostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration) {
			if(domain == null) {
				throw new ArgumentNullException(ParamNameDomain);
			}
			if(categoryName == null) {
				throw new ArgumentNullException(ParamNameCategoryname);
			}
			return GetPosts(null, IdParameterType.None, false, domain, categoryName, resultConfiguration, pagingConfiguration, null);
		}
		public override PagedPostsResult GetPosts(PostId id, PostType postType, ThreadedPostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration) {
			if(id == null) {
				throw new ArgumentNullException(ParamNameId);
			}
			return GetPosts(id, GetIdParameterType(postType), true, null, null, resultConfiguration, pagingConfiguration, null);
		}

		public override int GetNumberOfPosts(NumberOfPostsResultConfiguration resultConfiguration) {
			return GetNumberOfPosts(null, null, null, resultConfiguration);
		}
		public override int GetNumberOfPosts(string domain, NumberOfPostsResultConfiguration resultConfiguration) {
			if(domain == null) {
				throw new ArgumentNullException("domain");
			}
			return GetNumberOfPosts(null, domain, null, resultConfiguration);
		}
		public override int GetNumberOfPosts(CategoryId categoryId, NumberOfPostsResultConfiguration resultConfiguration) {
			if(categoryId == null) {
				throw new ArgumentNullException("categoryId");
			}
			return GetNumberOfPosts(categoryId, null, null, resultConfiguration);
		}
		public override int GetNumberOfPosts(string domain, string categoryName, NumberOfPostsResultConfiguration resultConfiguration) {
			if(domain == null) {
				throw new ArgumentNullException("domain");
			}
			if(categoryName == null) {
				throw new ArgumentNullException("categoryName");
			}
			return GetNumberOfPosts(null, domain, categoryName, resultConfiguration);
		}
		public override int GetNumberOfPosts(PostId postId, NumberOfPostsResultConfiguration resultConfiguration) {
			if(postId == null) {
				throw new ArgumentNullException("postId");
			}
			return GetNumberOfPosts(postId, null, null, resultConfiguration);
		}

		public override PagedPostsResult SearchPosts(SearchCriteria searchCriteria, PostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration) {
			if(searchCriteria == null) {
				throw new ArgumentNullException(ParamNameSearchcriteria);
			}
			return GetPosts(null, IdParameterType.None, false, null, null, resultConfiguration == null ? null : GetThreadedPostsResultConfiguration(resultConfiguration), pagingConfiguration, searchCriteria);
		}
		public override PagedPostsResult SearchPosts(string domain, SearchCriteria searchCriteria, PostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration) {
			if(domain == null) {
				throw new ArgumentNullException(ParamNameDomain);
			}
			if(searchCriteria == null) {
				throw new ArgumentNullException(ParamNameSearchcriteria);
			}
			return GetPosts(null, IdParameterType.None, false, domain, null, resultConfiguration == null ? null : GetThreadedPostsResultConfiguration(resultConfiguration), pagingConfiguration, searchCriteria);
		}
		public override PagedPostsResult SearchPosts(CategoryId id, SearchCriteria searchCriteria, PostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration) {
			if(id == null) {
				throw new ArgumentNullException(ParamNameId);
			}
			if(searchCriteria == null) {
				throw new ArgumentNullException(ParamNameSearchcriteria);
			}
			return GetPosts(id, IdParameterType.Category, false, null, null, resultConfiguration == null ? null : GetThreadedPostsResultConfiguration(resultConfiguration), pagingConfiguration, searchCriteria);
		}
		public override PagedPostsResult SearchPosts(string domain, string categoryName, SearchCriteria searchCriteria, PostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration) {
			if(domain == null) {
				throw new ArgumentNullException(ParamNameDomain);
			}
			if(categoryName == null) {
				throw new ArgumentNullException(ParamNameCategoryname);
			}
			if(searchCriteria == null) {
				throw new ArgumentNullException(ParamNameSearchcriteria);
			}
			return GetPosts(null, IdParameterType.None, false, domain, categoryName, resultConfiguration == null ? null : GetThreadedPostsResultConfiguration(resultConfiguration), pagingConfiguration, searchCriteria);
		}
		public override PagedPostsResult SearchPosts(PostId id, PostType postType, SearchCriteria searchCriteria, PostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration) {
			if(id == null) {
				throw new ArgumentNullException(ParamNameId);
			}
			if(searchCriteria == null) {
				throw new ArgumentNullException(ParamNameSearchcriteria);
			}
			return GetPosts(id, GetIdParameterType(postType), false, null, null, resultConfiguration == null ? null : GetThreadedPostsResultConfiguration(resultConfiguration), pagingConfiguration, searchCriteria);
		}

		public override void SavePost(Post post) {
			if(post == null) {
				throw new ArgumentNullException(ParamNamePost);
			}
			using(Transaction transaction = Transaction.BeginTransaction(this.CurrentDataSource)) {
				SavePost(post, transaction);
				transaction.Commit();
			}
		}


		public override int MovePosts(string fromDomain, string fromCategoryName, string toDomain, string toCategoryName, DateTime until) {
			if(fromDomain == null) {
				throw new ArgumentNullException(ParamNameFromdomain);
			}
			if(fromCategoryName == null) {
				throw new ArgumentNullException(ParamNameFromcategoryname);
			}
			if(toDomain == null) {
				throw new ArgumentNullException(ParamNameTodomain);
			}
			if(toCategoryName == null) {
				throw new ArgumentNullException(ParamNameTocategoryname);
			}
			using(Transaction transaction = Transaction.BeginTransaction(this.CurrentDataSource)) {
				int postsAffected = MovePosts(null, fromDomain, fromCategoryName, null, toDomain, toCategoryName, until, transaction);
				transaction.Commit();
				return postsAffected;
			}
		}
		public override int MovePosts(string fromDomain, string fromCategoryName, CategoryId toCategoryId, DateTime until) {
			if(fromDomain == null) {
				throw new ArgumentNullException(ParamNameFromdomain);
			}
			if(fromCategoryName == null) {
				throw new ArgumentNullException(ParamNameFromcategoryname);
			}
			if(toCategoryId == null) {
				throw new ArgumentNullException(ParamNameTocategoryid);
			}
			using(Transaction transaction = Transaction.BeginTransaction(this.CurrentDataSource)) {
				int postsAffected = MovePosts(null, fromDomain, fromCategoryName, toCategoryId, null, null, until, transaction);
				transaction.Commit();
				return postsAffected;
			}
		}
		public override int MovePosts(CategoryId fromCategoryId, string toDomain, string toCategoryName, DateTime until) {
			if(fromCategoryId == null) {
				throw new ArgumentNullException(ParamNameFromcategoryid);
			}
			if(toDomain == null) {
				throw new ArgumentNullException(ParamNameTodomain);
			}
			if(toCategoryName == null) {
				throw new ArgumentNullException(ParamNameTocategoryname);
			}
			using(Transaction transaction = Transaction.BeginTransaction(this.CurrentDataSource)) {
				int postsAffected = MovePosts(fromCategoryId, null, null, null, toDomain, toCategoryName, until, transaction);
				transaction.Commit();
				return postsAffected;
			}
		}
		public override int MovePosts(CategoryId fromCategoryId, CategoryId toCategoryId, DateTime until) {
			if(fromCategoryId == null) {
				throw new ArgumentNullException(ParamNameFromcategoryid);
			}
			if(toCategoryId == null) {
				throw new ArgumentNullException(ParamNameTocategoryid);
			}
			using(Transaction transaction = Transaction.BeginTransaction(this.CurrentDataSource)) {
				int postsAffected = MovePosts(fromCategoryId, null, null, toCategoryId, null, null, until, transaction);
				transaction.Commit();
				return postsAffected;
			}
		}

		public override int DeletePost(PostId id) {
			if(id == null) {
				throw new ArgumentNullException(ParamNameId);
			}
			using(Transaction transaction = Transaction.BeginTransaction(this.CurrentDataSource)) {
				int postsAffected = DeletePosts(id, false, null, null, DateTime.MinValue, transaction);
				transaction.Commit();
				return postsAffected;
			}
		}

		public override int DeletePosts(DateTime until) {
			using(Transaction transaction = Transaction.BeginTransaction(this.CurrentDataSource)) {
				int postsAffected = DeletePosts(null, false, null, null, until, transaction);
				transaction.Commit();
				return postsAffected;
			}
		}
		public override int DeletePosts(string domain, DateTime until) {
			if(domain == null) {
				throw new ArgumentNullException(ParamNameDomain);
			}
			using(Transaction transaction = Transaction.BeginTransaction(this.CurrentDataSource)) {
				int postsAffected = DeletePosts(null, false, domain, null, until, transaction);
				transaction.Commit();
				return postsAffected;
			}
		}
		public override int DeletePosts(CategoryId id, DateTime until) {
			if(id == null) {
				throw new ArgumentNullException(ParamNameId);
			}
			using(Transaction transaction = Transaction.BeginTransaction(this.CurrentDataSource)) {
				int postsAffected = DeletePosts(id, false, null, null, until, transaction);
				transaction.Commit();
				return postsAffected;
			}
		}
		public override int DeletePosts(string domain, string categoryName, DateTime until) {
			if(domain == null) {
				throw new ArgumentNullException(ParamNameDomain);
			}
			if(categoryName == null) {
				throw new ArgumentNullException(ParamNameCategoryname);
			}
			using(Transaction transaction = Transaction.BeginTransaction(this.CurrentDataSource)) {
				int postsAffected = DeletePosts(null, false, domain, categoryName, until, transaction);
				transaction.Commit();
				return postsAffected;
			}
		}
		public override int DeletePosts(PostId id, DateTime until) {
			if(id == null) {
				throw new ArgumentNullException(ParamNameId);
			}
			using(Transaction transaction = Transaction.BeginTransaction(this.CurrentDataSource)) {
				int postsAffected = DeletePosts(id, true, null, null, until, transaction);
				transaction.Commit();
				return postsAffected;
			}
		}
		#endregion

		#region Helper Methods
		private PagedPostsResult GetPosts(GuidId id, IdParameterType idParameterType, bool getOnlyChildren, string domain, string categoryName, ThreadedPostsResultConfiguration resultConfiguration, PagingConfiguration pagingConfiguration, SearchCriteria searchCriteria) {
			if(resultConfiguration == null) {
				resultConfiguration = new ThreadedPostsResultConfiguration();
				//sort by search relevance if we have not provided a 
				//result configuration and we have provided a search criteria
				if(searchCriteria != null) {
					resultConfiguration.SortProperty = Post.Property.SearchRelevance;
				}
			}
			Attribute attribute = null;
			if(resultConfiguration.SortAttributeName != null) {
				attribute = CheckAttributeParameter(base.PostAttributes, resultConfiguration.SortAttributeName);
			}
			if(pagingConfiguration == null) {
				pagingConfiguration = new PagingConfiguration();
			}
			ArrayList filterParams	= new ArrayList();
			if(!idParameterType.Equals(IdParameterType.None)) {
				string filterParamName = null;
				switch(idParameterType) {
					case IdParameterType.Post:
						filterParamName		= SpParamNameId;
						break;
					case IdParameterType.ImmediatePostDescendant:
						filterParamName		= SpParamNameImmediatedescendantid;
						break;
					case IdParameterType.RootPostDescendant:
						filterParamName		= SpParamNameRootdescendantid;
						break;
					case IdParameterType.Category:
						filterParamName		= SpParamNameCategoryid;
						break;
				}
				filterParams.Add(this.CurrentDataSource.CreateInputParameter(filterParamName, DbType.Guid, id.Value));
				if(getOnlyChildren) {
					filterParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNameGetonlychildren, DbType.Boolean, getOnlyChildren));
				}
			} else if(domain != null) {
				filterParams.Add(this.CurrentDataSource.CreateStringInputParameter(SpParamNameDomain, DbType.String, domain));
				if(categoryName != null) {
					filterParams.Add(this.CurrentDataSource.CreateStringInputParameter(SpParamNameCategoryname, DbType.String, categoryName));
				}
			}
			filterParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNameIncludehidden, DbType.Boolean, resultConfiguration.IncludeHidden));
			//always load attributes when we sort by an attribute
			filterParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNameLoadattributes, DbType.Boolean, resultConfiguration.SortAttributeName != null || resultConfiguration.LoadAttributes));
			if(resultConfiguration.Levels > 0) {
				filterParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNameLevels, DbType.Int32, resultConfiguration.Levels));
			}
			if(pagingConfiguration.PageSize > 0) {
				filterParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNameLimitsize, DbType.Int32, pagingConfiguration.PageSize));
				if(attribute == null) {
					filterParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNameLimitsortcolumn, DbType.Int32, resultConfiguration.SortProperty));
				} else {
					if(!attribute.SerializationPreservesOrder) {
						throw new InvalidOperationException("Can not sort on an attribute that does not maintain sort order of its underlying type in its serialized form.");
					}
					filterParams.Add(this.CurrentDataSource.CreateStringInputParameter(SpParamNameLimitsortattributename, DbType.AnsiString, resultConfiguration.SortAttributeName));
					filterParams.Add(this.CurrentDataSource.CreateStringInputParameter(SpParamNameLimitsortattributedefaultvalue, DbType.AnsiString, attribute.ToSerializedString()));
				}
				filterParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNameLimitsortdirectionasc, DbType.Boolean, resultConfiguration.SortAscending));
				if(pagingConfiguration.PageNumber > 0) {
					filterParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNameLimitpage, DbType.Int32, pagingConfiguration.PageNumber));
				}
			}
			bool fromIsDefault	= resultConfiguration.DateFilterFrom.Equals(DateTime.MinValue);
			bool toIsDefault	= resultConfiguration.DateFilterTo.Equals(DateTime.MaxValue);
			if(!fromIsDefault) {
				filterParams.Add(this.CurrentDataSource.CreateDateInputParameter(SpParamNameDatefilterfrom, GetUtcDateTime(resultConfiguration.DateFilterFrom)));
			}
			if(!toIsDefault) {
				filterParams.Add(this.CurrentDataSource.CreateDateInputParameter(SpParamNameDatefilterto, GetUtcDateTime(resultConfiguration.DateFilterTo)));
			}
			if(!fromIsDefault || !toIsDefault) {
				filterParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNameDatefiltercolumn, DbType.Int32, resultConfiguration.DateFilterProperty));
			}
			PostCollection posts = ForumDao.CreatePostCollectionInstance(resultConfiguration.SortProperty, resultConfiguration.SortAttributeName, resultConfiguration.SortAscending);
			if(searchCriteria != null) {
				if(searchCriteria.FullTextSearchText == null && searchCriteria.UserIdentity == null && searchCriteria.AttributeCriteria.Count.Equals(0)) {
					return ForumDao.CreatePagedPostsResultInstance(posts, 0);
				}
				if(searchCriteria.FullTextSearchText != null) {
					filterParams.Add(this.CurrentDataSource.CreateStringInputParameter(SpParamNameSearchtext, DbType.String, FullTextHandler.GetContainsSearchCondition(searchCriteria.FullTextSearchText, GetFullTextHandlerFullTextSearchType(searchCriteria.FullTextSearchType))));
				}
				if(searchCriteria.UserIdentity != null) {
					filterParams.Add(this.CurrentDataSource.CreateStringInputParameter(SpParamNameUseridentity, DbType.AnsiString, searchCriteria.UserIdentity));
				}
				if(!searchCriteria.AttributeCriteria.Count.Equals(0)) {
					filterParams.Add(this.CurrentDataSource.CreateStringInputParameter(SpParamNamePostssearchquery, DbType.String, ConstructSqlFromAttributeCriteria(searchCriteria.AttributeCriteria)));
				}
			}
			IDataParameter returnParam	= this.CurrentDataSource.CreateReturnParameter(SpParamNameReturnvalue, DbType.Int32);
			filterParams.Add(returnParam);
			DataSet dsPosts;
			try {
				dsPosts = this.CurrentDataSource.ExecuteDataSet(SpNameFilterposts, filterParams.ToArray());
			} catch(SqlException e) {
				switch(e.Number) {
					case 7603: //the execution of a full-text operation failed, full-text predicate was null or empty (SQL Server 2000)
					case 7619: //the execution of a full-text operation failed, the clause of the query contained only ignored words (SQL Server 2000)
					case 7643: //the execution of a full-text operation failed, the search generated too many results
					case 7645: //the execution of a full-text operation failed, full-text predicate was null or empty (SQL Server 2005)
						return ForumDao.CreatePagedPostsResultInstance(posts, 0);
					default:
						throw;
				}
			}
			AddPostsToCollectionFromDataTables(posts, dsPosts.Tables[0], resultConfiguration.SortAttributeName != null || resultConfiguration.LoadAttributes ? dsPosts.Tables[1] : null, resultConfiguration.Levels, resultConfiguration.SortProperty, resultConfiguration.SortAttributeName, resultConfiguration.SortAscending, searchCriteria != null || resultConfiguration.Levels.Equals(1));
			return ForumDao.CreatePagedPostsResultInstance(posts, (int)returnParam.Value);
		}
		private int GetNumberOfPosts(GuidId id, string domain, string categoryName, NumberOfPostsResultConfiguration resultConfiguration) {
			if(resultConfiguration == null) {
				resultConfiguration = new NumberOfPostsResultConfiguration();
			}
			ArrayList filterParams = new ArrayList();
			if(id != null) {
				filterParams.Add(this.CurrentDataSource.CreateInputParameter(id is PostId ? SpParamNamePostid : SpParamNameCategoryid, DbType.Guid, id.Value));
			} else if(domain != null) {
				filterParams.Add(this.CurrentDataSource.CreateStringInputParameter(SpParamNameDomain, DbType.String, domain));
				if(categoryName != null) {
					filterParams.Add(this.CurrentDataSource.CreateStringInputParameter(SpParamNameCategoryname, DbType.String, categoryName));
				}
			}
			filterParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNameIncludehidden, DbType.Boolean, resultConfiguration.IncludeHidden));
			bool fromIsDefault	= resultConfiguration.DateFilterFrom.Equals(DateTime.MinValue);
			bool toIsDefault	= resultConfiguration.DateFilterTo.Equals(DateTime.MaxValue);
			if(!fromIsDefault) {
				filterParams.Add(this.CurrentDataSource.CreateDateInputParameter(SpParamNameDatefilterfrom, GetUtcDateTime(resultConfiguration.DateFilterFrom)));
			}
			if(!toIsDefault) {
				filterParams.Add(this.CurrentDataSource.CreateDateInputParameter(SpParamNameDatefilterto, GetUtcDateTime(resultConfiguration.DateFilterTo)));
			}
			IDataParameter returnParam	= this.CurrentDataSource.CreateReturnParameter(SpParamNameReturnvalue, DbType.Int32);
			filterParams.Add(returnParam);
			this.CurrentDataSource.ExecuteNonQuery(SpNameGetnumberofposts, filterParams.ToArray());
			return (int)returnParam.Value;
		}
		private CategoryCollection GetCategories(string domain, GuidId id, string name, CategoriesResultConfiguration resultConfiguration, ThreadedPostsResultConfiguration postsResultConfiguration, PagingConfiguration pagingConfiguration, out Hashtable totalCounts) {
			if(resultConfiguration == null) {
				resultConfiguration			= new CategoriesResultConfiguration();
			}
			if(resultConfiguration.SortAttributeName != null) {
				CheckAttributeParameter(base.CategoryAttributes, resultConfiguration.SortAttributeName);
			}
			if(postsResultConfiguration == null) {
				postsResultConfiguration	= new ThreadedPostsResultConfiguration();
			}
			Attribute postAttribute = null;
			if(postsResultConfiguration.SortAttributeName != null) {
				postAttribute = CheckAttributeParameter(base.PostAttributes, postsResultConfiguration.SortAttributeName);
			}
			if(pagingConfiguration == null) {
				pagingConfiguration			= new PagingConfiguration();
			}
			ArrayList filterParams		= new ArrayList();
			if(id != null) {
				filterParams.Add(this.CurrentDataSource.CreateInputParameter(id is CategoryId ? SpParamNameId : SpParamNamePostid, DbType.Guid, id.Value));
			} else {
				if(domain != null) {
					filterParams.Add(this.CurrentDataSource.CreateStringInputParameter(SpParamNameDomain, DbType.String, domain));
					if(name != null) {
						filterParams.Add(this.CurrentDataSource.CreateStringInputParameter(SpParamNameName, DbType.String, name));
					}
				}
			}
			filterParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNameIncludehidden, DbType.Boolean, resultConfiguration.IncludeHidden));
			filterParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNamePostincludehidden, DbType.Boolean, postsResultConfiguration.IncludeHidden));
			//always load attributes when we sort by an attribute
			filterParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNameLoadattributes, DbType.Boolean, resultConfiguration.SortAttributeName != null || resultConfiguration.LoadAttributes));
			//always load post attributes when we sort by a post attribute
			filterParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNamePostloadattributes, DbType.Boolean, postsResultConfiguration.SortAttributeName != null || postsResultConfiguration.LoadAttributes));
			if(postsResultConfiguration.Levels >= 0) {
				filterParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNamePostlevels, DbType.Int32, postsResultConfiguration.Levels));
				if(pagingConfiguration.PageSize > 0) {
					filterParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNamePostlimitsize, DbType.Int32, pagingConfiguration.PageSize));
					if(postAttribute == null) {
						filterParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNamePostlimitsortcolumn, DbType.Int32, postsResultConfiguration.SortProperty));
					} else {
						filterParams.Add(this.CurrentDataSource.CreateStringInputParameter(SpParamNamePostlimitsortattributename, DbType.AnsiString, postsResultConfiguration.SortAttributeName));
						filterParams.Add(this.CurrentDataSource.CreateStringInputParameter(SpParamNamePostlimitsortattributedefaultvalue, DbType.AnsiString, postAttribute.ToSerializedString()));
					}
					filterParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNamePostlimitsortdirectionasc, DbType.Boolean, postsResultConfiguration.SortAscending));
					if(pagingConfiguration.PageNumber > 0) {
						filterParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNamePostlimitpage, DbType.Int32, pagingConfiguration.PageNumber));
					}
				}				
			}
			bool fromIsDefault	= postsResultConfiguration.DateFilterFrom.Equals(DateTime.MinValue);
			bool toIsDefault	= postsResultConfiguration.DateFilterTo.Equals(DateTime.MaxValue);
			if(!fromIsDefault) {
				filterParams.Add(this.CurrentDataSource.CreateDateInputParameter(SpParamNamePostdatefilterfrom, GetUtcDateTime(postsResultConfiguration.DateFilterFrom)));
			}
			if(!toIsDefault) {
				filterParams.Add(this.CurrentDataSource.CreateDateInputParameter(SpParamNamePostdatefilterto, GetUtcDateTime(postsResultConfiguration.DateFilterTo)));
			}
			if(!fromIsDefault || !toIsDefault) {
				filterParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNamePostdatefiltercolumn, DbType.Boolean, postsResultConfiguration.DateFilterProperty));
			}
			DataSet dsCategories			= this.CurrentDataSource.ExecuteDataSet(SpNameFiltercategories, filterParams.ToArray());
			CategoryCollection categories	= ForumDao.CreateCategoryCollectionInstance(resultConfiguration.SortProperty, resultConfiguration.SortAttributeName, resultConfiguration.SortAscending);
			foreach(DataRow row in dsCategories.Tables[0].Rows) {
				Category category	= ForumDao.CreateCategoryInstance(ForumDao.CreateCategoryIdInstance((Guid)row[ColNameId]), (string)row[ColNameDomain], (string)row[ColNameName], resultConfiguration.LoadAttributes ? ForumDao.CloneAttributeCollection(base.CategoryAttributes) : null, postsResultConfiguration.Levels.Equals(0) ? null : ForumDao.CreatePostCollectionInstance(postsResultConfiguration.SortProperty, postsResultConfiguration.SortAttributeName, postsResultConfiguration.SortAscending), (int)row[ColNameRootpostcount], ToLongString((byte[])row[ColNameTimestamp]));
				category.Visible	= (bool)row[ColNameVisible];
				ForumDao.AddCategoryToCategoryCollection(categories, category);
			}
			if(resultConfiguration.LoadAttributes) {
				foreach(DataRow row in dsCategories.Tables[1].Rows) {
					Attribute attribute	= categories[ForumDao.CreateCategoryIdInstance((Guid)row[ColNameId])].Attributes[(string)row[ColNameName]];
					attribute.Value		= attribute.DeserializeAttributeValue((string)(row.IsNull(ColNameValue) ? row[ColNameExtendedvalue] : row[ColNameValue]));
				}
			}
			if(!postsResultConfiguration.Levels.Equals(0)) {
				int tableOffset = resultConfiguration.LoadAttributes ? 0 : -1;
				totalCounts = new Hashtable(dsCategories.Tables[0].Rows.Count);
				foreach(DataRow row in dsCategories.Tables[tableOffset + 3].Rows) {
					totalCounts.Add(ForumDao.CreateCategoryIdInstance((Guid)row[ColNameId]), (int)row[ColNamePagingtotalnumberofposts]);
				}
				AddPostsToCollectionFromDataTables(categories, dsCategories.Tables[tableOffset + 2], postsResultConfiguration.SortAttributeName != null || postsResultConfiguration.LoadAttributes ? dsCategories.Tables[tableOffset + 4] : null, postsResultConfiguration.Levels, postsResultConfiguration.SortProperty, postsResultConfiguration.SortAttributeName, postsResultConfiguration.SortAscending, postsResultConfiguration.Levels.Equals(1));
			} else {
				totalCounts = null;
			}
			return categories;
		}
		private AttributeCollection GetAttributes(AttributeType attributeType) {
			string spName;
			switch(attributeType) {
				case AttributeType.PostAttribute:
					spName	= SpNameGetpostattributes;
					break;
				default:
					spName	= SpNameGetcategoryattributes;
					break;
			}
			DataSet dsAttributes			= this.CurrentDataSource.ExecuteDataSet(spName);
			AttributeCollection attributes	= ForumDao.CreateAttributeCollectionInstance();
			foreach(DataRow row in dsAttributes.Tables[0].Rows) {
				Type type;
				string typeName = (string)row[ColNameTypename];
				if(!row.IsNull(ColNameAssemblypath)) {
					type	= Assembly.LoadFrom((string)row[ColNameAssemblypath]).GetType(typeName, true);
				} else if(!row.IsNull(ColNameAssemblyname)) {
					type	= Assembly.Load((string)row[ColNameAssemblyname]).GetType(typeName, true);
				} else {
					type	= Type.GetType(typeName, true);
				}
				ForumDao.AddAttributeToAttributeCollection(attributes, ForumDao.CreateAttributeInstance((string)row[ColNameName], type));
			}
			return attributes;
		}

		private void SavePost(Post post, Transaction transaction) {
			IDataParameter returnParam;
			ArrayList saveParams = new ArrayList(
				new object[] {
					returnParam = this.CurrentDataSource.CreateReturnParameter(SpParamNameReturnvalue, DbType.Int32),
					this.CurrentDataSource.CreateInputParameter(SpParamNameId, DbType.Guid, post.Id.Value),
					this.CurrentDataSource.CreateStringInputParameter(SpParamNameUseridentity, DbType.String, post.UserIdentity, true),
					this.CurrentDataSource.CreateStringInputParameter(SpParamNameAuthor, DbType.String, post.Author, true),
					this.CurrentDataSource.CreateStringInputParameter(SpParamNameTitle, DbType.String, post.Title),
					this.CurrentDataSource.CreateStringInputParameter(SpParamNameBody, DbType.String, post.Body),
					this.CurrentDataSource.CreateInputParameter(SpParamNameVisible, DbType.Boolean, post.Visible)
				});

			if(post.ParentId != null) {
				saveParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNameParentid, DbType.Guid, post.ParentId.Value));
			}else {
				saveParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNameCategoryid, DbType.Guid, post.CategoryId.Value));
			}

			if(post.ConcurrencyIdentity != null) {
				saveParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNameTimestamp, DbType.Binary, ToByteArray(post.ConcurrencyIdentity)));
			}
			this.CurrentDataSource.ExecuteNonQuery(SpNameSavepost, transaction, saveParams.ToArray());
			switch((SavePostErrorCode)returnParam.Value) {
				case SavePostErrorCode.CategoryNotFound:
					throw new CategoryNotFoundException("The specified category cannot be found.");
				case SavePostErrorCode.ParentNotFound:
					throw new PostNotFoundException("The specified parent post cannot be found.");
				case SavePostErrorCode.PostNeedsParentOrCategory:
					throw new OrphanInsertException("The post needs a parent or a category.");
				case SavePostErrorCode.CannotMoveChildPostToCategory:
					throw new InvalidPostMoveException("A child post cannot be moved to a category.");
				case SavePostErrorCode.CannotMovePostToOtherPost:
					throw new InvalidPostMoveException("A post cannot be moved to another post.");
				case SavePostErrorCode.PostDeleted:
					throw new PostNotFoundException("The post has been deleted.");
				case SavePostErrorCode.PostConcurrentlyUpdated:
					throw new ConcurrentUpdateException("The post has been concurrently updated.");
			}
			if(post.Attributes != null) {
				SaveAttributes(post.Id, post.Attributes, AttributeType.PostAttribute, transaction);
			}
		}
		private void SaveCategory(Category category, string domain, Transaction transaction) {
			IDataParameter returnParam;
			ArrayList saveParams = new ArrayList(
				new object[] {
					returnParam = this.CurrentDataSource.CreateReturnParameter(SpParamNameReturnvalue, DbType.Int32),
					this.CurrentDataSource.CreateInputParameter(SpParamNameId, DbType.Guid, category.Id.Value),
					this.CurrentDataSource.CreateStringInputParameter(SpParamNameName, DbType.String, category.Name),
					this.CurrentDataSource.CreateInputParameter(SpParamNameVisible, DbType.Boolean, category.Visible)
				});
			if(domain != null) {
				saveParams.Add(this.CurrentDataSource.CreateStringInputParameter(SpParamNameDomain, DbType.String, domain));
			}
			if(category.ConcurrencyIdentity != null) {
				saveParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNameTimestamp, DbType.Binary, ToByteArray(category.ConcurrencyIdentity)));
			}
			this.CurrentDataSource.ExecuteNonQuery(SpNameSavecategory, transaction, saveParams.ToArray());
			switch((SaveCategoryErrorCode)returnParam.Value) {
				case SaveCategoryErrorCode.CategoryNeedsDomain:
					throw new OrphanInsertException("The category needs a domain.");
				case SaveCategoryErrorCode.NameAlreadyExists:
					throw new CategoryNameAlreadyExistsException("The specified category name already exists.");
				case SaveCategoryErrorCode.CategoryDeleted:
					throw new PostNotFoundException("The category has been deleted.");
				case SaveCategoryErrorCode.CategoryConcurrentlyUpdated:
					throw new ConcurrentUpdateException("The post has been concurrently updated.");
			}
			if(category.Attributes != null) {
				SaveAttributes(category.Id, category.Attributes, AttributeType.CategoryAttribute, transaction);
			}
		}
		private void SaveAttributes(GuidId id, AttributeCollection attributes, AttributeType attributeType, Transaction transaction) {
			string spNameSave;
			string spNameDelete;
			string paramNameId;
			switch(attributeType) {
				case AttributeType.PostAttribute:
					spNameSave		= SpNameSavepostattribute;
					spNameDelete	= SpNameDeletepostattribute;
					paramNameId		= SpParamNamePostid;
					break;
				default:
					spNameSave		= SpNameSavecategoryattribute;
					spNameDelete	= SpNameDeletecategoryattribute;
					paramNameId		= SpParamNameCategoryid;
					break;
			}
			foreach(Attribute attribute in attributes) {
				if(attribute.IsEmpty) {
					this.CurrentDataSource.ExecuteNonQuery(spNameDelete, transaction,
						this.CurrentDataSource.CreateStringInputParameter(SpParamNameAttributename, DbType.AnsiString, attribute.Name),
						this.CurrentDataSource.CreateInputParameter(paramNameId, DbType.Guid, id.Value));
				} else {
					string attributeValue = attribute.ToSerializedString();
					this.CurrentDataSource.ExecuteNonQuery(spNameSave, transaction,
						this.CurrentDataSource.CreateStringInputParameter(SpParamNameAttributename, DbType.AnsiString, attribute.Name),
						this.CurrentDataSource.CreateInputParameter(paramNameId, DbType.Guid, id.Value),
						this.CurrentDataSource.CreateStringInputParameter(SpParamNameValue, DbType.String, attributeValue.Length <= 3500 ? attributeValue : null),
						this.CurrentDataSource.CreateStringInputParameter(SpParamNameExtendedvalue, DbType.String, attributeValue.Length > 3500 ? attributeValue : null));
				}
			}
		}
		
		private int MovePosts(CategoryId fromCategoryId, string fromDomain, string fromCategoryName, CategoryId toCategoryId, string toDomain, string toCategoryName, DateTime until, Transaction transaction) {
			ArrayList moveParams = new ArrayList();
			if(fromCategoryId != null) {
				moveParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNameFromcategoryid, DbType.Guid, fromCategoryId.Value));
			} else {
				moveParams.Add(this.CurrentDataSource.CreateStringInputParameter(SpParamNameFromdomain, DbType.String, fromDomain));
				moveParams.Add(this.CurrentDataSource.CreateStringInputParameter(SpParamNameFromcategoryname, DbType.String, fromCategoryName));
			}
			if(toCategoryId != null) {
				moveParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNameTocategoryid, DbType.Guid, toCategoryId.Value));
			} else {
				moveParams.Add(this.CurrentDataSource.CreateStringInputParameter(SpParamNameTodomain, DbType.String, toDomain));
				moveParams.Add(this.CurrentDataSource.CreateStringInputParameter(SpParamNameTocategoryname, DbType.String, toCategoryName));
			}
			if(until > DateTime.MinValue) {
				moveParams.Add(this.CurrentDataSource.CreateDateInputParameter(SpParamNameUntil, GetUtcDateTime(until)));
			}
			IDataParameter returnParam	= this.CurrentDataSource.CreateReturnParameter(SpParamNameReturnvalue, DbType.Int32);
			moveParams.Add(returnParam);
			this.CurrentDataSource.ExecuteNonQuery(SpNameMoveposts, transaction, moveParams.ToArray());
			return (int)returnParam.Value;
		}
		
		private int DeletePosts(GuidId id, bool deleteOnlyChildren, string domain, string categoryName, DateTime until, Transaction transaction) {
			ArrayList deleteParams = new ArrayList();
			if(id != null) {
				deleteParams.Add(this.CurrentDataSource.CreateInputParameter(id is PostId ? SpParamNameId : SpParamNameCategoryid, DbType.Guid, id.Value));
				if(deleteOnlyChildren) {
					deleteParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNameDeleteonlychildren, DbType.Boolean, deleteOnlyChildren));
				}
			} else if(domain != null) {
				deleteParams.Add(this.CurrentDataSource.CreateStringInputParameter(SpParamNameDomain, DbType.String, domain));
				if(categoryName != null) {
					deleteParams.Add(this.CurrentDataSource.CreateStringInputParameter(SpParamNameCategoryname, DbType.String, categoryName));
				}
			}
			if(until > DateTime.MinValue) {
				deleteParams.Add(this.CurrentDataSource.CreateDateInputParameter(SpParamNameUntil, GetUtcDateTime(until)));
			}
			IDataParameter returnParam	= this.CurrentDataSource.CreateReturnParameter(SpParamNameReturnvalue, DbType.Int32);
			deleteParams.Add(returnParam);
			this.CurrentDataSource.ExecuteNonQuery(SpNameDeleteposts, transaction, deleteParams.ToArray());
			return (int)returnParam.Value;
		}
		private int DeleteCategories(CategoryId id, string domain, string name, Transaction transaction) {
			ArrayList deleteParams = new ArrayList();
			if(id != null) {
				deleteParams.Add(this.CurrentDataSource.CreateInputParameter(SpParamNameId, DbType.Guid, id.Value));
			} else if(domain != null) {
				deleteParams.Add(this.CurrentDataSource.CreateStringInputParameter(SpParamNameDomain, DbType.String, domain));
				if(name != null) {
					deleteParams.Add(this.CurrentDataSource.CreateStringInputParameter(SpParamNameName, DbType.String, name));
				}
			}
			IDataParameter returnParam	= this.CurrentDataSource.CreateReturnParameter(SpParamNameReturnvalue, DbType.Int32);
			deleteParams.Add(returnParam);
			this.CurrentDataSource.ExecuteNonQuery(SpNameDeletecategories, transaction, deleteParams.ToArray());
			return (int)returnParam.Value;
		}

		private void AddPostsToCollectionFromDataTables(ICollection collection, DataTable dtPosts, DataTable dtAttributes, int levels, Post.Property sortProperty, string sortAttributeName, bool sortAscending, bool addFlat) {
			Hashtable postLookupTable		= addFlat && dtAttributes == null ? null : new Hashtable();
			Hashtable postLevels			= null;
			PostCollection posts			= collection as PostCollection;
			CategoryCollection categories	= collection as CategoryCollection;
			if(!addFlat) {
				dtPosts.DefaultView.Sort	= ColNameTimeposted;
				postLevels					= new Hashtable();
			}
			bool rankColumnExists			= dtPosts.Columns.Contains(ColNameRank);
			foreach(DataRowView rowView in dtPosts.DefaultView) {
				Guid id				= (Guid)rowView[ColNameId];
				bool parentIdEmpty	= rowView.Row.IsNull(ColNameParentid);
				Guid parentId		= parentIdEmpty ? Guid.Empty : (Guid)rowView[ColNameParentid];
				Post post			= ForumDao.CreatePostInstance(
					ForumDao.CreatePostIdInstance(id),
					parentIdEmpty ? null : ForumDao.CreatePostIdInstance(parentId),
					rowView.Row.IsNull(ColNameCategoryid) ? null : ForumDao.CreateCategoryIdInstance((Guid)rowView[ColNameCategoryid]),
					ForumDao.CreatePostIdInstance((Guid)rowView[ColNameRootpostid]),
					(string)rowView[ColNameRootposttitle],
					ForumDao.CreateCategoryIdInstance((Guid)rowView[ColNameEffectivecategoryid]),
					(string)rowView[ColNameEffectivecategoryname], 
					dtAttributes == null ? null : ForumDao.CloneAttributeCollection(base.PostAttributes), 
					addFlat || (!parentIdEmpty && (levels - 1).Equals(postLevels[parentId])) ? null : ForumDao.CreatePostCollectionInstance(sortProperty, sortAttributeName, sortAscending),
					(DateTime)rowView[ColNameTimeposted],
					(DateTime)rowView[ColNameTimelastpost],
					(int)rowView[ColNamePostcount],
					!rankColumnExists || rowView.Row.IsNull(ColNameRank) ? 0F : (short)rowView[ColNameRank] / 1000F,
					(bool)rowView[ColNameEffectivelyvisible],
					ToLongString((byte[])rowView[ColNameTimestamp]));
				post.UserIdentity	= rowView.Row.IsNull(ColNameUseridentity) ? null : (string)rowView[ColNameUseridentity];
				post.Author			= rowView.Row.IsNull(ColNameAuthor) ? null : (string)rowView[ColNameAuthor];
				post.Title			= (string)rowView[ColNameTitle];
				post.Body			= (string)rowView[ColNameBody];
				post.Visible		= (bool)rowView[ColNameVisible];
				if(postLookupTable != null && (dtAttributes != null || post.Posts != null)) {
					postLookupTable.Add(id, post);
				}
				if(postLevels != null && post.Posts != null) {
					postLevels.Add(id, parentIdEmpty || !postLevels.ContainsKey(parentId) ? 1 : (int)postLevels[parentId] + 1);
				}

				if(posts != null || categories != null){
					ForumDao.AddPostToPostCollection(addFlat || parentIdEmpty || !postLookupTable.ContainsKey(parentId) ? 
						posts ?? categories[post.CategoryId].Posts : 
						((Post)postLookupTable[parentId]).Posts, post);
				}
			}
			if(dtAttributes != null) {
				foreach(DataRow row in dtAttributes.Rows) {
					Attribute attribute	= ((Post)postLookupTable[row[ColNameId]]).Attributes[(string)row[ColNameName]];
					attribute.Value		= attribute.DeserializeAttributeValue((string)(row.IsNull(ColNameValue) ? row[ColNameExtendedvalue] : row[ColNameValue]));
				}
			}
		}
		private static IdParameterType GetIdParameterType(PostType postType) {
			switch(postType) {
				case PostType.Parent:
					return IdParameterType.ImmediatePostDescendant;
				case PostType.Root:
					return IdParameterType.RootPostDescendant;
				default:
					return IdParameterType.Post;
			}
		}
		private static FullTextHandler.FullTextSearchType GetFullTextHandlerFullTextSearchType(FullTextSearchType fullTextSearchType) {
			switch(fullTextSearchType) {
				case FullTextSearchType.Inflectional:
					return FullTextHandler.FullTextSearchType.Inflectional;
				case FullTextSearchType.Thesaurus:
					return FullTextHandler.FullTextSearchType.Thesaurus;
				case FullTextSearchType.Prefix:
					return FullTextHandler.FullTextSearchType.Prefix;
				default:
					return FullTextHandler.FullTextSearchType.Normal;
			}
		}
		private string ConstructSqlFromAttributeCriteria(AttributeCriterionCollection attributeCriteria) {
			const string quote						= "'";
			const string quoteEscaped				= "''";
			const string parenStart					= "(";
			const string parenEnd					= ")";
			const string space						= " ";
			const string not						= "NOT";
			const string and						= "AND";
			const string or							= "OR";

			const string query						= "SELECT p.ID FROM dbo.FORUM_Post p WHERE ";
			const string subqueryStart				= "EXISTS(SELECT*FROM dbo.FORUM_PostAttribute pa JOIN dbo.FORUM_PostAttributeType pat ON pa.PostAttributeTypeID=pat.ID WHERE pa.PostID=p.ID AND pat.Name='{0}'";
			const string subqueryEnd				= parenEnd;

			const string conditionEqual				= " AND pa.Value=N'{0}'";
			const string conditionEqualExtended		= " AND pa.ExtendedValue LIKE N'{0}'";
			const string conditionNotequal			= "	AND pa.Value<>N'{0}'";
			const string conditionNotequalExtended	= " AND pa.ExtendedValue NOT LIKE N'{0}'";
			const string conditionGreater			= " AND(pa.Value>N'{0}'OR CAST(pa.ExtendedValue AS NVARCHAR(4000))>N'{0}')";
			const string conditionGreaterequal		= " AND(pa.Value>=N'{0}'OR CAST(pa.ExtendedValue AS NVARCHAR(4000))>=N'{0}')";
			const string conditionLess				= " AND(pa.Value<N'{0}'OR CAST(pa.ExtendedValue AS NVARCHAR(4000))<N'{0}')";
			const string conditionLessequal			= " AND(pa.Value<=N'{0}'OR CAST(pa.ExtendedValue AS NVARCHAR(4000))<=N'{0}')";

			StringBuilder queryBuilder		= new StringBuilder(query);
			ArrayList requiredAttributes	= new ArrayList();
			ArrayList nonRequiredAttributes	= new ArrayList();

			foreach(AttributeCriterion attributeCriterion in attributeCriteria) {
				if(!base.PostAttributes.Contains(attributeCriterion.Attribute)) {
					throw new InvalidOperationException("Attribute does not exist.");
				}
				StringBuilder criterionBuilder = new StringBuilder();
				criterionBuilder.AppendFormat(CultureInfo.InvariantCulture, subqueryStart, attributeCriterion.Attribute.Name.Replace(quote, quoteEscaped));
				switch(attributeCriterion.Comparison) {
					case Comparison.Equal:
					case Comparison.NotEqual:
						//not default value
						if(!attributeCriterion.Attribute.IsEmpty) { 
							string serializedValue			= attributeCriterion.Attribute.ToSerializedString();
							bool searchExtendedValue		= serializedValue.Length > 3500;
							string escapedSerializedValue	= serializedValue.Replace(quote, quoteEscaped);
							if(searchExtendedValue) {
								escapedSerializedValue		= EscapeForLikeClause(escapedSerializedValue);
							}
							if(attributeCriterion.Comparison.Equals(Comparison.Equal)) {
								criterionBuilder.AppendFormat(CultureInfo.InvariantCulture, searchExtendedValue ? conditionEqualExtended : conditionEqual, escapedSerializedValue);
							} else {
								criterionBuilder.AppendFormat(CultureInfo.InvariantCulture, searchExtendedValue ? conditionNotequalExtended : conditionNotequal, escapedSerializedValue);
							}
						} else if(attributeCriterion.Comparison.Equals(Comparison.Equal)) {
							criterionBuilder.Insert(0, space).Insert(0, not);
						}
						break;
					case Comparison.Greater:
					case Comparison.GreaterEqual:
					case Comparison.Less:
					case Comparison.LessEqual:
						if(!attributeCriterion.Attribute.SerializationPreservesOrder) {
							throw new InvalidOperationException("Can not use inequality comparison on an attribute that does not maintain sort order of its underlying type in its serialized form.");
						}
						int comparedWithDefaultValue	= attributeCriterion.Attribute.Value == null ?
							attributeCriterion.Attribute.DefaultValue == null ? 0 : -1 :
							((IComparable)attributeCriterion.Attribute.Value).CompareTo(attributeCriterion.Attribute.DefaultValue);
						string escapedValue = attributeCriterion.Attribute.ToSerializedString().Replace(quote, quoteEscaped);
						switch(attributeCriterion.Comparison) {
							case Comparison.Greater:
								if(comparedWithDefaultValue < 0) {	// value > (<defaultValue)
									criterionBuilder.Insert(0, space).Insert(0, not).AppendFormat(CultureInfo.InvariantCulture, conditionLessequal, escapedValue);
								} else {							// value > (>=defaultValue)
									criterionBuilder.AppendFormat(CultureInfo.InvariantCulture, conditionGreater, escapedValue);
								}
								break;
							case Comparison.GreaterEqual:
								if(comparedWithDefaultValue > 0) {	//value >= (>defaultValue)
									criterionBuilder.AppendFormat(CultureInfo.InvariantCulture, conditionGreaterequal, escapedValue);
								} else {							//value >= (<=defaultValue>
									criterionBuilder.Insert(0, space).Insert(0, not).AppendFormat(CultureInfo.InvariantCulture, conditionLess, escapedValue);
								}
								break;
							case Comparison.Less:
								if(comparedWithDefaultValue > 0) {	//value < (>defaultValue>
									criterionBuilder.Insert(0, space).Insert(0, not).AppendFormat(CultureInfo.InvariantCulture, conditionGreaterequal, escapedValue);
								} else {							//value < (<=defaultValue)
									criterionBuilder.AppendFormat(CultureInfo.InvariantCulture, conditionLess, escapedValue);
								}
								break;
							case Comparison.LessEqual:
								if(comparedWithDefaultValue < 0) {	//value <= (<defaultValue)
									criterionBuilder.AppendFormat(CultureInfo.InvariantCulture, conditionLessequal, escapedValue);
								} else {							//value <= (>=defaultValue>
									criterionBuilder.Insert(0, space).Insert(0, not).AppendFormat(CultureInfo.InvariantCulture, conditionGreater, escapedValue);
								}
								break;
						}
						break;
				}
				(attributeCriterion.Required ? requiredAttributes : nonRequiredAttributes).Add(criterionBuilder.Append(subqueryEnd).ToString());
			}
			if(requiredAttributes.Count > 0) {
				queryBuilder.Append(parenStart);
				for(int i = 0; i < requiredAttributes.Count; i++){
					queryBuilder.Append(requiredAttributes[i]);
					if(!i.Equals(requiredAttributes.Count - 1)) {
						queryBuilder.Append(and).Append(space);
					}
				}
				queryBuilder.Append(parenEnd);
				if(nonRequiredAttributes.Count > 0) {
					queryBuilder.Append(and);
				}
			}
			if(nonRequiredAttributes.Count > 0) {
				queryBuilder.Append(parenStart);
				for(int i = 0; i < nonRequiredAttributes.Count; i++) {
					queryBuilder.Append(nonRequiredAttributes[i]);
					if(!i.Equals(nonRequiredAttributes.Count - 1)) {
						queryBuilder.Append(or).Append(space);
					}
				}
				queryBuilder.Append(parenEnd);
			}
			return queryBuilder.ToString();
		}
		private static string EscapeForLikeClause(string value) {
			const char	squarebracketOpening	= '[';
			const char	squarebracketClosing	= ']';
			const char	underscore				= '_';
			const char	percent					= '%';

			StringBuilder escapedValue = new StringBuilder(value);
			for(int i = 0; i < escapedValue.Length; i++) {
				switch(escapedValue[i]) {
					case squarebracketOpening:
					case underscore:
					case percent:
						escapedValue.Insert(i, squarebracketOpening).Insert(i += 2, squarebracketClosing);
						break;
				}
			}
			return escapedValue.ToString();
		}
		private static CategoriesResultConfiguration GetCategoriesResultConfiguration(CategoryResultConfiguration resultConfiguration) {
			if(resultConfiguration != null) {
				CategoriesResultConfiguration categoriesResultConfiguration = new CategoriesResultConfiguration();
				categoriesResultConfiguration.LoadAttributes = resultConfiguration.LoadAttributes;
				return categoriesResultConfiguration;
			}
			return null;
		}
		private static ThreadedPostsResultConfiguration GetThreadedPostsResultConfiguration(PostsResultConfiguration resultConfiguration) {
			if(resultConfiguration != null) {
				ThreadedPostsResultConfiguration threadedPostsResultConfiguration = new ThreadedPostsResultConfiguration();
				threadedPostsResultConfiguration.IncludeHidden = resultConfiguration.IncludeHidden;
				threadedPostsResultConfiguration.LoadAttributes = resultConfiguration.LoadAttributes;
				threadedPostsResultConfiguration.SortProperty = resultConfiguration.SortProperty;
				threadedPostsResultConfiguration.SortAttributeName = resultConfiguration.SortAttributeName;
				threadedPostsResultConfiguration.SortAscending = resultConfiguration.SortAscending;
				threadedPostsResultConfiguration.DateFilterFrom = resultConfiguration.DateFilterFrom;
				threadedPostsResultConfiguration.DateFilterTo = resultConfiguration.DateFilterTo;
				threadedPostsResultConfiguration.DateFilterProperty = resultConfiguration.DateFilterProperty;
				return threadedPostsResultConfiguration;
			}
			return null;
		}
		private static DateTime GetUtcDateTime(DateTime dateTime) {
			switch(dateTime.Kind) {
				case DateTimeKind.Local:
					return dateTime.ToUniversalTime();
				case DateTimeKind.Utc:
					return dateTime;
				default:
					return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
			}
		}
		private static string ToLongString(byte[] bytes) {
			const string format = "D19";
			return (bytes[0] << 56 | bytes[1] << 48 | bytes[2] << 40 | bytes[3] << 32 | bytes[4] << 24 | bytes[5] << 16 | bytes[6] << 8 | bytes[7]).ToString(format, NumberFormatInfo.InvariantInfo);
		}
		private static byte[] ToByteArray(string longAsText) {
			long @long = long.Parse(longAsText, NumberFormatInfo.InvariantInfo);
			byte[] bytes = new byte[8];
			for(int i = 0; i < 8; i++) {
				bytes[7 - i] = (byte)(@long >> (i * 8) & 255);
			}
			return bytes;
		}
		private static Attribute CheckAttributeParameter(AttributeCollection attributes, string attributeName) {
			Attribute attribute = attributes[attributeName];
			if(attribute == null) {
				throw new InvalidOperationException("Attribute does not exist.");
			}
			if(!typeof(IComparable).IsAssignableFrom(attribute.AttributeValueType)) {
				throw new InvalidOperationException("Can not sort on an attribute that is not of a comparable type.");
			}
			return attribute;
		}
		#endregion
	}

}