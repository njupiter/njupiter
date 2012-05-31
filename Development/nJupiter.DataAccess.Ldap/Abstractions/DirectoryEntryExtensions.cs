using System.DirectoryServices;

namespace nJupiter.DataAccess.Ldap.Abstractions {
	internal static class DirectoryEntryExtensions {
		public static DirectoryEntry UnWrap(this IDirectoryEntry wrapped) {
			var wrapper = wrapped as DirectoryEntryWrapper;
			if(wrapper == null) {
				return null;
			}
			return wrapper.WrappedEntry;
		}
		
		public static IDirectoryEntry Wrap(this DirectoryEntry wrapped) {
			if(wrapped == null) {
				return null;
			}
			return new DirectoryEntryWrapper(wrapped);
		}	
	}
}