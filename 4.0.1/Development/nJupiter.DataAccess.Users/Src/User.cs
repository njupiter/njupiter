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

	[Serializable]
	public class User : IUser {

		private IPropertyHandler properties;
		private readonly string id;
		private readonly string domain;
		private readonly string userName;
		private bool isReadOnly;

		private User() {
		}

		public User(string userId, string userName, string domain, IPropertyCollection properties, IPredefinedNames propertyNames) {
			if(userId == null) {
				throw new ArgumentNullException("userId");
			}
			if(string.IsNullOrEmpty(userName)) {
				throw new ArgumentException("User name can not be empty.");
			}
			this.id = userId;
			this.domain = (domain ?? string.Empty);
			this.userName = userName;
			this.properties = new PropertyHandler(userName, properties, propertyNames);
		}

		public string Id { get { return this.id; } }
		public string UserName { get { return this.userName; } }
		public string Domain { get { return this.domain; } }
		public IPropertyHandler Properties { get { return this.properties; } }


		public override int GetHashCode() {
			return this.Id.GetHashCode();
		}

		public object Clone() {
			var newUser = (User)MemberwiseClone();
			newUser.isReadOnly = false;
			newUser.properties = (IPropertyHandler)properties.Clone();
			return newUser;
		}

		public override bool Equals(object obj) {
			var objUser = obj as User;
			return objUser != null && objUser.Id.Equals(this.Id);
		}

		public void MakeReadOnly() {
			isReadOnly = true;
			properties.MakeReadOnly();
		}

		public bool IsReadOnly { get { return isReadOnly; } }
	}
}
