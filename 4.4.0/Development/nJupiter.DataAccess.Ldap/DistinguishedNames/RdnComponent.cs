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

using System.Text;

namespace nJupiter.DataAccess.Ldap.DistinguishedNames {

	// based on http://www.codeproject.com/KB/IP/dnparser.aspx?msg=1758400 by by (C) 2005 Pete Everett (pete@CynicalPirate.com)
	// Code will be cleaned up later
	internal sealed class RdnComponent : IRdnComponent {
		public enum RdnValueType {
			StringValue,
			HexValue
		};

		private readonly string componentType;
		private readonly string componentValue;
		private readonly RdnValueType componentValueType;

		public string ComponentType {
			get {
				return componentType;
			}
		}

		public string ComponentValue {
			get {
				return componentValue;
			}
		}

		internal RdnComponent(string componentType, string componentValue, RdnValueType componentValueType) {
			this.componentType = componentType;
			this.componentValue = componentValue;
			this.componentValueType = componentValueType;
		}

		public override string ToString() {
			return ToString(Dn.DefaultEscapeChars);
		}

		public string ToString(EscapeChars escapeChars) {
			if(componentValueType == RdnValueType.HexValue) {
				return string.Format("{0}={1}", componentType, componentValue);
			}
			return string.Format("{0}={1}", componentType, EscapeValue(componentValue, escapeChars));
		}

		private static string EscapeValue(string s, EscapeChars escapeChars) {
			var returnValue = new StringBuilder();

			for(var i = 0; i < s.Length; i++) {
				if(Rdn.IsSpecialChar(s[i]) || ((i == 0 || i == s.Length - 1) && s[i] == ' ')) {
					if((escapeChars & EscapeChars.SpecialChars) != EscapeChars.None)
						returnValue.Append('\\');

					returnValue.Append(s[i]);
				} else if(s[i] < 32 && ((escapeChars & EscapeChars.ControlChars) != EscapeChars.None)) {
					returnValue.AppendFormat("\\{0:X2}", (int)s[i]);
				} else if(s[i] >= 128 && ((escapeChars & EscapeChars.MultibyteChars) != EscapeChars.None)) {
					var bytes = Encoding.UTF8.GetBytes(new[] { s[i] });

					foreach(var b in bytes) {
						returnValue.AppendFormat("\\{0:X2}", b);
					}
				} else {
					returnValue.Append(s[i]);
				}
			}

			return returnValue.ToString();
		}

	}
}

