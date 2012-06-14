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

using nJupiter.DataAccess.Ldap.Configuration;
using nJupiter.DataAccess.Ldap.DirectoryServices.Abstractions;

namespace nJupiter.DataAccess.Ldap {
	internal class LdapMembershipUserFactory : IMembershipUserFactory {

		private readonly ILdapConfig config;
		private readonly string providerName;

		public LdapMembershipUserFactory(string providerName, ILdapConfig config) {
			this.config = config;
			this.providerName = providerName;
		}

		public MembershipUserCollection CreateCollection(IEnumerable<IEntry> entries) {
			var users = new MembershipUserCollection();
			foreach(var result in entries) {
				var user = Create(result);
				if(user != null) {
					users.Add(user);
				}
			}
			return users;
		}

		public MembershipUser Create(IEntry entry) {
			if(entry == null) {
				return null;
			}

			var name = entry.GetProperties<string>(config.Users.RdnAttribute).FirstOrDefault();
			var id = name;
			var email = entry.GetProperties<string>(config.Users.EmailAttribute).FirstOrDefault();
			var description = entry.GetProperties<string>(config.Users.DescriptionAttribute).FirstOrDefault();

			var creationDate = entry.GetProperties<DateTime>(config.Users.CreationDateAttribute).FirstOrDefault();
			var lastLoginDate = entry.GetProperties<DateTime>(config.Users.LastLoginDateAttribute).FirstOrDefault();
			var lastPasswordChangedDate = entry.GetProperties<DateTime>(config.Users.LastPasswordChangedDateAttribute).FirstOrDefault();
			var lastLockoutDate = creationDate;
			var lastActivitiyDate = DateTime.Now;

			return new LdapMembershipUser(providerName, name, id, email, String.Empty, description, true, false, creationDate, lastLoginDate, lastActivitiyDate, lastPasswordChangedDate, lastLockoutDate, entry.Properties, entry.Path);
		}

	}
}
