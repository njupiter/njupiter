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