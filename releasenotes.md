This release is focused primarily on enhancing the existing Xamarin support. There is now a Csla.dll targeting PCL Profile111, which is the current profile for Xamarin.Forms projects and .NET Core. 

There is also now a CSLA-Xamarin NuGet package that includes a Csla.Xaml.dll with support for Xamarin.Forms. This includes the same viewmodel base classes as the other XAML platforms, and an implementation of the `PropertyInfo` control tailored for use in Xamarin.Forms.

[@JasonBock](https://github.com/JasonBock) added even more analyzers for Visual Studio 2015 to help developers avoid common coding mistakes when building CSLA .NET business classes.

We now have support for the prerelease of Entity Framework 7.

The pt and pt-BR resources for Csla.dll have been updated. **Other languages need updates as well - please contribute if you are a native speaker!**

There is a new way to customize the server-side data portal by implementing an interceptor that is invoked via the new `DataPortalBroker`. ([#564](https://github.com/marimerllc/csla/issues/564))

This release is available via NuGet.

* [#550](https://github.com/marimerllc/csla/issues/550) Add Xamarin.Forms support in Csla.Xaml 
* [#568](https://github.com/marimerllc/csla/issues/568) Add PCL support for Profile111 (Xamarin.Forms and .NET Core) 
* [#457](https://github.com/marimerllc/csla/issues/457) Analyzer: Find Usages of Non-Serializable Types 
* [#458](https://github.com/marimerllc/csla/issues/458) Analyzer: Catch when Save() is Called on Itself 
* [#553](https://github.com/marimerllc/csla/issues/553) Analyzer: Address => Property Syntax in Analyzers 
* [#555](https://github.com/marimerllc/csla/issues/555) Analyzer: Fix Issue with Partial Classes in Analyzers 
* [#518](https://github.com/marimerllc/csla/issues/518) EF7 Support 
* [#539](https://github.com/marimerllc/csla/issues/539) Fix WinRT bug with OnDeserializedHandler
* [#576](https://github.com/marimerllc/csla/issues/576) Update Resources pt and pt-BR 
* [#564](https://github.com/marimerllc/csla/issues/564) Add DataPortalBroker to allow DataPortal Interception 
* [#574](https://github.com/marimerllc/csla/issues/574) Fix issue with HttpProxy and Xamarin
