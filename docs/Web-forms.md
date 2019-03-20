## CSLA 4 ApplicationContext in ASP.NET host
When referencing CSLA .NET in your web projects make sure to use the NuGet package for the ASP.NET environment you will be using. This means the `CSLA-ASP.NET` package for Web Forms, or the appropriately versioned MVC package if you are using MVC. The MVC packages bring in `Csla.Web.dll` automatically to provide basic ASP.NET support.

The ApplicationContext object must be managed differently in ASP.NET than other environments. In CSLA 4, Csla.dll doesn't (can't) reference System.Web.dll, so correct management of ApplicationContext for ASP.NET is in Csla.Web.dll. **All ASP.NET hosted code must reference Csla.Web.dll** to function correctly in CSLA 4. [More info...](https://cslanet.com/old-forum/9583.html)

## Using a web host to run a CSLA .NET app
I'm trying to run my Csla web application on a hosted web server, but I'm getting SecurityExceptions stating that the assembly does not allow partially trusted callers.

Answer:  Csla requires full trust to run because of its use of reflection, dynamic method invocation, WCF and/or Remoting and other high-end .NET features.  You'll need to grant Csla full trust to run.  Please see [this thread](https://cslanet.com/old-forum/4401.html) for more details.

Here are a list of hosting sites that may provide full trust:

* [discountasp.net](http://discountasp.net)<br>
* [orcsweb.com](http://www.orcsweb.com/)<br>
* [webhost4life.com](http://www.webhost4life.com)

## Type is not resolved for custom principal in web app
I'm getting a System.Runtime.Serialization.SerializationException: Type is not resolved for member 'MyProject.MyLibrary.MyCustomPrincipal, MyProject.MyLibrary' in my Asp.Net application.

Answer:  You are likely running the application through Cassini (the ASP.NET Development Web Server) and not IIS.  Due to limitations in Cassini, you cannot run Csla applications through it unless you install your assembly into the GAC.  For more information please see the [Install document](http://www.lhotka.net/cslanet/download.aspx) and [Rocky's explanation](http://www.lhotka.net/weblog/UpdateOnMyStrugglesWithTheASPNETDevelopmentServer.aspx).

## How can I use the ASP.NET provider model with a CSLA principal?
[http://forums.lhotka.net/forums/thread/14844.aspx](https://cslanet.com/old-forum/477.html)

## How do I sort/filter/page data with CslaDataSource?
[This forum post](https://cslanet.com/old-forum/8629.html) has a good summary of the options.
