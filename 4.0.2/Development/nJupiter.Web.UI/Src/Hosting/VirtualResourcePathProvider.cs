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
using System.Linq;
using System.Web.Caching;
using System.Web.Hosting;

namespace nJupiter.Web.UI.Hosting {
	public class VirtualResourcePathProvider : VirtualPathProvider {
		
		private readonly IEnumerable<VirtualResourceFile> resources;
		
		public VirtualResourcePathProvider(IEnumerable<VirtualResourceFile> resources) {
			if(resources == null) {
				throw new ArgumentNullException("resources");
			}
			this.resources = resources;
		}
		
		public override bool FileExists(string virtualPath) {
			var pathExists = resources.Any(file => FileInPath(virtualPath, file));
			return pathExists || Previous.FileExists(virtualPath);
		}

		private static bool FileInPath(string virtualPath, VirtualResourceFile c) {
			return string.Equals(c.VirtualPath, virtualPath, StringComparison.InvariantCultureIgnoreCase);
		}

		public override VirtualFile GetFile(string virtualPath) {
			var virtualFile = resources.FirstOrDefault(file => FileInPath(virtualPath, file));
			if(virtualFile == null) {
				return Previous.GetFile(virtualPath);
			}
			return virtualFile;
		}

		public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart) {
			if(FileExists(virtualPath)) {
				return null;
			}
			return Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
		}


	}
}