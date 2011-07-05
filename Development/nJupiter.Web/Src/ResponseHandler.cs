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

namespace nJupiter.Web {
	public class ResponseHandler : IResponseHandler {
		
		private readonly HttpContextBase context;
		private readonly IMimeTypeHandler mimeTypeHandler;
		private readonly IMimeType htmlMimeType = new MimeType("text/html");
		private readonly IMimeType xhtmlMimeType = new MimeType("application/xhtml+xml");

		private HttpContextBase CurrentContext { get { return context ?? new HttpContextWrapper(HttpContext.Current); } }

		public ResponseHandler(IMimeTypeHandler mimeTypeHandler, HttpContextBase context) {
			this.mimeTypeHandler = mimeTypeHandler;
			this.context = context;
		}

		public void Redirect(string url) {
			Redirect(url, false);
		}
		
		public void Redirect(string url, bool permanently) {
			Redirect(url, permanently, true);
		}
		
		public void Redirect(string url, bool permanently, bool endResponse) {
			if(url == null)
				throw new ArgumentNullException("url");
			if(CurrentContext != null) {
				CurrentContext.Response.Redirect(url, false);
				if(permanently) {
					// http://www.faqs.org/rfcs/rfc2616.html
					CurrentContext.Response.StatusCode = 301;
					CurrentContext.Response.StatusDescription = "Moved Permanently";
				}
				if(endResponse) {
					CurrentContext.Response.End();
				}
			}
		}

		public void PerformXhtmlContentNegotiation() {
			if(CurrentContext.Response.ContentType != this.xhtmlMimeType.ContentType) {
				var xhtml = mimeTypeHandler.GetHighestQuality(this.xhtmlMimeType);
				var html = mimeTypeHandler.GetHighestQuality(this.htmlMimeType);
				if(xhtml != null && xhtml.Quality != 0 && (html == null || xhtml.Quality >= html.Quality)) {
					CurrentContext.Response.ContentType = this.xhtmlMimeType.ContentType;
				}
			}
		}

		/// <summary>
		/// Returns the default instance of IResponseHandler
		/// </summary>
		public static IResponseHandler Instance { get { return NestedSingleton.instance; } }

		// thread safe Singleton implementation with fully lazy instantiation and with full performance
		private sealed class NestedSingleton {
			// ReSharper disable EmptyConstructor
			static NestedSingleton() {} // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
			// ReSharper restore EmptyConstructor
			internal static readonly IResponseHandler instance = new ResponseHandler(MimeTypeHandler.Instance, null);
		}
	}

}
