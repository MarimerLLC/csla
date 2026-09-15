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
  /// <para>
  /// Apply the attribute to a business class, or to an assembly
  /// (<c>[assembly: DataPortalExtensions]</c>) to generate extension
  /// methods for every business class in the assembly that can have
  /// them: non-abstract, non-generic classes implementing
  /// <see cref="Core.ICslaObject"/> that are accessible outside their
  /// containing type. Use <see cref="NoDataPortalExtensionAttribute"/>
  /// to exclude a class or an operation method.
  /// </para>
  /// <para>
  /// When both are used, the <see cref="Prefix"/> set on the class takes
  /// precedence over the prefix set on the assembly.
  /// </para>
  /// </remarks>
  [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public sealed class DataPortalExtensionsAttribute : Attribute
  {
    /// <summary>
    /// Gets or sets an optional prefix added to the name of
    /// each generated extension method. Defaults to no prefix,
    /// or on a class, to the prefix set on the assembly.
    /// </summary>
    public string? Prefix { get; set; }
  }
}
