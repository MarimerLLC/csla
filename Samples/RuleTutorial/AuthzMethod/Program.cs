﻿// --------------------------------------------------------------------------------------------------------------------
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

namespace AuthzMethod
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

      Console.WriteLine("This sample shows how to create Authz for your own methods on a business object.");
      try
      {
        var root = Root.NewEditableRoot();
        root.DoCalc();
      }
      catch (Exception ex)
      {
        Console.WriteLine("DoCalc: {0}", ex.Message);
      }

      Console.WriteLine("Press <enter> to continue.");
      Console.ReadLine();
    }
  }
}
