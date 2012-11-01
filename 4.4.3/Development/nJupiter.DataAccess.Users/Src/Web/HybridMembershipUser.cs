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

namespace nJupiter.DataAccess.Users.Web {
	public class HybridMembershipUser : MembershipUser {
		private readonly System.Web.Security.MembershipUser primaryMembershipUser;

		public HybridMembershipUser(System.Web.Security.MembershipUser membershipUser, IUser user, string provider)
			: base(user, provider) {
			primaryMembershipUser = membershipUser;
		}

		public System.Web.Security.MembershipUser PrimaryMembershipUser { get { return primaryMembershipUser; } }

		public override string UserName { get { return primaryMembershipUser.UserName; } }

		public override DateTime CreationDate {
			get {
				try {
					if(primaryMembershipUser.CreationDate.ToUniversalTime().Equals(DateTime.MinValue)) {
						return base.CreationDate;
					}
					return primaryMembershipUser.CreationDate;
				} catch(NotSupportedException) {}
				return base.CreationDate;
			}
		}

		public override bool IsLockedOut { get { return primaryMembershipUser.IsLockedOut; } }

		public override bool IsApproved {
			get { return primaryMembershipUser.IsApproved; }
			set {
				primaryMembershipUser.IsApproved = value;
				base.IsApproved = value;
			}
		}

		public override string Comment {
			get {
				if(string.IsNullOrEmpty(primaryMembershipUser.Comment)) {
					return base.Comment;
				}
				return primaryMembershipUser.Comment;
			}
			set {
				primaryMembershipUser.Comment = value;
				base.Comment = value;
			}
		}

		public override string Email {
			get {
				if(string.IsNullOrEmpty(primaryMembershipUser.Email)) {
					return base.Email;
				}
				return primaryMembershipUser.Email;
			}
			set {
				primaryMembershipUser.Email = value;
				base.Email = value;
			}
		}

		public override DateTime LastActivityDate {
			get {
				try {
					if(primaryMembershipUser.LastActivityDate.ToUniversalTime().Equals(DateTime.MinValue)) {
						return base.LastActivityDate;
					}
					return primaryMembershipUser.LastActivityDate;
				} catch(NotSupportedException) {}
				return base.LastActivityDate;
			}
			set {
				try {
					primaryMembershipUser.LastActivityDate = value;
				} catch(NotSupportedException) {}
				base.LastActivityDate = value;
			}
		}

		public override DateTime LastLockoutDate {
			get {
				try {
					if(primaryMembershipUser.LastLockoutDate.ToUniversalTime().Equals(DateTime.MinValue)) {
						return base.LastLockoutDate;
					}
					return primaryMembershipUser.LastLockoutDate;
				} catch(NotSupportedException) {}
				return base.LastLockoutDate;
			}
		}

		public override DateTime LastLoginDate {
			get {
				try {
					if(primaryMembershipUser.LastLoginDate.ToUniversalTime().Equals(DateTime.MinValue)) {
						return base.LastLoginDate;
					}
					return primaryMembershipUser.LastLoginDate;
				} catch(NotSupportedException) {}
				return base.LastLoginDate;
			}
			set {
				try {
					primaryMembershipUser.LastLoginDate = value;
				} catch(NotSupportedException) {}
				base.LastLoginDate = value;
			}
		}

		public override DateTime LastPasswordChangedDate {
			get {
				try {
					if(primaryMembershipUser.LastPasswordChangedDate.ToUniversalTime().Equals(DateTime.MinValue)) {
						return base.LastPasswordChangedDate;
					}
					return primaryMembershipUser.LastPasswordChangedDate;
				} catch(NotSupportedException) {}
				return base.LastPasswordChangedDate;
			}
		}

		public override string PasswordQuestion { get { return primaryMembershipUser.PasswordQuestion; } }

		public override object ProviderUserKey { get { return primaryMembershipUser.ProviderUserKey; } }
	}
}