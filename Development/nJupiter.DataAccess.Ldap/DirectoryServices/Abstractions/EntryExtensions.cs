using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace nJupiter.DataAccess.Ldap.DirectoryServices.Abstractions {
	internal static class EntryExtensions {
		public static bool IsBound(this IEntry entry) {
			return entry != null && entry.NativeObject != null;
		}		

		public static bool ContainsProperty(this IEntry entry, string propertyName) {
			return entry.IsBound() && entry.Properties.Contains(FormatPropertyName(propertyName));
		}

		public static IEnumerable<T> GetProperties<T>(this IEntry entry, string propertyName) {
			return entry.GetProperties(propertyName).Select(PropertyValueParser.Parse<T>);
		}

		public static IEnumerable<object> GetProperties(this IEntry entry, string propertyName) {
			var properties = new List<object>();
			propertyName = FormatPropertyName(propertyName);
			if(entry.ContainsProperty(propertyName)) {
				foreach(var group in entry.GetPropertyCollection(propertyName)) {
					properties.Add(group);
				}
			}
			return properties;
		}

		private static IEnumerable GetPropertyCollection(this IEntry entry, string propertyName) {
			return entry.Properties[FormatPropertyName(propertyName)] as IEnumerable; 
		}

		public static IEntityCollection GetPaged(this IEntityCollection entries, int pageIndex, int pageSize) {
			if(pageIndex < 0) {
				throw new ArgumentOutOfRangeException("pageIndex");
			}
			if(pageSize < 1) {
				throw new ArgumentOutOfRangeException("pageSize");
			}
			var pagedCollection = new EntityCollection();
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
			return propertyName.ToLower(CultureInfo.InvariantCulture);
		}
	}
}