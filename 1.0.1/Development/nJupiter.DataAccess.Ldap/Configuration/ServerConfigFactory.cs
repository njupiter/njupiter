using System;
using System.DirectoryServices;

using nJupiter.Configuration;

namespace nJupiter.DataAccess.Ldap.Configuration {
	internal class ServerConfigFactory : IServerConfigFactory {
		public IServerConfig Create(IConfig configSection) {
			var server = new ServerConfig();
			if(configSection == null) {
				return server;
			}

			server.Url = new Uri(configSection.GetValue("url"));

			if(configSection.ContainsKey("username")) {
				server.Username = configSection.GetValue("username");
			}

			if(configSection.ContainsKey("password")) {
				server.Password = configSection.GetValue("password");
			}

			if(configSection.ContainsKey("allowWildcardSearch")) {
				server.AllowWildcardSearch = configSection.GetValue<bool>("allowWildcardSearch");
			}

			if(configSection.ContainsKey("timeLimit")) {
				var timeLimit = configSection.GetValue<int>("timeLimit");
				server.TimeLimit = TimeSpan.FromSeconds(timeLimit);
			} else {
				server.TimeLimit = TimeSpan.FromSeconds(30);
			}

			if(configSection.ContainsKey("pageSize")) {
				var pageSize = configSection.GetValue<int>("pageSize");
				server.PageSize = pageSize;
			} else {
				server.PageSize = 0;
			}

			if(configSection.ContainsKey("authenticationTypes")) {
				var authenticationTypes = configSection.GetValueArray("authenticationTypes", "authenticationType");
				foreach(var authenticationType in authenticationTypes) {
					server.AuthenticationTypes |=
						(AuthenticationTypes)Enum.Parse(typeof(AuthenticationTypes), authenticationType, true);
				}
			} else {
				server.AuthenticationTypes = AuthenticationTypes.None;
			}

			if(configSection.ContainsKey("rangeRetrievalSupport")) {
				server.RangeRetrievalSupport = configSection.GetValue<bool>("rangeRetrievalSupport");
			} else {
				server.RangeRetrievalSupport = true;
			}

			if(configSection.ContainsKey("propertySortingSupport")) {
				server.PropertySortingSupport = configSection.GetValue<bool>("propertySortingSupport");
			} else {
				server.PropertySortingSupport = true;
			}

			return server;
		}
	}
}