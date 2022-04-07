using Csla;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RuleSubclass
{
  class Program
  {
    public static ApplicationContext ApplicationContext { get; set; }

    static void Main(string[] args)
    {
      var services = new ServiceCollection();
      services.AddCsla();
      var provider = services.BuildServiceProvider();
      ApplicationContext = provider.GetRequiredService<ApplicationContext>();
    }
  }
}
