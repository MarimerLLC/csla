//-----------------------------------------------------------------------
// <copyright file="IUseApplicationContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement if a class requires access to the CSLA ApplicationContext type.</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Core
{
  /// <summary>
  /// Implement if a class requires access to the CSLA ApplicationContext type.
  /// </summary>
  public interface IUseApplicationContext
  {
    /// <summary>
    /// Gets or sets the current ApplicationContext object.
    /// </summary>
    ApplicationContext ApplicationContext { get; set; }
  }
}
