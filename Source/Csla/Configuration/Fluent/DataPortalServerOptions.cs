//-----------------------------------------------------------------------
// <copyright file="DataPortalServerOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Server-side data portal options.</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Csla.Server;

namespace Csla.Configuration
{
  /// <summary>
  /// Server-side data portal options.
  /// </summary>
  public class DataPortalServerOptions
  {
    /// <summary>
    /// Gets or sets a value containing the type of the
    /// IDataPortalAuthorizer to be used by the data portal.
    /// An instance of this type is created using dependency
    /// injection.
    /// </summary>
    public Type AuthorizerProviderType { get; set; } = typeof(ActiveAuthorizer);

    /// <summary>
    /// Gets a list of the IInterceptDataPortal instances
    /// that should be executed by the server-side data portal.
    /// injection.
    /// </summary>
    public List<IInterceptDataPortal> InterceptorProviders { get; } = new List<IInterceptDataPortal>();

    /// <summary>
    /// Gets or sets the type of the ExceptionInspector.
    /// </summary>
    public Type ExceptionInspectorType { get; set; } = typeof(DefaultExceptionInspector);

    /// <summary>
    /// Gets or sets the type of the Activator.
    /// </summary>
    public Type ActivatorType { get; set; } = typeof(DefaultDataPortalActivator);

    /// <summary>
    /// Gets or sets the type name of the factor loader used to create
    /// server-side instances of business object factories when using
    /// the FactoryDataPortal model. Type must implement
    /// <see cref="IObjectFactoryLoader"/>.
    /// </summary>
    public Type ObjectFactoryLoaderType { get; set; } = typeof(ObjectFactoryLoader);
  }
}
