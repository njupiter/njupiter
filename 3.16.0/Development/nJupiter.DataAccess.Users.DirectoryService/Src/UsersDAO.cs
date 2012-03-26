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
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

using nJupiter.DataAccess.DirectoryService;

using ds = nJupiter.DataAccess.DirectoryService;

namespace nJupiter.DataAccess.Users.DirectoryService {

	public class UsersDAO : Users.UsersDAO {
		#region Members
		private PropertySchemaTable defaultPropertySchemaTable;
		private ContextCollection contexts;
		private DataAccess.DirectoryService.DirectoryService directoryService;
		#endregion

		#region Constants
		private const string OctetStringDivider = @"\";
		private const string OctetStringPattern = @"^(\\[a-fA-F_0-9]{2}){16}$";
		private static readonly Regex OctetStringRegEx = new Regex(OctetStringPattern);
		#endregion

		#region Properties
		private DataAccess.DirectoryService.DirectoryService CurrentDS {
			get {
				if(this.directoryService == null) {
					this.directoryService = Config.ContainsKey("directoryService") ? DataAccess.DirectoryService.DirectoryService.GetInstance(Config.GetValue("directoryService")) : DataAccess.DirectoryService.DirectoryService.GetInstance();
				}
				return this.directoryService;
			}
		}
		#endregion

		#region Methods
		public override User GetUserById(string userId) {
			// Convert to .NET guid format if Id is of type octet string
			userId = ConvertGuid(userId);
			User user = this.UserCache.GetUserById(userId);
			if(user != null)
				return user;
			DirectoryObject directoryObject = CurrentDS.GetDirectoryObjectById(userId);
			if(directoryObject != null) {
				user = GetUserFromDirectoryObject(directoryObject);
				this.UserCache.AddUserToCache(user);
				return user;
			}
			return null;
		}

		public override User GetUserByUserName(string userName, string domain) {
			User user = this.UserCache.GetUserByUserName(userName, string.Empty); // Directory service does not have support for domains so we use an empty string
			if(user != null)
				return user;

			SearchCriteria searchCriteria = new SearchCriteria(this.PropertyNames.UserName, userName);
			UserCollection uc = GetUsersBySearchCriteria(searchCriteria);
			if(uc.Count > 0) {
				user = uc[0];
				this.UserCache.AddUserToCache(user);
			}
			return user;
		}

		public override UserCollection GetUsersBySearchCriteria(SearchCriteriaCollection searchCriteriaCollection) {
			if(searchCriteriaCollection == null)
				throw new ArgumentNullException("searchCriteriaCollection");

			UserCollection uc = new UserCollection();
			ArrayList arrayList = new ArrayList();
			foreach(SearchCriteria searchCriteria in searchCriteriaCollection) {
				DataAccess.DirectoryService.SearchCriteria sc = new DataAccess.DirectoryService.SearchCriteria(searchCriteria.Property.Name, searchCriteria.Property.ToSerializedString(), searchCriteria.Required);
				arrayList.Add(sc);
			}
			DataAccess.DirectoryService.SearchCriteria[] scArray = (DataAccess.DirectoryService.SearchCriteria[])arrayList.ToArray(typeof(DataAccess.DirectoryService.SearchCriteria));
			DirectoryObject[] dirObjs = CurrentDS.GetDirectoryObjectsBySearchCriteria(scArray);
			foreach(DirectoryObject dirObj in dirObjs) {
				uc.Add(GetUserFromDirectoryObject(dirObj));
			}
			this.UserCache.AddUsersToCache(uc);
			return uc;
		}

		public override UserCollection GetUsersByDomain(string domain) {
			return new UserCollection();
		}

		public override User CreateUserInstance(string userName, string domain) {
			if(GetUserById(userName) != null)
				throw new UserNameAlreadyExistsException("Cannot create user. User name already exists.");
			string userId = userName;
			User user = new User(userId, userName, string.Empty, GetPropertiesFromDirectoryObject(null), this.PropertyNames);  // Directory service does not have support for domains so we use an empty string
			return user;
		}

		public override void SaveUser(User user) {
			if(user == null)
				throw new ArgumentNullException("user");

			this.UserCache.RemoveUserFromCache(user);
			SaveProperties(user, this.GetProperties(user));
			if(GetAttachedContextsToUser(user).Length > 0) {
				foreach(Context context in GetAttachedContextsToUser(user)) {
					SaveProperties(user, user.GetProperties(context));
				}
			}
		}

		public override void SaveUsers(UserCollection users) {
			if(users == null)
				throw new ArgumentNullException("users");

			foreach(User user in users) {
				this.SaveUser(user);
			}
		}

		public override string[] GetDomains() {
			throw new NotImplementedException();
		}

		public override PropertyCollection GetProperties() {
			return GetPropertiesFromDirectoryObject(null);
		}

		public override PropertyCollection GetProperties(Context context) {
			if(context == null)
				throw new ArgumentNullException("context");
			return GetPropertiesFromDirectoryObject(null);
		}

		public override PropertyCollection GetProperties(User user, Context context) {
			if(user == null)
				throw new ArgumentNullException("user");
			PropertyCollection pc = base.GetProperties(user, context);
			if(pc != null)
				return pc;
			return CreatePropertyCollectionInstance(CreatePropertySchemaTableInstance());
		}

		public override void SaveProperties(User user, PropertyCollection propertyCollection) {
			if(user == null)
				throw new ArgumentNullException("user");

			this.UserCache.RemoveUserFromCache(user);

			if(propertyCollection == null)
				throw new ArgumentNullException("propertyCollection");

			DirectoryObject dirObj = CurrentDS.CreateDirectoryObjectInstance();
			dirObj.Id = user.Id;
			foreach(AbstractProperty property in propertyCollection) {
				if(dirObj.Contains(property.Name)) {
					dirObj[property.Name] = property.ToSerializedString();
					property.IsDirty = false;
				}
			}
			CurrentDS.SaveDirectoryObject(dirObj);
		}

		public override Context GetContext(string contextName) {
			if(!this.GetContexts().Contains(contextName))
				return null;
			return this.GetContexts()[contextName];
		}

		public override ContextCollection GetContexts() {
			if(this.contexts == null)
				this.contexts = CreateContextCollectionInstance();
			return this.contexts;
		}

		public override Context CreateContext(string contextName, PropertySchemaTable schemaTable) {
			throw new NotImplementedException();
		}

		public override void DeleteContext(Context context) {
			throw new NotImplementedException();
		}

		public override void DeleteUser(User user) {
			if(user == null)
				throw new ArgumentNullException("user");

			this.UserCache.RemoveUserFromCache(user);

			throw new NotImplementedException();
		}

		public override PropertySchemaTable GetPropertySchemas() {
			if(this.defaultPropertySchemaTable != null)
				return this.defaultPropertySchemaTable;

			PropertySchemaTable pdt = CreatePropertySchemaTableInstance();

			DirectoryObject directoryObject = CurrentDS.CreateDirectoryObjectInstance();

			foreach(Property property in directoryObject.Properties) {
				string propertyName = property.Name;
				Type propertyType = typeof(StringProperty);

				PropertySchema pd = CreatePropertySchemaInstance(propertyName, propertyType);
				AddPropertySchemaToTable(pd, pdt);
			}

			this.defaultPropertySchemaTable = pdt;
			return this.defaultPropertySchemaTable;
		}

		public override void SetPassword(User user, string password) {
			throw new NotImplementedException();
		}

		public override bool CheckPassword(User user, string password) {
			if(user == null)
				throw new ArgumentNullException("user");
			throw new NotImplementedException();
		}
		#endregion

		#region Helper Methods
		private User GetUserFromDirectoryObject(DirectoryObject doUser) {
			// If ID is formated as a guid then convert to LDAP searchable octet string
			string userId = ConvertOctetString(doUser.Id);
			User user = new User(userId, doUser[this.PropertyNames.UserName] ?? doUser.Id, string.Empty, GetPropertiesFromDirectoryObject(doUser), this.PropertyNames);
			return user;
		}
		private PropertyCollection GetPropertiesFromDirectoryObject(DirectoryObject doUser) {
			PropertySchemaTable pdt = this.GetPropertySchemas();
			PropertyCollection upc = CreatePropertyCollectionInstance(pdt);

			foreach(PropertySchema pd in pdt) {
				string propertyValue = ((doUser != null && doUser.Contains(pd.PropertyName)) ? doUser[pd.PropertyName] : null);
				string propertyName = pd.PropertyName;
				Type propertyType = pd.DataType;
				AbstractProperty property = CreatePropertyInstance(propertyName, propertyValue, propertyType, null);
				if(property != null)
					AddPropertyToCollection(property, upc);
			}
			return upc;
		}
		// Converts a string from an octet string to a guid string.
		// If it is not of correct format, return the original string
		private static string ConvertOctetString(string input) {
			if(OctetStringRegEx.IsMatch(input)) {
				input = input.Replace(OctetStringDivider, string.Empty);
				input = new Guid(input).ToString();
			}
			return input;
		}
		// Converts a string from a guid string to an octet string.
		// If it is not of correct format, return the original string
		private static string ConvertGuid(string input) {
			try {
				input = new Guid(input).ToString("N");
				StringBuilder sb = new StringBuilder();
				for(int i = 0; i < input.Length; i = i + 2) {
					sb.Append(OctetStringDivider);
					sb.Append(input[i]);
					sb.Append(input[i + 1]);
				}
				input = sb.ToString();
			} catch(FormatException) { }
			return input;
		}
		#endregion
	}
}