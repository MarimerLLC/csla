using Csla.Channels.RabbitMq;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddCsla(o => o.
DataPortal(o => o.
  AddServerSideDataPortal(o => o.
    UseRabbitMqPortal(o => o.
      DataPortalUri = new Uri("rabbitmq://localhost:5672/myservice")))));
var serviceProvider = services.BuildServiceProvider();

Console.WriteLine("RabbitMq Service starting");

var factory = serviceProvider.GetRequiredService<IRabbitMqPortalFactory>();
var rabbitMqService = factory.CreateRabbitMqPortal();
using (rabbitMqService)
{
  rabbitMqService.StartListening();
  Console.WriteLine("Press any key to exit");
  Console.ReadKey();
}
