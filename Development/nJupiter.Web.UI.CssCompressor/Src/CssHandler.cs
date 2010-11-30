#region Copyright & License
/*
	Copyright (c) 2005-2010 nJupiter

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
using System.IO;
using System.Web;
using System.Collections;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Web.Hosting;

using nJupiter.Configuration;

namespace nJupiter.Web.UI.CssCompressor {

	public sealed class CssHandler : IHttpHandler {

		#region Members
		private static readonly Regex CommentsRegex = new Regex("/\\*.*?\\*/");
		private static readonly Regex ImportsRegex = new Regex("^*@import\\surl\\((.*)\\).*$", RegexOptions.Multiline);
		private HttpContext httpContext;
		private ArrayList parsedFiles;
		#endregion

		public void ProcessRequest(HttpContext context) {
			if(context == null)
				throw new ArgumentNullException("context");

			this.httpContext = context;

			string path = this.httpContext.Server.MapPath(this.httpContext.Request.Url.AbsolutePath);

			Config config = ConfigHandler.GetConfig();
			int minutesToCache = 0;
			if(config.ContainsKey("cssCompressor/minutesToCache")) {
				minutesToCache = config.GetIntValue("cssCompressor/minutesToCache");
			}

			if(minutesToCache > 0) {
				// Check if the client has this file and return 304 if it does
				string ifModifiedSince = context.Request.Headers.Get("If-Modified-Since");
				if(ifModifiedSince != null && File.Exists(path)) {
					DateTime ifModifiedSinceDate;
					if(DateTime.TryParse(ifModifiedSince, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out ifModifiedSinceDate)) {
						if(!ifModifiedSinceDate.Equals(DateTime.MinValue) && ifModifiedSinceDate >= File.GetLastWriteTime(path)) {
							this.httpContext.Response.StatusCode = 304;
							this.httpContext.Response.End();
						}
					}
				}
			}
			// Check if the master file exists. All inner files not found will just be ignored in the GetCss method
			this.parsedFiles = new ArrayList();
			string css = this.GetCss(this.httpContext.Request.Url.PathAndQuery);
			if(css != null) {

				this.httpContext.Response.Write(css);

				this.httpContext.Response.ContentType = "text/css";
				this.httpContext.Response.StatusCode = 200;
				this.httpContext.Response.Cache.SetLastModified(File.Exists(path) ? File.GetLastWriteTime(path) : DateTime.Now);
				this.httpContext.Response.Cache.SetCacheability(HttpCacheability.Public);

				if(minutesToCache > 0) {
					this.httpContext.Response.Cache.SetExpires(DateTime.Now.AddMinutes(minutesToCache));
					this.httpContext.Response.Cache.SetValidUntilExpires(true);
				}
			} else {
				this.httpContext.Response.StatusCode = 404;
			}
		}

		/// <summary>
		/// Returns the css from the specified file, and all files imported with the css directive @import
		/// </summary>
		/// <param name="cssFileUrl">The css file url</param>
		/// <returns>A string containing all css from the specified file</returns>
		private string GetCss(string cssFileUrl) {
			if(cssFileUrl == null) {
				return null;
			}
			cssFileUrl = HttpUtility.UrlDecode(cssFileUrl);
			if(this.parsedFiles.Contains(cssFileUrl)) {
				return null;
			}
			Uri uri = new Uri(this.httpContext.Request.Url, cssFileUrl);
			string filePath = HttpContext.Current.Server.MapPath(uri.AbsolutePath);
			string css = null;
			if(File.Exists(filePath)) {
				css = File.ReadAllText(filePath);
			} else if(HostingEnvironment.VirtualPathProvider.FileExists(cssFileUrl)) {
				VirtualFile virtualFile = HostingEnvironment.VirtualPathProvider.GetFile(cssFileUrl);
				using(Stream virtualFileStream = virtualFile.Open()) {
					StreamReader sr = new StreamReader(virtualFileStream);
					css = sr.ReadToEnd();
				}
			}
			if(css != null) {
				this.parsedFiles.Add(cssFileUrl);
				css = CommentsRegex.Replace(css, " ");

				MatchCollection matches = ImportsRegex.Matches(css);
				if(matches.Count > 0) {
					foreach(Match match in matches) {
						if(match.Groups.Count >= 2) {
							string relativeUrl = match.Groups[1].Value;
							string importCss = this.GetCss(this.GetAbsoluteUrl(cssFileUrl, relativeUrl));
							if(importCss != null) {
								css = css.Replace(match.Value, importCss);
							}
						}
					}
				}
				return css;
			}
			return null;
		}

		/// <summary>
		/// Converts a relative url to an absolute url
		/// </summary>
		/// <param name="sourceUrl">Base absolute url</param>
		/// <param name="relativeUrl">Url relative to base url</param>
		/// <returns>An absolute url</returns>
		private string GetAbsoluteUrl(string sourceUrl, string relativeUrl) {
			sourceUrl = this.httpContext.Request.Url.Scheme + "://" + this.httpContext.Request.Url.Host + System.Web.VirtualPathUtility.GetDirectory(sourceUrl);
			Uri sourceUri = new Uri(sourceUrl);
			Uri combinedUri = new Uri(sourceUri, relativeUrl);
			return combinedUri.PathAndQuery;
		}

		bool System.Web.IHttpHandler.IsReusable {
			get {
				return true;
			}
		}

	}
}