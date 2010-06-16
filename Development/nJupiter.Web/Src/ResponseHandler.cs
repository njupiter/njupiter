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
using System.Web;

namespace nJupiter.Web {

	public static class ResponseHandler {
		#region Constants
		private const string HtmlMimeType = "text/html";
		private const string XhtmlMimeType = "application/xhtml+xml";
		#endregion

		#region Constructors
		#endregion

		#region Static Methods
		public static void Redirect(string url) {
			Redirect(url, false);
		}
		public static void Redirect(string url, bool permanently) {
			Redirect(url, permanently, true);
		}
		public static void Redirect(string url, bool permanently, bool endResponse) {
			if(url == null)
				throw new ArgumentNullException("url");
			if(HttpContext.Current != null) {
				HttpContext.Current.Response.Redirect(url, false);
				if(permanently) {
					// http://www.faqs.org/rfcs/rfc2616.html
					HttpContext.Current.Response.StatusCode = 301;
					HttpContext.Current.Response.StatusDescription = "Moved Permanently";
				}
				if(endResponse) {
					HttpContext.Current.Response.End();
				}
			}
		}

		public static void PerformXhtmlContentNegotiation() {
			if(HttpContext.Current.Response.ContentType != XhtmlMimeType) {
				MimeTypeCollection mtc = RequestHandler.GetAcceptedTypes();
				MimeType xhtml = mtc.GetHighestQuality(XhtmlMimeType);
				MimeType html = mtc.GetHighestQuality(HtmlMimeType);
				if(xhtml != null && xhtml.Quality != 0 && (html == null || xhtml.Quality >= html.Quality)) {
					HttpContext.Current.Response.ContentType = XhtmlMimeType;
				}
			}
		}
		#endregion
	}

}
