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
using System.ComponentModel;
using System.Globalization;

namespace nJupiter.DataAccess.Users {
	[Serializable]
	public class Property<T> : PropertyBase<T> {
		private readonly CultureInfo culture;
		private readonly TypeConverter converter;

		public Property(string name, IContext context, CultureInfo culture) : base(name, context) {
			this.culture = culture;
			converter = TypeDescriptor.GetConverter(typeof(T));
			if(converter == null || !converter.CanConvertFrom(typeof(string))) {
				throw new NotSupportedException(
					string.Format("The specified type {0} has no TypeConverter and can therefor not be converted", typeof(T).Name));
			}
		}

		protected override bool SetDirtyOnTouch { get { return !Type.IsPrimitive && !Type.IsValueType && Type != typeof(string); } }

		public override string ToSerializedString() {
			return converter.ConvertToString(null, culture, Value);
		}

		public override T DeserializePropertyValue(string value) {
			if(string.IsNullOrEmpty(value)) {
				return DefaultValue;
			}
			return (T)converter.ConvertFromString(null, culture, value);
		}
	}
}