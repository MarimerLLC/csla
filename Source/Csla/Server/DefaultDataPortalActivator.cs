//-----------------------------------------------------------------------
// <copyright file="DefaultDataPortalActivator.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines a type used to activate concrete business instances.</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Server
{
  /// <summary>
  /// Default data portal activator. Can also be used as a base class.
  /// </summary>
  /// <remarks>
  /// Creates an instance of the type.
  /// </remarks>
  /// <param name="serviceProvider"></param>
  public class DefaultDataPortalActivator(IServiceProvider serviceProvider) : IDataPortalActivator
  {
    /// <summary>
    /// Gets a reference to the current DI service provider.
    /// </summary>
    protected IServiceProvider ServiceProvider { get; private set; } = serviceProvider;

    /// <summary>
    /// Gets a new instance of the requested type.
    /// </summary>
    /// <param name="requestedType">Requested type (class or interface).</param>
    /// <param name="parameters">Param array for the constructor</param>
    /// <returns>Instance of requested type</returns>
    public virtual object CreateInstance(Type requestedType, params object[] parameters)
    {
      object result;
      var realType = ResolveType(requestedType);
      if (ServiceProvider != null)
        result = ActivatorUtilities.CreateInstance(ServiceProvider, realType, parameters);
      else
        result = Activator.CreateInstance(realType, parameters);
      if (result is IUseApplicationContext tmp)
      {
        tmp.ApplicationContext = ServiceProvider.GetRequiredService<ApplicationContext>();
      }
      InitializeInstance(result);
      return result;
    }

    /// <summary>
    /// Initializes an object instance.
    /// </summary>
    /// <param name="obj">Reference to the business object.</param>
    public virtual void InitializeInstance(object obj)
    {
      // do no work by default
    }

    /// <summary>
    /// Finalizes an object instance. Called
    /// after a data portal operation is complete.
    /// </summary>
    /// <param name="obj">Reference to the business object.</param>
    public virtual void FinalizeInstance(object obj)
    {
      // do no work by default
    }

    /// <summary>
    /// Gets the actual business domain class type based on the
    /// requested type (which might be an interface).
    /// </summary>
    /// <param name="requestedType">Type requested from the data portal.</param>
    public virtual Type ResolveType(Type requestedType)
    {
      // return requested type by default
      return requestedType;
    }
  }
}