using System.Web.Hosting;

using nJupiter.Configuration;
using nJupiter.Web.UI.Hosting;

namespace nJupiter.Web.UI.Controls {
	internal static class WebImageResourceRegistrator {

		private static bool registered;
		private static readonly object padLock = new object();
		private const string section = "disableEmbededResources";
		
		public const string StreamPagePath = "/nJupiter/nJupiter.Web.UI/Web/StreamImage.aspx";
		public const string StreamPageResourceName = "nJupiter.Web.UI.Web.StreamImage.aspx";

		public static void Register() {
			if(HttpContextHandler.Instance.Current == null || registered || DisableEmbeddedResources) {
				return;
			}
			lock(padLock) {
				if(!registered) {
					var resource = new VirtualResourceFile(StreamPagePath, StreamPageResourceName, typeof(WebImageResourceRegistrator).Assembly);
					var pathProvider = new VirtualResourcePathProvider(new[] { resource });
					HostingEnvironment.RegisterVirtualPathProvider(pathProvider);
					registered = true;
				}
			}
		}

		private static bool DisableEmbeddedResources {
			get {
				
				var config = ConfigRepository.Instance.GetConfig(true);
				if(config != null && config.ContainsKey(section)) {
					return config.GetValue<bool>(section);
				}
				return false;
			}
		}
	}
}