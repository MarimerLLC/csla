# CSLA 8 releases

CSLA 8 is a substantial update to CSLA .NET, adding support for .NET 8 and other enhancements.

## CSLA .NET version 8.2.6 release

Fixes issues with the way `IContextManager` is resolved in Blazor apps in #4089 and #4182.

### Change List

* https://github.com/MarimerLLC/csla/compare/v8.2.5...v8.2.6

### Contributors

* @rockfordlhotka

## CSLA .NET version 8.2.5 release

* [#4075](https://github.com/MarimerLLC/csla/issues/4075) Fix configuration issue with Blazor DI services

### Change List

* https://github.com/MarimerLLC/csla/compare/v8.2.4...v8.2.5

### Contributors

* @jmpotvin
* @rockfordlhotka

## CSLA .NET version 8.2.4 release

CSLA .NET version 8.2.4 fixes bugs.

* [#4047](https://github.com/MarimerLLC/csla/issues/4047) Cannot change ContextManager through CslaOptions.ContextManagerType

### Change List

* https://github.com/MarimerLLC/csla/compare/v8.2.3...v8.2.4

### Contributors

* @mirecg
* @rockfordlhotka

## CSLA .NET version 8.2.3 release

CSLA .NET version 8.2.3 fixes bugs.

* [#4026](https://github.com/MarimerLLC/csla/issues/4026) Blazor ViewModel with ManagedObjectLifetime save issue

### Change List

* https://github.com/MarimerLLC/csla/compare/v8.2.2...v8.2.3

### Contributors

* @russblair
* @wfacey

## CSLA .NET version 8.2.2 release

Bug fixes.

### Change List

* https://github.com/MarimerLLC/csla/compare/v8.2.1...v8.2.2

### Contributors

* @swegel

## CSLA .NET version 8.2.1 release

Bug fixes.

### Change List

* https://github.com/MarimerLLC/csla/compare/v8.2.0...v8.2.1

### Contributors

* @mtavares628
* @rockfordlhotka

## CSLA .NET version 8.2.0 release

Bug fixes and enhancements.

### Change List

* https://github.com/MarimerLLC/csla/compare/v8.1.1...v8.2.0

### Contributors

* @swegele

## CSLA .NET version 8.1.1 release

Bug fixes.

### Change List

* https://github.com/MarimerLLC/csla/compare/v8.1.0...v8.1.1

### Contributors

* @swegele

## CSLA .NET version 8.1.0 release

CSLA .NET version 8.1.0 adds features and bug fixes. It also drops support for .NET 7.

Primary changes in this release include:

* [#3637](https://github.com/MarimerLLC/csla/issues/3637) Remove support for .NET 7
* [#1376](https://github.com/MarimerLLC/csla/issues/1376) CSLA now supports async serialization in aspnetcore
* [#3755](https://github.com/MarimerLLC/csla/issues/3755) Server-side data portal now waits for all async rules to complete

And _massive_ amounts of overall code modernization thanks to @SimonCropp

### Change List

* https://github.com/MarimerLLC/csla/compare/v8.0.0...v8.1.0

### Contributors

* @nico159
* @rockfordlhotka
* @ProDInfo
* @SimonCropp
* @StefanOssendorf

## CSLA .NET version 8.0.1 release

CSLA .NET version 8.0.1 fixes a NuGet bug in the 8.0.0 release.

.NET 8 Windows Forms and WPF projects can now reference the CSLA .NET NuGet packages.

### Contributors

* @prodinfo
* @rockfordlhotka

**Full Changelog**: https://github.com/MarimerLLC/csla/compare/v8.0.0...v8.0.1

## CSLA .NET version 8.0.0 release

See full release details here: https://github.com/MarimerLLC/csla/releases/tag/v8.0.0

## CSLA .NET version 8.0.0 release

CSLA .NET version 8.0.0 adds support for .NET 8 and other enhancements.

* [#3374](https://github.com/MarimerLLC/csla/issues/3374) Add support for .NET 8
* [#3371](https://github.com/MarimerLLC/csla/issues/3371) Remove support for Xamarin
* [#3463](https://github.com/MarimerLLC/csla/issues/3463) Remove .NET Core 3.1 code
* [#3491](https://github.com/MarimerLLC/csla/issues/3491) LocalProxy, HttpClient are kept alive in the root container (ServiceProvider), which causes a Memory Leak
* [#3481](https://github.com/MarimerLLC/csla/issues/3481) GraphMerge loses child objects
* [#3617](https://github.com/MarimerLLC/csla/issues/3617) Blazor ViewModel silently fails if model IsBusy
* [#3235](https://github.com/MarimerLLC/csla/issues/3235) HttpProxy throw System.NullReferenceException when cannot connected to server
* [#3616](https://github.com/MarimerLLC/csla/issues/3616) Optimize when rules cascade based on input properties
* [#3622](https://github.com/MarimerLLC/csla/issues/3662) Add WaitForIdle method to base types with IsBusy property
* [#3623](https://github.com/MarimerLLC/csla/issues/3623) Add client-side data portal cache capability
* [#3635](https://github.com/MarimerLLC/csla/issues/3635) Ensure IDataPortalCache interface works with async/await
* [#3655](https://github.com/MarimerLLC/csla/issues/3655) Improve IDataPortalCache API to be atomic
* [#3338](https://github.com/MarimerLLC/csla/issues/3338) `IDataPortalTarget` now includes `CheckRulesAsync`; `CslaModelBinder` now calls `CheckRulesAsync` instead of `CheckRules`
* [#3596](https://github.com/MarimerLLC/csla/issues/3596) Implement Blazor 8 state management for `ClientContext` and `LocalContext`
* [#3657](https://github.com/MarimerLLC/csla/issues/3657) Support legacy Blazor 7 state management
* [#3676](https://github.com/MarimerLLC/csla/issues/3676) Fix spin waiting for rule completion at the server
* [#3668](https://github.com/MarimerLLC/csla/issues/3668) Fixed: Blazor ViewModel Save doesn't call begin edit after save
* [#3395](https://github.com/MarimerLLC/csla/issues/3395) Update unit tests for CSLA 8
* Numerous updates to dependencies

### Change List

**Full Changelog**: https://github.com/MarimerLLC/csla/compare/v7.0.3...v8.0.0

* [Issues closed in this release](https://github.com/MarimerLLC/csla/issues?q=is%3Aclosed+project%3Amarimerllc%2F9+)

### Contributors

* @Inmobilis
* @michaelcsikos
* mtavares628
* @ossendorf-at-hoelscher / @StefanOssendorf
* @prodinfo
* @rockfordlhotka
* @russblair
* @swegele
* @TanguyIngels
* @tony20221

Thank you all so much for your support!
