This release adds powerful analyzers for Visual Studio 2015 to help developers avoid common coding mistakes when building CSLA .NET business classes. Many thanks to Jason Bock ([JasonBock](https://github.com/JasonBock)) for all the work putting these together (and for the planned future analyzers!).

Version 4.6.200 also includes numerous important changes to support UWP apps, as well as some Xamarin scenarios. There are also some optimizations in MobileFormatter to help reduce the bytes transferred via the data portal.

This release is available via NuGet.

 - [#480](https://github.com/MarimerLLC/csla/pull/480) - #386 Get basic ProjectEdit working in Xamarin apps contributed by Rockford Lhotka ([rockfordlhotka](https://github.com/rockfordlhotka))
 - [#479](https://github.com/MarimerLLC/csla/issues/479) - Include pdb files for the analyzers via NuGet +enhancement
 - [#478](https://github.com/MarimerLLC/csla/pull/478) - #477 Fixes analyzer issue contributed by Jason Bock ([JasonBock](https://github.com/JasonBock))
 - [#477](https://github.com/MarimerLLC/csla/issues/477) - CheckConstructorsAnalyzerPublicConstructorCodeFix Throw Exception With Nested Classes +fix
 - [#363](https://github.com/MarimerLLC/csla/issues/363) - Analyzer: Simplify Property Implementations +enhancement
 - [#464](https://github.com/MarimerLLC/csla/issues/464) - Fix threading issue with data portal create and XAML apps
 - [#459](https://github.com/MarimerLLC/csla/pull/459) - Fixes #410 so the PropertyStatus control displays correctly in WPF contributed by Rockford Lhotka ([rockfordlhotka](https://github.com/rockfordlhotka))
 - [#456](https://github.com/MarimerLLC/csla/pull/456) - #455 Fixed contributed by Jason Hardman ([JasonHardman](https://github.com/JasonHardman))
 - [#455](https://github.com/MarimerLLC/csla/issues/455) - IOS resources are missing +fix
 - [#454](https://github.com/MarimerLLC/csla/pull/454) - #453 Fixed contributed by Jason Bock ([JasonBock](https://github.com/JasonBock))
 - [#453](https://github.com/MarimerLLC/csla/issues/453) - Bug: Calling Save() Within a BO Causes Error with CSLA0005 +fix
 - [#451](https://github.com/MarimerLLC/csla/pull/451) - Issue #450 contributed by Rockford Lhotka ([rockfordlhotka](https://github.com/rockfordlhotka))
 - [#450](https://github.com/MarimerLLC/csla/issues/450) - Fix warnings with analyzers +fix
 - [#449](https://github.com/MarimerLLC/csla/pull/449) - Add LazyGetProperty, update Android SDK, work on ProjectTracker contributed by Rockford Lhotka ([rockfordlhotka](https://github.com/rockfordlhotka))
 - [#448](https://github.com/MarimerLLC/csla/issues/448) - Update Android projects to latest SDK (22) +enhancement
 - [#429](https://github.com/MarimerLLC/csla/issues/429) - Update URLs in NuGet definitions +enhancement
 - [#410](https://github.com/MarimerLLC/csla/issues/410) - PropertyStatus control doesn't show up on WPF (CSLA v4.6.1) +fix
 - [#380](https://github.com/MarimerLLC/csla/issues/380) - Add HttpProxy to Android and iOS +enhancement
 - [#204](https://github.com/MarimerLLC/csla/issues/204) - Investigate DynamicRootList.SaveItem actually does an async save

* Fixed [serialization issue](http://www.lhotka.net/weblog/UWPCoreAssemblyName.aspx) between UWP and .NET
* The NuGet packages now support UWP apps
* Fixed issues with WinRT/UWP support for Windows 10
* New and updated analyzers
* **Potential breaking changes:**
 * Moved `Serializable` attribute to System namespace
 * Moved `NotSerializable` attribute to System namespace
 * Moved `ICloneable` interface to the system namespace
* Made progress updating `Samples\ProjectTracker` example *(note new path)*
* Now using [AppVeyor](https://ci.appveyor.com/project/rockfordlhotka/csla) to build CSLA .NET and run unit tests
* Now using [gitter](https://gitter.im/MarimerLLC/cslaforum) for real-time chat about CSLA .NET

Commits: ...
