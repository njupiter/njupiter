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
using System.Text;

using nJupiter.DataAccess.Ldap.Configuration;

namespace nJupiter.DataAccess.Ldap.DirectoryServices {
	public class FilterBuilder : IFilterBuilder {

		private readonly IServerConfig serverConfig;

		public FilterBuilder(IServerConfig serverConfig) {
			this.serverConfig = serverConfig;
		}

		public virtual string CreatePropertyRangeFilter(string propertyName, int rangeLow, int rangeHigh, bool isLastQuery) {
			rangeHigh = isLastQuery ? int.MaxValue: rangeHigh;
			return CreatePropertyRangeFilter(propertyName, rangeLow, rangeHigh);
		}

		public virtual string CreatePropertyRangeFilter(string propertyName, int startIndex, int endIndex) {
			if(endIndex < startIndex || int.MaxValue.Equals(endIndex)) {
				return String.Format("{0};range={1}-*", propertyName, startIndex);
			}
			return String.Format("{0};range={1}-{2}", propertyName, startIndex, endIndex);
		}

		public virtual string AttachFilter(string attributeToMatch, string valueToMatch, string defaultFilter) {
			var escapedValue = EscapeSearchFilter(valueToMatch);
			return String.Format("(&{0}({1}={2}))", defaultFilter, attributeToMatch, escapedValue);
		}


		public virtual string AttachAttributeFilters(string valueToMatch, string defaultFilter, string rdnAttribute, IEnumerable<IAttributeDefinition> attributeDefinitinon) {
			var escapedName = EscapeSearchFilter(valueToMatch);
			var builder = new StringBuilder();
			foreach(var attributes in attributeDefinitinon) {
				if(!attributes.ExcludeFromNameSearch) {
					builder.Append(String.Format("({0}={1})", attributes.Name, escapedName));
				}
			}
			return String.Format("(&{0}(|({1}={2}){3}))", defaultFilter, rdnAttribute, escapedName, builder);
		}

		public virtual string AttachRdnFilter(string valueToMatch, string defaultFilter) {
			var escapedValue = EscapeSearchFilter(valueToMatch);
			return String.Format("(&{0}({1}))", defaultFilter, escapedValue);
		}

		private string EscapeSearchFilter(string searchFilter) {
			//http://stackoverflow.com/questions/649149/how-to-escape-a-string-in-c-for-use-in-an-ldap-query
			var escape = new StringBuilder();
			foreach(var current in searchFilter) {
				switch(current) {
					case '\\':
						escape.Append(@"\5c");
						break;
					case '*':
						if(serverConfig.AllowWildcardSearch) {
							escape.Append(current);
						} else {
							escape.Append(@"\2a");
						}
						break;
					case '(':
						escape.Append(@"\28");
						break;
					case ')':
						escape.Append(@"\29");
						break;
					case '\u0000':
						escape.Append(@"\00");
						break;
					case '/':
						escape.Append(@"\2f");
						break;
					default:
						escape.Append(current);
						break;
				}
			}
			return escape.ToString();
		}

	}
}
