// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using Csla;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TransformationRules
{
  /// <summary>
  /// The program.
  /// </summary>
  class Program
  {
    public static ApplicationContext ApplicationContext { get; set; }

    /// <summary>
    /// The main.
    /// </summary>
    /// <param name="args">
    /// The args.
    /// </param>
    static void Main(string[] args)
    {
      var services = new ServiceCollection();
      services.AddCsla();
      var provider = services.BuildServiceProvider();
      ApplicationContext = provider.GetRequiredService<ApplicationContext>();

      var root = Root.NewEditableRoot();

      root.Name = "RoCkY        lHoTkA";
      Console.WriteLine("set value: \"{0}\" and actual is \"{1}\"", "RoCkY        lHoTkA", root.Name);
      Console.WriteLine();
      Console.WriteLine("About to set Num1,  old value: {0}", root.Num1);
      root.Num1 = 8;
      Console.WriteLine("Num1 set to: {0} and Sum is {1}", root.Num1, root.Sum);
      Console.WriteLine();
      Console.WriteLine("About to set Num2, old value: {0}", root.Num2);
      root.Num2 = 14;
      Console.WriteLine("set Num2: {0} and Sum is {1}", root.Num2, root.Sum);
      Console.WriteLine();
      Console.WriteLine("Press <ENTER> to continue.");
      Console.ReadLine();

    }
  }
}
