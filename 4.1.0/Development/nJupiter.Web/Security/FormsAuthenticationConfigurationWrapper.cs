#region Copyright & License
/*
	Copyright (c) 2005-2012 nJupiter

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
using System.Configuration;
using System.Web;
using System.Web.Configuration;

namespace nJupiter.Web.Security {
	public class FormsAuthenticationConfigurationWrapper : IFormsAuthenticationConfiguration {
		private readonly FormsAuthenticationConfiguration configuration;

		public FormsAuthenticationConfigurationWrapper(FormsAuthenticationConfiguration configuration) {
			this.configuration = configuration;
		}

		public HttpCookieMode Cookieless { get { return configuration.Cookieless; }
			set { configuration.Cookieless = value; } }

		public FormsAuthenticationCredentials Credentials {
			get { return configuration.Credentials; } }

		public string DefaultUrl { get { return configuration.DefaultUrl; }
			set { configuration.DefaultUrl = value; } }

		public string Domain { get { return configuration.Domain; }
			set { configuration.Domain = value; } }

		public ElementInformation ElementInformation {
			get { return configuration.ElementInformation; } }

		public bool EnableCrossAppRedirects { get { return configuration.EnableCrossAppRedirects; }
			set { configuration.EnableCrossAppRedirects = value; } }

		public bool IsReadOnly() {
			return configuration.IsReadOnly();
		}

		public ConfigurationLockCollection LockAllAttributesExcept {
			get { return configuration.LockAllAttributesExcept; } }

		public ConfigurationLockCollection LockAllElementsExcept {
			get { return configuration.LockAllElementsExcept; } }

		public ConfigurationLockCollection LockAttributes {
			get { return configuration.LockAttributes; } }

		public ConfigurationLockCollection LockElements {
			get { return configuration.LockElements; } }

		public bool LockItem { get { return configuration.LockItem; }
			set { configuration.LockItem = value; } }

		public string LoginUrl { get { return configuration.LoginUrl; }
			set { configuration.LoginUrl = value; } }

		public string Name { get { return configuration.Name; }
			set { configuration.Name = value; } }

		public string Path { get { return configuration.Path; }
			set { configuration.Path = value; } }

		public FormsProtectionEnum Protection { get { return configuration.Protection; }
			set { configuration.Protection = value; } }

		public bool RequireSSL { get { return configuration.RequireSSL; }
			set { configuration.RequireSSL = value; } }

		public bool SlidingExpiration { get { return configuration.SlidingExpiration; }
			set { configuration.SlidingExpiration = value; } }

		public TimeSpan Timeout { get { return configuration.Timeout; }
			set { configuration.Timeout = value; } }
	}
}