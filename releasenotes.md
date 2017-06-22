This release is focused primarily on supporting .NET Core and NetStandard 1.6. There are also a number of bug fixes, enhancements, and improvements to some samples.

This release is available via NuGet.

* [#733](https://github.com/marimerllc/csla/issues/733) Remove WinRT packages from NuGet - CSLA no longer supports Win8 development as of version 4.6.600 **BREAKING CHANGE**
* [#722](https://github.com/marimerllc/csla/issues/722) Add support for ASP.NET Core
* [#708](https://github.com/marimerllc/csla/issues/708) Csla.AspNetCore.Mvc implementation
* [#725](https://github.com/marimerllc/csla/issues/725) Update samples
* [#701](https://github.com/marimerllc/csla/issues/701) Windows Forms now stores `Csla.ApplicationContext.User` in a `static` field instead of per-thread **BREAKING CHANGE**
* Provide More Information in UndoException [#472](https://github.com/marimerllc/csla/issues/472) (#718)  **BREAKING CHANGE**
* RegisterProperty overload (propertyLambdaExpression 
* Fix wrong role name in ProjectTracker Sample MockDb (#715) 
* Mark SingleCriteria as obsolete [#656](https://github.com/marimerllc/csla/issues/656) (#716) 
* Update NuGet targets/dependencies (#713) 
* [#707](https://github.com/marimerllc/csla/issues/707) Add editorconfig file for solution (#714) 
* [#691](https://github.com/marimerllc/csla/issues/691) Update for VS 2017 (#706)  **BREAKING CHANGE FOR FRAMEWORK DEVS**
* Samples/ProjectTracker Code fixes (#699) 
* Fix PropertyConvert of nullable type to nullable enum property. [#692](https://github.com/marimerllc/csla/issues/692) (#695) 
* Fix DynamicBindingListBase<T> constraint is missing Csla.Serialization.Mobile.IMobileObject [#689](https://github.com/marimerllc/csla/issues/689) (#690) 
* Delete Silverlight dependencies. (#685) 
* Fix Sample ExtendableWcfPortalForDotNet (Rolodex) [#678](https://github.com/marimerllc/csla/issues/678) (#683) 
* CslaFastStart LastName update fix (#682) 
* [#680](https://github.com/marimerllc/csla/issues/680) Fixes trivia issue (#681) 
* Allow SerializableAttribute on Enums (#672) 
* Add fast start sample and readme (#670) 
* Fixes [#664](https://github.com/marimerllc/csla/issues/664) - Add logic to also ConvertFrom TypeConverter inside CoerceValue (#665) 
* Throwing exception when sync dataportal tries to call async method on client ([#643](https://github.com/marimerllc/csla/issues/643))