# CSLA 11 releases

CSLA 11 is the next major version of CSLA .NET, adding support for .NET 11 and removing support for .NET 8 and .NET 9. It is currently in development on the `main` branch.

Release notes for CSLA 10 are maintained on the [v10.x branch](https://github.com/MarimerLLC/csla/blob/v10.x/releasenotes.md), which is the maintenance branch for version 10.

## CSLA .NET version 11.0.0 release

CSLA 11 includes all of the changes released in CSLA 10.2.0. The full list of changes made since that release can be found in the [GitHub compare view](https://github.com/MarimerLLC/csla/compare/v10.2.0...main).

### Highlights

**Platform Updates** ([#4699](https://github.com/MarimerLLC/csla/issues/4699), [#4348](https://github.com/MarimerLLC/csla/issues/4348), [#4581](https://github.com/MarimerLLC/csla/issues/4581), [#4922](https://github.com/MarimerLLC/csla/pull/4922))

* Add support for .NET 11
* Remove support for .NET 8 and .NET 9, which Microsoft stops supporting in November 2026
* `netstandard2.0` and .NET Framework 4.6.2 through 4.8 targets are unchanged
* `Csla.Maui` targets `net11.0-android`, `net11.0-ios`, `net11.0-maccatalyst`, and `net11.0-windows` alongside the .NET 10 targets

**Source generators bundled in the Csla package** ([#4359](https://github.com/MarimerLLC/csla/issues/4359), [#4925](https://github.com/MarimerLLC/csla/pull/4925))

The CSLA source generators now ship inside the `Csla` NuGet package and run automatically in any C# project that references `Csla`. There is no separate generator package to install.

* The `[CslaImplementProperties]`, `[CslaImplementPropertiesInterface<T>]` and `[CslaIgnoreProperty]` attributes are now part of `Csla.dll`.
* For every `partial` class that declares data portal operation methods (`[Create]`, `[Fetch]`, `[Insert]`, `[Update]`, `[Execute]`, `[Delete]`, `[DeleteSelf]` and the child equivalents), the generator emits an internal nested `IDataPortalOperations` interface that names each operation method, plus implementations of `IDataPortalOperationMapping` and `IDataPortalOperationNamedMapping` for concrete classes. The data portal then calls operation methods directly instead of using reflection, and IDEs, analyzers and the trimmer see private operation methods as used.
* Business classes with data portal operation methods should be declared `partial`. Analyzer **CSLA0024** reports classes (or containing types) that are not partial and provides a code fix that adds the `partial` modifier. Non-partial classes continue to work through reflection.

**Strongly typed data portal extension methods** ([#4359](https://github.com/MarimerLLC/csla/issues/4359), [#4925](https://github.com/MarimerLLC/csla/pull/4925))

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
    <!-- Generate extensions for sync methods (default true) -->
    <CslaGenerateSyncDataPortalExtensions>false</CslaGenerateSyncDataPortalExtensions>
    <!-- Suffix of the async method names (default Async); none for no suffix -->
    <CslaDataPortalExtensionsAsyncSuffix>none</CslaDataPortalExtensionsAsyncSuffix>
  </PropertyGroup>
  ```

  With no async suffix, the sync methods would have the same names as the async methods, so they are not generated.
* The `CSLADP002` to `CSLADP011` diagnostics, about operation methods that share an operation name and about extension methods that are not generated, are reported by analyzers rather than by the source generators, so they can be suppressed in source. They are described in the [analyzer documentation](docs/analyzers/index.md#data-portal-generator-diagnostics).
* Custom or mock `IDataPortal<T>` implementations continue to work; the generated methods fall back to the existing `params object[]` methods.
* Analyzer **CSLA0025** suggests using the generated extension methods instead of calling `IDataPortal<T>` methods with untyped criteria, and provides a code fix that replaces the call.

The data portal extension generator and the CSLA0025 analyzer are adapted from [Csla.DataPortalExtensions](https://github.com/StefanOssendorf/Csla.DataPortalExtensions) by [Stefan Ossendorf](https://github.com/StefanOssendorf), used under the MIT License. Thank you, Stefan!

**Attribute-only data portal operations** ([#4828](https://github.com/MarimerLLC/csla/issues/4828), [#4943](https://github.com/MarimerLLC/csla/pull/4943))

The data portal no longer finds operation methods by name (`DataPortal_Fetch`, `Child_Update`, and so on). Only methods marked with an operation attribute are invoked. Matching by name could call an unrelated method that happened to have an operation name ([#4595](https://github.com/MarimerLLC/csla/issues/4595)). See [Upgrading to CSLA 11](docs/Upgrading%20to%20CSLA%2011.md) for migration guidance.

**Common view model base type** ([#1766](https://github.com/MarimerLLC/csla/issues/1766))

The new `Csla.Mvvm.ViewModel<T>` in the `Csla` package has the view model behavior that was implemented separately for Blazor, WPF, MAUI and Avalonia. The existing `Csla.Blazor.ViewModel<T>` and `Csla.Xaml.ViewModelBase<T>` types build on it, so all view models handle errors, object lifetime and authorization the same way. See [Upgrading to CSLA 11](docs/Upgrading%20to%20CSLA%2011.md#common-cslamvvmviewmodelt-base-type).

### Changes

**Data Portal**

* [#4359](https://github.com/MarimerLLC/csla/issues/4359) Call data portal operation methods explicitly instead of through reflection, using source generators bundled in the `Csla` package ([#4925](https://github.com/MarimerLLC/csla/pull/4925))
* [#4828](https://github.com/MarimerLLC/csla/issues/4828) Remove the data portal's fallback of finding operation methods by name; only methods with an operation attribute are invoked ([#4943](https://github.com/MarimerLLC/csla/pull/4943))
* [#4780](https://github.com/MarimerLLC/csla/issues/4780) Resolve transaction settings when the transaction is created, and add `ITransactionScopeFactory` to control how the data portal creates `TransactionScope` instances ([#4949](https://github.com/MarimerLLC/csla/pull/4949))

**Authorization**

* [#4707](https://github.com/MarimerLLC/csla/issues/4707) XAML and Blazor view models support async authorization rules, and `HasPermission` reports a still-running async rule with an `InvalidOperationException` that points to `HasPermissionAsync` ([#4948](https://github.com/MarimerLLC/csla/pull/4948))
* [#4947](https://github.com/MarimerLLC/csla/issues/4947) `IsSavable` supports async per-type authorization rules: the object is busy while the rule runs, and the result is cached per action and user ([#4948](https://github.com/MarimerLLC/csla/pull/4948))

**View Models**

* [#1766](https://github.com/MarimerLLC/csla/issues/1766) Add `Csla.Mvvm.ViewModel<T>` as the common base for the Blazor and XAML view models

**Bug Fixes**

* [#4891](https://github.com/MarimerLLC/csla/issues/4891) Remove the unusable parameterless constructor of `LazySingleton<T>` ([#4944](https://github.com/MarimerLLC/csla/pull/4944))

**Collections**

* [#4848](https://github.com/MarimerLLC/csla/issues/4848) Editable lists enforce `AllowNew` and `AllowRemove`: adding or removing items when the matching property is `false` throws `NotSupportedException` ([#4945](https://github.com/MarimerLLC/csla/pull/4945))
* [#4266](https://github.com/MarimerLLC/csla/issues/4266) `BusinessListBase`, `BusinessBindingListBase` and `BusinessDocumentBase` no longer serialize an empty deleted list, which reduces MobileFormatter payloads by about 44 bytes per list (156 bytes for the first list of a type) ([#4963](https://github.com/MarimerLLC/csla/pull/4963))

**N-level Undo**

* [#4582](https://github.com/MarimerLLC/csla/issues/4582) Remove the unused `StateMode.Undo` value and its MobileFormatter-specific code paths. `CancelEdit` restores a field that held a serializable object (such as a `MobileList<T>`) even when the field was set to `null` during the edit; this used to throw ([#4974](https://github.com/MarimerLLC/csla/pull/4974))

**Build and Release**

* [#4917](https://github.com/MarimerLLC/csla/issues/4917) Set the `main` version to 11.0.0 and start the CSLA 11 release notes ([#4918](https://github.com/MarimerLLC/csla/pull/4918))

### Breaking Changes

* CSLA 11 no longer targets .NET 8 or .NET 9. Applications must target .NET 10 or later (or continue to use .NET Framework 4.6.2 through 4.8, or a `netstandard2.0`-compatible runtime).
* On .NET 11, Android apps using `Csla.Maui` require Android API level 24 or later, because .NET 11 for Android requires it. The `net10.0-android` target still supports API level 21.
* The `Csla.Generator.AutoImplementProperties.CSharp` package is retired because its generators are now included in the `Csla` package. Remove any `PackageReference` (and `PackageVersion`) for it; a build error (`CSLABUILD001`) is reported while the reference remains. Business classes with data portal operation methods should be `partial`; the new `CSLA0024` analyzer warns for classes that aren't. See [Upgrading to CSLA 11](docs/Upgrading%20to%20CSLA%2011.md#source-generators-bundled-in-the-csla-package).
* The data portal no longer finds operation methods by name. Only methods with an operation attribute (such as `[Fetch]`, `[Update]`, or `[UpdateChild]`) are invoked, and the `DataPortalOptions.UseLegacyOperationMethods` option is removed. Add the matching attribute to any `DataPortal_XYZ` or `Child_XYZ` method that doesn't have one. The `CSLA0014` analyzer is now a warning and its code fix adds the attribute for you. See [Upgrading to CSLA 11](docs/Upgrading%20to%20CSLA%2011.md) ([#4828](https://github.com/MarimerLLC/csla/issues/4828)).
  * The default `Child_Create()` methods on `BusinessBase`, `BusinessListBase`, and `BusinessBindingListBase` are now marked with `[CreateChild]`, so a child with no create method of its own still gets its business rules checked.
  * The private "not supported" `DataPortal_XYZ` methods on `CommandBase`, `ReadOnlyBase`, and the read-only, name/value, and dynamic list base classes are removed. Unsupported operations now fail with the data portal's normal "method not found" error.
  * Creating a child of an `[ObjectFactory]` type no longer looks for a method named `Child_Create`. It calls the business class's `[CreateChild]` method.
* The parameterless constructor of `LazySingleton<T>` is removed, and `LazySingleton<T>` no longer implements `IUseApplicationContext`. Pass a factory delegate to `LazySingleton(Func<T>)`, or use `System.Lazy<T>`. See [Upgrading to CSLA 11](docs/Upgrading%20to%20CSLA%2011.md) ([#4891](https://github.com/MarimerLLC/csla/issues/4891)).
* `BusinessListBase`, `BusinessBindingListBase`, `DynamicListBase` and `DynamicBindingListBase` throw `NotSupportedException` when items are added while `AllowNew` is `false`, or removed while `AllowRemove` is `false`. `AllowRemove` now defaults to `true` for `BusinessListBase` and `DynamicListBase`. See [Upgrading to CSLA 11](docs/Upgrading%20to%20CSLA%2011.md#allownew-and-allowremove-are-enforced).
* With async authorization rules, view model permission properties (such as `CanCreateObject`) start out `false` and update when the rules complete. Reading `IsSavable` while an async `CreateObject`, `EditObject` or `DeleteObject` rule is running marks the object busy and returns `false`, and calling `Save` before the rule completes throws. See [Upgrading to CSLA 11](docs/Upgrading%20to%20CSLA%2011.md#async-authorization-rules-in-view-models-and-issavable) ([#4707](https://github.com/MarimerLLC/csla/issues/4707), [#4947](https://github.com/MarimerLLC/csla/issues/4947)).
* `TransactionalAttribute` no longer copies defaults from `DataOptions`: `TimeoutInSeconds` is now `int?` and `AsyncFlowOption` is now `TransactionScopeAsyncFlowOption?`, and the `TransactionalDataPortal` constructor takes an `ITransactionScopeFactory` and `TransactionScopeFactoryContext`. The `DataOptions` transaction defaults are now per-instance instead of static. See [Upgrading to CSLA 11](docs/Upgrading%20to%20CSLA%2011.md#transaction-settings-resolved-at-runtime-and-itransactionscopefactory) ([#4780](https://github.com/MarimerLLC/csla/issues/4780)).
* The XAML view model's `SaveAsync` and `RefreshAsync` capture exceptions in `Exception` and `ViewModelErrorText`, and raise `Error`, instead of throwing. An exception from an authorization rule during the status calculation is reported the same way on every view model. `SaveAsync` returns `Task<T?>` on all view models. On MAUI and Avalonia, `ManageObjectLifetime` defaults to `true` and the public `ManageObjectLifetimeProperty` field is removed. The Blazor `OnModelChanged()` override is replaced by `OnModelChanged(T? oldValue, T? newValue)`. See [Upgrading to CSLA 11](docs/Upgrading%20to%20CSLA%2011.md#common-cslamvvmviewmodelt-base-type) ([#1766](https://github.com/MarimerLLC/csla/issues/1766)).
* `StateMode.Undo` is removed. N-level undo hasn't called `OnGetState`/`OnSetState` with that value for many versions, so delete any `mode == StateMode.Undo` branches from your overrides. Undo state serialized by an earlier CSLA version (an object sent at an edit level above zero) can't be restored by CSLA 11. See [Upgrading to CSLA 11](docs/Upgrading%20to%20CSLA%2011.md#statemodeundo-removed) ([#4582](https://github.com/MarimerLLC/csla/issues/4582), [#4974](https://github.com/MarimerLLC/csla/pull/4974)).

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

### Contributors

* [@rockfordlhotka](https://github.com/rockfordlhotka)
* [@StefanOssendorf](https://github.com/StefanOssendorf)

### Third-party notices

* Portions of the data portal extension generator and analyzers are adapted from [Csla.DataPortalExtensions](https://github.com/StefanOssendorf/Csla.DataPortalExtensions), Copyright (c) 2023 Stefan Ossendorf, licensed under the [MIT License](https://github.com/StefanOssendorf/Csla.DataPortalExtensions/blob/main/LICENSE).

Thank you all for your contributions!
