//-----------------------------------------------------------------------
// <copyright file="DefaultDataPortalActivator.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines a type used to activate concrete business instances.</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
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
  /// <exception cref="ArgumentNullException"><paramref name="serviceProvider"/> is <see langword="null"/>.</exception>
  public class DefaultDataPortalActivator(IServiceProvider serviceProvider) : IDataPortalActivator
  {
    /// <summary>
    /// Gets a reference to the current DI service provider.
    /// </summary>
    protected IServiceProvider ServiceProvider { get; } = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    /// <inheritdoc />
    public virtual object CreateInstance([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type requestedType, params object[] parameters)
    {
      if (requestedType is null)
        throw new ArgumentNullException(nameof(requestedType));
      if (parameters is null)
        throw new ArgumentNullException(nameof(parameters));

      var realType = ResolveType(requestedType);
      object result = ActivatorUtilities.CreateInstance(ServiceProvider, realType, parameters);
      
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
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    public virtual Type ResolveType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type requestedType)
    {
      // return requested type by default
      return requestedType;
    }
  }
}