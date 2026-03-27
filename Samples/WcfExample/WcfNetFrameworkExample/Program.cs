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
wcfHost.Opened += (o, e) => Console.WriteLine($"WCF Data Portal now listening on {wcfHost.Description.Endpoints.First().Address.Uri}");

wcfHost.Open();

Console.ReadLine();