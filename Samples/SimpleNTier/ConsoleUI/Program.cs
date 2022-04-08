using Csla;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BusinessLibrary;

var services = new ServiceCollection();

// use local data portal
//services.AddCsla();

// use remote data portal
services.AddTransient<HttpClient>();
services.AddCsla(o => o
  .DataPortal(dp => dp
    .UseHttpProxy(hp => hp
      .DataPortalUrl = "https://localhost:44332/api/dataportal")));

var provider = services.BuildServiceProvider();
var applicationContext = provider.GetRequiredService<ApplicationContext>();


try
{
  var portal = applicationContext.GetRequiredService<IDataPortal<Order>>();
  var obj = portal.Fetch(441);
  PrintOrder(obj);
  obj.CustomerName = "oops";
  obj = obj.Save();
  PrintOrder(obj);
}
catch (Exception ex)
{
  Console.WriteLine("ERROR");
  Console.WriteLine(ex.ToString());
}


void PrintOrder(Order obj)
{
  Console.WriteLine($"Id:   {obj.Id}");
  Console.WriteLine($"Name: {obj.CustomerName}");
  Console.WriteLine();
  foreach (var item in obj.LineItems)
    Console.WriteLine($"  - {item.Name}");
  Console.WriteLine();
}