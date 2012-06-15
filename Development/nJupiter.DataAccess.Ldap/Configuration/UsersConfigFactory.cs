using System;
using System.Collections.Generic;

using nJupiter.Configuration;
using nJupiter.DataAccess.Ldap.NameParser;

namespace nJupiter.DataAccess.Ldap.Configuration {
	internal class UsersConfigFactory : IUsersConfigFactory {
		public IUsersConfig Create(IConfig configSection) {
			var users = new UsersConfig();
			if(configSection == null) {
				return users;
			}

			if(configSection.ContainsKey("users", "filter")) {
				users.Filter = configSection.GetValue("users", "filter");
			} else {
				users.Filter = "(objectClass=person)";
			}

			if(configSection.ContainsKey("users", "base")) {
				users.Base = configSection.GetValue("users", "base");
			}

			if(configSection.ContainsKey("users", "rdnAttribute")) {
				users.RdnAttribute = configSection.GetValue("users", "rdnAttribute");
			} else {
				users.RdnAttribute = "cn";
			}

			var userAttributeDefinitionList = new List<IAttributeDefinition>();
			if(configSection.ContainsKey("users", "attributes")) {
				var attributes = configSection.GetValueArray("users/attributes", "attribute");
				foreach(var attribute in attributes) {
					var excludeFromNameSearch = false;
					var attributeKey = String.Format("users/attributes/attribute[@value='{0}']", attribute);
					if(configSection.ContainsAttribute(attributeKey, "excludeFromNameSearch")) {
						excludeFromNameSearch = configSection.GetAttribute<bool>(attributeKey, "excludeFromNameSearch");
					}
					var attributeDefinition = new AttributeDefinition(attribute, excludeFromNameSearch);
					userAttributeDefinitionList.Add(attributeDefinition);
				}
			} else {
				var attributeDefinition = new AttributeDefinition("cn");
				userAttributeDefinitionList.Add(attributeDefinition);
			}
			users.Attributes = userAttributeDefinitionList;

			if(configSection.ContainsKey("users", "membershipAttribute")) {
				users.MembershipAttribute = configSection.GetValue("users", "membershipAttribute");
			}

			if(configSection.ContainsKey("users", "emailAttribute")) {
				users.EmailAttribute = configSection.GetValue("users", "emailAttribute");
			} else {
				users.EmailAttribute = "mail";
			}

			if(configSection.ContainsKey("users", "creationDateAttribute")) {
				users.CreationDateAttribute = configSection.GetValue("users", "creationDateAttribute");
			}

			if(configSection.ContainsKey("users", "lastLoginDateAttribute")) {
				users.LastLoginDateAttribute = configSection.GetValue("users", "lastLoginDateAttribute");
			}

			if(configSection.ContainsKey("users", "lastPasswordChangedDateAttribute")) {
				users.LastPasswordChangedDateAttribute = configSection.GetValue("users", "lastPasswordChangedDateAttribute");
			}

			if(configSection.ContainsKey("users", "descriptionAttribute")) {
				users.DescriptionAttribute = configSection.GetValue("users", "descriptionAttribute");
			}

			if(configSection.ContainsKey("users", "nameType")) {
				var nameType = configSection.GetValue("users", "nameType");
				users.NameType = (NameType)Enum.Parse(typeof(NameType), nameType, true);
			} else {
				users.NameType = NameType.Cn;
			}

			Uri userUri = new Uri(configSection.GetValue("url"));
			if(!String.IsNullOrEmpty(users.Base)) {
				userUri = new Uri(userUri, users.Base);
			}
			users.Path = LdapPathHandler.UriToPath(userUri);

			return users;
		}
	}
}