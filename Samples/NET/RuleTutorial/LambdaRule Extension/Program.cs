// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.ComponentModel;
using System.Linq;

namespace LambdaRules
{
  /// <summary>
  /// The program.
  /// </summary>
  class Program
  {
    /// <summary>
    /// The main.
    /// </summary>
    /// <param name="args">
    /// The args.
    /// </param>
    static void Main(string[] args)
    {
      var root = Root.NewEditableRoot();
      var idei = (IDataErrorInfo)root;

      var err1 = idei[Root.Num1Property.Name];
      var err2 = idei[Root.Num2Property.Name];

      Console.WriteLine("NEW Root object is {0} valid", root.IsValid ? string.Empty : "not");
      Console.WriteLine("\"{0}\" has error message \"{1}\"", Root.Num1Property.Name, err1);
      Console.WriteLine("\"{0}\" has error message \"{1}\"", Root.Num2Property.Name, err2);

      // rule for Num2 has severity warning 
      var broken = root.BrokenRulesCollection.Where(p => p.Property == Root.Num2Property.Name);
      foreach (var brokenRule in broken)
      {
        Console.WriteLine("\"{0}\" has {2} message \"{1}\"", Root.Num2Property.Name, brokenRule.Description, brokenRule.Severity);
      }

      Console.WriteLine();


      root.Num1 = 4;
      root.Num2 = 8;
      err1 = idei[Root.Num1Property.Name];
      err2 = idei[Root.Num2Property.Name];
      
      Console.WriteLine("EXISTING Root object is {0} valid", root.IsValid ? string.Empty : "not");
      Console.WriteLine();
      Console.WriteLine("Root object is {0} valid", root.IsValid ? string.Empty : "not");
      Console.WriteLine("\"{0}\" has error message \"{1}\"", Root.Num1Property.Name, err1);
      Console.WriteLine("\"{0}\" has error message \"{1}\"", Root.Num2Property.Name, err2);


      Console.WriteLine();
      Console.WriteLine("Press <ENTER> to continue.");
      Console.ReadLine();
    }
  }
}
