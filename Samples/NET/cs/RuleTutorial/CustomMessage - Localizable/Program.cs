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

namespace CustomMessageLocalizable
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

      Console.WriteLine("Root object is {0} valid", root.IsValid ? string.Empty : "not");
      var err1 = idei[Root.NameProperty.Name];
      var err2 = idei[Root.Num1Property.Name];
      Console.WriteLine("\"{0}\" has custom error message \"{1}\"", Root.NameProperty.Name, err1);
      Console.WriteLine("\"{0}\" has custom error message \"{1}\"", Root.Num1Property.Name, err2);
      Console.WriteLine();
      Console.WriteLine("Setting name to a long string.");
      root.Name = string.Empty.PadRight(55, 'a');
      err1 = idei[Root.NameProperty.Name];
      Console.WriteLine("\"{0}\" has custom error message \"{1}\"", Root.NameProperty.Name, err1);
      Console.WriteLine();
      Console.WriteLine("Now setting valid values into BO.");

      // set valid values
      root.Name = "rocky lhotka";
      root.Num1 = 55;
      err1 = idei[Root.NameProperty.Name];
      err2 = idei[Root.Num1Property.Name];
      Console.WriteLine();
      Console.WriteLine("Root object is {0} valid", root.IsValid ? string.Empty : "not");
      Console.WriteLine("\"{0}\" has no error message \"{1}\"", Root.NameProperty.Name, err1);
      Console.WriteLine("\"{0}\" has no error message \"{1}\"", Root.Num1Property.Name, err2);
      Console.WriteLine();
      Console.WriteLine("Press <ENTER> to continue.");
      Console.ReadLine();
    }
  }
}
