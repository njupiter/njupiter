nJupiter - a component toolkit for development based on .NET
============================================================

nJupiter is a component toolkit that contains a large amount of
functionality across a wide spectrum. nJupiter offering a set of tools
that you can use either independently or together with each other in
your own projects. nJupiter has been under development since 2005 and is
today considered to be a mature project. nJupiter is completely written
in C \# and is released under a MIT / X11 license. Some of the
components in the project was developed in Microsoft .NET 1.1 but was
later updated to .NET 2.0 and 3.5 and today the project is .NET 3.5+
only.

### [You can find nJupiter releases on NuGet](https://nuget.org/packages?q=njupiter)

-   Please do not send private mails with questions or issues, please do
    this at either [Stack Overflow](http://stackoverflow.com/) or in the
    [Issue Tracker](https://github.com/njupiter/njupiter/issues), in
    this way your question and its answer can help other people with the
    same problems.\*

News
----

### 16 march 2015: Moved to GitHub

Since Google Code is about to close down we have now moved to [GitHub](https://github.com/njupiter/njupiter)

### 4 december 2012: All NuGet packages now contain builds for .NET 4.0

All NuGet-packages do now contain specific builds for both .NET 2.0/3.5
and .NET 4.0. Some minor fixes have also been done to some of the
components, please refere to the individual project changelogs for more
information.

### 30 august 2012: nJupiter.DataAccess.Ldap 5.1 released

Support for serverless binding added. Set the url element in the
configuration to just “<LDAP://>” to activate. [Read more about nJupiter.DataAccess.Ldap here](https://github.com/njupiter/njupiter/wiki/nJupiterDataAccessLdap).

### 27 june 2012: nJupiter.DataAccess.Ldap 5.0 released

The Ldap MembershipProvider and RoleProvider in nJupiter.DataAccess.Ldap
now supports paged loading via Virtual List View for LDAP servers that
supports it. The RoleProvider shall now also support servers that do not
cross refer members in both the group entry and user entry but only
store membership in groups, for example Oracle Internet Directory.
nJupiter.DataAccess.Ldap now also refer to nJupiter.Abstraction.Logging
which means that if you need to log why users can not be validated you
can now choose to log this to the logging framework of your own choice.
Facades and abstractions for the System.DirectoryServices namespace has
also been made public so you can use them if you need to do own
operations on the LDAP server which can not be done on the providers
directly. The Config objects are also now public, Besides that the
component has been completely refactored and cleaned up to make it
easier to add new functionality to it in the future.
[Read more about nJupiter.DataAccess.Ldap here](https://github.com/njupiter/njupiter/wiki/nJupiterDataAccessLdap).

### 8 june 2012: nJupiter.Abstraction.Logging released

nJupiter.Abstraction.Logging is an abstraction library which makes it
easy to change between different logging frameworks in runtime.
nJupiter.Abstraction.Logging currently have implementations for NLog,
log4net (both the old and the new public key token) and Enterprise
Library Logging Application Block. The goal with
nJupiter.Abstraction.Logging is simplicity and the component uses the
logger pattern known from log4xxx. nJupiter.Abstraction.Logging is also
designed to be easy to use together with IoC-container where you can
registered generically typed loggers that you easily can use via
constructor injection. [Read more about it
here](https://github.com/njupiter/njupiter/wiki/nJupiterAbstractionLogging).

Functionality
-------------

nJupiter today contains about 10 components and handles functionality
for:

-   [Configuration](https://github.com/njupiter/njupiter/wiki/nJupiterConfiguration)
-   [Logging Abstractions](https://github.com/njupiter/njupiter/wiki/nJupiterAbstractionLogging).
-   [Generic LDAP MembershipProvider and RoleProvider](https://github.com/njupiter/njupiter/wiki/nJupiterDataAccessLdap)
-   [User Management](https://github.com/njupiter/njupiter/wiki/nJupiterDataAccessUsers)
-   [Syndication](https://github.com/njupiter/njupiter/wiki/nJupiterWebSyndication)
-   [E-mail Obfuscation Filter for HTML-pages](https://github.com/njupiter/njupiter/wiki/nJupiterWebUIEmailObfuscator)
-   [Data Access](https://github.com/njupiter/njupiter/wiki/nJupiterDataAccess)
-   [Mail](https://github.com/njupiter/njupiter/wiki/nJupiterNetMail)
-   [Control Adapters for cleaner output](https://github.com/njupiter/njupiter/wiki/nJupiterWebUI)
-   [W3C compatible web components](https://github.com/njupiter/njupiter/wiki/nJupiterWebUI)
-   [Globalization](https://github.com/njupiter/njupiter/wiki/nJupiterGlobalization)
-   [...and a lot more](https://github.com/njupiter/njupiter/wiki)

See the [wiki](https://github.com/njupiter/njupiter/wiki) for a more detailed overview of the functionality and all the components
in nJupiter.

Contribute
----------

If you want to contribute to this project, please feel free to contact
one of the project owners.
