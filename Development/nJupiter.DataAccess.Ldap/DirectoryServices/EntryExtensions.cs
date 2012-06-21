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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace nJupiter.DataAccess.Ldap.DirectoryServices {
	internal static class EntryExtensions {
		public static bool IsBound(this IEntry entry) {
			return entry != null && entry.NativeObject != null;
		}

		public static bool ContainsProperty(this IEntry entry, string propertyName) {
			return entry.ContainsPropertyInternal(FormatPropertyName(propertyName));
		}

		public static IEnumerable<T> GetProperties<T>(this IEntry entry, string propertyName) {
			return entry.GetProperties(propertyName).Select(PropertyValueParser.Parse<T>);
		}

		public static IEnumerable<object> GetProperties(this IEntry entry, string propertyName) {
			var properties = new List<object>();
			propertyName = FormatPropertyName(propertyName);
			if(entry.ContainsPropertyInternal(propertyName)) {
				foreach(var group in entry.GetPropertyCollection(propertyName)) {
					properties.Add(group);
				}
			}
			return properties;
		}

		private static bool ContainsPropertyInternal(this IEntry entry, string propertyName) {
			return !string.IsNullOrEmpty(propertyName) && entry.IsBound() && entry.Properties.Contains(propertyName);
		}

		private static IEnumerable GetPropertyCollection(this IEntry entry, string propertyName) {
			return entry.Properties[propertyName] as IEnumerable;
		}

		public static IEntryCollection GetPaged(this IEntryCollection entries, int pageIndex, int pageSize) {
			if(pageIndex < 0) {
				throw new ArgumentOutOfRangeException("pageIndex");
			}
			if(pageSize < 1) {
				throw new ArgumentOutOfRangeException("pageSize");
			}
			var pagedCollection = new EntryCollection();
			var index = 0;
			var startIndex = pageIndex * pageSize;
			var endIndex = startIndex + pageSize;
			foreach(var entry in entries) {
				if(index >= startIndex) {
					pagedCollection.Add(entry);
				}
				index++;
				if(index >= endIndex) {
					break;
				}
			}
			return pagedCollection;
		}

		private static string FormatPropertyName(string propertyName) {
			if(propertyName == null) {
				return null;
			}
			return propertyName.ToLower(CultureInfo.InvariantCulture);
		}
	}
}