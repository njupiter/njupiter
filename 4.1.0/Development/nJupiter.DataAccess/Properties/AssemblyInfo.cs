using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyDescription("Data Access Component")]

// Increment AssemblyVersion only on major and minor releases
[assembly: AssemblyVersion("4.0.1.308")]
// Increment AssemblyFileVersion on every release
[assembly: AssemblyFileVersion("4.1.0.516")]

#if DEBUG
[assembly: InternalsVisibleTo("nJupiter.DataAccess.Tests.Unit")]
[assembly: InternalsVisibleTo("nJupiter.DataAccess.Tests.Integration")]
#endif