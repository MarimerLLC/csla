using BenchmarkDotNet.Running;
using Csla;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataPortalPerf
{
  class Program
  {
    static void Main()
    {
      var services = new ServiceCollection();
      services.AddCsla();
      var provider = services.BuildServiceProvider();
      ApplicationContext = provider.GetService<ApplicationContext>();

      BenchmarkRunner.Run<RetrieveEntities>();
    }

    public static ApplicationContext ApplicationContext { get; set; }
  }
}
