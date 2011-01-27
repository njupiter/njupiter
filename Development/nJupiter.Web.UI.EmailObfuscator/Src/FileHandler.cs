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
using System.Web;
using System.Text;

namespace nJupiter.Web.UI.EmailObfuscator {

	public sealed class FileHandler : IHttpHandler {

		public void ProcessRequest(HttpContext context) {
			if(context == null)
				throw new ArgumentNullException("context");
			string encstr = context.Request["encstr"];
			//string encimage	= context.Request["encimage"];
			//string encanchor	= context.Request["encanchor"];

			if(encstr != null) {
				// See if client has a cached version of the image
				string ifModifiedSince = context.Request.Headers.Get("If-Modified-Since");
				// If so, always let the client use the cached version cause this image shall not change ever.
				if(ifModifiedSince != null) {
					context.Response.StatusCode = 304;
					context.Response.End();
				}
				using(EmailImage image = new EmailImage()) {
					image.Email = Encoding.UTF8.GetString(Convert.FromBase64String(encstr));
					image.RenderImage(context.Response.OutputStream); //Render image to output stream
				}
				context.Response.ContentType = "image/gif";
				context.Response.StatusCode = 200;
				context.Response.Cache.SetLastModified(DateTime.Now);
				context.Response.Cache.SetCacheability(HttpCacheability.Public);
				context.Response.Cache.SetValidUntilExpires(true);
				context.Response.Cache.VaryByParams["*"] = true;
			} else {
				context.Response.StatusCode = 404;
			}
		}

		bool System.Web.IHttpHandler.IsReusable {
			get {
				return true;
			}
		}

	}
}

