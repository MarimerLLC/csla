using Csla;
using Csla.Configuration;
using DataPortalCacheExample;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
// use standard memory cache
services.AddMemoryCache();
// use CSLA with client-side data portal cache
services.AddCsla(o => o
  .DataPortal(o => o
    .ClientSideDataPortal(o => o
      .DataPortalCacheType = typeof(DataPortalCache))));
var provider = services.BuildServiceProvider();

// test code
var portal = provider.GetRequiredService<IDataPortal<ReferenceData>>();
var obj = portal.Create(1);
Console.WriteLine($"{obj} from data portal");
obj = portal.Create(1);
Console.WriteLine($"{obj} from data portal");
obj = portal.Create(2);
Console.WriteLine($"{obj} from data portal");
obj = portal.Create(2);
Console.WriteLine($"{obj} from data portal");

Console.WriteLine($"{Environment.NewLine}Waiting...");
await Task.Delay(TimeSpan.FromSeconds(6));

obj = portal.Create(1);
Console.WriteLine($"{obj} from data portal");
obj = portal.Create(2);
Console.WriteLine($"{obj} from data portal");
