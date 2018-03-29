I am pleased to announce the release of CSLA .NET version 4.7.100 with full support for netstandard 2.0 and .NET Core 2. 

The packages are now in NuGet. Once some final updates to the samples are complete and merged into master I'll create a formal release tag/page on GitHub.

This release also includes some other very exciting capabilities, including:

* [#760](https://github.com/MarimerLLC/csla/issues/760) Add support for ASP.NET Core 2.0
* [#759](https://github.com/MarimerLLC/csla/issues/759) Add support for EF Core (EntityFrameworkCore)
* [#813](https://github.com/MarimerLLC/csla/issues/813) Major performance improvement when data binding to large object graphs thanks to [keithdv](https://github.com/keithdv)
* [#795](https://github.com/MarimerLLC/csla/issues/795) Add `Transactional` attribute back into netstandard 2.0 code 
* [#800](https://github.com/MarimerLLC/csla/issues/800) Changes to configuration so it is possible to configure CSLA without any `web.config` or `app.config` files (such as in .NET Core, etc.)
* [#496](https://github.com/MarimerLLC/csla/issues/496) Support `ClaimsPrincipal` via new `CslaClaimsPrincipal` type
* [#729](https://github.com/MarimerLLC/csla/issues/729) `ApplicationContext` now defaults to using `AsyncLocal` to maintain values on the current thread/context with help from [j055](https://github.com/j055) **BREAKING CHANGE**
* [#712](https://github.com/MarimerLLC/csla/issues/712) Support in-place deserialization of an object graph
* [#748](https://github.com/MarimerLLC/csla/issues/748) Major improvements to serialization via `MobileFormatter` thanks to [jasonbock](https://github.com/JasonBock)
* [#763](https://github.com/MarimerLLC/csla/issues/763) Update to samples thanks to [tfreitasleal](https://github.com/tfreitasleal)
* [#688](https://github.com/MarimerLLC/csla/issues/688) Get `ApplicationContext.User` authentication working with ASP.NET Core thanks to [dazinator](https://github.com/dazinator)
* [#766](https://github.com/MarimerLLC/csla/issues/766) Update to use latest UWP libraries for Windows 10 Fall Creators Update **BREAKING CHANGE**
* [#790](https://github.com/MarimerLLC/csla/issues/790) BUG: Fix `AmbiguousMatchException` in data portal thanks to [iherwald](https://github.com/iherwald)
* [#710](https://github.com/MarimerLLC/csla/issues/710) BUG: Fix ambiguous `Save` method issue thanks to [rabidkitten](https://github.com/rabidkitten)
 
There are a couple known issues with this release:

* [#818](https://github.com/MarimerLLC/csla/issues/818) Xamarin projects using `System.Data.SqlClient` show a warning about this assembly's location
* [#794](https://github.com/MarimerLLC/csla/issues/794) UWP projects show `warning PRI263: 0xdef01051` messages relative to CSLA resource strings
* [#822](https://github.com/MarimerLLC/csla/issues/822) There are two "sets" of CSLA packages/assemblies: one for full .NET 4.6.1+ and one for netstandard (including Xamarin, UWP, .NET Core, etc.) due to a type error between netstandard and full .NET
* [#703](https://github.com/MarimerLLC/csla/issues/703) Though netstandard supports BinaryFormatter, that is currently not an option from the CSLA configuration, and this needs to be addressed

Regarding the NuGet/assembly split noted in [#822](https://github.com/MarimerLLC/csla/issues/822):

Right now there are two "families" of CSLA .NET packages in NuGet. One that supports full .NET and one that supports all other runtimes.

The full .NET family must be used for the following types of app:
* Windows Forms
* WPF
* ASP.NET (other than Core)
* Console apps (other than Core)
* Azure Functions
* Any other runtime hosted by full .NET 4.6.1+

The netstandard family must be used for the following types of app:
* Xamrin
* UWP
* .NET Core
* ASP.NET Core
* Any other runtime _not_ hosted by full .NET 4.6.1+

What this means for you is that if your n-tier app is 100% full .NET or full netstandard then you can live within one of those families. BUT if your server is full .NET and your client is Xamarin (for example) then your _business library_ assemblies need to be compiled twice: once for full .NET and once for netstandard.

The `Samples\ProjectTracker` app shows how this is done by using a Shared Project to contain the code, and two concrete class library projects that compile the code for full .NET and netstandard respectively.
