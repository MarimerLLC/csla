//-----------------------------------------------------------------------
// <copyright file="ServiceProviderMethodCaller.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Dynamically find/invoke methods with DI provided params</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Csla.Server;

namespace Csla.Reflection
{
  /// <summary>
  /// Provides extension methods for <see cref="ServiceProviderMethodCaller"/>.
  /// </summary>
  internal static class ServiceProviderMethodCallerExtensions
  {
    public static DataPortalMethodInfo GetDataPortalMethodInfoFor<TAttribute>(this ServiceProviderMethodCaller source, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType)
      where TAttribute : DataPortalOperationAttribute
    {
      return GetDataPortalMethodInfoFor<TAttribute>(source, objectType, EmptyCriteria.Instance);
    }

    public static DataPortalMethodInfo GetDataPortalMethodInfoFor<TAttribute>(this ServiceProviderMethodCaller source, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria)
      where TAttribute : DataPortalOperationAttribute
    {
      var dataPortalMethod = source.FindDataPortalMethod<TAttribute>(objectType, UnpackCriteria(criteria));
      dataPortalMethod.PrepForInvocation();
      return dataPortalMethod.DataPortalMethodInfo!;
    }

    public static bool TryGetProviderMethodInfoFor<TAttribute>(this ServiceProviderMethodCaller source, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, out ServiceProviderMethodInfo? providerMethodInfo)
      where TAttribute : DataPortalOperationAttribute
    {
      return source.TryFindDataPortalMethod<TAttribute>(objectType, UnpackCriteria(criteria), out providerMethodInfo);
    }

    private static object?[]? UnpackCriteria(object criteria)
    {
      object?[]? methodCriteria = null;
      if (criteria is not EmptyCriteria)
      {
        methodCriteria = Server.DataPortal.GetCriteriaArray(criteria);
      }

      return methodCriteria;
    }
  }
}
