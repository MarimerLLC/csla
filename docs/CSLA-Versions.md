## Primary Versions
CSLA version 4.8 supports
* Runtimes
  * .NET Standard 2.0
  * .NET Core 2.1
  * .NET Framework 4, 4.5, 4.6
  * mono
* Platform support
  * ASP.NET Core 2.1
  * Xamarin (iOS, Android, UWP, Mac, Linux)
  * Windows Forms, WPF
  * UWP

CSLA 4 version 4.5 supports 
* .NET 4, 4.5
* WinRT (Windows 8 Store apps)
* Windows Phone 8
* Silverlight 5.

Older versions of CSLA .NET provide support for older versions of Microsoft .NET and Silverlight.

[CSLA.NET releases](https://github.com/MarimerLLC/csla/releases)
[Older CSLA .NET downloads](http://www.cslanet.com/Download.html)

## Upgrading
Some posts with information about upgrading:

* [From 4 to 4.5](https://cslanet.com/old-forum/11624.html)
* [From 3.8 to 4](https://cslanet.com/old-forum/10688.html)
* [from version 3.8 to 4](https://cslanet.com/old-forum/9225.html)
* [Using multiple versions of CSLA in one app](https://cslanet.com/old-forum/9893.html)

And some more:

* https://cslanet.com/old-forum/10810.html
* https://cslanet.com/old-forum/11408.html

* https://cslanet.com/old-forum/4083.html


## Non-Microsoft Platforms
CSLA 4 version 4.3 provides support for mono (Linux, OS X, etc.) and Mono for Android (Android).

## Future Roadmap
The roadmap is [available here](https://github.com/MarimerLLC/csla/issues?q=is%3Aopen+is%3Aissue+label%3Aroadmap).

<!---## Alternate Versions
CSLA .NET is also available in two other versions.

## Version Background
CSLA .NET 3.0.5 is the recommended version for .NET 2.0 and 3.0. It introduced support for .NET 3.0 features: WPF, WCF, WF. It includes a wide array of bug fixes and features around Windows Forms, and other .NET 2.0 scenarios.

Anyone using CSLA .NET 2.x should upgrade to 3.0.5.

CSLA .NET 3.6-3.8 added support for Silverlight, ADO.NET Entity Framework and other .NET 3.5 and 3.5 SP1 features. This also includes support for ASP.NET MVC.

Starting in 3.6, code reduction was a major focus. So writing classes against 3.8 (current) means writing probably less than 50% of the code from early versions.

If you are using .NET 3.5 or Silverlight 3, you should use CSLA .NET 3.8.

If you are using .NET 4 or Silverlight 4, you should use CSLA 4, because CSLA 4 was created for this platform.

CSLA 4 also includes code reduction, and more abstraction around the data portal, and support for ASP.NET MVC 3.

CSLA 4 version 4.2 adds support for mono on Mac, Linux, iOS, and Android. It includes bug fixes for .NET, Silverlight, and WP7.

CSLA 4 version 4.5 adds support for .NET 4.5 and WinRT (Windows 8), including the use of the new async and await keywords. It drops support for WP7 and Silverlight 4. Silverlight 5 and .NET 4 are supported through the use of Microsoft's async targeting pack library for Visual Studio 2012 (via nuget).
