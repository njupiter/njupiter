using System;
using System.DirectoryServices;

namespace nJupiter.DataAccess.Ldap.Configuration {
	internal interface IServerConfig {
		Uri Url { get; }
		string Username { get; }
		string Password { get; }
		AuthenticationTypes AuthenticationTypes { get; }
		TimeSpan TimeLimit { get; }
		int PageSize { get; }
		bool AllowWildcardSearch { get; }
		bool PropertySortingSupport { get; }
		bool RangeRetrievalSupport { get; }
	}
}