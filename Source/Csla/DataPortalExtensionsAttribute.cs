//-----------------------------------------------------------------------
// <copyright file="DataPortalExtensionsAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Requests generation of strongly typed data portal extension methods</summary>
//-----------------------------------------------------------------------

namespace Csla
{
  /// <summary>
  /// Requests that the CSLA source generator emit strongly typed
  /// extension methods on <see cref="IDataPortal{T}"/> and
  /// <see cref="IChildDataPortal{T}"/> for each data portal
  /// operation method declared by the business class.
  /// </summary>
  /// <remarks>
  /// The business class must be <c>partial</c> and non-generic.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class DataPortalExtensionsAttribute : Attribute
  {
    /// <summary>
    /// Gets or sets an optional prefix added to the name of
    /// each generated extension method. Defaults to no prefix.
    /// </summary>
    public string? Prefix { get; set; }
  }
}
