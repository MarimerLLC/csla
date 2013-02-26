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
using System.Linq;
using System.Text;

namespace AuthzReadWriteProperty
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

      Console.WriteLine("Root has authz rules:");

      Console.WriteLine("CanReadProperty(Name): {0}", root.CanReadProperty(Root.NameProperty.Name));
      Console.WriteLine("CanReadProperty(Num1): {0}", root.CanReadProperty(Root.Num1Property.Name));
      Console.WriteLine("CanReadProperty(Num2): {0}", root.CanReadProperty(Root.Num2Property.Name));
      Console.WriteLine("CanWriteProperty(Name): {0}", root.CanWriteProperty(Root.NameProperty.Name));
      Console.WriteLine("CanWriteProperty(Num1): {0}", root.CanWriteProperty(Root.Num1Property.Name));
      Console.WriteLine("CanWriteProperty(Num2): {0}", root.CanWriteProperty(Root.Num2Property.Name));
      Console.WriteLine();

      Console.WriteLine("Root has the following values (apparently)");
      Console.WriteLine("Name: {0}", root.Name);
      Console.WriteLine("Num1: {0} - is actually 1001 but the registered default value of 9 is returned", root.Num1);
      Console.WriteLine("Num2: {0} - is actually 666 but the registered default value of 0 is returned", root.Num2);

      try
      {
        root.Name = "Customer name";
      }
      catch (Exception ex)
      {
        Console.WriteLine("Name setter: {0}", ex.Message);
      }

      try
      {
        root.Num1 = 555;
      }
      catch (Exception ex)
      {
        Console.WriteLine("Num1 setter: {0}", ex.Message);
      }

      try
      {
        root.Num2 = 555;
      }
      catch (Exception ex)
      {
        Console.WriteLine("Num2 setter: {0}", ex.Message);
      }

      Console.WriteLine("Press <enter> to continue.");
      Console.ReadLine();
    }
  }
}
