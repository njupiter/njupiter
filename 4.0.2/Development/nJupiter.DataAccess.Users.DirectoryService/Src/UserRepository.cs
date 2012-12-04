#region Copyright & License
/*
	Copyright (c) 2005-2011 nJupiter

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
using System.Globalization;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using nJupiter.Configuration;
using nJupiter.DataAccess.Users.Caching;

namespace nJupiter.DataAccess.Users.DirectoryService {

	public class UserRepository : UserRepositoryBase {
		#region Members
		private ContextSchema defaultContextSchema;
		private IList<IContext> contexts;
		private DirectoryService directoryService;
		#endregion

		#region Constants
		private const string OctetStringDivider = @"\";
		private const string OctetStringPattern = @"^(\\[a-fA-F_0-9]{2}){16}$";
		private static readonly Regex OctetStringRegEx = new Regex(OctetStringPattern);
		#endregion

		#region Properties
		private DirectoryService CurrentDS {
			get {
				if(this.directoryService == null) {
					this.directoryService = Config.ContainsKey("directoryService") ? DirectoryService.GetInstance(Config.GetValue("directoryService")) : DirectoryService.GetInstance();
				}
				return this.directoryService;
			}
		}
		#endregion

		#region Methods
		public UserRepository(string name, IConfig config, IPredefinedNames predefinedNames, IUserCache cache) : base(name, config, predefinedNames, cache) {}

		public override IUser GetUserById(string userId) {
			// Convert to .NET guid format if Id is of type octet string
			userId = ConvertGuid(userId);
			IUser user = this.UserCache.GetUserById(userId);
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

		public override IUser GetUserByUserName(string userName, string domain) {
			IUser user = this.UserCache.GetUserByUserName(userName, string.Empty); // Directory service does not have support for domains so we use an empty string
			if(user != null)
				return user;

			Users.SearchCriteria searchCriteria = new Users.SearchCriteria(this.PropertyNames.UserName, userName);
			var uc = GetUsersBySearchCriteria(searchCriteria);
			if(uc.Count > 0) {
				user = uc[0];
				this.UserCache.AddUserToCache(user);
			}
			return user;
		}

		public override IList<IUser> GetUsersBySearchCriteria(IEnumerable<Users.SearchCriteria> searchCriteriaCollection) {
			if(searchCriteriaCollection == null)
				throw new ArgumentNullException("searchCriteriaCollection");

			var uc = new List<IUser>();
			ArrayList arrayList = new ArrayList();
			foreach(var searchCriteria in searchCriteriaCollection) {
				SearchCriteria sc = new SearchCriteria(searchCriteria.Property.Name, searchCriteria.Property.ToSerializedString(), searchCriteria.Required);
				arrayList.Add(sc);
			}
			SearchCriteria[] scArray = (SearchCriteria[])arrayList.ToArray(typeof(SearchCriteria));
			DirectoryObject[] dirObjs = CurrentDS.GetDirectoryObjectsBySearchCriteria(scArray);
			foreach(DirectoryObject dirObj in dirObjs) {
				uc.Add(GetUserFromDirectoryObject(dirObj));
			}
			this.UserCache.AddUsersToCache(uc);
			return uc;
		}

		public override IList<IUser> GetUsersByDomain(string domain) {
			return new List<IUser>();
		}

		public override IUser CreateUserInstance(string userName, string domain) {
			if(GetUserById(userName) != null)
				throw new UserNameAlreadyExistsException("Cannot create user. User name already exists.");
			string userId = userName;
			var user = new User(userId, userName, string.Empty, GetPropertiesFromDirectoryObject(null), this.PropertyNames);  // Directory service does not have support for domains so we use an empty string
			return user;
		}

		public override void SaveUser(IUser user) {
			if(user == null)
				throw new ArgumentNullException("user");

			this.UserCache.RemoveUserFromCache(user);
			SaveProperties(user, user.Properties.GetProperties());
			if(user.Properties.AttachedContexts.Any()) {
				foreach(IContext context in user.Properties.AttachedContexts) {
					SaveProperties(user, user.Properties.GetProperties(context));
				}
			}
		}

		public override void SaveUsers(IList<IUser> users) {
			if(users == null)
				throw new ArgumentNullException("users");

			foreach(IUser user in users) {
				this.SaveUser(user);
			}
		}

		public override string[] GetDomains() {
			throw new NotImplementedException();
		}

		public override IPropertyCollection GetProperties() {
			return GetPropertiesFromDirectoryObject(null);
		}

		public override IPropertyCollection GetProperties(IContext context) {
			if(context == null)
				throw new ArgumentNullException("context");
			return GetPropertiesFromDirectoryObject(null);
		}


		public override IPropertyCollection GetProperties(IUser user, IContext context) {
			return new PropertyCollection();
		}

		public override void SaveProperties(IUser user, IPropertyCollection propertyCollection) {
			if(user == null)
				throw new ArgumentNullException("user");

			this.UserCache.RemoveUserFromCache(user);

			if(propertyCollection == null)
				throw new ArgumentNullException("propertyCollection");

			DirectoryObject dirObj = CurrentDS.CreateDirectoryObjectInstance();
			dirObj.Id = user.Id;
			foreach(IProperty property in propertyCollection) {
				if(dirObj.Contains(property.Name)) {
					dirObj[property.Name] = property.ToSerializedString();
					property.IsDirty = false;
				}
			}
			CurrentDS.SaveDirectoryObject(dirObj);
		}

		public override IContext GetContext(string contextName) {
			if(!this.GetContexts().Any(c => string.Equals(c.Name, contextName, StringComparison.InvariantCultureIgnoreCase)))
				return null;
			return this.GetContexts().FirstOrDefault(c => string.Equals(c.Name, contextName, StringComparison.InvariantCultureIgnoreCase));
		}

		public override IEnumerable<IContext> GetContexts() {
			if(this.contexts == null)
				this.contexts = new List<IContext>();
			return this.contexts;
		}

		public override IContext CreateContext(string contextName, ContextSchema schemaTable) {
			throw new NotImplementedException();
		}

		public override void DeleteContext(IContext context) {
			throw new NotImplementedException();
		}

		public override void DeleteUser(IUser user) {
			if(user == null)
				throw new ArgumentNullException("user");

			this.UserCache.RemoveUserFromCache(user);

			throw new NotImplementedException();
		}

		public override ContextSchema GetDefaultContextSchema() {
			if(this.defaultContextSchema != null)
				return this.defaultContextSchema;

			var pdt = new List<PropertyDefinition>();

			DirectoryObject directoryObject = CurrentDS.CreateDirectoryObjectInstance();

			foreach(Property property in directoryObject.Properties) {
				string propertyName = property.Name;
				Type propertyType = typeof(Property<string>);

				PropertyDefinition pd = new PropertyDefinition(propertyName, propertyType);
				pdt.Add(pd);
			}

			this.defaultContextSchema = new ContextSchema(pdt);
			return this.defaultContextSchema;
		}

		public override void SetPassword(IUser user, string password) {
			throw new NotImplementedException();
		}

		public override bool CheckPassword(IUser user, string password) {
			if(user == null)
				throw new ArgumentNullException("user");
			throw new NotImplementedException();
		}
		#endregion

		#region Helper Methods
		private IUser GetUserFromDirectoryObject(DirectoryObject doUser) {
			// If ID is formated as a guid then convert to LDAP searchable octet string
			string userId = ConvertOctetString(doUser.Id);
			var user = new User(userId, doUser[this.PropertyNames.UserName] ?? doUser.Id, string.Empty, GetPropertiesFromDirectoryObject(doUser), this.PropertyNames);
			return user;
		}

		private IPropertyCollection GetPropertiesFromDirectoryObject(DirectoryObject doUser) {
			var schema = this.GetDefaultContextSchema();
			var propertyList = new List<IProperty>();

			foreach(PropertyDefinition pd in schema) {
				string propertyValue = (doUser != null && doUser.Contains(pd.PropertyName)) ? doUser[pd.PropertyName] : null;
				string propertyName = pd.PropertyName;
				IProperty property = new Property<string>(propertyName, Context.DefaultContext, CultureInfo.InvariantCulture);
				property.Value = propertyValue;
				propertyList.Add(property);
			}
			return new PropertyCollection(propertyList, schema);
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