using System;
using System.Collections.Generic;

using nJupiter.Configuration;
using nJupiter.DataAccess.Ldap.NameParser;

namespace nJupiter.DataAccess.Ldap.Configuration {
	internal class GroupsConfigFactory : IGroupConfigFactory {
		public IGroupsConfig Create(IConfig configSection) {
			var groups = new GroupsConfig();
			if(configSection == null) {
				return groups;
			}
			if(configSection.ContainsKey("groups", "filter")) {
				groups.Filter = configSection.GetValue("groups", "filter");
			} else {
				groups.Filter = "(objectClass=groupOfNames)";
			}

			if(configSection.ContainsKey("groups", "base")) {
				groups.Base = configSection.GetValue("groups", "base");
			}

			if(configSection.ContainsKey("groups", "rdnAttribute")) {
				groups.RdnAttribute = configSection.GetValue("groups", "rdnAttribute");
			} else {
				groups.RdnAttribute = "cn";
			}

			var groupAttributeDefinitionList = new List<IAttributeDefinition>();
			if(configSection.ContainsKey("groups", "attributes")) {
				var attributes = configSection.GetValueArray("groups/attributes", "attribute");
				foreach(var attribute in attributes) {
					var excludeFromNameSearch = false;
					var attributeKey = String.Format("groups/attributes/attribute[@value='{0}']", attribute);
					if(configSection.ContainsAttribute(attributeKey, "excludeFromNameSearch")) {
						excludeFromNameSearch = configSection.GetAttribute<bool>(attributeKey, "excludeFromNameSearch");
					}
					var attributeDefinition = new AttributeDefinition(attribute, excludeFromNameSearch);
					groupAttributeDefinitionList.Add(attributeDefinition);
				}
			} else {
				var attributeDefinition = new AttributeDefinition("cn");
				groupAttributeDefinitionList.Add(attributeDefinition);
			}
			groups.Attributes = groupAttributeDefinitionList;

			if(configSection.ContainsKey("groups", "membershipAttribute")) {
				groups.MembershipAttribute = configSection.GetValue("groups", "membershipAttribute");
			} else {
				groups.MembershipAttribute = "groupMembership";
			}

			if(configSection.ContainsKey("groups", "membershipAttributeNameType")) {
				var nameType = configSection.GetValue("groups", "membershipAttributeNameType");
				groups.MembershipAttributeNameType = (NameType)Enum.Parse(typeof(NameType), nameType, true);
			} else {
				groups.MembershipAttributeNameType = NameType.Cn;
			}

			if(configSection.ContainsKey("groups", "nameType")) {
				var nameType = configSection.GetValue("groups", "nameType");
				groups.NameType = (NameType)Enum.Parse(typeof(NameType), nameType, true);
			} else {
				groups.NameType = NameType.Cn;
			}



			Uri groupUri = new Uri(configSection.GetValue("url"));
			if(!String.IsNullOrEmpty(groups.Base)) {
				groupUri = new Uri(groupUri, groups.Base);
			}
			groups.Path = LdapPathHandler.UriToPath(groupUri);

			return groups;
		}
	}
}