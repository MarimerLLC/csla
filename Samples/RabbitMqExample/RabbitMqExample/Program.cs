using Csla;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMqBusiness;

var services = new ServiceCollection();
services.AddCsla(o => o
  .DataPortal(o => o
    .AddClientSideDataPortal(o => o
      .UseRabbitMqProxy(o => o
        .DataPortalUrl = "rabbitmq://localhost:5672/myservice"))));
var serviceProvider = services.BuildServiceProvider();

Console.WriteLine("RabbitMq Example starting");
var portal = serviceProvider.GetRequiredService<IDataPortal<PersonEdit>>();
var person = await portal.FetchAsync("Abdi");
Console.WriteLine("Person fetched");
Console.WriteLine(person.Name);
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
