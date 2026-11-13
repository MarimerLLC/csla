using System.Security.Claims;
using Csla;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NullablePropertiesExample
{
  class Program
  {
    static async Task Main(string[] args)
    {
      var services = new ServiceCollection();
      services.AddCsla();
      var provider = services.BuildServiceProvider();

      var applicationContext = provider.GetRequiredService<ApplicationContext>();
      var portal = provider.GetRequiredService<IDataPortal<OrderEdit>>();

      Console.WriteLine("Anonymous user");
      var order = await portal.FetchAsync(42);
      Console.WriteLine($"Customer: {order.CustomerName}");

      // Synchronous lazy load: the first read fetches the list and returns it.
      Console.WriteLine($"Line items: {order.LineItems.Count}");

      // Asynchronous lazy load: the first read starts the fetch and returns null.
      Console.WriteLine($"Shipment: {order.Shipment?.Carrier ?? "(loading)"}");
      await order.WaitForIdle();
      Console.WriteLine($"Shipment: {order.Shipment?.Carrier ?? "(loading)"}");

      // Read authorization: an anonymous user gets null.
      Console.WriteLine($"Credit limit: {order.Credit?.Limit.ToString("C") ?? "(not authorized)"}");

      Console.WriteLine();
      Console.WriteLine("Manager");
      applicationContext.User = new ClaimsPrincipal(
        new ClaimsIdentity([new Claim(ClaimTypes.Name, "manager"), new Claim(ClaimTypes.Role, "Manager")], "Sample"));
      order = await portal.FetchAsync(42);
      Console.WriteLine($"Credit limit: {order.Credit?.Limit.ToString("C") ?? "(not authorized)"}");
    }
  }
}
