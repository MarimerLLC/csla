//-----------------------------------------------------------------------
// <copyright file="GeneratedDataPortalExtensionAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Marks a generated data portal extension method</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;

namespace Csla
{
  /// <summary>
  /// Marks a strongly typed data portal extension method generated for
  /// <see cref="DataPortalExtensionsAttribute"/>, and names the
  /// <see cref="IDataPortal{T}"/> or <see cref="IChildDataPortal{T}"/>
  /// method it replaces. Applied by the CSLA source generator; not
  /// intended to be used directly.
  /// </summary>
  /// <remarks>
  /// The CSLA analyzers read this attribute to recognize the generated
  /// extension methods of business types declared in referenced assemblies,
  /// whose private operation methods are not visible.
  /// </remarks>
  /// <param name="portalMethodName">
  /// Name of the data portal method the extension method replaces,
  /// for example <c>FetchAsync</c> or <c>CreateChild</c>.
  /// </param>
  [EditorBrowsable(EditorBrowsableState.Never)]
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
  public sealed class GeneratedDataPortalExtensionAttribute(string portalMethodName) : Attribute
  {
    /// <summary>
    /// Gets the name of the data portal method the extension method replaces.
    /// </summary>
    public string PortalMethodName { get; } = portalMethodName;
  }
}
