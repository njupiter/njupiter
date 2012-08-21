using System;
using System.Collections;
using System.Web.Security;

namespace nJupiter.DataAccess.Ldap {

	[Serializable]
	public class LdapMembershipUser : MembershipUser {
		public LdapMembershipUser() {}

		public LdapMembershipUser(string providerName,
									string name,
									object providerUserKey,
									string email,
									string passwordQuestion,
									string comment,
									bool isApproved,
									bool isLockedOut,
									DateTime creationDate,
									DateTime lastLoginDate,
									DateTime lastActivityDate,
									DateTime lastPasswordChangedDate,
									DateTime lastLockoutDate,
									IDictionary propertyCollection,
									string path)
									: base(providerName,
											name,
											providerUserKey,
											email,
											passwordQuestion,
											comment,
											isApproved,
											isLockedOut,
											creationDate,
											lastLoginDate,
											lastActivityDate,
											lastPasswordChangedDate,
											lastLockoutDate) {
			attributes = new AttributeCollection(propertyCollection);
			this.path = path;
		}

		private readonly IAttributeCollection attributes;
		private readonly string path;

		public virtual IAttributeCollection Attributes {
			get {
				return attributes;
			}
		}

		public virtual string Path {
			get {
				return path;
			}
		}


	}
}
