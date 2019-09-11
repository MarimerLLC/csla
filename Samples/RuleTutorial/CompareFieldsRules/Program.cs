// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CompareFieldsRules
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
      var err2 = idei[Root.Num2Property.Name];
      var err3 = idei[Root.StartDateProperty.Name];
      var err4 = idei[Root.EndDateProperty.Name];
      Console.WriteLine("\"{0}\" has error message \"{1}\"", Root.NameProperty.Name, err1);
      Console.WriteLine("\"{0}\" has error message \"{1}\"", Root.Num2Property.Name, err2);
      Console.WriteLine("\"{0}\" has error message \"{1}\"", Root.StartDateProperty.Name, err3);
      Console.WriteLine("\"{0}\" has error message \"{1}\"", Root.EndDateProperty.Name, err4);
      Console.WriteLine();
      Console.WriteLine("Now setting valid values into BO.");

      // set valid values
      root.Name = "rocky lhotka";
      root.Num2 = 55;
      root.EndDate = DateTime.Now.AddDays(1).ToString();
      err1 = idei[Root.NameProperty.Name];
      err2 = idei[Root.Num2Property.Name];
      err3 = idei[Root.StartDateProperty.Name];
      err4 = idei[Root.EndDateProperty.Name];
      Console.WriteLine();
      Console.WriteLine("Root object is {0} valid", root.IsValid ? string.Empty : "not");
      Console.WriteLine("\"{0}\" has error message \"{1}\"", Root.NameProperty.Name, err1);
      Console.WriteLine("\"{0}\" has error message \"{1}\"", Root.Num2Property.Name, err2);
      Console.WriteLine("\"{0}\" has error message \"{1}\"", Root.StartDateProperty.Name, err3);
      Console.WriteLine("\"{0}\" has error message \"{1}\"", Root.EndDateProperty.Name, err4);
      Console.WriteLine();
      Console.WriteLine("Press <ENTER> to continue.");
      Console.ReadLine();
    }
  }
}
