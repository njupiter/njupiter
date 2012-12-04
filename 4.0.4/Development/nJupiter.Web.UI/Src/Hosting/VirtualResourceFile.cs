using System;
using System.IO;
using System.Reflection;
using System.Web.Hosting;

namespace nJupiter.Web.UI.Hosting {
	public class VirtualResourceFile : VirtualFile {
		
		private readonly string resourceName;
		private readonly Assembly assembly;
		
		public VirtualResourceFile(string virtualPath, string resourceName, Assembly assembly) : base(virtualPath) {
			if(resourceName == null) {
				throw new ArgumentNullException("resourceName");
			}
			if(assembly == null) {
				throw new ArgumentNullException("assembly");
			}

			this.resourceName = resourceName;
			this.assembly = assembly;
		}
		
		public override Stream Open() {
			return assembly.GetManifestResourceStream(resourceName);
		}
	}
}