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

using System.Web.Security;
using System;

namespace nJupiter.DataAccess.Users {
	public class UsersDAOMembershipUser : MembershipUser {
		private readonly string id;
		private readonly string userName;
		private readonly string providerName;
		private readonly DateTime creationDate;
		private readonly bool isLockedOut;
		private readonly DateTime lastLockoutDate;
		private readonly DateTime lastPasswordChangedDate;
		private readonly string passwordQuestion;
		private DateTime lastLoginDate;
		private string description;
		private string email;
		private bool active;
		private DateTime lastActivityDate;

		public UsersDAOMembershipUser(User user, string provider) {
			this.providerName = provider;
			if(!string.IsNullOrEmpty(user.Domain)) {
				this.userName = string.Format("{0}\\{1}", user.Domain, user.UserName);
			} else {
				this.userName = user.UserName;
			}
			this.id = user.Id ?? string.Empty;
			this.creationDate = user.Properties.CreationDate;
			this.isLockedOut = user.Properties.Locked;
			this.lastLockoutDate = user.Properties.LastLockoutDate;
			this.lastPasswordChangedDate = user.Properties.LastPasswordChangedDate;
			this.passwordQuestion = user.Properties.PasswordQuestion;
			this.lastLoginDate = user.Properties.LastLoginDate;
			this.description = user.Properties.Description;
			this.email = user.Properties.Email;
			this.active = user.Properties.Active;
			this.lastActivityDate = user.Properties.LastActivityDate;

		}

		#region MembershipUser Implementation
		/// <summary>
		/// Gets the logon name of the membership user.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The logon name of the membership user.
		/// </returns>
		public override string UserName {
			get {
				return this.userName;
			}
		}

		/// <summary>
		/// Gets the date and time when the user was added to the membership data store.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The date and time when the user was added to the membership data store.
		/// </returns>
		public override DateTime CreationDate {
			get {
				return creationDate;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the membership user is locked out and unable to be validated.
		/// </summary>
		/// <value></value>
		/// <returns>true if the membership user is locked out and unable to be validated; otherwise, false.
		/// </returns>
		public override bool IsLockedOut {
			get {
				return isLockedOut;
			}
		}


		/// <summary>
		/// Gets the most recent date and time that the membership user was locked out.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// A <see cref="T:System.DateTime"/> object that represents the most recent date and time that the membership user was locked out.
		/// </returns>
		public override DateTime LastLockoutDate {
			get {
				return lastLockoutDate;
			}
		}

		/// <summary>
		/// Gets or sets the date and time when the user was last authenticated.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The date and time when the user was last authenticated.
		/// </returns>
		public override DateTime LastLoginDate {
			get {
				return lastLoginDate;
			}
			set {
				lastLoginDate = value;
			}
		}

		/// <summary>
		/// Gets the date and time when the membership user's password was last updated.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The date and time when the membership user's password was last updated.
		/// </returns>
		public override DateTime LastPasswordChangedDate {
			get {
				return lastPasswordChangedDate;
			}
		}

		/// <summary>
		/// Gets or sets application-specific information for the membership user.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// Application-specific information for the membership user.
		/// </returns>
		public override string Comment {
			get {
				return description;
			}
			set {
				description = value;
			}
		}

		/// <summary>
		/// Gets or sets the e-mail address for the membership user.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The e-mail address for the membership user.
		/// </returns>
		public override string Email {
			get {
				return email;
			}
			set {
				email = value;
			}
		}

		/// <summary>
		/// Gets or sets whether the membership user can be authenticated.
		/// </summary>
		/// <value></value>
		/// <returns>true if the user can be authenticated; otherwise, false.
		/// </returns>
		public override bool IsApproved {
			get {
				return active;
			}
			set {
				active = value;
			}
		}

		/// <summary>
		/// Gets or sets the date and time when the membership user was last authenticated or accessed the application.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The date and time when the membership user was last authenticated or accessed the application.
		/// </returns>
		public override DateTime LastActivityDate {
			get {
				return lastActivityDate;
			}
			set {
				lastActivityDate = value;
			}
		}

		/// <summary>
		/// Gets the password question for the membership user.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The password question for the membership user.
		/// </returns>
		public override string PasswordQuestion {
			get {
				return passwordQuestion;
			}
		}

		/// <summary>
		/// Gets the name of the membership provider that stores and retrieves user information for the membership user.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The name of the membership provider that stores and retrieves user information for the membership user.
		/// </returns>
		public override string ProviderName {
			get {
				return this.providerName;
			}
		}

		/// <summary>
		/// Gets the user identifier from the membership data source for the user.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The user identifier from the membership data source for the user.
		/// </returns>
		public override object ProviderUserKey {
			get {
				return id;
			}
		}

		public virtual string UserDAOId {
			get {
				return id;
			}
		}
		#endregion

	}
}
