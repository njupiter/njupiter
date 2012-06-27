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
using System.DirectoryServices;

using nJupiter.Configuration;

namespace nJupiter.DataAccess.Ldap.Configuration {
	internal class ServerConfigFactory : IServerConfigFactory {
		public IServerConfig Create(IConfig configSection) {
			var server = new ServerConfig();
			if(configSection == null) {
				throw new ArgumentNullException("configSection");
			}

			SetUrl(configSection, server);
			SetUsername(configSection, server);
			SetPassword(configSection, server);
			SetAllowWildcardSearch(configSection, server);
			SetTimeLimit(configSection, server); 
			SetPageSize(configSection, server);
			SetSizeLimit(configSection, server);
			SetRangeRetrievalSupport(configSection, server);
			SetPropertySortingSupport(configSection, server);
			SetVirtualListViewSupport(configSection, server);
			SetAuthenticationTypes(configSection, server);

			return server;
		}

		private static void SetUrl(IConfig configSection, ServerConfig server) {
			server.Url = new Uri(configSection.GetValue("url"));
		}

		private static void SetUsername(IConfig configSection, ServerConfig server) {
			if(configSection.ContainsKey("username")) {
				server.Username = configSection.GetValue("username");
			}
		}

		private static void SetPassword(IConfig configSection, ServerConfig server) {
			if(configSection.ContainsKey("password")) {
				server.Password = configSection.GetValue("password");
			}
		}

		private static void SetAllowWildcardSearch(IConfig configSection, ServerConfig server) {
			if(configSection.ContainsKey("allowWildcardSearch")) {
				server.AllowWildcardSearch = configSection.GetValue<bool>("allowWildcardSearch");
			}
		}

		private static void SetTimeLimit(IConfig configSection, ServerConfig server) {
			if(configSection.ContainsKey("timeLimit")) {
				var timeLimit = configSection.GetValue<int>("timeLimit");
				server.TimeLimit = TimeSpan.FromSeconds(timeLimit);
			}
		}

		private static void SetPageSize(IConfig configSection, ServerConfig server) {
			if(configSection.ContainsKey("pageSize")) {
				var pageSize = configSection.GetValue<int>("pageSize");
				server.PageSize = pageSize;
			}
		}

		private static void SetSizeLimit(IConfig configSection, ServerConfig server) {
			if(configSection.ContainsKey("sizeLimit")) {
				var sizeLimit = configSection.GetValue<int>("sizeLimit");
				server.SizeLimit = sizeLimit;
			}
		}

		private static void SetRangeRetrievalSupport(IConfig configSection, ServerConfig server) {
			if(configSection.ContainsKey("rangeRetrievalSupport")) {
				server.RangeRetrievalSupport = configSection.GetValue<bool>("rangeRetrievalSupport");
			}
		}

		private static void SetPropertySortingSupport(IConfig configSection, ServerConfig server) {
			if(configSection.ContainsKey("propertySortingSupport")) {
				server.PropertySortingSupport = configSection.GetValue<bool>("propertySortingSupport");
			}
		}

		private static void SetVirtualListViewSupport(IConfig configSection, ServerConfig server) {
			if(configSection.ContainsKey("virtualListViewSupport")) {
				server.VirtualListViewSupport = configSection.GetValue<bool>("virtualListViewSupport");
			}
		}

		private static void SetAuthenticationTypes(IConfig configSection, ServerConfig server) {
			if(configSection.ContainsKey("authenticationTypes")) {
				var authenticationTypes = configSection.GetValueArray("authenticationTypes", "authenticationType");
				foreach(var authenticationType in authenticationTypes) {
					server.AuthenticationTypes |=
						(AuthenticationTypes)Enum.Parse(typeof(AuthenticationTypes), authenticationType, true);
				}
			}
		}
	}
}