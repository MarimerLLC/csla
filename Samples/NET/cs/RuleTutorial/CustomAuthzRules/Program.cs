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
      var idei = (IDataErrorInfo)root;

      Console.WriteLine("Root has authz rules:");

      Console.WriteLine("CanWriteProperty(State): {0}", root.CanWriteProperty(Root.StateProperty.Name));
      Console.WriteLine("State error info: {0}", idei[Root.StateProperty.Name]);
      Console.WriteLine();

      Console.WriteLine("Setting Country to \"US\"");
      root.Country = "US";

      Console.WriteLine("CanWriteProperty(State): {0}", root.CanWriteProperty(Root.StateProperty.Name));
      Console.WriteLine("State error info: {0}", idei[Root.StateProperty.Name]);

      Console.WriteLine();
      Console.WriteLine("Setting Country to \"NO\"");
      root.Country = "NO";

      Console.WriteLine("CanWriteProperty(State): {0}", root.CanWriteProperty(Root.StateProperty.Name));
      Console.WriteLine("State error info: {0}", idei[Root.StateProperty.Name]);

      Console.WriteLine("Press <enter> to continue.");
      Console.ReadLine();
    }
  }
}
