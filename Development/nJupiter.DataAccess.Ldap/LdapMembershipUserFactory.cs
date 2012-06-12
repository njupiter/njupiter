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
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;

using nJupiter.DataAccess.Ldap.Abstractions;
using nJupiter.DataAccess.Ldap.Configuration;

namespace nJupiter.DataAccess.Ldap {
	internal class LdapMembershipUserFactory : IMembershipUserFactory {

		private readonly ILdapConfig config;
		private readonly string providerName;

		public LdapMembershipUserFactory(string providerName, ILdapConfig config) {
			this.config = config;
			this.providerName = providerName;
		}

		public MembershipUser CreateUserFromSearcher(IDirectorySearcher searcher) {
			var result = searcher.FindOne();
			return CreateUserFromResult(result);
		}

		public MembershipUserCollection CreateUsersFromSearcher(IDirectorySearcher searcher) {
			var results = searcher.FindAll();
			return CreateUsersFromResult(results);
		}

		private MembershipUserCollection CreateUsersFromResult(IEnumerable<ISearchResult> results) {

			var users = new MembershipUserCollection();
			if((results.Any())) {
				foreach(var result in results) {
					var user = CreateUserFromResult(result);
					if(user != null) {
						users.Add(user);
					}
				}
			}
			return users;
		}

		private MembershipUser CreateUserFromResult(ISearchResult result) {
			if(result == null) {
				return null;
			}
			var name = GetStringAttributeFromSearchResult(config.Users.RdnAttribute, result);
			var id = name;
			var email = GetStringAttributeFromSearchResult(config.Users.EmailAttribute, result);
			var description = GetStringAttributeFromSearchResult(config.Users.DescriptionAttribute, result);

			var creationDate = GetDateTimeAttributeFromSearchResult(config.Users.CreationDateAttribute, result);
			var lastLoginDate = GetDateTimeAttributeFromSearchResult(config.Users.LastLoginDateAttribute, result);
			var lastPasswordChangedDate = GetDateTimeAttributeFromSearchResult(config.Users.LastPasswordChangedDateAttribute, result);
			var lastLockoutDate = creationDate;
			var lastActivitiyDate = DateTime.Now;

			return new LdapMembershipUser(providerName, name, id, email, String.Empty, description, true, false, creationDate, lastLoginDate, lastActivitiyDate, lastPasswordChangedDate, lastLockoutDate, result.Properties, result.Path);
		}

		private static string GetStringAttributeFromSearchResult(string attribute, ISearchResult result) {
			var value = GetAttributeFromSearchResult(attribute, result);
			var stringValue = value as string;
			return stringValue ?? string.Empty;
		}

		private static DateTime GetDateTimeAttributeFromSearchResult(string attribute, ISearchResult result) {
			var value = GetAttributeFromSearchResult(attribute, result);
			if(value == null) {
				return DateTime.MinValue;
			}
			long parsedLong;
			if(long.TryParse(value.ToString(), out parsedLong)) {
				return DateTime.FromFileTime(parsedLong);
			}
			return (DateTime)value;
		}

		private static object GetAttributeFromSearchResult(string attribute, ISearchResult result) {
			if(string.IsNullOrEmpty(attribute)) {
				return null;
			}
			if(result.Properties.Contains(attribute)) {
				var values = result.Properties[attribute];
				foreach(var value in values) {
					return value;
				}
			}
			return null;

		}
	}
}
