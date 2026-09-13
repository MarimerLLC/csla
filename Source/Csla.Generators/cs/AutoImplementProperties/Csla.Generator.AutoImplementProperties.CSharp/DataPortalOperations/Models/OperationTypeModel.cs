//-----------------------------------------------------------------------
// <copyright file="OperationTypeModel.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>A type that declares data portal operation methods</summary>
//-----------------------------------------------------------------------

namespace Csla.Generator.AutoImplementProperties.CSharp.DataPortalOperations.Models
{
  /// <summary>
  /// Type-level information about a class declaring data portal
  /// operation methods. Computed independently for each method so a
  /// partial class spread across files is grouped by <see cref="MetadataName"/>.
  /// </summary>
  internal sealed record OperationTypeHeader
  {
    /// <summary>Fully qualified metadata name, used as the grouping key.</summary>
    public required string MetadataName { get; init; }

    /// <summary>Namespace, or empty for the global namespace.</summary>
    public required string Namespace { get; init; }

    /// <summary>Declarations of containing types, outermost first, e.g. "partial class Outer".</summary>
    public required EquatableArray<string> ContainerDeclarations { get; init; }

    /// <summary>Declaration of the type, e.g. "partial class PersonEdit&lt;T&gt;".</summary>
    public required string TypeDeclaration { get; init; }

    /// <summary>Fully qualified type name with type parameters, e.g. "global::NS.PersonEdit".</summary>
    public required string FullyQualifiedName { get; init; }

    /// <summary>Simple type name without type parameters.</summary>
    public required string TypeName { get; init; }

    /// <summary>Containing type names, outermost first.</summary>
    public required EquatableArray<string> ContainerNames { get; init; }

    /// <summary>File name friendly unique name, e.g. "NS.Outer.PersonEdit_1".</summary>
    public required string HintName { get; init; }

    /// <summary>The type and all containing types are declared partial.</summary>
    public bool IsPartial { get; init; }

    public bool IsAbstract { get; init; }

    /// <summary>The type or a containing type is generic.</summary>
    public bool IsGeneric { get; init; }

    /// <summary>An accessible base type already exposes a nested IDataPortalOperations.</summary>
    public bool HidesInheritedOperationsInterface { get; init; }

    public bool ImplementsOperationMapping { get; init; }

    public bool ImplementsNamedOperationMapping { get; init; }
  }

  /// <summary>
  /// One discovered operation method together with its type header.
  /// </summary>
  internal sealed record OperationMethodEntry(OperationTypeHeader Type, OperationMethodModel Method);

  /// <summary>
  /// A type and all of its data portal operation methods.
  /// </summary>
  internal sealed record OperationTypeModel(OperationTypeHeader Type, EquatableArray<OperationMethodModel> Methods);
}
