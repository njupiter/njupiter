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

using System.Text;

namespace nJupiter.DataAccess.Ldap.NameParser {

	// based on http://www.codeproject.com/KB/IP/dnparser.aspx?msg=1758400 by by (C) 2005 Pete Everett (pete@CynicalPirate.com)
	// Code will be cleaned up later
	internal sealed class RdnComponent {
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

		public string ToString(Dn.EscapeChars escapeChars) {
			if(this.componentValueType == RdnValueType.HexValue) {
				return this.componentType + "=" + this.componentValue;
			}
			return this.componentType + "=" + EscapeValue(this.componentValue, escapeChars);
		}

		private static string EscapeValue(string s, Dn.EscapeChars escapeChars) {
			StringBuilder returnValue = new StringBuilder();

			for(int i = 0; i < s.Length; i++) {
				if(Rdn.IsSpecialChar(s[i]) || ((i == 0 || i == s.Length - 1) && s[i] == ' ')) {
					if((escapeChars & Dn.EscapeChars.SpecialChars) != Dn.EscapeChars.None)
						returnValue.Append('\\');

					returnValue.Append(s[i]);
				} else if(s[i] < 32 && ((escapeChars & Dn.EscapeChars.ControlChars) != Dn.EscapeChars.None)) {
					returnValue.AppendFormat("\\{0:X2}", (int)s[i]);
				} else if(s[i] >= 128 && ((escapeChars & Dn.EscapeChars.MultibyteChars) != Dn.EscapeChars.None)) {
					byte[] bytes = Encoding.UTF8.GetBytes(new[] { s[i] });

					foreach(byte b in bytes) {
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

