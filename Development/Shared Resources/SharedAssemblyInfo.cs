using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]
[assembly: AssemblyCompany("nJupiter")]
[assembly: AssemblyProduct("nJupiter")]
[assembly: AssemblyCopyright("\x00a9 2005-2010 nJupiter. Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php")]
[assembly: AssemblyTrademark("\x00a9 2005-2010 nJupiter. Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php")]
[assembly: AssemblyCulture("")]

[assembly: SecurityTransparent]

[assembly: AssemblyDelaySign(false)]
#if DEBUG
[assembly: AssemblyKeyFile("")]
[assembly: AssemblyVersion("3.15.1.*")]
[assembly: AssemblyConfiguration("Debug")]
#else
// Increment AssemblyVersion only on major and minor releases
[assembly: AssemblyVersion("3.15.1.0")]
// Increment AssemblyFileVersion on every release
[assembly: AssemblyFileVersion("3.15.1.0")]
[assembly: AssemblyConfiguration("Release")]
#if SIGN
[assembly: AssemblyKeyFile(@"C:\Projects\nJupiter\Development\nJupiter.snk")]
#else
[assembly: AssemblyKeyFile("")]
#endif
#endif
[assembly: AssemblyKeyName("")]
[assembly: PermissionSet(SecurityAction.RequestMinimum, Name="Nothing")]

[assembly: AssemblyTitle(""

#if SIGN
+ "Signed "
#endif

#if DEBUG
+ "Debug"
#else
+ "Release"
#endif

+ " Build for .NET 2.0"

)]