//-----------------------------------------------------------------------
// <copyright file="ExtractedTypeDefinition.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>The definition of a type with data portal operations</summary>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Csla.Generator.DataPortalInterfaces.CSharp.Extractors
{

  /// <summary>
  /// The definition of a type with data portal operations,
  /// extracted from the syntax tree provided by Roslyn
  /// </summary>
  public class ExtractedTypeDefinition : IEquatable<ExtractedTypeDefinition>
  {

    /// <summary>
    /// The namespace in which the type resides
    /// </summary>
    public string Namespace { get; set; } = string.Empty;

    /// <summary>
    /// The scope of the class (e.g. "public", "internal")
    /// </summary>
    public string Scope { get; set; } = "public";

    /// <summary>
    /// The name of the type, excluding any namespace
    /// </summary>
    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// The type parameters (e.g. "&lt;T&gt;") or empty string
    /// </summary>
    public string TypeParameters { get; set; } = string.Empty;

    /// <summary>
    /// The fully qualified name of the type, including namespace
    /// </summary>
    public string FullyQualifiedName { get; set; } = string.Empty;

    /// <summary>
    /// The container definitions for this type
    /// </summary>
    public IList<ExtractedContainerDefinition> ContainerDefinitions { get; } = new List<ExtractedContainerDefinition>();

    /// <summary>
    /// The data portal operation methods on this type
    /// </summary>
    public IList<ExtractedOperationMethod> OperationMethods { get; } = new List<ExtractedOperationMethod>();

    /// <summary>
    /// Whether the class is partial
    /// </summary>
    public bool IsPartial { get; set; }

    /// <inheritdoc/>
    public bool Equals(ExtractedTypeDefinition other)
    {
      if (other is null) return false;
      return FullyQualifiedName == other.FullyQualifiedName;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ExtractedTypeDefinition other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => FullyQualifiedName?.GetHashCode() ?? 0;
  }
}
