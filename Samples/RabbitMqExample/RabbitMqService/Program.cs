using Csla.Channels.RabbitMq;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddCsla(o => o.
DataPortal(o => o.
  AddServerSideDataPortal(o => o.
    UseRabbitMqPortal(o => o.
      DataPortalUri = new Uri("rabbitmq://rabbithost/myservice")))));
var serviceProvider = services.BuildServiceProvider();

Console.WriteLine("RabbitMq Service starting");

var rabbitMqService = serviceProvider.GetRequiredService<RabbitMqPortal>();
rabbitMqService.StartListening();

Console.WriteLine("Press any key to exit");
Console.ReadKey();
