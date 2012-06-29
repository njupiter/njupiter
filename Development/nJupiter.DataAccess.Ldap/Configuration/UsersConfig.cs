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

using nJupiter.DataAccess.Ldap.DistinguishedNames;

namespace nJupiter.DataAccess.Ldap.Configuration {
	internal class UsersConfig : IUsersConfig {

		public UsersConfig() {
			Filter = "(objectClass=*)";
			RdnAttribute = "cn";
			RdnInPath = true;
			var attributes = new List<IAttributeDefinition>();
			attributes.Add(new AttributeDefinition("cn"));
			Attributes = attributes;
			EmailAttribute = "mail";
			NameType = NameType.Cn;
		}

		public string Filter { get; set; }
		public string Base { get; set; }
		public string Path { get; set; }
		public string RdnAttribute { get; set; }
		public bool RdnInPath {  get; set; }
		public IEnumerable<IAttributeDefinition> Attributes { get; set; }
		public string MembershipAttribute { get; set; }
		public string EmailAttribute { get; set; }
		public string CreationDateAttribute { get; set; }
		public string LastLoginDateAttribute { get; set; }
		public string LastPasswordChangedDateAttribute { get; set; }
		public string DescriptionAttribute { get; set; }
		public NameType NameType { get; set; }
	}
}