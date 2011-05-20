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

namespace nJupiter.DataAccess.Users {
	// TODO: Implement clonable

	[Serializable]
	public class User : IUser {

		private readonly Properties properties;
		private readonly string id;
		private readonly string domain;
		private readonly string userName;

		private User() {
		}

		public User(string userId, string userName, string domain, PropertyCollection properties, ICommonNames propertyNames)
			: this() {
			if(string.IsNullOrEmpty(userName))
				throw new UserNameEmptyException("User name can not be empty.");
			this.id = userId;
			this.domain = (domain ?? string.Empty);
			this.userName = userName;
			this.properties = new Properties(userName, properties, propertyNames);
		}

		public string Id { get { return this.id; } }
		public string UserName { get { return this.userName; } }
		public string Domain { get { return this.domain; } }
		public Properties Properties { get { return this.properties; } }


		public override int GetHashCode() {
			return this.Id.GetHashCode();
		}

		public override bool Equals(object obj) {
			User objUser = obj as User;
			return objUser != null && objUser.Id.Equals(this.Id);
		}
	}
}
