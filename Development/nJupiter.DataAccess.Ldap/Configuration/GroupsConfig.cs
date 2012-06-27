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
	internal class GroupsConfig : IGroupsConfig {
		public GroupsConfig() {
			RdnAttribute = "cn";
			RdnInPath = true;
			var attributes = new List<IAttributeDefinition>();
			attributes.Add(new AttributeDefinition("cn"));
			Attributes = attributes;
			MembershipAttribute = "member";
			NameType = NameType.Cn;
		}

		public string Filter { get; internal set; }
		public string Base { get; internal set; }
		public string Path { get; internal set; }
		public string RdnAttribute { get; internal set; }
		public bool RdnInPath {  get; internal set; }
		public IEnumerable<IAttributeDefinition> Attributes { get; internal set; }
		public string MembershipAttribute { get; internal set; }
		public NameType NameType { get; internal set; }

	}
}