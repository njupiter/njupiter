#region Copyright & License
// 
// 	Copyright (c) 2005-2012 nJupiter
// 
// 	Permission is hereby granted, free of charge, to any person obtaining a copy
// 	of this software and associated documentation files (the "Software"), to deal
// 	in the Software without restriction, including without limitation the rights
// 	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// 	copies of the Software, and to permit persons to whom the Software is
// 	furnished to do so, subject to the following conditions:
// 
// 	The above copyright notice and this permission notice shall be included in
// 	all copies or substantial portions of the Software.
// 
// 	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// 	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// 	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// 	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// 	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// 	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// 	THE SOFTWARE.
// 
#endregion

using System;
using System.Collections.Generic;

using nJupiter.Configuration;
using nJupiter.DataAccess.Ldap.DistinguishedNames;

namespace nJupiter.DataAccess.Ldap.Configuration {
	internal class UsersConfigFactory : IUsersConfigFactory {
		public IUsersConfig Create(IConfig configSection) {
			var users = new UsersConfig();
			if(configSection == null) {
				throw new ArgumentNullException("configSection");
			}

			SetFilter(configSection, users);
			SetBase(configSection, users);
			SetRdnAttribute(configSection, users);
			SetMembershipAttribute(configSection, users);
			SetEmailAttribute(configSection, users);
			SetCreationDateAttribute(configSection, users);
			SetLastLoginDateAttribute(configSection, users);
			SetLastPasswordChangedDateAttribute(configSection, users);
			SetDescriptionAttribute(configSection, users);
			SetNameType(configSection, users);
			SetPath(configSection, users);
			SetMemershipUserWrappingEnabled(configSection, users);
			SetAttributeDefinitionList(configSection, users);

			return users;
		}

		private static void SetBase(IConfig configSection, UsersConfig users) {
			if(configSection.ContainsKey("users", "base")) {
				users.Base = configSection.GetValue("users", "base");
			}
		}

		private static void SetFilter(IConfig configSection, UsersConfig users) {
			if(configSection.ContainsKey("users", "filter")) {
				users.Filter = configSection.GetValue("users", "filter");
			}
		}

		private static void SetLastLoginDateAttribute(IConfig configSection, UsersConfig users) {
			if(configSection.ContainsKey("users", "lastLoginDateAttribute")) {
				users.LastLoginDateAttribute = configSection.GetValue("users", "lastLoginDateAttribute");
			}
		}

		private static void SetRdnAttribute(IConfig configSection, UsersConfig users) {
			if(configSection.ContainsKey("users", "rdnAttribute")) {
				users.RdnAttribute = configSection.GetValue("users", "rdnAttribute");
			}
		}

		private static void SetMembershipAttribute(IConfig configSection, UsersConfig users) {
			if(configSection.ContainsKey("users", "membershipAttribute")) {
				users.MembershipAttribute = configSection.GetValue("users", "membershipAttribute");
			}
		}

		private static void SetEmailAttribute(IConfig configSection, UsersConfig users) {
			if(configSection.ContainsKey("users", "emailAttribute")) {
				users.EmailAttribute = configSection.GetValue("users", "emailAttribute");
			}
		}

		private static void SetCreationDateAttribute(IConfig configSection, UsersConfig users) {
			if(configSection.ContainsKey("users", "creationDateAttribute")) {
				users.CreationDateAttribute = configSection.GetValue("users", "creationDateAttribute");
			}
		}

		private static void SetLastPasswordChangedDateAttribute(IConfig configSection, UsersConfig users) {
			if(configSection.ContainsKey("users", "lastPasswordChangedDateAttribute")) {
				users.LastPasswordChangedDateAttribute = configSection.GetValue("users", "lastPasswordChangedDateAttribute");
			}
		}

		private static void SetDescriptionAttribute(IConfig configSection, UsersConfig users) {
			if(configSection.ContainsKey("users", "descriptionAttribute")) {
				users.DescriptionAttribute = configSection.GetValue("users", "descriptionAttribute");
			}
		}

		private static void SetNameType(IConfig configSection, UsersConfig users) {
			if(configSection.ContainsKey("users", "nameType")) {
				var nameType = configSection.GetValue("users", "nameType");
				users.NameType = (NameType)Enum.Parse(typeof(NameType), nameType, true);
			}
		}

		private static void SetPath(IConfig configSection, UsersConfig users) {
			var userUri = new Uri(configSection.GetValue("url"));
			if(!String.IsNullOrEmpty(users.Base)) {
				userUri = new Uri(userUri, users.Base);
			}
			users.Path = userUri.OriginalString;
		}

		private static void SetMemershipUserWrappingEnabled(IConfig configSection, UsersConfig users) {
			if(configSection.ContainsKey("users", "membershipUserWrappingEnabled")) {
				users.MembershipUserWrappingEnabled = configSection.GetValue<bool>("users", "membershipUserWrappingEnabled");
			}
		}

		private static void SetAttributeDefinitionList(IConfig configSection, UsersConfig users) {
			var containsCustomAttributes = configSection.ContainsKey("users", "attributes");
			var attributes = new List<IAttributeDefinition>(users.Attributes);
			if(containsCustomAttributes) {
				attributes.Clear();
			}
			attributes.Attach(users.EmailAttribute, true);
			attributes.Attach(users.CreationDateAttribute, true);
			attributes.Attach(users.LastLoginDateAttribute, true);
			attributes.Attach(users.LastPasswordChangedDateAttribute, true);
			attributes.Attach(users.DescriptionAttribute, true);
			attributes.Attach(users.MembershipAttribute, true);
			if(containsCustomAttributes) {
				var attributeValues = configSection.GetValueArray("users/attributes", "attribute");
				foreach(var attribute in attributeValues) {
					var excludeFromNameSearch = false;
					var attributeKey = String.Format("users/attributes/attribute[@value='{0}']", attribute);
					if(configSection.ContainsAttribute(attributeKey, "excludeFromNameSearch")) {
						excludeFromNameSearch = configSection.GetAttribute<bool>(attributeKey, "excludeFromNameSearch");
					}
					attributes.Attach(attribute, excludeFromNameSearch);
				}
			}
			users.Attributes = attributes;
		}

	}
}