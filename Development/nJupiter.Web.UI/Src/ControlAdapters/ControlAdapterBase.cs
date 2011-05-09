#region Copyright & License
/*
	Copyright (c) 2005-2011 nJupiter

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.
*/
#endregion

using System;
using System.Web.UI.Adapters;

using System.Collections.Generic;
using System.Xml;

using nJupiter.Configuration;

namespace nJupiter.Web.UI.ControlAdapters {
	public abstract class ControlAdapterBase : ControlAdapter {

		private static readonly object PadLock = new object();
		private static Dictionary<string, List<string>> excludedPathArray;

		static ControlAdapterBase() {
			ControlAdapterBase.Configure(null, EventArgs.Empty);
		}

		private static void Configure(object sender, EventArgs e) {
			lock(PadLock) {
				Dictionary<string, List<string>> array = new Dictionary<string, List<string>>();
				IConfig config = ConfigHandler.GetSystemConfig();
				if(config != null) {
					IConfig controlAdapterConfig = config.GetConfigSection("serverConfig/controlAdapters");
					if(controlAdapterConfig != null) {
						XmlNodeList adapters = controlAdapterConfig.ConfigXml.SelectNodes("controlAdapter");
						if(adapters != null) {
							foreach(XmlNode adapter in adapters) {
								XmlAttribute nameAttribute = adapter.Attributes["name"];
								if(nameAttribute != null && nameAttribute.Value != null) {
									XmlNodeList excludedPaths = adapter.SelectNodes("excludedPaths/path");
									List<string> excludedPathList = new List<string>();
									if(excludedPaths != null) {
										foreach(XmlNode excludedPath in excludedPaths) {
											XmlAttribute excludedPathAttribute = excludedPath.Attributes["value"];
											if(excludedPathAttribute != null && excludedPathAttribute.Value != null) {
												excludedPathList.Add(excludedPathAttribute.Value);
											}
										}
									}
									array.Add(nameAttribute.Value, excludedPathList);
								}
							}
						}
					}
					config.Disposed += ControlAdapterBase.Configure;
				}
				excludedPathArray = array;
			}
		}


		protected virtual bool AdapterEnabled {
			get {
				if(this.Page == null) {
					return false;
				}
				string[] adapters = { "*", this.GetType().FullName };
				foreach(string adapter in adapters) {
					if(excludedPathArray.ContainsKey(adapter)) {
						List<string> excludedPaths = excludedPathArray["*"];
						foreach(string excludedPath in excludedPaths) {
							if(this.Page.Request.Path.StartsWith(excludedPath, StringComparison.OrdinalIgnoreCase)) {
								return false;
							}
						}
					}
				}
				return true;
			}
		}
	}
}
