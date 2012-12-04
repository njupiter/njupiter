using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyDescription("Abstraction Layer for Logging")]

// Increment AssemblyVersion only on major and minor releases
[assembly: AssemblyVersion("1.0.0.377")]
// Increment AssemblyFileVersion on every release
[assembly: AssemblyFileVersion("1.2.0.516")]

#if DEBUG
[assembly: InternalsVisibleTo("nJupiter.Abstraction.Logging.Tests.Unit")]
#endif