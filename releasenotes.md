# CSLA 10 releases

CSLA 10 is a substantial update to CSLA .NET, adding support for .NET 10 and including many enhancements and bug fixes.

For detailed migration guidance, see [Upgrading to CSLA 10](docs/Upgrading%20to%20CSLA%2010.md).

## CSLA .NET version 10.1.0 release

The full list of changes in this release can be found in the [GitHub compare view](https://github.com/MarimerLLC/csla/compare/v10.0.0...v10.1.0).

### Highlights

**New BusinessDocumentBase type** ([#1830](https://github.com/MarimerLLC/csla/issues/1830))

New `BusinessDocumentBase<T,C>` class that combines the features of `BusinessBase` and `BusinessListBase` in a single type. This allows you to create an editable business object that has properties _and also_ contains a list of child objects, without needing a separate class for the child list. The `[CslaImplementProperties]` source generator has been updated to support this new base type. The ProjectTracker sample has been updated to demonstrate the new pattern.

**New WCF data portal channel** ([#4851](https://github.com/MarimerLLC/csla/pull/4851))

New `Csla.Channels.Wcf` NuGet package providing a WCF data portal channel. Uses classic WCF (`System.ServiceModel`) for .NET Framework applications and CoreWCF for modern .NET applications. Supports both client and server side, including routing tag-based operation dispatch. Similar in structure to the existing HTTP, gRPC, and RabbitMQ channels.

**Data portal source generator** ([#4359](https://github.com/MarimerLLC/csla/issues/4359), [#4816](https://github.com/MarimerLLC/csla/pull/4816))

New source generator for data portal operation dispatch, improving performance and trimming support.

### Changes

**Data Portal**

* [#4616](https://github.com/MarimerLLC/csla/issues/4616) Make legacy `DataPortal_XYZ` method resolution optional ([#4829](https://github.com/MarimerLLC/csla/pull/4829))
* [#4817](https://github.com/MarimerLLC/csla/pull/4817) Flow operation names from client to server
* [#4823](https://github.com/MarimerLLC/csla/pull/4823) Support keyed DI services in data portal parameter injection
* [#4825](https://github.com/MarimerLLC/csla/pull/4825) Refactor portal error handling, add integration tests
* [#4849](https://github.com/MarimerLLC/csla/pull/4849) Update gRPC and RabbitMQ deserialization
* [#4844](https://github.com/MarimerLLC/csla/pull/4844) Remove `DataPortalAsyncRequest`/`Result` nested classes

**Rules Engine**

* [#4649](https://github.com/MarimerLLC/csla/issues/4649) Thread-safe broken rules collection ([#4819](https://github.com/MarimerLLC/csla/pull/4819))

**Blazor**

* [#4565](https://github.com/MarimerLLC/csla/issues/4565) Abstract out session store from `SessionManager` ([#4832](https://github.com/MarimerLLC/csla/pull/4832))

**Bug Fixes**

* [#4852](https://github.com/MarimerLLC/csla/issues/4852) Fix task conversion for data portal operations ([#4854](https://github.com/MarimerLLC/csla/pull/4854))
* [#4841](https://github.com/MarimerLLC/csla/issues/4841) Fix `DataPortalServerRoutingTagAttribute` ([#4847](https://github.com/MarimerLLC/csla/pull/4847))
* [#4821](https://github.com/MarimerLLC/csla/pull/4821) Fix `EditLevelMismatch` when cloning object graphs during edit sessions

**Code Quality**

* [#3214](https://github.com/MarimerLLC/csla/issues/3214) Add `[StringSyntax]` attribute to `RegExMatch` for IDE regex highlighting ([#4827](https://github.com/MarimerLLC/csla/pull/4827))
* [#4842](https://github.com/MarimerLLC/csla/pull/4842) Replace duplicate dictionary lookups with `TryGetValue` and cached locals
* [#4843](https://github.com/MarimerLLC/csla/pull/4843) Null-coalescing assignment cleanup
* [#4845](https://github.com/MarimerLLC/csla/pull/4845) Update Polyfill and use char-based string join
* [#4812](https://github.com/MarimerLLC/csla/pull/4812) Fix documentation warnings and improve portability
* [#4822](https://github.com/MarimerLLC/csla/pull/4822) Clarify `MobileFormatter` collection type limitations in docs
* [#4820](https://github.com/MarimerLLC/csla/pull/4820) Migrate database tests from SQL Server to SQLite

### Contributors

* [@rockfordlhotka](https://github.com/rockfordlhotka)
* [@StefanOssendorf](https://github.com/StefanOssendorf)
* [@SimonCropp](https://github.com/SimonCropp)
* [@Bowman74](https://github.com/Bowman74)
* [@b-higginbotham](https://github.com/b-higginbotham)
* [@joshhanson314](https://github.com/joshhanson314)
* [@luizfbicalho](https://github.com/luizfbicalho)

## CSLA .NET version 10.0.0 release

CSLA 10 is a substantial update to CSLA .NET, adding support for .NET 10 and including many enhancements and bug fixes.

### Highlights

**Platform Updates**
* Add support for .NET 10
* Supports .NET Framework 4.6.2+

**Nullable Reference Types (NRT)**
* Full nullable reference type support throughout the entire codebase
* Improved API clarity with explicit nullability annotations
* Note: Assigning CSLA properties in constructors is no longer supported due to NRT changes

**Observability**
* New OpenTelemetry instrumentation for data portal operations
* Metrics for total calls, completed calls, failed calls, and call duration
* Integration with Prometheus, Grafana, Azure Monitor, and .NET Aspire dashboards

**Rules Engine Enhancements**
* New `IUnhandledAsyncRuleExceptionHandler` interface for handling exceptions in async rules
* New `RuleContextModes` enum for fine-grained control over when rules execute
* Option to disable DataAnnotations scanning for improved performance

**Data Portal Improvements**
* `IDataPortal` and `IChildDataPortal` now return `ICslaObject` for improved trimming support
* Binary serialization for metastate data (improved performance)
* New `IMobileObjectMetastate` interface for custom serializer implementations
* Principal caching removed for better multi-threaded behavior

**Extensibility**
* Virtual `Deserialized()` method for custom post-deserialization logic
* `IDataErrorInfo` and `INotifyDataErrorInfo` now provide virtual methods for customization
* `InjectAttribute.AllowNull` property for optional service injection
* `RevalidatingInterceptor` now supports skipping validation during Delete operations

**Other Enhancements**
* `TransactionIsolationLevel.Snapshot` support
* `FriendlyName` property on XAML `PropertyInfo` component
* `ViewModel<T>.SaveAsync` and `CslaDataProvider.Refresh` now return `Task` instead of `async void`

### Breaking Changes

This is a major release with several breaking changes. Please review the [Upgrading to CSLA 10](docs/Upgrading%20to%20CSLA%2010.md) document for detailed migration guidance.

Key breaking changes include:
* Cannot assign CSLA properties in constructors
* `IDataPortal`/`IChildDataPortal` interface changes
* `RevalidatingInterceptor` constructor changes
* Various API changes for nullable reference type support
* `Csla.Server.DataPortal` and `Csla.Rules.BusinessRules` constructor changes

### Supported Platforms

* .NET 8 through 10
* .NET Framework 4.6.2 through 4.8
* Blazor (Server, WebAssembly, Auto)
* MAUI
* ASP.NET Core MVC, Razor Pages, Web API
* Windows Forms, WPF
* ASP.NET MVC 5, WebForms

Also expected to work on:

* Uno Platform
* Avalonia

### Change List

* https://github.com/MarimerLLC/csla/compare/v9.1.1...v10.0.0

### Contributors

* [@rockfordlhotka](https://github.com/rockfordlhotka)
* [@StefanOssendorf](https://github.com/StefanOssendorf)
* [@SimonCropp](https://github.com/SimonCropp)
* [@Bowman74](https://github.com/Bowman74)
* [@xal1983](https://github.com/xal1983)
* [@Youssef1313](https://github.com/Youssef1313)

Thank you all for your contributions!
