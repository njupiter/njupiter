using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyDescription("Contains Control Adapters for striping ugly content that ASP.NET is generating by default (for example unnecessary id-attributes), also contains a set o web controls that either is not represented in ASP.NET or that extend the functionality of the native controls. The component also have extended functionality for registering scripts to your page and to find controls by type rather than name and also a set of string constants that contains the most commonly used HTML entities.")]

// Increment AssemblyVersion only on major and minor releases
[assembly: AssemblyVersion("4.0.1.308")]
// Increment AssemblyFileVersion on every release
[assembly: AssemblyFileVersion("4.4.6.542")]
[assembly: AssemblyInformationalVersion("4.4.6.542")]

#if DEBUG
[assembly: InternalsVisibleTo("nJupiter.Web.UI.Tests")]
#endif