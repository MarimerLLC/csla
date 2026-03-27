using BusinessLibrary;
using Csla;
using Csla.Channels.Wcf.Client;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services
  .AddCsla(csla => csla
  .DataPortal(dp => dp
  .AddClientSideDataPortal(c => c
  .UseWcfProxy())));

var provider = services.BuildServiceProvider();
var applicationContext = provider.GetRequiredService<ApplicationContext>();
var portal = applicationContext.GetRequiredService<IDataPortal<Person>>();

var user = applicationContext.User;

int personId = 1;
var p = portal.Fetch(personId);

Console.WriteLine(p.Name);
Console.ReadLine();
