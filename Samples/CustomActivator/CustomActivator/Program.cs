using System;
using Csla;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CustomActivator
{
  class Program
  {
    static void Main(string[] args)
    {
      var services = new ServiceCollection();
      services.AddCsla(opt => opt
        .DataPortal(dpo => dpo
          .AddServerSideDataPortal(sso => sso
            .RegisterActivator<CustomActivator>()
            .AddInterceptorProvider<CustomIntercepter>()
            )
          )
        );
      var provider = services.BuildServiceProvider();
      var portal = provider.GetRequiredService<IDataPortal<TestItem>>();

      var obj = portal.Fetch("Rocky");
      Console.WriteLine(obj.Name);
      Console.ReadLine();
    }
  }
}
