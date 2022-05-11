# CSLA 6 releases

CSLA 6 is a major new version of CSLA .NET, fully supporting (and requiring) dependency injection and other features of modern .NET.

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
