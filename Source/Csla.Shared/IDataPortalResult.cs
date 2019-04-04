//-----------------------------------------------------------------------
// <copyright file="IDataPortalResult.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>IDataPortalResult defines the results of DataPortal operation</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla
{
  /// <summary>
  /// IDataPortalResult defines the results of DataPortal operation
  /// </summary>
  public interface IDataPortalResult
  {
    /// <summary>
    /// Gets the resulting object.
    /// </summary>
    object Object { get; }
    /// <summary>
    /// Gets any resulting error information.
    /// </summary>
    Exception Error { get; }
    /// <summary>
    /// Gets the user state, pass through object
    /// </summary>
    /// <value>
    /// The user state.
    /// </value>
    object UserState { get; }
  }
}