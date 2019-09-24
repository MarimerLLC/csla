using System;
using System.Threading.Tasks;
using Csla;

namespace DataPortalFactoryExample
{
  class Program
  {
    static async Task Main(string[] args)
    {
      var obj = await DataPortal.CreateAsync<PersonEdit>();
      Console.WriteLine($"Person {obj.Name}");
      Console.ReadLine();
    }
  }
}
