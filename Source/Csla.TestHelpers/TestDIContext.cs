//-----------------------------------------------------------------------
// <copyright file="TestDIContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Test DI context for use in tests</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.TestHelpers
{

  /// <summary>
  /// Type to carry context information for DI in unit tests
  /// </summary>
  public class TestDIContext
  {

    /// <summary>
    /// Main constructor
    /// </summary>
    /// <param name="serviceProvider">The service provider forming part of this context</param>
    public TestDIContext(IServiceProvider serviceProvider)
    {
      ServiceProvider = serviceProvider;
    }

    /// <summary>
    /// The service provider used to perform DI operations
    /// </summary>
    public IServiceProvider ServiceProvider { get; private set; }

  }
}
