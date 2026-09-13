# CSLA 11 releases

CSLA 11 is the next major version of CSLA .NET, currently in development on the `main` branch.

Release notes for CSLA 10 are maintained on the [v10.x branch](https://github.com/MarimerLLC/csla/blob/v10.x/releasenotes.md), which is the maintenance branch for version 10.

## CSLA .NET version 11.0.0 release

Primary changes in this release include:

* Add support for .NET 11 ([#4699](https://github.com/MarimerLLC/csla/issues/4699))
* Remove support for .NET 8 and .NET 9, which Microsoft stops supporting in November 2026 ([#4348](https://github.com/MarimerLLC/csla/issues/4348), [#4581](https://github.com/MarimerLLC/csla/issues/4581))

### Breaking Changes

* CSLA 11 no longer targets .NET 8 or .NET 9. Applications must target .NET 10 or later (or continue to use .NET Framework 4.6.2 through 4.8, or a `netstandard2.0`-compatible runtime).

### Supported Platforms

* .NET 10 and 11
* .NET Framework 4.6.2 through 4.8
* Blazor (Server, WebAssembly, Auto)
* MAUI
* ASP.NET Core MVC, Razor Pages, Web API
* Windows Forms, WPF
* Avalonia
* ASP.NET MVC 5, WebForms

Also expected to work on:

* Uno Platform

### Change List

* https://github.com/MarimerLLC/csla/compare/v10.1.0...main

### Contributors

* [@rockfordlhotka](https://github.com/rockfordlhotka)

Thank you all for your contributions!
