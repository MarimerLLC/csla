using System;
using System.Threading.Tasks;
using Csla;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataPortalFactoryExample
{
  class Program
  {
    static async Task Main(string[] args)
    {
      var services = new ServiceCollection();
      services.AddCsla();
      var provider = services.BuildServiceProvider();

      var portal = provider.GetRequiredService<IDataPortal<PersonEdit>>();
      var obj = await portal.CreateAsync();
      Console.WriteLine($"Person {obj.Name}");
      Console.ReadLine();
    }
  }
}
