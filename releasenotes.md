I am pleased to announce the release of CSLA .NET version 4.7.200 with support for Blazor, and enhanced support for ASP.NET Core and Xamarin.

The packages are now in NuGet.

This is a pretty big release, focused on four key areas:

* Improved sample code
* Enhanced support for ASP.NET Core
  * The new `CslaModelBinderProvider` and `CslaModelBinder` types support data binding in Razor Pages scenarios
* Enhanced support for Xamarin
  * Updated for Xamarin Forms 3.0; Enhanced the `PropertyInfo` type, Fixed critical bug in `GraphMerger` on iOS
* Support for Blazor (and mono/wasm)
  * Add support for Blazor so it is possible to use CSLA objects in client-side mono/wasm code in a Blazor app, reference the `CSLA-Core-NS` package from your Blazor client app to use CSLA

More details are listed here:

### ASP.NET Core
* [#900](https://github.com/marimerllc/csla/issues/900) Update model binder to support editable list root objects 
* [#900](https://github.com/marimerllc/csla/issues/900) Add CslaModelBinder for IBusinessBase types 

### Xamarin
* [#903](https://github.com/marimerllc/csla/issues/903) Fix the GraphMerger so it works on iOS devices - Find child type on iOS when indexer is not found 
* [#886](https://github.com/marimerllc/csla/issues/886) Upgrade to Xamarin Forms 3.0 
* [#886](https://github.com/marimerllc/csla/issues/886) Upgrade to Xamarin Forms 3.0 (also consolidate all json.net references to the same version) 
* [#697](https://github.com/marimerllc/csla/issues/697) Add Tag property to PropertyInfo class. 

### Blazor
* [#829](https://github.com/marimerllc/csla/issues/829) Make UseTextSerialization property static 
* [#829](https://github.com/marimerllc/csla/issues/829) Remove MobileList test with complex types (breaking change - scenario no longer supported) 
* [#829](https://github.com/marimerllc/csla/issues/829) Add UseReflectionFallback setting to prevent use of System.Linq.Expressions 
* [#829](https://github.com/marimerllc/csla/issues/829) Make HttpProxy and HttpPortalController support optional text serialization 
* [#829](https://github.com/marimerllc/csla/issues/829) Expose GetValueOrNull method as public 
* [#829](https://github.com/marimerllc/csla/issues/829) Remove DCS dependency in MobileList 

### Bug fixes
* [#897](https://github.com/marimerllc/csla/issues/897) Get BeginEdit working in netstandard 2.0 implementation 
* Closes [#545](https://github.com/marimerllc/csla/issues/545) Unhook event handlers before save operations. 
* Closes [#205](https://github.com/marimerllc/csla/issues/205) Call OnRefreshing before each OnRefreshed call. 
* Fixes [#761](https://github.com/marimerllc/csla/issues/761) Resolve exception when calling BeginRefresh 
* Fix threading issue in InitializePerTypeRules (#614) 
* Fix threading issue in InitializePerTypeRules (#614) 

### Misc

* [#879](https://github.com/marimerllc/csla/issues/879) Update tests to work in .NET Core. 
* [#879](https://github.com/marimerllc/csla/issues/879) Add basic .NET Core undoable tests 
* Closes [#633](https://github.com/marimerllc/csla/issues/633) Closes [#657](https://github.com/marimerllc/csla/issues/657) Closes [#697](https://github.com/marimerllc/csla/issues/697) Add CustomTag property and fix null ref exception during control init. Make UpdateState public. 
* Complete pt resources.Add some fr resources. (#884) 
* Add ReadOnlyBase support for property setter anlyzer 
* Mention contributor agreement in the getting started list 
* [#836](https://github.com/marimerllc/csla/issues/836) Make ConnectionManager types match; Optimize locking in DeRef method 

### Samples

* Add Wisej Web UI to ProjectTracker sample (#881) 
* Update Project Tracker to 4.7.200-R18051402 ( [#879)](https://github.com/marimerllc/csla/issues/879)) 
* [#829](https://github.com/marimerllc/csla/issues/829) Add BlazorExample sample code 
* Remove useless overload RoleEdit GetRole(int id) (#687) 
* ProjectTracker.BusinessLibrary.Netstandard is missing a reference to ProjectTracker.Dal (#870) 
* ProjectTracker WinForms - show validation errors on Save (#868) 
* Fix for ProjectTracker WinForms RolesEdit is broken (#863) UI 
* Missing Display attribute (#863) 
* Fix for ProjectTracker WinForms RolesEdit is broken (#863) 
* ProjectTracker BO: add missing RoleList.CacheList (#865) 
* Revert to RoleList cache refresh (#861) 
* Async RoleList cache refresh (#861) 
* Consistent codig of ResourceEdit and ProjectEdit (#861 (#861) 
* Improve Project Tracker WinForms (#861) 
* Updating samples to 4.7.101 (#859) 
* Update sample to CSLA .NET 4.7.101 (#844) Cosmetic changes on some forms. 
* Update SimpleNTier (broken Wcf configuration) sample to CSLA .NET 4.7.101 [#844](https://github.com/marimerllc/csla/issues/844) 
* Update Project Tracker sample to CSLA .NET 4.7.101 [#844](https://github.com/marimerllc/csla/issues/844) 
* Update CslaMVC sample to CSLA .NET 4.7.101 [#844](https://github.com/marimerllc/csla/issues/844) 
* Refresh generated code (#847) 
* Add sample to verify NuGet versions (#841) 
* [#832](https://github.com/marimerllc/csla/issues/832) Change NuGet targets to lib/net461 

## Important notes

There continue to be two "families" of CSLA .NET packages in NuGet. One that supports full .NET and one that supports all other runtimes. The following work item is tracking when this is resolved (after Microsoft fixes a bug in full .NET):

* [#822](https://github.com/marimerllc/csla/issues/822)OnDeserialized attribute from NS2 causes runtime error in full .NET

In the meantime, the full .NET family (based on the `CSLA-Core` package) must be used for the following types of app:
* Windows Forms
* WPF
* ASP.NET (other than Core)
* Console apps (other than Core)
* Azure Functions
* Any other runtime hosted by full .NET 4.6.1+

The netstandard family (based on the `CSLA-Core-NS` package) must be used for the following types of app:
* Xamrin
* UWP
* .NET Core
* ASP.NET Core
* Blazor
* Any other runtime _not_ hosted by full .NET 4.6.1+

What this means for you is that if your n-tier app is 100% full .NET or full netstandard then you can live within one of those families. BUT if your server is full .NET and your client is Xamarin (for example) then your _business library_ assemblies need to be compiled twice: once for full .NET and once for netstandard.

The `Samples\ProjectTracker` app shows how this is done by using a Shared Project to contain the code, and two concrete class library projects that compile the code for full .NET and netstandard respectively.
