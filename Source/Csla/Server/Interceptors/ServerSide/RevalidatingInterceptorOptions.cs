//-----------------------------------------------------------------------
// <copyright file="RevalidatingInterceptorOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Initiates revalidation on business objects during data portal operations</summary>
//-----------------------------------------------------------------------

namespace Csla.Server.Interceptors.ServerSide
{
  /// <summary>
  /// Options for <see cref="RevalidatingInterceptor"/>.
  /// </summary>
  public class RevalidatingInterceptorOptions
  {
    /// <summary>
    /// Indicates whether the <see cref="RevalidatingInterceptor"/> should not re-validate business objects during a <see cref="DataPortalOperations.Delete"/> operation.
    /// </summary>
    public bool IgnoreDeleteOperation { get; set; }
  }
}
