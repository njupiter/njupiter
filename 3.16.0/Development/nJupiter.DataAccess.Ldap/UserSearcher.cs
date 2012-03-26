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

using System.DirectoryServices;

namespace nJupiter.DataAccess.Ldap {
	internal class UserSearcher : Searcher {

		public UserSearcher(Configuration config) : base(config) { }

		public override DirectorySearcher Create(DirectoryEntry entry, SearchScope searchScope) {

			DirectorySearcher searcher = base.CreateSearcher(entry, searchScope, this.Config.Users.RdnAttribute, this.Config.Users.Attributes);

			searcher.PropertiesToLoad.Add(this.Config.Users.EmailAttribute);

			if(!string.IsNullOrEmpty(this.Config.Users.CreationDateAttribute)) {
				searcher.PropertiesToLoad.Add(this.Config.Users.CreationDateAttribute);
			}
			if(!string.IsNullOrEmpty(this.Config.Users.LastLoginDateAttribute)) {
				searcher.PropertiesToLoad.Add(this.Config.Users.LastLoginDateAttribute);
			}
			if(!string.IsNullOrEmpty(this.Config.Users.LastPasswordChangedDateAttribute)) {
				searcher.PropertiesToLoad.Add(this.Config.Users.LastPasswordChangedDateAttribute);
			}
			if(!string.IsNullOrEmpty(this.Config.Users.DescriptionAttribute)) {
				searcher.PropertiesToLoad.Add(this.Config.Users.DescriptionAttribute);
			}

			searcher.PropertiesToLoad.Add(this.Config.Users.MembershipAttribute);
			return searcher;
		}

	}
}
