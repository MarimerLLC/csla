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

      // Not-null child: Create loads it, so a new order has an (empty) list.
      var newOrder = await portal.CreateAsync();
      Console.WriteLine($"New order line items: {newOrder.LineItems.Count}");
      Console.WriteLine();

      Console.WriteLine("Anonymous user");
      var order = await portal.FetchAsync(42);
      Console.WriteLine($"Customer: {order.CustomerName}");

      // Not-null child: Fetch loads it, so the getter returns it without !.
      Console.WriteLine($"Line items: {order.LineItems.Count}");

      // Synchronous lazy load: the first read fetches the history and returns it.
      Console.WriteLine($"History: {order.History.Summary}");

      // Asynchronous lazy load: the first read starts the fetch and returns null.
      Console.WriteLine($"Shipment: {order.Shipment?.Carrier ?? "(loading)"}");
      await order.WaitForIdle();
      Console.WriteLine($"Shipment: {order.Shipment?.Carrier ?? "(loading)"}");

      // Read authorization: an anonymous user gets null.
      Console.WriteLine($"Credit limit: {order.Credit?.Limit.ToString("C") ?? "(not authorized)"}");

      // A not-null string stores null as string.Empty, as when a data-bound
      // control writes null, so its getter still doesn't return null.
      order.CustomerName = null!;
      Console.WriteLine($"Customer after writing null: \"{order.CustomerName}\"");

      Console.WriteLine();
      Console.WriteLine("Manager");
      applicationContext.User = new ClaimsPrincipal(
        new ClaimsIdentity([new Claim(ClaimTypes.Name, "manager"), new Claim(ClaimTypes.Role, "Manager")], "Sample"));
      order = await portal.FetchAsync(42);
      Console.WriteLine($"Credit limit: {order.Credit?.Limit.ToString("C") ?? "(not authorized)"}");
    }
  }
}
