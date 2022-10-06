# CSLA 6 releases

CSLA 6 is a major new version of CSLA .NET, fully supporting (and requiring) dependency injection and other features of modern .NET.

## CSLA .NET version 6.2.0 release

CSLA .NET version 6.2.0 enhances `ExecuteAsync` to accept parameters, includes bug fixes, and numerous dependency version updates.

* [#1775](https://github.com/MarimerLLC/csla/issues/1775) Enhance data portal `ExecuteAsync` to allow passing parameters
* [#3100](https://github.com/MarimerLLC/csla/issues/3100) Linux build issue fix
* [#2975](https://github.com/MarimerLLC/csla/issues/2975) Linux build issue fix
* [#3083](https://github.com/MarimerLLC/csla/issues/3083) `NameValueList` serialization fix
* [#3044](https://github.com/MarimerLLC/csla/issues/3044) Don't use `WindowsPrincipal` on unsupported platforms
* [#3035](https://github.com/MarimerLLC/csla/issues/3035) Add `ViewModelBase` type to `Csla.Maui` namespace

### Change List

* [Changes in this release](https://github.com/MarimerLLC/csla/issues?q=project%3Amarimerllc%2F6+is%3Aclosed+)

### Contributors

@ajj7060
@bujma
@jasonBock
@rockfordlhotka
@TheCakeMonster

## CSLA .NET version 6.1.0 release

CSLA .NET version 6.1.0 adds support for Maui and includes bug fixes and one breaking change.

* [#2549](https://github.com/MarimerLLC/csla/issues/2549) Add UI helpers for Maui
* [#2672](https://github.com/MarimerLLC/csla/issues/2672) Enhance PropertyChanged behavior in `Csla.Xaml.ViewModelBase`
* [#2764](https://github.com/MarimerLLC/csla/issues/2764) Update snippets NuGet installer for VS2022
* [#2922](https://github.com/MarimerLLC/csla/issues/2922) Blazor no longer requires text-based serialization
* [#2946](https://github.com/MarimerLLC/csla/issues/2946) LocalProxy now properly disposes DI scope and dependencies
* [#2953](https://github.com/MarimerLLC/csla/issues/2953) DataAnnotations rules now support DI
* [#2957](https://github.com/MarimerLLC/csla/issues/2957) Child data portal expections now flow up with details
* [#2981](https://github.com/MarimerLLC/csla/issues/2981) You can now get a `System.Reflection.PropertyInfo` from `Csla.Blazor.PropertyInfo`
* [#3007](https://github.com/MarimerLLC/csla/issues/3007) Add new BlazorServerExample project to the `Samples` folder
* [#3014](https://github.com/MarimerLLC/csla/issues/3014) Fix bug in child data portal
* [#3025](https://github.com/MarimerLLC/csla/issues/3025) Fix issue with analyzers

### Change List

* [Changes in this release](https://github.com/MarimerLLC/csla/issues?q=is%3Aclosed+project%3Amarimerllc%2F3+)
* [Breaking changes](https://github.com/MarimerLLC/csla/issues?q=is%3Aissue+is%3Aclosed+project%3Amarimerllc%2F3+label%3A%22flag%2Fbreaking+change%22)

### Dependency Updates

* Grpc.Net.Client
* Grpc.Tools
* Newtonsoft.Json
* RabbitMQ.Client
* Microsoft.NETCore.UniversalWindowsPlatform
* Xamarin.Forms
* Microsoft.AspNet.WebApi.Client
* Microsoft.AspNet.WebPages
* Microsoft.Web.Infrastructure
* Microsoft.AspNet.WebApi.Core
* Microsoft.AspNet.Mvc
* Microsoft.AspNet.Razor
* Google.Protobuf
* Microsoft.CodeAnalysis.CSharp

### Contributors

@joshhanson314
@rockfordlhotka
@TheCakeMonster
@jasonbock

## CSLA .NET version 6.0.0 release

CSLA .NET version 6.0.0 is a major release with numerous breaking changes, including:

* Business domain types must have a public constructor
* Public constructors for many types will have parameters provided via depedency injection
* Support for .NET 4.0 and 4.5 has been dropped; .NET 4.6.2 is the minimum required
* The data and object context managers in `Csla.Data` have been removed in favor of using dependency injection

This version supports:

* .NET 6
* .NET Framework 4.6.2 to 4.8
* netstandard 2.0 and 2.1
* Blazor
* Xamarin
* mono

Operating Systems and Platforms:

* Windows (servers and UWP, WPF, Windows Forms)
* Linux (servers and Xamarin)
* iOS and Android (Xamarin)
* Mac (servers and Xamarin)
* Kubernetes and other container-based runtimes
* ASP.NET Core and ASP.NET 5
* Other client and server environments where .NET Core or mono are available

### Change List

* [Changes in this release](https://github.com/MarimerLLC/csla/issues?q=is%3Aclosed+project%3Amarimerllc%2Fcsla%2F11+)
* [Breaking changes](https://github.com/MarimerLLC/csla/issues?q=is%3Aclosed+project%3Amarimerllc%2Fcsla%2F11+label%3A%22flag%2Fbreaking+change%22).

### Upgrade documentation

There is a document describing common issues people will likely encounter when upgrading from CSLA 5 to CSLA 6.

* [Upgrading to CSLA 6](https://github.com/MarimerLLC/csla/blob/main/docs/Upgrading%20to%20CSLA%206.md)

### Contributors

The CSLA community is fantastic! The people who've submitted bugs, helped think through solutions, lobbied for features and enhancements, and submitted pull requests are listed below.

@455986780
@adrianwright109
@ajohnstone-ks
@Art666OTS
@BaHXeLiSiHg
@coder-rr
@danielmartind
@dazinator
@devcs21
@dotMorten
@Eduardo-Micromust
@GreatBarrier86
@j055
@JacoJordaan
@JasonBock
@jhnsartain3
@jonparker
@joshhanson314
@kellyethridge
@michaelcsikos
@MTantos1
@peranborkett
@poepoe12002
@ProDInfo
@RaviPatelTheOne
@rfcdejong
@rockfordlhotka
@russblair
@SachinPNikam
@swegele
@TheCakeMonster

Thank you all so much for your support!
