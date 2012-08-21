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
using System.Text;

namespace nJupiter.DataAccess.Ldap.DirectoryServices {
	public class PropertyValueParser {
		public static T Parse<T>(object value) {
			if(value == null) {
				return default(T);	
			}
			if(typeof(T) == value.GetType()) {
				return (T)value;
			}
			if(typeof(T) == typeof(string)) {
			    return (T)ToString(value);
			}
			if(typeof(T) == typeof(DateTime)) {
			    return (T)ToDateTime(value);
			}
			return DefaultParse<T>(value);
		}

		private static T DefaultParse<T>(object value) {
			var converter = TypeDescriptor.GetConverter(typeof(T));
			if(converter.CanConvertFrom(value.GetType())) {
					return(T)converter.ConvertFrom(value);
			}
			return default(T);
		}

		private static object ToDateTime(object value) {
			long parsedLong;
			if(long.TryParse(value.ToString(), out parsedLong)) {
				return DateTime.FromFileTime(parsedLong);
			}
			var asString = (string)ToString(value);
			DateTime result;
			if(DateTime.TryParse(asString, out result)) {
				return result;
			}
			return DefaultParse<DateTime>(value);
		}

		private static object ToString(object value) {
			var byteArray = value as byte[];
			if(byteArray != null) {
				return Encoding.UTF8.GetString(byteArray);
			}
			return DefaultParse<string>(value);
		}
	}
}