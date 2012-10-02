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
using System.Globalization;

namespace nJupiter.DataAccess.Users.Sql {
	[Serializable]
	public class IntProperty : PropertyBase<int>, ISqlProperty {
		private const string Format = "D10";

		public IntProperty(string propertyName, IContext context) : base(propertyName, context) {}

		public override string ToSerializedString() {
			return ToLexicographicallyFormat();
		}

		private string ToLexicographicallyFormat() {
			long longPositiveValue = (long)Value - int.MinValue;
			return longPositiveValue.ToString(Format, NumberFormatInfo.InvariantInfo);
		}

		public override int DeserializePropertyValue(string value) {
			return string.IsNullOrEmpty(value)
				       ? DefaultValue
				       : (int)(long.Parse(value, NumberFormatInfo.InvariantInfo) + int.MinValue);
		}

		public bool SerializationPreservesOrder { get { return true; } }
	}
}