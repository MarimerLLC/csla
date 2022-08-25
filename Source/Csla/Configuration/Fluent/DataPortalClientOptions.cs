//-----------------------------------------------------------------------
// <copyright file="DataPortalClientOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Client-side data portal options.</summary>
//-----------------------------------------------------------------------
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
    /// <param name="options"></param>
    public DataPortalClientOptions(CslaOptions options)
    {
      CslaOptions = options;
    }

    /// <summary>
    /// Gets the current configuration object.
    /// </summary>
    internal CslaOptions CslaOptions { get; set; }

    /// <summary>
    /// Gets the current service collection.
    /// </summary>
    public IServiceCollection Services { get => CslaOptions.Services; }

    internal DataPortalServerOptions ServerOptions { get => CslaOptions.DataPortalServerOptions; }

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
  }
}
