using System;
using System.Collections.Generic;

namespace nJupiter.DataAccess.Ldap.DirectoryServices {
	internal interface IEntryCollection : IEnumerable<IEntry>, IDisposable {}
}