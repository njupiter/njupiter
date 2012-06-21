using System;

namespace nJupiter.DataAccess.Ldap.DistinguishedNames {
	[Flags]
	internal enum EscapeChars {
		None = 0,
		ControlChars = 1,
		SpecialChars = 2,
		MultibyteChars = 4
	}
}