## Primary Versions
CSLA 4 version 4.6 supports 
* .NET 4, 4.5, 4.6
* UWP
* Xamarin (iOS, Android, UWP)
* PCL Profile111
* WinRT (Windows 8/8.1/Phone)

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

* [From 4 to 4.5](http://forums.lhotka.net/forums/t/11624.aspx)
* [From 3.8 to 4](http://forums.lhotka.net/forums/p/10688/49917.aspx#49917)
* [from version 3.8 to 4](http://forums.lhotka.net/forums/t/9225.aspx|Moving)
* [Using multiple versions of CSLA in one app](http://forums.lhotka.net/forums/p/9893/46391.aspx#46391)

And some more:

* http://forums.lhotka.net/forums/p/10810/50414.aspx#50414|http://forums.lhotka.net/forums/p/10810/50414.aspx#50414
* http://forums.lhotka.net/forums/p/11408/52956.aspx#52956|http://forums.lhotka.net/forums/p/11408/52956.aspx#52956

* http://forums.lhotka.net/forums/p/4083/19986.aspx#19986|http://forums.lhotka.net/forums/p/4083/19986.aspx#19986


## Non-Microsoft Platforms
CSLA 4 version 4.3 provides support for mono (Linux, OS X, etc.) and Mono for Android (Android).

## Future Roadmap
The roadmap is [available here](http://www.lhotka.net/cslanet/roadmap.aspx).

## Alternate Versions
CSLA .NET is also available in two other versions.

**CSLA .NET N2**

[CSLA .NET N2](http://www.lhotka.net/cslanet/n2.aspx) is a version of CSLA .NET 3.7+ that has been made to build on NET 2.0. This is a contribution by Jonny Bekkum. 

CSLA .NET N2 is almost completely feature-compatible with CSLA .NET for Windows, and is a good way for people still using .NET 2.0 to leverage many of the new features in recent versions of CSLA .NET for Windows.

**CSLA .NET VB**

[CSLA .NET VB](http://www.lhotka.net/cslanet/vb.aspx) is a version of CSLA .NET for Windows maintained in the VB language. This is a community effort led by Sean Rhone.

CSLA .NET VB is almost completely feature-compatible with CSLA .NET for Windows, and is a great reference implementation VB developers can use to get a deeper understanding of the framework implementation details. 

It is not recommended that CSLA .NET VB be used in production environments.

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