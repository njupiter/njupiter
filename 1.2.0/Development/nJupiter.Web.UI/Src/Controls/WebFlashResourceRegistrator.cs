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

using System.Collections.Generic;
using System.Web.Hosting;

using nJupiter.Configuration;
using nJupiter.Web.UI.Hosting;

namespace nJupiter.Web.UI.Controls {
	internal static class WebFlashResourceRegistrator {

		private static bool registered;
		private static readonly object padLock = new object();
		private const string section = "disableEmbededResources";
		
		public const string SwfObjectJsPath =  "/nJupiter/nJupiter.Web.UI/Web/Scripts/SwfObject.js";
		public const string SwfObjectJsResourceName =  "nJupiter.Web.UI.Web.Scripts.SwfObject.js";
		public const string SwfObjectSwfPath =  "/nJupiter/nJupiter.Web.UI/Web/Scripts/SwfObject.swf";
		public const string SwfObjectSwfResourceName =  "nJupiter.Web.UI.Web.Scripts.SwfObject.swf";
		public const string UfoJsPath =  "/nJupiter/nJupiter.Web.UI/Web/Scripts/ufo.js";
		public const string UfoJsResourceName =  "nJupiter.Web.UI.Web.Scripts.ufo.js";
		public const string UfoSwfPath =  "/nJupiter/nJupiter.Web.UI/Web/Scripts/ufo.swf";
		public const string UfoSwfResourceName =  "nJupiter.Web.UI.Web.Scripts.ufo.swf";
		public const string SwfObject2JsPath =  "/nJupiter/nJupiter.Web.UI/Web/Scripts/swfobject2.js";
		public const string SwfObject2JsResourceName =  "nJupiter.Web.UI.Web.Scripts.swfobject2.js";
		public const string SwfObject2SwfPath =  "/nJupiter/nJupiter.Web.UI/Web/Scripts/swfobject2.swf";
		public const string SwfObject2SwfResourceName =  "nJupiter.Web.UI.Web.Scripts.swfobject2.swf";

		public static void Register() {
			if(HttpContextHandler.Instance.Current == null || registered || DisableEmbeddedResources) {
				return;
			}
			lock(padLock) {
				if(!registered) {
					var resources = GetResources();
					var pathProvider = new VirtualResourcePathProvider(resources);
					HostingEnvironment.RegisterVirtualPathProvider(pathProvider);
					registered = true;
				}
			}
		}

		private static IEnumerable<VirtualResourceFile> GetResources() {
			var assembly = typeof(WebImageResourceRegistrator).Assembly;

			return new List<VirtualResourceFile> {
				new VirtualResourceFile(SwfObjectJsPath, SwfObjectJsResourceName, assembly),
				new VirtualResourceFile(SwfObjectSwfPath, SwfObjectSwfResourceName, assembly),
				new VirtualResourceFile(UfoJsPath, UfoJsResourceName, assembly),
				new VirtualResourceFile(UfoSwfPath, UfoSwfResourceName, assembly),
				new VirtualResourceFile(SwfObject2JsPath, SwfObject2JsResourceName, assembly),
				new VirtualResourceFile(SwfObject2SwfPath, SwfObject2SwfResourceName, assembly)
			};
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