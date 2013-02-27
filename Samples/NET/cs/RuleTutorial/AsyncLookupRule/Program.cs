// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System;

namespace AsyncLookupRule
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
      root.ValidationComplete += (o, e) =>
                                   {
                                     var obj = (Root) o;
                                     Console.WriteLine("Rules completed, Name=\"{0}\"", obj.Name);
                                   };
      Console.WriteLine("About to set CustomerId, Name=\"{0}\"", root.Name);
      root.CustomerId = 1134;
      Console.WriteLine();
      Console.WriteLine("Press <ENTER> to continue. (Wait while async rule runs)");
      Console.ReadLine();
    }
  }
}
