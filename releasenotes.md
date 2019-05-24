This is a pre-release of CSLA .NET version 5.0.0.

This release contains numerous potential breaking changes so please pay attention to the ⚠ (possible) and 🛑 (likely) breaking symbols.

Some changes were made to address common issues people face using CSLA. I've tried to mark these as things to look out for because they'll likely simplify your life: 🎉

### NuGet Package Naming

* [#1151](https://github.com/marimerllc/csla/issues/1151) Rename NuGet packages to use modern conventions 🛑

**ALERT:** To use CSLA .NET version 5.0.0 or higher you will need to remove all older NuGet package references and reference the modern equivalents.

### Dependabot and NuGet dependencies

* Bump Microsoft.CodeAnalysis.Analyzers from 2.6.3 to 2.9.1 in /Source
* Bump Microsoft.EntityFrameworkCore from 2.2.0 to 2.2.3 in /Source
* Bump Microsoft.NETCore.UniversalWindowsPlatform in /Source
* Bump Xamarin.Forms from 3.4.0.1008975 to 4.0.0.425677 in /Source

### Data Portal 

* [#1060](https://github.com/marimerllc/csla/issues/1060) Mark Silverlight-style data portal methods (e.g. `BeginFetch`, `BeginSave`) obsolete 🛑
* [#1059](https://github.com/marimerllc/csla/issues/1059) `HttpProxy` now supports both sync and async operations 🎉
* [#1072](https://github.com/marimerllc/csla/issues/1072) Fix issue where `LocalProxy` didn't flow context to async calls as expected
* [#1038](https://github.com/marimerllc/csla/issues/1038) Set default transaction timeout to 600 seconds (to match Microsoft default) ⚠
* [#960](https://github.com/marimerllc/csla/issues/960) Make sure only one `SaveObject` method is virtual ⚠
* [#1103](https://github.com/marimerllc/csla/issues/1103) Fix null ref exception when interacting with types that don't implement `IDataPortalTarget`
* [#1152](https://github.com/marimerllc/csla/issues/1152) Fix server response content-length issue with Blazor serialization

### Basic Usage

* [#1043](https://github.com/marimerllc/csla/issues/1043) Default null principal to an unauthenticated principal` (no longer need to manually set principal on app startup) 🎉
* [#1080](https://github.com/marimerllc/csla/issues/1080) Fixed issues caused by using `MobileFormatter` for n-level undo in .NET Framework 
* [#974](https://github.com/marimerllc/csla/issues/974) Rename DbProvider config key to CslaDbProvider 🛑
* [#963](https://github.com/marimerllc/csla/issues/963) Mark `RelationshipTypes.Child` as Obsolete ⚠
* [#1117](https://github.com/marimerllc/csla/issues/1117) Fix issue where string properties would ignore explicit null default value

### Analyzers

* [#804](https://github.com/marimerllc/csla/issues/804) Update analyzers to modern project style and .NET Standard 1.3 (now requires Visual Studio 2017 or higher) ⚠
* [#623](https://github.com/marimerllc/csla/issues/623) Fix issue where analyzer would incorrectly flag an issue with serializable attributes 🎉
* [#925](https://github.com/marimerllc/csla/issues/925) Create analyzer to flag use of `new` keyword when creating domain objects (require use of data portal)
* [#1091](https://github.com/marimerllc/csla/issues/1091) Update/cleanup analyzer code. 

### Xamarin

* [#1010](https://github.com/marimerllc/csla/issues/1010) Fix issue where `ApplicationContext.User` wasn't persisted during async operations

### Data Access

* [#1138](https://github.com/marimerllc/csla/issues/1138) Fix NETfx-NS2 conflict with `TransactionTypes` enum
* [#1150](https://github.com/marimerllc/csla/issues/1150) Add support for `Microsoft.Data.SqlClient` via new `Csla.Data.SqlClient` package 🎉
* [#981](https://github.com/marimerllc/csla/issues/981) Move support for `System.Data.SqlClient` into new `Csla.Data.SqlClient.Fx` package - it is no longer in the core of CSLA 🛑

### Technical Debt

* [#1115](https://github.com/marimerllc/csla/issues/1115)Remove private constructors from templates
* [#968](https://github.com/marimerllc/csla/issues/968) Remove ApplicationContext.IsInRoleProvider property ⚠
* [#1053](https://github.com/marimerllc/csla/issues/1053) Update nuspec files to use license element 
* [#1070](https://github.com/marimerllc/csla/issues/1070) Remove unused legacy Silverlight test code and resolve SonarQube warning
* [#1109](https://github.com/marimerllc/csla/issues/1109) Add `ContextManager` property to `CslaConfiguration` fluent API
* [#1111](https://github.com/marimerllc/csla/issues/1111) Update website URL to `https://cslanet.com` in source files
* [#1119](https://github.com/marimerllc/csla/issues/1119) Update bootstrap version in ProjectTracker to resolve security warning
* [#750](https://github.com/marimerllc/csla/issues/750) Updated editorconfig styles 
* [#1004](https://github.com/marimerllc/csla/issues/1004) Update use of test database so data tests run on developer workstations

You can see all the [closed work items in GitHub](https://github.com/MarimerLLC/csla/projects/5?card_filter_query=is%3Aissue).

## Contributors

I want to extend special recognition to the following contributors to this release:

* [@JasonBock](https://github.com/JasonBock) has put a lot of work into the analyzers - fixing long-standing issues, and creating some cool new analyzers as well - really nice!
* [@itgoran](https://github.com/itgoran) has been helping ensure the tests all work
* [@oatsoda](https://github.com/oatsoda) has been fixing some issues encountered in 4.9 - resulting in fixes 5.0
* [@Hadronicus](https://github.com/Hadronicus) similarly has fixed an issue in 4.9 that's in 4.10 and 5.0
* [@kellyethridge](https://github.com/kellyethridge) fixed a LocalProxy issue
* [@skalpin](https://github.com/skalpin) helped review/update some of the docs
* [@dazinator](https://github.com/dazinator) and [@ajj7060](https://github.com/ajj7060) have been engaged in identifying issues and solutions
