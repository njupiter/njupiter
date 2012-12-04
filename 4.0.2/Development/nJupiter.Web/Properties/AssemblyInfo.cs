using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyDescription("nJupiter Web Component")]

// Increment AssemblyVersion only on major and minor releases
[assembly: AssemblyVersion("4.0.1.308")]
// Increment AssemblyFileVersion on every release
[assembly: AssemblyFileVersion("4.4.4.516")]

#if DEBUG
[assembly: InternalsVisibleTo("nJupiter.Web.Tests.Unit")]
#endif