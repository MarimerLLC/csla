//-----------------------------------------------------------------------
// <copyright file="IDataPortalProxyFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Creates the DataPortalProxy to use for DataPortal call on the objectType.</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// 
  /// </summary>
  public interface IDataPortalProxyFactory
  {
    /// <summary>
    /// Creates the DataPortalProxy to use for DataPortal call on the objectType.
    /// </summary>
    DataPortalClient.IDataPortalProxy Create(Type objectType);

    /// <summary>
    /// Resets the data portal proxy type, so the
    /// next data portal call will reload the proxy
    /// type based on current configuration values.
    /// </summary>
    void ResetProxyType();
  }
}