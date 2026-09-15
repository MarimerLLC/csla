# CSLA 11 releases

CSLA 11 is the next major version of CSLA .NET, currently in development on the `main` branch.

Release notes for CSLA 10 are maintained on the [v10.x branch](https://github.com/MarimerLLC/csla/blob/v10.x/releasenotes.md), which is the maintenance branch for version 10.

## CSLA .NET version 11.0.0 release

Primary changes in this release include:

* Add support for .NET 11 ([#4699](https://github.com/MarimerLLC/csla/issues/4699))
* Remove support for .NET 8 and .NET 9, which Microsoft stops supporting in November 2026 ([#4348](https://github.com/MarimerLLC/csla/issues/4348), [#4581](https://github.com/MarimerLLC/csla/issues/4581))
* Remove support for legacy data portal operation method names such as `DataPortal_Fetch` and `Child_Update` ([#4828](https://github.com/MarimerLLC/csla/issues/4828))

### Breaking Changes

* CSLA 11 no longer targets .NET 8 or .NET 9. Applications must target .NET 10 or later (or continue to use .NET Framework 4.6.2 through 4.8, or a `netstandard2.0`-compatible runtime).
* The data portal no longer finds operation methods by name. Only methods with an operation attribute (such as `[Fetch]`, `[Update]`, or `[UpdateChild]`) are invoked, and the `DataPortalOptions.UseLegacyOperationMethods` option is removed. Add the matching attribute to any `DataPortal_XYZ` or `Child_XYZ` method that doesn't have one. The `CSLA0014` analyzer is now a warning and its code fix adds the attribute for you. See [Upgrading to CSLA 11](docs/Upgrading%20to%20CSLA%2011.md) ([#4828](https://github.com/MarimerLLC/csla/issues/4828)).
  <!-- TODO(#4828 Stage B): add the D1-D4 base class behavior changes (default Child_Create / CheckRules, not-supported stubs, ObjectFactory Child_Create, sync Child_Update). -->

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
