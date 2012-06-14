using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyDescription("nJupiter.DataAccess.Ldap - containting generic LDAP MembershipProvider and RoleProvider")]

// Increment AssemblyVersion only on major and minor releases
[assembly: AssemblyVersion("4.0.2.322")]
// Increment AssemblyFileVersion on every release
[assembly: AssemblyFileVersion("4.0.2.322")]

#if DEBUG
[assembly: InternalsVisibleTo("nJupiter.DataAccess.Ldap.Tests.Unit")]
#endif