//-----------------------------------------------------------------------
// <copyright file="DataPortalServerOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Server-side data portal options.</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Csla.Server;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Configuration
{
  /// <summary>
  /// Server-side data portal options.
  /// </summary>
  /// <param name="services">Service collection.</param>
  /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>.</exception>
  public class DataPortalServerOptions(IServiceCollection services)
  {
    /// <summary>
    /// Gets the service collection.
    /// </summary>
    public IServiceCollection Services{ get; } = Guard.NotNull(services);

    /// <summary>
    /// Gets or sets a value containing the type of the
    /// IDashboard to be used by the data portal.
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    internal Type DashboardType { get; private set; } = typeof(Server.Dashboard.NullDashboard);

    /// <summary>
    /// Sets the type of the IDashboard to be 
    /// used by the data portal. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public DataPortalServerOptions RegisterDashboard<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>() where T : Server.Dashboard.IDashboard
    {
      DashboardType = typeof(T);
      return this;
    }

    /// <summary>
    /// Gets or sets a value containing the type of the
    /// IDataPortalAuthorizer to be used by the data portal.
    /// An instance of this type is created using dependency
    /// injection.
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    internal Type AuthorizerProviderType { get; private set; } = typeof(ActiveAuthorizer);

    /// <summary>
    /// Sets the type of the IDataPortalAuthorizer to be 
    /// used by the data portal. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public DataPortalServerOptions RegisterAuthorizerProvider<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>() where T : IAuthorizeDataPortal
    {
      AuthorizerProviderType = typeof(T);
      return this;
    } 

    /// <summary>
    /// Gets a list of the IInterceptDataPortal instances
    /// that should be executed by the server-side data portal.
    /// injection.
    /// </summary>
    internal List<Type> InterceptorProviders { get; } = [typeof(Server.Interceptors.ServerSide.RevalidatingInterceptor)];

    /// <summary>
    /// Adds the type of an IInterceptDataPortal that will
    /// be executed by the server-side data portal.
    /// </summary>
    public DataPortalServerOptions AddInterceptorProvider<T>() where T: IInterceptDataPortal
    {
      InterceptorProviders.Add(typeof(T));
      return this;
    }

    /// <summary>
    /// Adds the type of an IInterceptDataPortal that will
    /// be executed by the server-side data portal.
    /// </summary>
    /// <param name="index">Index at which new item should be added.</param>
    public DataPortalServerOptions AddInterceptorProvider<T>(int index) where T : IInterceptDataPortal
    {
      InterceptorProviders.Insert(index, typeof(T));
      return this;
    }

    /// <summary>
    /// Removes a type of an IInterceptDataPortal.
    /// </summary>
    /// <param name="index">Index from which item will be removed.</param>
    public DataPortalServerOptions RemoveInterceptorProvider(int index)
    {
      InterceptorProviders.RemoveAt(index);
      return this;
    }

    /// <summary>
    /// Gets or sets the type of the ExceptionInspector.
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    internal Type ExceptionInspectorType { get; private set; } = typeof(DefaultExceptionInspector);

    /// <summary>
    /// Sets the type of the ExceptionInspector.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public DataPortalServerOptions RegisterExceptionInspector<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>() where T: IDataPortalExceptionInspector
    {
      ExceptionInspectorType = typeof(T);
      return this;
    }

    /// <summary>
    /// Gets or sets the type of the Activator.
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    internal Type ActivatorType { get; private set; } = typeof(DefaultDataPortalActivator);

    /// <summary>
    /// Sets the type of the Activator.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public DataPortalServerOptions RegisterActivator<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>() where T: IDataPortalActivator
    {
      ActivatorType = typeof(T);
      return this;
    }

    /// <summary>
    /// Gets or sets the type name of the factor loader used to create
    /// server-side instances of business object factories when using
    /// the FactoryDataPortal model. Type must implement
    /// <see cref="IObjectFactoryLoader"/>.
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    internal Type ObjectFactoryLoaderType { get; private set; } = typeof(ObjectFactoryLoader);

    /// <summary>
    /// Gets or sets the type name of the factor loader used to create
    /// server-side instances of business object factories when using
    /// the FactoryDataPortal model. Type must implement
    /// <see cref="IObjectFactoryLoader"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public DataPortalServerOptions RegisterObjectFactoryLoader<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>() where T: IObjectFactoryLoader
    {
      ObjectFactoryLoaderType = typeof(T);
      return this;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// server-side business object should be returned to
    /// the client as part of the DataPortalException
    /// (default is false).
    /// </summary>
    public bool DataPortalReturnObjectOnException { get; set; } = false;
  }
}
