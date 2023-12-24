//-----------------------------------------------------------------------
// <copyright file="DataPortalClientOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Client-side data portal options.</summary>
//-----------------------------------------------------------------------
using System;
using Csla.DataPortalClient;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Configuration
{
  /// <summary>
  /// Client-side data portal options.
  /// </summary>
  public class DataPortalClientOptions
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public DataPortalClientOptions(DataPortalOptions dataPortalOptions)
    {
      _parent = dataPortalOptions;
    }

    private readonly DataPortalOptions _parent;

    /// <summary>
    /// Gets a reference to the current services collection.
    /// </summary>
    public IServiceCollection Services => _parent.CslaOptions.Services;

    /// <summary>
    /// Sets a value indicating whether objects should be
    /// automatically cloned by the data portal Update()
    /// method when using a local data portal configuration.
    /// </summary>
    /// <param name="autoCloneOnUpdate"></param>
    public DataPortalClientOptions AutoCloneOnUpdate(bool autoCloneOnUpdate)
    {
      ApplicationContext.AutoCloneOnUpdate = autoCloneOnUpdate;
      return this;
    }

    /// <summary>
    /// Sets the authentication type being used by the
    /// CSLA .NET framework.
    /// </summary>
    /// <param name="authenticationType"></param>
    /// <remarks>
    /// Set to "Windows" to use OS impersonation. Any other
    /// value causes the data portal to flow the client-side
    /// user principal to the server. Client and server must
    /// use the same authentication type.
    /// </remarks>
    public DataPortalClientOptions AuthenticationType(string authenticationType)
    {
      ApplicationContext.AuthenticationType = authenticationType;
      return this;
    }

    /// <summary>
    /// Enable flow of user's security principal from client to server.
    /// </summary>
    /// <returns>This instance, to support method chaining</returns>
    /// <remarks>
    /// You should avoid enabling this flow wherever possible. Anything on 
    /// the client can be manipulated, and using any data flowed from the 
    /// client to make security decisions on the server could make your 
    /// application vulnerable to impersonation exploits or elevation
    /// of privilege attacks.
    /// </remarks>
    public DataPortalClientOptions EnableSecurityPrincipalFlowFromClient()
    {
      ApplicationContext.FlowSecurityPrincipalFromClient = true;
      return this;
    }

    /// <summary>
    /// Sets a value indicating whether the
    /// server-side business object should be returned to
    /// the client as part of the DataPortalException.
    /// </summary>
    /// <param name="returnObjectOnException"></param>
    public DataPortalClientOptions DataPortalReturnObjectOnException(bool returnObjectOnException)
    {
      ApplicationContext.DataPortalReturnObjectOnException = returnObjectOnException;
      return this;
    }

    /// <summary>
    /// Gets or sets the type that implements 
    /// IDataPortalCache for client-side caching.
    /// </summary>
    public Type DataPortalCacheType { get; set; } = typeof(DataPortalCacheDefault);
  }
}
