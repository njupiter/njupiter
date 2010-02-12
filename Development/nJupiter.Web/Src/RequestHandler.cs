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

using System.Web;

namespace nJupiter.Web {

	public static class RequestHandler {
		/// <summary>
		/// Get accepted types for the current http-context implemented after RFC2616
		/// </summary>
		/// <returns>A collection of accepted types</returns>
		/// <seealso cref="http://www.faqs.org/rfcs/rfc2616.html"/>
		public static MimeTypeCollection GetAcceptedTypes() {
			if(HttpContext.Current != null)
				return GetAcceptedTypes(HttpContext.Current);
			return new MimeTypeCollection();
		}

		/// <summary>
		/// Get accepted types for a specific http-contect implemented after RFC2616
		/// </summary>
		/// <param name="httpContext">The specified http-context</param>
		/// <returns>A collection of accepted types</returns>
		/// <seealso cref="http://www.faqs.org/rfcs/rfc2616.html"/>
		public static MimeTypeCollection GetAcceptedTypes(HttpContext httpContext) {
			MimeTypeCollection acceptedTypes = new MimeTypeCollection();
			if(httpContext != null && httpContext.Request != null && httpContext.Request.AcceptTypes != null) {
				foreach(string acceptedType in httpContext.Request.AcceptTypes) {
					acceptedTypes.Add(new MimeType(acceptedType));
				}
			}
			return acceptedTypes;
		}
	}

}
