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
using System.Drawing.Drawing2D;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.Hosting;
using System.Reflection;
using System.Globalization;

using nJupiter.Configuration;
using nJupiter.Drawing.Imaging;

namespace nJupiter.Web.UI {

	public class StreamImage : Page {

		#region IFileToStream Interface
		public interface IFileToStream {
			#region Properties
			string Name { get; }
			string MimeType { get; }
			bool Exists { get; }
			DateTime LastModified { get; }
			#endregion

			#region Methods
			Stream OpenStream();
			#endregion
		}
		#endregion

		#region Default IFileToStream Implementation
		private sealed class FileToStreamImpl : IFileToStream {
			#region Members
			private readonly VirtualFile virtualFile;
			private readonly FileInfo fileInfo;
			#endregion

			#region Properties
			private string Extension { get { return Path.GetExtension(this.Name); } }
			public string Name { get { return this.fileInfo != null ? this.fileInfo.Name : (this.virtualFile != null ? this.virtualFile.Name : null); } }
			public string MimeType { get { return "image/" + this.Extension.Substring(1); } }
			public bool Exists { get { return this.fileInfo != null ? this.fileInfo.Exists : this.virtualFile != null; } }

			public DateTime LastModified {
				get {
					if(this.fileInfo != null) {
						return this.fileInfo.LastAccessTimeUtc;
					}
					DateTime dateNow = DateTime.Now;
					return new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, dateNow.Hour, 0, 0);
				}
			}
			#endregion

			#region Constructors
			public FileToStreamImpl(string path) {
				path = HttpUtility.UrlDecode(path);
				string filePath = HttpContext.Current.Server.MapPath(path);
				if(File.Exists(filePath)) {
					this.fileInfo = new FileInfo(filePath);
				} else if(HostingEnvironment.VirtualPathProvider.FileExists(path)) {
					this.virtualFile = HostingEnvironment.VirtualPathProvider.GetFile(path);
				}
			}
			#endregion

			#region Methods
			public Stream OpenStream() {
				if(this.fileInfo != null) {
					return this.fileInfo.OpenRead();
				}
				return this.virtualFile.Open();
			}
			#endregion
		}
		#endregion

		#region Methods
		public virtual IFileToStream GetFileToStream(string path) {

			IFileToStream file = null;
			try {
				file = GetFileToStreamInternal(path);
			} catch(FileNotFoundException) { }
			if(file == null)
				file = new FileToStreamImpl(path);
			return file;
		}

		private static IFileToStream GetFileToStreamInternal(string path) {
			const string section = "fileToStream";
			const string assemblypath = "assemblyPath";
			const string assembly = "assembly";
			const string type = "type";

			Config config = ConfigHandler.GetConfig(true);
			if(config != null && config.ContainsKey(section)) {
				return (IFileToStream)GetInstance(
					config.GetValue(section, assemblypath),
					config.GetValue(section, assembly),
					config.GetValue(section, type),
					new object[] { path });
			}
			return null;
		}

		private static object GetInstance(string assemblyPath, string assemblyName, string typeName, object[] prams) {
			Assembly assembly;
			if(!string.IsNullOrEmpty(assemblyPath)) {
				assembly = Assembly.LoadFrom(assemblyPath);
			} else if(assemblyName == null || assemblyName.Length.Equals(0) ||
				Assembly.GetExecutingAssembly().GetName().Name.Equals(assemblyName)) {
				assembly = Assembly.GetExecutingAssembly();
				//Load current assembly
			} else {
				assembly = Assembly.Load(assemblyName);
				// Late binding to an assembly on disk (current directory)
			}
			return assembly.CreateInstance(
				typeName, false,
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly |
				BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.ExactBinding,
				null, prams, null, null);
		}
		#endregion

		#region Overridden Methods
		protected override void OnLoad(EventArgs e) {
			// See if client has a cached version of the image
			string ifModifiedSince = this.Request.Headers.Get("If-Modified-Since");

			string reqPath = this.Request.QueryString["path"];
			string path = reqPath ?? string.Empty;

			IFileToStream fileToStream = this.GetFileToStream(path);
			if(fileToStream.Exists) {
				//Get the last modified time for the current file
				//Handle the situation where we get a LastModified that is in the future
				DateTime now = DateTime.Now;
				DateTime lastModifiedTime = fileToStream.LastModified > now ? now : fileToStream.LastModified;

				//Check to see if it is a conditional HTTP GET.
				if(ifModifiedSince != null) {
					//This is a conditional HTTP GET request. Compare the strings.
					try {
						DateTime incrementalIndexTime = DateTime.Parse(ifModifiedSince, DateTimeFormatInfo.InvariantInfo).ToUniversalTime();
						// Has to do a string compare because of the resolution
						if(incrementalIndexTime.ToString(DateTimeFormatInfo.InvariantInfo) ==
							lastModifiedTime.ToString(DateTimeFormatInfo.InvariantInfo)) {
							// If the file has not been modifed, send a not changed status
							this.Response.StatusCode = 304;
							this.Response.End();
						}
					} catch(FormatException) {
					}
				}

				string reqWidth = Page.Request.QueryString["width"];
				string reqHeight = Page.Request.QueryString["height"];
				string reqAllowEnlarging = Page.Request.QueryString["allowEnlarging"];
				string reqAllowStretching = Page.Request.QueryString["allowStretching"];

				int width = 0;
				int height = 0;
				bool allowEnlarging = reqAllowEnlarging != null && string.Compare(reqAllowEnlarging, "true", true, CultureInfo.InvariantCulture) == 0;
				bool allowStretching = reqAllowStretching != null && string.Compare(reqAllowStretching, "true", true, CultureInfo.InvariantCulture) == 0;

				Config config = ConfigHandler.GetSystemConfig();
				SmoothingMode smoothingMode = SmoothingMode.Default;
				if(config.ContainsKey("imageScaleConfig", "smoothingMode")) {
					smoothingMode = (SmoothingMode)Enum.Parse(typeof(SmoothingMode), config.GetValue("imageScaleConfig", "smoothingMode"), true);
				}
				InterpolationMode interpolationMode = InterpolationMode.Default;
				if(config.ContainsKey("imageScaleConfig", "interpolationMode")) {
					interpolationMode = (InterpolationMode)Enum.Parse(typeof(InterpolationMode), config.GetValue("imageScaleConfig", "interpolationMode"), true);
				}
				PixelOffsetMode pixelOffsetMode = PixelOffsetMode.Default;
				if(config.ContainsKey("imageScaleConfig", "pixelOffsetMode")) {
					pixelOffsetMode = (PixelOffsetMode)Enum.Parse(typeof(PixelOffsetMode), config.GetValue("imageScaleConfig", "pixelOffsetMode"), true);
				}

				try {
					width = reqWidth == null ? width : int.Parse(reqWidth, NumberFormatInfo.InvariantInfo);
				} catch(FormatException) { }
				try {
					height = reqHeight == null ? height : int.Parse(reqHeight, NumberFormatInfo.InvariantInfo);
				} catch(FormatException) { }

				using(Stream fileStream = fileToStream.OpenStream()) {
					ImageScale.ResizeFlags resizeFlags = ImageScale.ResizeFlags.None;
					if(allowEnlarging) {
						resizeFlags = resizeFlags | ImageScale.ResizeFlags.AllowEnlarging;
					}
					if(allowStretching) {
						resizeFlags = resizeFlags | ImageScale.ResizeFlags.AllowStretching;
					}

					this.Response.Clear();
					try {
						ImageScale.Resize(fileStream, this.Response.OutputStream, width, height, resizeFlags, smoothingMode, interpolationMode, pixelOffsetMode);
						this.Response.AddHeader("Content-Disposition", "inline;filename=\"" + fileToStream.Name + "\"");
						this.Response.ContentType = fileToStream.MimeType;
						this.Response.Cache.SetLastModified(lastModifiedTime);
						//The following lines enable downlevel caching in server or browser cache. But not in proxies.
						this.Response.Cache.SetCacheability(HttpCacheability.Public);
						//Set the expiration time for the downlevel cache
						this.Response.Cache.SetExpires(DateTime.Now.AddMinutes(5));
						this.Response.Cache.SetValidUntilExpires(true);
						this.Response.Cache.VaryByParams["*"] = true;
					} catch(OutOfMemoryException ex) {
						// If not image
						throw new FileNotFoundException(ex.Message, ex);
					} catch(FileNotFoundException) {
						// If file not found
						// We know that the file exists so we throw a forbidden request exception
						throw new HttpException(403, "Forbidden");
					}
				}
			} else {
				this.Response.Clear();
				throw new HttpException(404, "Not Found");
			}
		}
		#endregion
	}
}
