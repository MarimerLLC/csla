I am pleased to announce the release of CSLA .NET version 4.8.0.

🛑 **This is a big change, because I was able to eliminate the `CSLA-Core-NS` package. If you are using the `CSLA-Core-NS` package you will need to remove that reference and add a reference to the `CSLA-Core` package.**

⚠ If you have any custom rules (subclass of `BusinessRule` or implement `IBusinessRule` you will need to update all `Execute` methods to accept an `IRuleContext` parameter instead of a `RuleContext` parameter.

There are a number of other changes and bug fixes including:

* [#927](https://github.com/MarimerLLC/csla/issues/927) Eliminate `CSLA-Core-NS` package 🛑
* [#703](https://github.com/MarimerLLC/csla/issues/703) Allow use of `BinaryFormatter` in .NET Standard 2.0 and .NET Core
* [#344](https://github.com/MarimerLLC/csla/issues/344) Stop caching `DisplayName` value so localization works as expected
* [#827](https://github.com/MarimerLLC/csla/issues/827) Eliminate `NullReferenceException` in `Rule.Complete` ⚠
* [#340](https://github.com/MarimerLLC/csla/issues/340) Enable per-type authz rules when passing interface types to the data portal
* [#917](https://github.com/MarimerLLC/csla/issues/917) Fix issue with non-default timeout in `HttpProxy`
* [#635](https://github.com/MarimerLLC/csla/issues/635) Move wiki to the `/docs` directory so it is part of the repo
* [#650](https://github.com/MarimerLLC/csla/issues/650) Better exception when incorrect CslaDataPortalProxy is specified 
* [#938](https://github.com/MarimerLLC/csla/issues/938) Simplify version numbering so we can start using semver
* [#914](https://github.com/MarimerLLC/csla/issues/914) WARNING 0xdef01051 No default or neutral resource given for UWP

You can see all the [closed work items in GitHub](https://github.com/MarimerLLC/csla/milestone/27?closed=1).

💡 After this release I'll start officially using the semver (semantic versioning) concept, so breaking changes will be more clearly noted via version changes. In this release I've started that process by using a .0 instead of .100 for the patch number. 
