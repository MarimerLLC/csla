using System;
using System.ComponentModel;
using Csla;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AddonPropertyRules
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

      var root = Root.NewEditableRoot();
      var idei = (IDataErrorInfo)root;

      root.PropertyChanged += (o, e) =>
      {
        Console.WriteLine("  >PropertyChanged: {0}", e.PropertyName);
      };

      Console.WriteLine("Root object is {0} valid", root.IsValid ? string.Empty : "not");
      var err1 = idei[Root.Num1Property.Name];
      Console.WriteLine("\"{0}\" has error message \"{1}\"", Root.Num1Property.Name, err1);
      Console.WriteLine();
      Console.WriteLine("Now setting valid values into BO.");

      root.Num1 = 5;

      err1 = idei[Root.Num1Property.Name];
      Console.WriteLine("\"{0}\" has error message \"{1}\"", Root.Num1Property.Name, err1);
      Console.WriteLine("Root object is {0} valid", root.IsValid ? string.Empty : "not");
      Console.WriteLine();


      Console.WriteLine("Before set String1");
      root.String1 = "Rocky";
      Console.WriteLine("Before set String2");
      root.String2 = "Lhotka";
      
      Console.ReadLine();
    }
  }
}
