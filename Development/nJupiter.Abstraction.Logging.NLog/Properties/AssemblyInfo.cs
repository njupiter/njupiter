using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyDescription("NLog implementation for nJupiter.Abstraction.Logging")]

// Increment AssemblyVersion only on major and minor releases
[assembly: AssemblyVersion("1.0.0.370")]
// Increment AssemblyFileVersion on every release
[assembly: AssemblyFileVersion("1.0.0.370")]

#if DEBUG
[assembly: InternalsVisibleTo("nJupiter.Abstraction.Logging.Tests.Unit")]
#endif