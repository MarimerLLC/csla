# CSLA 6 releases

CSLA 6 is a major new version of CSLA .NET, fully supporting (and requiring) dependency injection and other modern features of modern .NET.

This version supports 

* .NET 6 
* .NET 5
* NetStandard 2.0
* .NET Framework 4.6.2 and higher
* Blazor (all modern browsers)
* Xamarin (iOS, Android, more)
* Windows
* Linux
* Mac
* Kubernetes and other container-based runtimes

# CSLA .NET version 6.0.0 release

This is a major release with numerous breaking changes, including:

* Business domain types must have a public constructor
* Public constructors for most types will have parameters provided via depedency injection
* Support for .NET 4.0 and 4.5 has been dropped; .NET 4.6.2 is the minimum required

## Change List

[Changes in this release](https://github.com/MarimerLLC/csla/issues?q=is%3Aissue+project%3AMarimerLLC%2Fcsla%2F11+is%3Aclosed)

### Supported Platforms and Features

* [#2120](https://github.com/MarimerLLC/csla/issues/2120) 🎉 Add .NET 6 support
* [#1362](https://github.com/MarimerLLC/csla/issues/1362) 🛑 Remove EF 5 support
* [#1315](https://github.com/MarimerLLC/csla/issues/1315) 🛑 Remove MVC 4 support
* [#1314](https://github.com/MarimerLLC/csla/issues/1314) 🛑 Remove EF 4 support
* [#1313](https://github.com/MarimerLLC/csla/issues/1313) 🛑 Remove .NET 4.0, .NET 4.5, .NET 4.6.1 support; add .NET 4.6.2 support

### Dependency Injection

* [#1738](https://github.com/MarimerLLC/csla/issues/1738) 🛑🎉 Support and require DI throughout CSLA

### Data Portal

* [#1994](https://github.com/MarimerLLC/csla/issues/1994) 🐜 Support factory methods in base classes

### Blazor

* [#1974](https://github.com/MarimerLLC/csla/issues/1974) 🐜 Fix formatting of ErrorText

### Misc

* [#1743](https://github.com/MarimerLLC/csla/issues/1743) 🛑🎉 Use only `ClaimsPrincipal` and `ClaimsIdentity`, as these are the only types supported in modern .NET

### NuGet dependencies

## Contributors

* [@joshhanson314](https://github.com/joshhanson314) DbContext resolution solution
* [@j055](https://github.com/j055) Authentication policy fix
* [@dazinator](https://github.com/dazinator) DI enhancements
* [@adrianwright109](https://github.com/adrianwright109) Numerous Blazor fixes and enhancements
* [@rockfordlhotka](https://github.com/rockfordlhotka) Enhancements
