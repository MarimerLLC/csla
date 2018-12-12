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
  public class DataPortalFactory : IDataPortalFactory
  {
    /// <summary>
    /// Get a client-side data portal instance.
    /// </summary>
    /// <typeparam name="T">Root business object type</typeparam>
    public IDataPortal<T> GetPortal<T>()
    {
      return new DataPortal<T>();
    }

    /// <summary>
    /// Get a client-side data portal instance.
    /// </summary>
    /// <typeparam name="T">Root business object type</typeparam>
    /// <param name="httpClient">Instance of HttpClient to be used
    /// by all instances of the CSLA HttpProxy type.</param>
    public IDataPortal<T> GetPortal<T>(System.Net.Http.HttpClient httpClient)
    {
      DataPortalClient.HttpProxy.SetHttpClient(httpClient);
      return new DataPortal<T>();
    }
  }
}
