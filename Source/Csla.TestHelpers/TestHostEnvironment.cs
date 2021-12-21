//-----------------------------------------------------------------------
// <copyright file="TestHostEnvironment.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Helper class to support testing of behaviour under different hosting configurations</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Csla.TestHelpers
{

  /// <summary>
  /// Basic implementation of IHostEnvironment, to support testing of anything 
  /// that is dependent upon there being an implementation available
  /// </summary>
  public class TestHostEnvironment : IHostEnvironment
  {

    /// <summary>
    /// The name of the environment in which the application is being run
    /// </summary>
    public string EnvironmentName { get; set; } = "Production";

    /// <summary>
    /// The name of the application
    /// </summary>
    public string ApplicationName { get; set; } = "ApplicationName";

    /// <summary>
    /// The path to the content root
    /// </summary>
    public string ContentRootPath { get; set; } = @"C:\Windows\Temp";

    /// <summary>
    /// The file provider to the content root; null by default
    /// </summary>
    public IFileProvider ContentRootFileProvider { get; set; }

  }
}
