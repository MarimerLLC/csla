# CSLA 5 releases

CSLA 5 includes major enhancements, and brings support for .NET Core 3 and WebAssembly (Blazor, Uno, etc.).

# CSLA .NET version 5.3.1 release

Bug fixes over the 5.3.0 release.

## Change List

[Changes in this release](https://github.com/MarimerLLC/csla/issues?q=is%3Aissue+project%3AMarimerLLC%2Fcsla%2F18+is%3Aclosed+).

### General

* [#1767](https://github.com/MarimerLLC/csla/issues/1767) 🐜 ViewModelBase always raises PropertyChanged for IsBusy & Model, even if they are set to the same values
* [#1764](https://github.com/MarimerLLC/csla/issues/1764) 🐜 DataPortalException.BusinessException returns wrong exception for data portal call
* [#1749](https://github.com/MarimerLLC/csla/issues/1749) 🐜 CheckRulesAsync is blocking the UI thread
* [#1746](https://github.com/MarimerLLC/csla/issues/1746) 🐜 busyChanged property in the BusinessRule of BusinessBase is marked as NonSerialized

## Contributors

* [@ajj7060](https://github.com/ajj7060) Bug fixes
* [@rockfordlhotka](https://github.com/rockfordlhotka) Bug fixes
* [@shushil](https://github.com/shushil) Bug reports
* [@ajohnstone-ks](https://github.com/ajohnstone-ks) Bug reports

# CSLA .NET version 5.3.0 release

Bug fixes and enhancements over the 5.2.0 release.

## Change List

[Changes in this release](https://github.com/MarimerLLC/csla/issues?q=is%3Aissue+project%3AMarimerLLC%2Fcsla%2F17+is%3Aclosed).

### General

* [#1696](https://github.com/MarimerLLC/csla/issues/1696) 🎉 Blazor GetPropertyInfo now supports lists and arrays
* [#1644](https://github.com/MarimerLLC/csla/issues/1644) 🐜 HttpProxy should get an HttpClient injected if one is available
* [#1635](https://github.com/MarimerLLC/csla/issues/1635) 🐜 PropertyChanged being invoked in a background thread
* [#1655](https://github.com/MarimerLLC/csla/issues/1655) 🐜 New SaveAndMergeAsync methods not exposed via ISavable and ISavable<T>
* [#1657](https://github.com/MarimerLLC/csla/issues/1657) 🐜 DataPortalExceptionInspector=> InspectException does not track actual objectType,and businessObject in case exception is thrown by any CommandBase
* [#1733](https://github.com/MarimerLLC/csla/issues/1733) 🎉 Updated docs for using GitHub from Visual Studio

## Contributors

* [@SachinPNikam](https://github.com/SachinPNikam) Data portal exception
* [@russblair](https://github.com/russblair) GitHub docs
* [@rockfordlhotka](https://github.com/rockfordlhotka) Bug fixes and enhancements

# CSLA .NET version 5.2.0 release

Enhancements related to Blazor, .NET core 3.1, and other good changes/fixes.

## Change List

[Changes in this release](https://github.com/MarimerLLC/csla/issues?q=is%3Aissue+is%3Aclosed+project%3AMarimerLLC%2Fcsla%2F16).

### General

* [#1498](https://github.com/MarimerLLC/csla/issues/1498) 🐜 Fix static initialization issue on "cold init" scenario
* [#1541](https://github.com/MarimerLLC/csla/issues/1541) 🐜 LocalProxy won't initialize with HttpClient added as service
* [#1539](https://github.com/MarimerLLC/csla/issues/1539) ⚠ Mark GlobalContext as obsolete (with warning)
* [#1522](https://github.com/MarimerLLC/csla/issues/1522) 🐜 Fix Csla.Data.EntityFrameworkCore=> GetManager returning disposed DbContext object
* [#1513](https://github.com/MarimerLLC/csla/issues/1513) 🎉 Improve exception info in MobileFormatter
* [#1568](https://github.com/MarimerLLC/csla/issues/1568) 🎉 Add `LoadListMode` concept to collection types
* [#1524](https://github.com/MarimerLLC/csla/issues/1524) 🎉 Add CheckRulesAsync method for checking rules
* [#1590](https://github.com/MarimerLLC/csla/issues/1590) 🐜 `DataMapper` now honors `Browsable` attribute

### Blazor

* [#1509](https://github.com/MarimerLLC/csla/issues/1509) 🎉 Add a For clause to CslaValidationMessages in Blazor
* [#1520](https://github.com/MarimerLLC/csla/issues/1520) 🐜 Fix Blazor Viewmodel GetPropertyInfo reference previous model property
* [#1544](https://github.com/MarimerLLC/csla/issues/1544) ⚠ Remove PropertyName from Blazor CslaValidationMessages
* [#1586](https://github.com/MarimerLLC/csla/issues/1586) ⚠ Move wasm dependencies to `Csla.Blazor.WebAssembly` package
* [#1585](https://github.com/MarimerLLC/csla/issues/1585) ⚠ Change `RefreshAsync` to require data portal or factory method
* [#1584](https://github.com/MarimerLLC/csla/issues/1584) 🎉 Add overload for `GetPropertyInfo` to accept expression

### Xamarin and XAML

* [#1570](https://github.com/MarimerLLC/csla/issues/1570) 🐜 Fix issue with PropertyInfo error/warn/info text

### Windows Forms and WPF (.NET Core 3)

* [#1454](https://github.com/MarimerLLC/csla/issues/1454) 🐜 Fix issue with `PropertyStatus` control
* [#1548](https://github.com/MarimerLLC/csla/issues/1548) 🎉 Add UseCsla methods for Windows Forms and WPF

### Build and Dev

* [#1188](https://github.com/MarimerLLC/csla/issues/1188) 🎉 Add support for NuGet symbol server

### Dependabot and NuGet dependencies

* Bump Microsoft.* packages to 3.1.4
* Bump Microsoft.AspNetCore.Components.WebAssembly to 3.2.0
* Bump System.Data.SqlClient from 4.8.0 to 4.8.1
* Bump Microsoft.Data.SqlClient 1.1.3
* Bump Microsoft.NETCore.UniversalWindowsPlatform from 6.2.9 to 6.2.10
* Bump System.Threading.Tasks.Extensions to 4.5.4
* Bump System.Runtime.CompilerServices.Unsafe to 4.7.1
* Bump Grpc.Net.Client to 2.28.0
* Bump Google.Protobuf to 3.12.0
* Bump Grpc.Tools to 2.28.1
* Bump System.Buffers to 4.5.1
* Bump System.Memory to 4.5.4
* Bump System.Text.Encodings.Web from 4.7.0 to 4.7.1
* Bump Microsoft.Bcl.AsyncInterfaces from 1.1.0 to 1.1.1
* Bump Xamarin.Forms to 4.6.0.772

## Contributors

* [@jacojordaan](https://github.com/jacojordaan) WPF and .NET Core 3
* [@thecakemonster](https://github.com/thecakemonster) Blazor
* [@SachinPNikam](https://github.com/SachinPNikam) Entity Framework
* [@BlagoCuljak](https://github.com/BlagoCuljak) WPF and Blazor
* [@JasonBock](https://github.com/JasonBock) Analyzers
* [@rockfordlhotka](https://github.com/rockfordlhotka) Bug fixes and enhancements

---

# CSLA .NET version 5.1.0 release

Enhancements primarily related to Blazor support, along with a few other good changes/fixes.

## Change List

[Changes in this release](https://github.com/MarimerLLC/csla/issues?q=is%3Aissue+is%3Aclosed+project%3AMarimerLLC%2Fcsla%2F12).

Highlights:

### Blazor

* [#1443](https://github.com/MarimerLLC/csla/issues/1443) Changed `Csla.Blazor.PropertyInfo` to not be generic
* [#1435](https://github.com/MarimerLLC/csla/issues/1435) Add authorization helpers for Blazor
* [#1440](https://github.com/MarimerLLC/csla/issues/1440) Add `ModelPropertyChanged` event to `ViewModel`
* [#1410](https://github.com/MarimerLLC/csla/issues/1410) Add ModelChanged/ModelChanging events to `ViewModel`
* [#1371](https://github.com/MarimerLLC/csla/issues/1371) Ensure `Csla.Blazor` works on server-side code too
* [#1295](https://github.com/MarimerLLC/csla/issues/1295) [#1309](https://github.com/MarimerLLC/csla/issues/1309) [#1436](https://github.com/MarimerLLC/csla/issues/1436) Add `CslaValidator` component for Blazor

### Authentication and Authorization

* [#1409](https://github.com/MarimerLLC/csla/issues/1409) Updates to `CslaClaimsPrincipal`

### Razor and MVC

* [#1389](https://github.com/MarimerLLC/csla/issues/1389) Add netcoreapp3.1 project for ASP.NET Core 3.1
* [#1368](https://github.com/MarimerLLC/csla/issues/1368) Add netcoreapp3.0 project for ASP.NET Core 3.0
* [#1367](https://github.com/MarimerLLC/csla/issues/1367) Add Razor tag helpers for validation (error/warn/info) text

### Data Portal

* [#1459](https://github.com/MarimerLLC/csla/issues/1459) Fix major perf issues with CSLA 5 data portal
* [#1444](https://github.com/MarimerLLC/csla/issues/1444) `DataPortalException` now returns better data via `ErrorInfo`
* [#1424](https://github.com/MarimerLLC/csla/issues/1424) Add `ApplicationContextManagerTls` type
* [#1417](https://github.com/MarimerLLC/csla/issues/1417) Fix issue with context disappearing after async call
* [#1387](https://github.com/MarimerLLC/csla/issues/1387) Improve exception messages from HttpProxy
* [#1386](https://github.com/MarimerLLC/csla/issues/1386) Auto-create scoped DI service provider on data portal server
* [#1332](https://github.com/MarimerLLC/csla/issues/1332) Factory data portal needs to support DI (via constructor injection)

### Templates

* [#752](https://github.com/MarimerLLC/csla/issues/752) Update NuGet templates package to work in VS 2017, 2019

### Analyzers

* [#1441](https://github.com/MarimerLLC/csla/issues/1441) Fix bug with `BusinessRuleDoesNotUseAddMethods` analyzer
* [#1348](https://github.com/MarimerLLC/csla/issues/1348) Analyzer: Operation attribute usage

### Examples

* [#1358](https://github.com/MarimerLLC/csla/issues/1358) Create `XamarinExample` sample app
* [#1364](https://github.com/MarimerLLC/csla/issues/1364) Create `RazorPagesExample` sample app

### Bug fixes

* [#1464](https://github.com/MarimerLLC/csla/issues/1464) `AmbiguousMatchException` no longer thrown due to method overrides
* [#1400](https://github.com/MarimerLLC/csla/issues/1400) Fix issue with `ApplicationContext` and `LocalProxy` and losing local/global context values
* [#1402](https://github.com/MarimerLLC/csla/issues/1402) Fix issue building CSLA in debug mode
* [#1369](https://github.com/MarimerLLC/csla/issues/1369) Fix issues with `ApplicationContext` in netcore 3
* [#1360](https://github.com/MarimerLLC/csla/issues/1360) Fix issue with `DbContext` in .NET Core 3

### Dependabot and NuGet dependencies

* Bump Microsoft.Extensions.DependencyInjection to 3.1.1
* Bump Microsoft.Extensions.DependencyInjection.Abstractions to 3.1.1
* Bump Microsoft.AspNetCore.Components.Web from 3.0.0 to 3.1.1
* Bump Microsoft.AspNetCore.Components from 3.0.0 to 3.1.1
* Bump System.Configuration.ConfigurationManager from 4.6.0 to 4.7.0
* Bump System.ComponentModel.Annotations from 4.6.0 to 4.7.0
* Bump System.Security.Permissions from 4.6.0 to 4.7.0
* Bump System.Security.AccessControl from 4.6.0 to 4.7.0
* Bump System.Security.Principal from 4.6.0 to 4.7.0
* Bump System.Data.SqlClient from 4.7.0 to 4.8.0
* Bump Microsoft.Data.SqlClient from 1.0.19249.1 to 1.1.1
* Bump Xamarin.Forms to 4.6.800
* Bump Google.Protobuf from 3.9.2 to 3.11.4
* Bump Grpc.Net.Client from 2.24.0 to 2.28.0
* Bump Microsoft.CodeAnalysis.Analyzers from 2.9.4 to 2.9.7

## Contributors

* [@dazinator](https://github.com/dazinator) lots of help with Blazor implementation
* [@TheCakeMonster](https://github.com/TheCakeMonster) lots of help with Blazor implementation
* [@JasonBock](https://github.com/JasonBock) lots of work with the analyzers
* [@skeeler88](https://github.com/skeeler88) bug fixes
* [@bradtwurst](https://github.com/bradtwurst) bug fixes
* [@jeddytier4](https://github.com/jeddytier4) got the NuGet templates package working again
* [@j055](https://github.com/j055) helped figure out and fix the local/global context issue
* [@Eduardo-Micromust](https://github.com/Eduardo-Micromust) found and fixed numerous method calling issues
* [@linkerro](https://github.com/linkerro) fixed some NuGet package reference issues
* [@SachinPNikam](https://github.com/SachinPNikam) updated the Blazor samples
* [@rockfordlhotka](https://github.com/rockfordlhotka) Bug fixes and enhancements

---

# CSLA .NET version 5.0.1 release

Bug fix release over 5.0.0.

## Change List

* [#1336](https://github.com/marimerllc/csla/issues/1336) Target `Microsoft.Extensions.*` 2.x versions, not 3.0 versions
* [#1337](https://github.com/marimerllc/csla/issues/1337) Fix issue with WPF depedendencies in .NET Core 3

---

# CSLA .NET version 5.0.0 release

CSLA 5 is a big release with some breaking changes, some major enhancements, and with new support for .NET Core 3 and WebAssembly (Blazor, etc.).

## Change List

This release contains numerous potential breaking changes so please pay attention to the ⚠ (possible) and 🛑 (likely) breaking symbols.

Some changes were made to address common issues people face using CSLA. I've tried to mark these as things to look out for because they'll likely simplify your life: 🎉

### NuGet Package Naming

* [#1151](https://github.com/marimerllc/csla/issues/1151) Rename NuGet packages to use modern conventions 🛑

**ALERT:** To use CSLA .NET version 5.0.0 or higher you will need to remove all older NuGet package references and reference the modern equivalents.

### Dependabot and NuGet dependencies

* Bump appropriate dependencies for .NET Core 3.0
* Bump Microsoft.AspNetCore from 2.0.3 to 2.2.0 in /Source
* Bump Xamarin.Forms from 3.4.0.1008975 to 4.1.0.815419 in /Source
* Add  Microsoft.EntityFrameworkCore from 3.0.0 in /Source
* Bump Microsoft.EntityFrameworkCore from 2.2.0 to 2.2.6 in /Source
* Bump Microsoft.CodeAnalysis.Analyzers from 2.6.3 to 2.9.4 in /Source
* Bump Microsoft.NETCore.UniversalWindowsPlatform to 6.2.9 in /Source
* Bump System.Data.SqlClient to 4.7.0 in /Source
* Bump System.Principal.Windows from 4.5.1 to 4.6.0 in /Source
* Bump System.Security.AccessControl from 4.5.9 to 4.6.0 in /Source

### Data Portal

* [#1293](https://github.com/MarimerLLC/csla/issues/1293) Fix issue where `ObjectFactory` wasn't working correctly with remote data portal
* [#1066](https://github.com/MarimerLLC/csla/issues/1066) Implement RabbitMQ data portal (`Csla.Channels.RabbitMq`) 🎉
* [#1140](https://github.com/MarimerLLC/csla/issues/1140) Implement gRPC data portal (`Csla.Channels.Grpc`) 🎉
* [#1176](https://github.com/MarimerLLC/csla/issues/1176) Enable passing multiple parameters to root DP methods (i.e. `FetchAsync`) 🎉
* [#787](https://github.com/MarimerLLC/csla/issues/787) Enable per-method dependency injection for DP_XYZ and Child_XYZ method params 🎉
* [#1102](https://github.com/MarimerLLC/csla/issues/1102) Eliminate hard-coded DP_XYZ and Child_XYZ method names 🎉
* [#1167](https://github.com/MarimerLLC/csla/issues/1167) Add `UpdateAllChildren` method to `FieldManager` to persist all child objects even if `IsDirty` is false
* [#1164](https://github.com/MarimerLLC/csla/issues/1164) Fix issue where `LocalProxy` would lock on async calls from Windows Forms
* [#1060](https://github.com/marimerllc/csla/issues/1060) Mark Silverlight-style data portal methods (e.g. `BeginFetch`, `BeginSave`) obsolete ⚠
* [#1059](https://github.com/marimerllc/csla/issues/1059) `HttpProxy` now supports both sync and async operations 🎉
* [#1072](https://github.com/marimerllc/csla/issues/1072) Fix issue where `LocalProxy` didn't flow context to async calls as expected
* [#1038](https://github.com/marimerllc/csla/issues/1038) Set default transaction timeout to 600 seconds (to match Microsoft default) ⚠
* [#960](https://github.com/marimerllc/csla/issues/960) Make sure only one `SaveObject` method is virtual ⚠
* [#1103](https://github.com/marimerllc/csla/issues/1103) Fix null ref exception when interacting with types that don't implement `IDataPortalTarget`
* [#1152](https://github.com/marimerllc/csla/issues/1152) Fix server response content-length issue with Blazor serialization

### Basic Usage

* [#1194](https://github.com/MarimerLLC/csla/issues/1194),[#1196](https://github.com/MarimerLLC/csla/issues/1196) Pass criteria to authorization rules via new `IAuthorizationContext` interface 🛑
* [#1212](https://github.com/MarimerLLC/csla/issues/1212) Fix issue with n-level undo and cloning
* [#1191](https://github.com/MarimerLLC/csla/issues/1191) Fix edit level issue with business rules
* [#1101](https://github.com/MarimerLLC/csla/issues/1101) Support `nameof` in `RegisterProperty`, replacing lambda expressions 🎉
* [#409](https://github.com/marimerllc/csla/issues/409) Allow use of `async Task ExecuteAsync` business rule implementations via new `BusinessRuleAsync` type 🎉
* [#1043](https://github.com/marimerllc/csla/issues/1043) Default null principal to an unauthenticated principal` (no longer need to manually set principal on app startup) 🎉
* [#1080](https://github.com/marimerllc/csla/issues/1080) Fixed issues caused by using `MobileFormatter` for n-level undo in .NET Framework 
* [#974](https://github.com/marimerllc/csla/issues/974) Rename DbProvider config key to CslaDbProvider 🛑
* [#963](https://github.com/marimerllc/csla/issues/963) Mark `RelationshipTypes.Child` as Obsolete ⚠
* [#1117](https://github.com/marimerllc/csla/issues/1117) Fix issue where string properties would ignore explicit null default value
* [#1209](https://github.com/marimerllc/csla/issues/1209) Add `ObjectAuthorizationRules` attribute so `AddObjectAuthorizationRules` is no longer a "magic name" method 🎉

### Analyzers

* [#1100](https://github.com/MarimerLLC/csla/issues/1100) Ensure DP methods return void or Task
* [#804](https://github.com/marimerllc/csla/issues/804) Update analyzers to modern project style and .NET Standard 1.3 (now requires Visual Studio 2017 or higher) ⚠
* [#623](https://github.com/marimerllc/csla/issues/623) Fix issue where analyzer would incorrectly flag an issue with serializable attributes 🎉
* [#925](https://github.com/marimerllc/csla/issues/925) Create analyzer to flag use of `new` keyword when creating domain objects (require use of data portal)
* [#1091](https://github.com/marimerllc/csla/issues/1091) Update/cleanup analyzer code

### Blazor and WebAssembly

* [#1283](https://github.com/marimerllc/csla/issues/1283) Add Blazor validation component
* [#1142](https://github.com/marimerllc/csla/issues/1142) Add UI helper support for client-side Blazor and WebAssembly 🎉
* [#1270](https://github.com/marimerllc/csla/issues/1270) Add `UnoExample` sample

### Xamarin

* [#1291](https://github.com/marimerllc/csla/issues/1291) Obsolete `Save` method in favor of `SaveAsync` 🛑
* [#1304](https://github.com/marimerllc/csla/issues/1304) Add properties to get info/warn/error text from `PropertyInfo`
* [#1010](https://github.com/marimerllc/csla/issues/1010) Fix issue where `ApplicationContext.User` wasn't persisted during async operations
* [#1168](https://github.com/MarimerLLC/csla/issues/1168) Mark old-style viewmodel async methods as obsolete and modernize class overall 🛑

### Data Access

* [#1138](https://github.com/marimerllc/csla/issues/1138) Fix NETfx-NS2 conflict with `TransactionTypes` enum
* [#1150](https://github.com/marimerllc/csla/issues/1150) Add support for `Microsoft.Data.SqlClient` via new `Csla.Data.SqlClient` package 🎉
* [#981](https://github.com/marimerllc/csla/issues/981) Move support for `System.Data.SqlClient` into new `Csla.Data.SqlClient.Fx` package - it is no longer in the core of CSLA 🛑

### ASP.NET Core

🛑 ASP.NET Core 2.2 projects often rely on the `Microsoft.AspNetCore.App` metapackage. You can not use this metapackage with CSLA 5, because the metapackage prevents the use of .NET Core 3.0 dependencies. The easiest workaround is to switch to the `Microsoft.AspNetCore.All` metapackage, though other options exist. 🛑

* [#905](https://github.com/MarimerLLC/csla/issues/905) Add support for ASP.NET Core MVC validation mechanism
* [#649](https://github.com/MarimerLLC/csla/issues/649) Implement CSLA permission requirement handler

### WPF

* [#1291](https://github.com/marimerllc/csla/issues/1291) Obsolete `Save` method in favor of `SaveAsync` 🛑
* [#1304](https://github.com/marimerllc/csla/issues/1304) Add properties to get info/warn/error text from `PropertyInfo`
* [#1235](https://github.com/MarimerLLC/csla/issues/1235) Bring `Csla.Xaml` for WPF forward to .NET Core 3
* [#1168](https://github.com/MarimerLLC/csla/issues/1168) Mark old-style viewmodel async methods as obsolete and modernize class overall 🛑

### Windows Forms

* [#1234](https://github.com/MarimerLLC/csla/issues/1234) Bring `Csla.Windows` forward to .NET Core 3

### Technical Debt

* [#1288](https://github.com/marimerllc/csla/issues/1288) Updated `/Samples` for CSLA 5
* [#1115](https://github.com/marimerllc/csla/issues/1115) Remove private constructors from templates
* [#968](https://github.com/marimerllc/csla/issues/968) Remove ApplicationContext.IsInRoleProvider property ⚠
* [#1053](https://github.com/marimerllc/csla/issues/1053) Update nuspec files to use license element
* [#1070](https://github.com/marimerllc/csla/issues/1070) Remove unused legacy Silverlight test code and resolve SonarQube warning
* [#1109](https://github.com/marimerllc/csla/issues/1109) Add `ContextManager` property to `CslaConfiguration` fluent API
* [#1111](https://github.com/marimerllc/csla/issues/1111) Update website URL to `https://cslanet.com` in source files
* [#1119](https://github.com/marimerllc/csla/issues/1119) Update bootstrap version in ProjectTracker to resolve security warning
* [#750](https://github.com/marimerllc/csla/issues/750) Updated editorconfig styles
* [#1004](https://github.com/marimerllc/csla/issues/1004) Update use of test database so data tests run on developer workstations

You can see all the [closed work items in GitHub](https://github.com/MarimerLLC/csla/issues?q=is%3Aissue+project%3AMarimerLLC%2Fcsla%2F5+is%3Aclosed).

## Contributors

I want to extend special recognition to the following contributors to this release:

* [@JasonBock](https://github.com/JasonBock) has put a lot of work into the analyzers - fixing long-standing issues, and creating some cool new analyzers as well - really nice!
* [@GillesBer](https://github.com/GillesBer) ported the Windows Forms and WPF satellite projects for use in .NET Core 3
* [@itgoran](https://github.com/itgoran) has been helping ensure the tests all work
* [@oatsoda](https://github.com/oatsoda) has been fixing some issues encountered in 4.9 - resulting in fixes 5.0
* [@Hadronicus](https://github.com/Hadronicus) similarly has fixed an issue in 4.9 that's in 4.10 and 5.0
* [@kellyethridge](https://github.com/kellyethridge) fixed a LocalProxy issue
* [@skalpin](https://github.com/skalpin) helped review/update some of the docs
* [@ajj7060](https://github.com/ajj7060) added features to authorization rules
* [@dazinator](https://github.com/dazinator) and [@ajj7060](https://github.com/ajj7060) have been engaged in identifying issues and solutions
* [@j055](https://github.com/j055) for data portal enhancements
* [@TheCakeMonster](https://github.com/TheCakeMonster) for help building out Blazor support
