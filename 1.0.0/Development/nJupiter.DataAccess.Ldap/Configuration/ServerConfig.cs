using System;
using System.DirectoryServices;

namespace nJupiter.DataAccess.Ldap.Configuration {
	internal class ServerConfig : IServerConfig {
		public Uri Url { get; internal set; }
		public string Username { get; internal set; }
		public string Password { get; internal set; }
		public AuthenticationTypes AuthenticationTypes { get; internal set; }
		public TimeSpan TimeLimit { get; internal set; }
		public int PageSize { get; internal set; }
		public bool AllowWildcardSearch { get; internal set; }
		public bool PropertySortingSupport { get; internal set; }
		public bool RangeRetrievalSupport { get; internal set; }
	}
}