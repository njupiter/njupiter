using System.DirectoryServices;

namespace nJupiter.DataAccess.Ldap.DirectoryServices.Abstraction {
	internal static class DirectoryEntryExtensions {
		public static DirectoryEntry UnWrap(this IEntry wrapped) {
			var wrapper = wrapped.GetDirectoryEntry() as DirectoryEntryWrapper;
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