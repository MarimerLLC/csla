I am pleased to announce the release of CSLA .NET version 4.9.0.

### Dependabot and NuGet dependencies
We've started using Dependabot to help keep NuGet dependencies current. As a result, the following updates are included in this release:

* Bump Microsoft.EntityFrameworkCore from 2.0.2 to 2.1.4 in /Source 
* Bump System.Security.Principal.Windows from 4.5.0 to 4.5.1 in /Source 
* Bump System.Data.SqlClient from 4.4.3 to 4.5.1 in /Source 
* Bump Microsoft.AspNetCore from 2.0.2 to 2.1.6 in /Source 
* Bump Microsoft.AspNet.WebPages from 2.0.20710 to 3.2.7 in /Source 
* Bump System.ComponentModel.Annotations from 4.4.1 to 4.5.0 in /Source 
* Bump Microsoft.AspNetCore.Mvc from 2.0.3 to 2.1.3 in /Source 
* [Security] Bump Microsoft.NETCore.UniversalWindowsPlatform 

### Docs and wiki
We've also updated some of the docs and officially removed the wiki. All the wiki/docs content is now in the [/docs directory](https://github.com/MarimerLLC/csla/blob/master/docs/index.md).

### Data portal enhancements

* [#993](https://github.com/marimerllc/csla/issues/993) Implement `SaveAndMergeAsync` methods 
* [#972](https://github.com/marimerllc/csla/issues/972) Implement data portal router
* [#959](https://github.com/marimerllc/csla/issues/959) Enhance data portal to use different endpoint per business type
* [#961](https://github.com/marimerllc/csla/issues/961) Create data portal health/instrumentation endpoint
* [#1011](https://github.com/marimerllc/csla/issues/1011) Implement `IsOffline` property for data portal
* [#982](https://github.com/marimerllc/csla/issues/982) Add new `IDataPortalFactory` singleton for page injection in ASP.NET Core

### Configuration enhancements

* [#966](https://github.com/marimerllc/csla/issues/966) Add fluent config types to CSLA: `Csla.Configuration.CslaConfiguration`
* [#967](https://github.com/marimerllc/csla/issues/967) Read CSLA config values from .NET Core config subsystem: `CslaConfigurationOptions`
* [#1012](https://github.com/marimerllc/csla/issues/1012) Implement `ConfigureCsla` extension method to `IConfiguration` for use in .NET Core configuration
* [#982](https://github.com/marimerllc/csla/issues/982) Implement `AddCsla` method for use in ASP.NET Core `ConfigureServices` 

### Bug fixes

* [#949](https://github.com/marimerllc/csla/issues/949) [samples] Fix WinForms and WPF DataPortal configuration to use Azure
* [#956](https://github.com/marimerllc/csla/issues/956) [bug] Check for null User in authz rules 
* [#962](https://github.com/marimerllc/csla/issues/962) [bug] Object not serializable (Csla.Core.ContextDictionary) after compiled with .Net Native tool chain and Optimize Code Enabled

You can see all the [closed work items in GitHub](https://github.com/MarimerLLC/csla/projects/4).
