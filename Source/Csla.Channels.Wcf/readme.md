# Windows Communication Foundation (WCF) Data Portal Channel

This data portal channel uses WCF as a means of configuring communication between data portal clients and servers.

WCF itself allows for the use of a wide array of communication protocols for different network topologies and this data portal channel can be configured to support any protocol that WCF supports.

The client is implemented using the System.ServiceModel libraries that are included in .NET Framework and can be included via package references in modern .net.

The server is implemented using the System.Servicemodel libraries in .NET Framework when using a .NET Framework target (i.e. net462, net472, net48). When targeting modern .net (i.e. net8.0+) the server is implemented using the CoreWCF packages that are designed to bring legacy WCF functionality to modern ASP.NET Core. Because there are fundamental difference between how WCF is hosted on the .NET Framework and how CoreWCF extends ASP.NET Core to host WCF services, a WCF data portal server must be hosted differently depending on the target framework of your server side project.

## Modern .net Hosting

CoreWCF allows for the hosting of WCF endpoints along side other elements of web applications and is configured in a similar way.

Here is an example Program.cs file to demonstrate a ASP.NET Core hosted data portal:
```c#
using CoreWCF.Configuration;
using CoreWCF.Description;
using Csla.Configuration;
using Csla.Channels.Wcf.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceModelServices();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();
builder.Services
    .AddCsla(csla => csla
    .DataPortal(dp => dp
    .AddServerSideDataPortal(p => p
    .UseWcfPortal())));

var app = builder.Build();

var wcfPortalOptions = app.Services.GetRequiredService<WcfPortalOptions>();

app.UseServiceModel(builder =>
{
  builder.AddService<WcfPortal>()
  .AddServiceEndpoint<WcfPortal, IWcfPortalServer>(wcfPortalOptions.Binding, wcfPortalOptions.DataPortalUrl);
});

app.Run();
```

## .NET Frameowrk Hosting

WCF Services in .NET Framework can be self hosted via the System.ServiceModel.ServiceHost class or through IIS via a .svc file. Self hosted services offer the most flexibility and are the default way to host the data portal service. It is likely that the service can be hosted via IIS by using a [ServiceHostFactory](https://learn.microsoft.com/en-us/dotnet/framework/wcf/extending/extending-hosting-using-servicehostfactory) to handle dependency injection though.

Here is an example Program.cs file to demonstrate a self hosted WCF data portal:
```c#
using Csla.Channels.Wcf.Server;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services
    .AddCsla(csla => csla
    .DataPortal(dp => dp
    .AddServerSideDataPortal(p => p
    .UseWcfPortal())));

var provider = services.BuildServiceProvider();

var wcfHost = provider.GetRequiredService<WcfPortalHost>();
wcfHost.Open();

Console.ReadLine();
```

## Client Configuration

The client configuration is the same regardless of target framework. Additionally, the client is agnostic of the server's target framework. In other words, a .NET Framework client can communicate with a ASP.NET Core hosted server and a modern .net client can communicate with a .NET Framework hosted server.

Here is an example client configuration:
```c#
services
  .AddCsla(csla => csla
  .DataPortal(dp => dp
  .AddClientSideDataPortal(c => c
  .UseWcfProxy())));
```
