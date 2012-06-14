using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace nJupiter.DataAccess.Ldap.DirectoryServices.Abstractions {
	internal static class EntryExtensions {
		public static bool IsBound(this IEntry entry) {
			return entry != null && entry.NativeObject != null;
		}		

		public static bool ContainsProperty(this IEntry result, string property) {
			return result.IsBound() && result.Properties.Contains(property);
		}

		public static IEnumerable<T> GetProperties<T>(this IEntry entry, string propertyName) {
			return entry.GetProperties(propertyName).Select(PropertyValueParser.Parse<T>);
		}

		public static IEnumerable<object> GetProperties(this IEntry entry, string propertyName) {
			if(entry.ContainsProperty(propertyName)) {
				foreach(var group in entry.GetPropertyCollection(propertyName)) {
					yield return group;
				}
			}
		}

		private static IEnumerable GetPropertyCollection(this IEntry entry, string propertyName) {
			return entry.Properties[propertyName] as IEnumerable; 
		}

		public static IEnumerable<IEntry> GetPaged(this IEnumerable<IEntry> entries, int pageIndex, int pageSize) {
			if(pageIndex < 0) {
				throw new ArgumentOutOfRangeException("pageIndex");
			}
			if(pageSize < 1) {
				throw new ArgumentOutOfRangeException("pageSize");
			}
			var index = 0;
			var startIndex = pageIndex * pageSize;
			var endIndex = startIndex + pageSize;
			foreach(var entry in entries) {
				if(index >= startIndex) {
					yield return entry;
				}
				index++;
				if(index >= endIndex) {
					yield break;
				}
			}			
		}
	}
}