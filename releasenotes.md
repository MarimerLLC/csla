# CSLA 8 releases

CSLA 8 is a substantial update to CSLA .NET, adding support for .NET 8 and other enhancements.

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

* [Changes in this release](https://github.com/MarimerLLC/csla/issues?q=is%3Aclosed+project%3Amarimerllc%2F9+)

### Contributors

* @Inmobilis
* @michaelcsikos
* mtavares628
* @ossendorf-at-hoelscher / @StefanOssendorf
* @rockfordlhotka
* @russblair
* @swegele
* @TanguyIngels

Thank you all so much for your support!
