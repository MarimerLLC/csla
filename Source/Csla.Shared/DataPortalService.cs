//-----------------------------------------------------------------------
// <copyright file="DataPortalService.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Implements a data portal service</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla
{
  /// <summary>
  /// Implements a data portal service
  /// used to get an access to a client-side data portal
  /// instance.
  /// </summary>
  public class DataPortalService : IDataPortalService
  {
    /// <summary>
    /// Get a client-side data portal instance.
    /// </summary>
    /// <typeparam name="T">Root business object type</typeparam>
    public IDataPortal<T> GetDataPortal<T>()
    {
      return new DataPortal<T>();
    }
  }
}
