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
using System.DirectoryServices;
using System.Web.Security;

namespace nJupiter.DataAccess.Ldap {

	internal class LdapMembershipUserFactory {

		private readonly Configuration config;

		public static LdapMembershipUserFactory GetInstance(Configuration config) {
			if(config == null) {
				throw new ArgumentNullException("config");
			}

			return new LdapMembershipUserFactory(config);
		}

		private LdapMembershipUserFactory(Configuration config) {
			this.config = config;
		}

		public MembershipUser CreateUserFromSearcher(string providerName, DirectorySearcher searcher) {
			SearchResult result = searcher.FindOne();
			return CreateUserFromResult(providerName, result);
		}

		public MembershipUserCollection CreateUsersFromSearcher(string providerName, DirectorySearcher searcher) {
			SearchResultCollection results = searcher.FindAll();
			return CreateUsersFromResult(results, providerName);
		}

		private MembershipUserCollection CreateUsersFromResult(SearchResultCollection results, string providerName) {

			MembershipUserCollection users = new MembershipUserCollection();
			if((results.Count > 0)) {
				foreach(SearchResult result in results) {
					MembershipUser user = CreateUserFromResult(providerName, result);
					if(user != null) {
						users.Add(user);
					}
				}
			}
			return users;
		}

		private MembershipUser CreateUserFromResult(string providerName, SearchResult result) {
			if(result == null) {
				return null;
			}
			string name = GetStringAttributeFromSearchResult(config.Users.RdnAttribute, result);
			string id = name;
			string email = GetStringAttributeFromSearchResult(config.Users.EmailAttribute, result);
			string description = GetStringAttributeFromSearchResult(config.Users.DescriptionAttribute, result);

			DateTime creationDate = GetDateTimeAttributeFromSearchResult(config.Users.CreationDateAttribute, result);
			DateTime lastLoginDate = GetDateTimeAttributeFromSearchResult(config.Users.LastLoginDateAttribute, result);
			DateTime lastPasswordChangedDate = GetDateTimeAttributeFromSearchResult(config.Users.LastPasswordChangedDateAttribute, result);
			DateTime lastLockoutDate = creationDate;
			DateTime lastActivitiyDate = DateTime.Now;

			return new LdapMembershipUser(providerName, name, id, email, String.Empty, description, true, false, creationDate, lastLoginDate, lastActivitiyDate, lastPasswordChangedDate, lastLockoutDate, result.Properties, result.Path);
		}

		private static string GetStringAttributeFromSearchResult(string attribute, SearchResult result) {
			object value = GetAttributeFromSearchResult(attribute, result);
			string stringValue = value as string;
			return stringValue ?? string.Empty;
		}

		private static DateTime GetDateTimeAttributeFromSearchResult(string attribute, SearchResult result) {
			object value = GetAttributeFromSearchResult(attribute, result);
			if(value == null) {
				return DateTime.MinValue;
			}
			long parsedLong;
			if(long.TryParse(value.ToString(), out parsedLong)) {
				return DateTime.FromFileTime(parsedLong);
			}
			return (DateTime)value;
		}

		private static object GetAttributeFromSearchResult(string attribute, SearchResult result) {
			if(string.IsNullOrEmpty(attribute)) {
				return null;
			}
			if(result.Properties.Contains(attribute)) {
				ResultPropertyValueCollection values = result.Properties[attribute];
				foreach(object value in values) {
					return value;
				}
			}
			return null;

		}
	}
}
