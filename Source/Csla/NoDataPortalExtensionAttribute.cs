//-----------------------------------------------------------------------
// <copyright file="NoDataPortalExtensionAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Excludes a class or operation method from data portal extension generation</summary>
//-----------------------------------------------------------------------

namespace Csla
{
  /// <summary>
  /// Excludes a business class, or a data portal operation method, from
  /// strongly typed extension method generation requested by
  /// <see cref="DataPortalExtensionsAttribute"/>.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
  public sealed class NoDataPortalExtensionAttribute : Attribute;
}
