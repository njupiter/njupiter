using System;

namespace nJupiter.DataAccess.Ldap.NameParser {
	[Flags]
	internal enum EscapeChars {
		None = 0,
		ControlChars = 1,
		SpecialChars = 2,
		MultibyteChars = 4
	}
}