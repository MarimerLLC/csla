# CSLA 10 releases

CSLA 10 is a substantial update to CSLA .NET, adding support for .NET 10 and including many enhancements and bug fixes.

For detailed migration guidance, see [Upgrading to CSLA 10](docs/Upgrading%20to%20CSLA%2010.md).

## CSLA .NET version 10.0.0 release

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

* https://github.com/MarimerLLC/csla/compare/v9.1.0...v10.0.0

### Contributors

* [@rockfordlhotka](https://github.com/rockfordlhotka)
* [@StefanOssendorf](https://github.com/StefanOssendorf)
* [@SimonCropp](https://github.com/SimonCropp)
* [@Bowman74](https://github.com/Bowman74)
* [@xal1983](https://github.com/xal1983)
* [@Youssef1313](https://github.com/Youssef1313)

Thank you all for your contributions!
