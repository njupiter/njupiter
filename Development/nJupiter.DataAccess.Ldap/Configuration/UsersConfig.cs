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

using System.Collections.Generic;

using nJupiter.DataAccess.Ldap.NameParser;

namespace nJupiter.DataAccess.Ldap.Configuration {
	internal class UsersConfig : IUsersConfig {

		public UsersConfig() {
			Filter = "(objectClass=person)";
			RdnAttribute = "cn";
			Attributes = new List<IAttributeDefinition>();
			Attributes.Add(new AttributeDefinition("cn"));
			MembershipAttribute = "memberOf";
			EmailAttribute = "mail";
			NameType = NameType.Cn;
		}

		public string Filter { get; internal set; }
		public string Base { get; internal set; }
		public string Path { get; internal set; }
		public string RdnAttribute { get; internal set; }
		public List<IAttributeDefinition> Attributes { get; internal set; }
		public string MembershipAttribute { get; internal set; }
		public string EmailAttribute { get; internal set; }
		public string CreationDateAttribute { get; internal set; }
		public string LastLoginDateAttribute { get; internal set; }
		public string LastPasswordChangedDateAttribute { get; internal set; }
		public string DescriptionAttribute { get; internal set; }
		public NameType NameType { get; internal set; }
	}
}