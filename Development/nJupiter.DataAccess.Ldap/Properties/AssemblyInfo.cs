using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyDescription("nJupiter.DataAccess.Ldap - containting generic LDAP MembershipProvider and RoleProvider")]

// Increment AssemblyVersion only on major and minor releases
[assembly: AssemblyVersion("5.0.0.424")]
// Increment AssemblyFileVersion on every release
[assembly: AssemblyFileVersion("5.0.0.424")]

#if DEBUG
[assembly: InternalsVisibleTo("nJupiter.DataAccess.Ldap.Tests.Unit")]
// FakeItEasy
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
#endif