# CSLA 9 releases

CSLA 9 is a substantial update to CSLA .NET, adding support for .NET 9, removing .NET 6 and UWP, and many enhancements and bug fixes.

## CSLA .NET version 9.1.0 release

CSLA .NET version 9.1.0 includes several enhancements and bug fixes.

Primary changes in this release include:

* Ensure ApplicationContext isn't null during initialization
* Stop caching the user principal in Blazor WebAssembly
* Fix issue with CancellationTokenSource
* Add option to ignore `DataAnnotation` attributes in `AddBusinessRules`
* Ongoing code modernization and cleanup thanks to @SimonCropp and others

https://github.com/MarimerLLC/csla/compare/v9.0.0...v9.1.0

### Contributors

* @rockfordlhotka
* @SimonCropp
* @StefanOssendorf

## CSLA .NET version 9.0.0 release

Primary changes in this release include:

* Add support for .NET 9
* Numerous changes and updates to configuration
* Remove support for .NET 6
* Remove support for UWP
* Remove support for Xamarin
* Remove BinaryFormatter and NetDataContractSerializer (NDCS) artifacts
* New and enhanced analyzers
* Support for async authorization rules
* Enhancements to business rules
* Enhancements to MobileFormatter, including support for custom serializers
* Data portal default configuration now works on Android
* Rework RabbitMQ data portal channel to support dependency injection
* Updates and improvements to several analyzers
* Code base now uses nullable reference types
* Over a dozen bug fixes

And _massive_ amounts of overall code modernization and cleanup thanks to @SimonCropp and others

### Supported Platforms

* .NET 9
* .NET 8
* .NET Framework 4.6.2 through 4.8
* Blazor
* MAUI
* ASP.NET Core MVC, Razor Pages, Web API
* Windows Forms, WPF
* ASP.NET MVC 5, WebForms

Also expected to work on:

* Uno.Platform
* Avalonia

### Change List

* https://github.com/MarimerLLC/csla/compare/v8.2.9...v9.0.0

### Contributors

* @Bowman74
* @Chicagoan2016
* @crazyfox55
* @EricNgo1972
* @Freelancingonupwork
* @jensbrand
* @kant2002
* @luizfbicalho
* @michaelcsikos
* @mirecg
* @mtavares628
* @rockfordlhotka
* @russblair
* @SimonCropp
* @sshushliapin
* @StefanOssendorf
* @swegele
* @TheCakeMonster
* @wfacey

Thank you all so much for your support!
