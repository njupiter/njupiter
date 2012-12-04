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

namespace nJupiter.Web.UI.EmailObfuscator {

	public sealed class EmailObfuscatorModule : IHttpModule {

		private const string ObfuscationDisabledKey = "NJUPITER_EMAIL_OBFUSCATION_DISABLED";

		static void ReleaseRequestState(object sender, EventArgs e) {
			var response = HttpContext.Current.Response;
			if(response.ContentType.Contains("html") && !DisableObfuscationForCurrentRequest) {
				response.Filter = new EmailObfuscatorFilter(response.Filter);
			}
		}

		public static bool DisableObfuscationForCurrentRequest {
			get {
				return HttpContext.Current.Items[ObfuscationDisabledKey] != null;
			}
			set {
				HttpContext.Current.Items[ObfuscationDisabledKey] = (!value ? null : (object)true);
			}
		}

		void IHttpModule.Init(HttpApplication context) {
			context.ReleaseRequestState += ReleaseRequestState;
			ResourceRegistrator.Register();
		}

		void IHttpModule.Dispose() {
		}
	}
}
