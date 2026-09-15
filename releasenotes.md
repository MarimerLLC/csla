# CSLA 11 releases

CSLA 11 is the next major version of CSLA .NET, currently in development on the `main` branch.

Release notes for CSLA 10 are maintained on the [v10.x branch](https://github.com/MarimerLLC/csla/blob/v10.x/releasenotes.md), which is the maintenance branch for version 10.

## CSLA .NET version 11.0.0 release

Primary changes in this release include:

* Add support for .NET 11 ([#4699](https://github.com/MarimerLLC/csla/issues/4699))
* Remove support for .NET 8 and .NET 9, which Microsoft stops supporting in November 2026 ([#4348](https://github.com/MarimerLLC/csla/issues/4348), [#4581](https://github.com/MarimerLLC/csla/issues/4581))
* Call data portal operation methods explicitly instead of through reflection ([#4359](https://github.com/MarimerLLC/csla/issues/4359))

### Source generators bundled in the Csla package

The CSLA source generators now ship inside the `Csla` NuGet package and run automatically in any C# project that references `Csla`. There is no separate generator package to install.

* The `[CslaImplementProperties]`, `[CslaImplementPropertiesInterface<T>]` and `[CslaIgnoreProperty]` attributes are now part of `Csla.dll`.
* For every `partial` class that declares data portal operation methods (`[Create]`, `[Fetch]`, `[Insert]`, `[Update]`, `[Execute]`, `[Delete]`, `[DeleteSelf]` and the child equivalents), the generator emits an internal nested `IDataPortalOperations` interface that names each operation method, plus implementations of `IDataPortalOperationMapping` and `IDataPortalOperationNamedMapping` for concrete classes. The data portal then calls operation methods directly instead of using reflection, and IDEs, analyzers and the trimmer see private operation methods as used.
* Business classes with data portal operation methods should be declared `partial`. Analyzer **CSLA0024** reports classes (or containing types) that are not partial and provides a code fix that adds the `partial` modifier. Non-partial classes continue to work through reflection.

### Strongly typed data portal extension methods

Add `[DataPortalExtensions]` to a business class to generate strongly typed extension methods on `IDataPortal<T>` and `IChildDataPortal<T>` for its `[Create]`, `[Fetch]`, `[Execute]`, `[Delete]`, `[CreateChild]` and `[FetchChild]` methods, in both async and sync forms:

```csharp
[DataPortalExtensions(Prefix = "Portal")]
public partial class PersonEdit : BusinessBase<PersonEdit>
{
  [Fetch]
  private async Task Fetch(int id, [Inject] IPersonDal dal) { ... }
}

var person = await portal.PortalFetchAsync(42);
```

* Generated methods take only the criteria parameters (`[Inject]` parameters are omitted), and pass the pre-computed operation name and `[RunLocal]` setting to the data portal, so the client no longer searches for the operation method using reflection.
* Because `IDataPortal<T>` instance methods such as `FetchAsync` take precedence over extension methods, a generated name that matches one is not generated and is reported as a warning. Use `Prefix` to give the generated methods distinct names.
* Add `[assembly: DataPortalExtensions]` to generate extension methods for every business class in the assembly that can have them (non-abstract, non-generic classes that implement `ICslaObject` and are accessible outside their containing type). A `Prefix` on a class attribute takes precedence over the assembly `Prefix`.
* Use `[NoDataPortalExtension]` to exclude a class or an operation method.
* MSBuild properties control the generated names and forms:

  ```xml
  <PropertyGroup>
    <!-- generate only the async methods (default true) -->
    <CslaGenerateSyncDataPortalExtensions>false</CslaGenerateSyncDataPortalExtensions>
    <!-- suffix of the async method names (default Async); none for no suffix -->
    <CslaDataPortalExtensionsAsyncSuffix>none</CslaDataPortalExtensionsAsyncSuffix>
  </PropertyGroup>
  ```

  With no async suffix, the sync methods would have the same names as the async methods, so they are not generated.
* The `CSLADP002` to `CSLADP011` diagnostics, about operation methods that share an operation name and about extension methods that are not generated, are reported by analyzers rather than by the source generators, so they can be suppressed in source.
* Custom or mock `IDataPortal<T>` implementations continue to work; the generated methods fall back to the existing `params object[]` methods.
* Analyzer **CSLA0025** suggests using the generated extension methods instead of calling `IDataPortal<T>` methods with untyped criteria, and provides a code fix that replaces the call.

The data portal extension generator and the CSLA0025 analyzer are adapted from [Csla.DataPortalExtensions](https://github.com/StefanOssendorf/Csla.DataPortalExtensions) by [Stefan Ossendorf](https://github.com/StefanOssendorf), used under the MIT License. Thank you, Stefan!

### Breaking Changes

* CSLA 11 no longer targets .NET 8 or .NET 9. Applications must target .NET 10 or later (or continue to use .NET Framework 4.6.2 through 4.8, or a `netstandard2.0`-compatible runtime).
* The `Csla.Generator.AutoImplementProperties.CSharp` package is retired because its generators are now included in the `Csla` package. Remove any `PackageReference` (and `PackageVersion`) for it; a build error (`CSLABUILD001`) is reported while the reference remains.
* Csla's abstract base classes that declare data portal operation methods (`BusinessListBase`, `CommandBase`, `ReadOnlyBase`, and others) are now `partial`.

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
* [@StefanOssendorf](https://github.com/StefanOssendorf)

### Third-party notices

* Portions of the data portal extension generator and analyzers are adapted from [Csla.DataPortalExtensions](https://github.com/StefanOssendorf/Csla.DataPortalExtensions), Copyright (c) 2023 Stefan Ossendorf, licensed under the [MIT License](https://github.com/StefanOssendorf/Csla.DataPortalExtensions/blob/main/LICENSE).

Thank you all for your contributions!
