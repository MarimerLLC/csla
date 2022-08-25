# Primary Versions

This document lists the versions of CSLA .NET that are considered "primary" versions.

## CSLA 6

* Runtimes
  * .NET 6
  * .NET Standard 2.0 and 2.1
  * .NET Framework 4.6.2 through 4.8
  * mono
  * UWP
* Platform support
  * ASP.NET Core
  * Blazor (and other .NET WebAssembly targets)
  * ASP.NET (Blazor, MVC, Razor Pages, and other app models)
  * Xamarin (iOS, Android, UWP, Mac, Linux)
  * Windows Forms, WPF
  * UWP
* Operating systems
  * Windows
  * Linux (including containers)
  * Mac
  * WebAssembly
  * Others (via .NET 6)

## CSLA 5

* Runtimes
  * .NET 5
  * .NET Standard 2.0 and 2.1
  * .NET Framework 4.6.2 through 4.8
  * mono
  * UWP
* Platform support
  * ASP.NET Core
  * Blazor (and other .NET WebAssembly targets)
  * ASP.NET (Blazor, MVC, Razor Pages, and other app models)
  * Xamarin (iOS, Android, UWP, Mac, Linux)
  * Windows Forms, WPF
  * UWP
* Operating systems
  * Windows
  * Linux (including containers)
  * Mac
  * WebAssembly
  * Others (via .NET 5)

Older versions of CSLA .NET provide support for older versions of Microsoft .NET, Silverlight, and other platforms.

* [CSLA.NET releases](https://github.com/MarimerLLC/csla/releases)
* [Older CSLA .NET downloads](http://www.cslanet.com/Download.html)

## Upgrading

1. The upgrade difficulty depends on your current version
   1. From 1.x – it is a rewrite (but to be fair, that code was obsoleted 17 years ago 😊 )
   1. From 2.x-3.6 – it requires quite a lot of changes (removing now-obsolete code), but the process is typically mechanical/repetitive, not hard
      1. EXCEPT if your UI is WinForms, in which case we enhanced CSLA to reveal some nasty UI bugs with data binding, so those bugs are often revealed as people move to or past v3.0.5
   1. From 3.8 – requires wrapping of old-style business rules into the new rules engine model
   1. From 4.0 – there’ve been some minor breaking changes that don’t affect everyone, but reviewing the release notes to see if a given codebase is affected is required
   1. From 5.0 – CSLA 6 is a major update that embraces dependency injection, so nearly all apps will be affected in their startup code, and in any interaction with the `ApplicationContext` or data portal types (all of which are now available as DI services)

Some older posts with information about upgrading:

* [from CSLA 1.51 to CSLA 3.5](https://cslanet.com/old-forum/4083.html)
* [from CSLA 3.6.3 to 4.1](https://cslanet.com/old-forum/10810.html)
* [from 3.0.2 to 4.5](https://cslanet.com/old-forum/11408.html)
* [From 4 to 4.5](https://cslanet.com/old-forum/11624.html)
* [From 3.8 to 4](https://cslanet.com/old-forum/10688.html)
* [from version 3.8 to 4](https://cslanet.com/old-forum/9225.html)
* [Using multiple versions of CSLA in one app](https://cslanet.com/old-forum/9893.html)
* [from 3.8 to 5.0](https://github.com/MarimerLLC/csla/discussions/1914)

## Future Roadmap

The roadmap is [available here](https://github.com/MarimerLLC/csla/issues?q=is%3Aopen+is%3Aissue+label%3Aflag%2Froadmap+).

## Version Background

* CSLA .NET 3.0.5 is the recommended version for .NET 2.0 and 3.0. It introduced support for .NET 3.0 features: WPF, WCF, WF. It includes a wide array of bug fixes and features around Windows Forms, and other .NET 2.0 scenarios.
  * Anyone using CSLA .NET 2.x should upgrade to 3.0.5.
* CSLA .NET 3.6-3.8 added support for Silverlight, ADO.NET Entity Framework and other .NET 3.5 and 3.5 SP1 features. This also includes support for ASP.NET MVC.
* Starting in 3.6, code reduction was a major focus. So writing classes against 3.8 (current) means writing probably less than 50% of the code from early versions.
* If you are using .NET 3.5 or Silverlight 3, you should use CSLA .NET 3.8.
* If you are using .NET 4 or Silverlight 4, you should use CSLA 4, because CSLA 4 was created for this platform.
* CSLA 4 also includes code reduction, and more abstraction around the data portal, and support for ASP.NET MVC 3.
* CSLA 4 version 4.2 adds support for mono on Mac, Linux, iOS, and Android. It includes bug fixes for .NET, Silverlight, and WP7.
* CSLA 4 version 4.5 adds support for .NET 4.5 and WinRT (Windows 8), including the use of the new async and await keywords. It drops support for WP7 and Silverlight 4. Silverlight 5 and .NET 4 are supported through the use of Microsoft's async targeting pack library for Visual Studio 2012 (via nuget).
* CSLA 4 version 4.6.603 drops support for Silverlight and Windows Phone.
* CSLA 4 version 4.7 introduces early support for WebAssembly and Blazor, ASP.NET Core, and Xamarin updates.
* CSLA 4 version 4.9.0 is the beginning of our use of semantic versioning. It supports .NET Core configuration via a fluent API, plus major data portal enhancements for container-based server environments such as Kubernetes.
* CSLA version 5.0.0 has a number of [breaking changes](https://github.com/MarimerLLC/csla/issues?q=is%3Aissue+project%3AMarimerLLC%2Fcsla%2F5+label%3A%22flag%2Fbreaking+change%22). The primary focus in this version is _major_ enhancements to the data portal, including support for dependency injection, flexible method names, and multiple criteria parameters. And of course, support for .NET Core 3.
* CSLA 6 is a major update that requires the use of dependency injection, so _all_ apps must configure an `IServiceCollection` and `IServiceProvider` on startup. This is normal for ASP.NET and Blazor apps, but is not necessarily familiar to Windows Forms or WPF developers. Some important static types are now DI services, most notably `ApplicationContext` and the data portal types. The data portal types are now injected as `IDataPortal<T>`, `IChildDataPortal<T>`, `IDataPortalFactory`, and `IChildDataPortalFactory`.
