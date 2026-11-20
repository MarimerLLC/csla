//-----------------------------------------------------------------------
// <copyright file="OperationParameterModel.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>A parameter of a data portal operation method</summary>
//-----------------------------------------------------------------------

namespace Csla.Generator.AutoImplementProperties.CSharp.DataPortalOperations.Models
{
  /// <summary>
  /// Effective accessibility of a parameter type, as seen from
  /// a top-level type in the same assembly.
  /// </summary>
  internal enum TypeVisibility
  {
    Public,
    Internal,
    Private
  }

  /// <summary>
  /// The kind of a parameter type, used to decide whether one criteria value
  /// can match two parameter types.
  /// </summary>
  internal enum CriteriaTypeCategory
  {
    /// <summary>object, dynamic, or a type the generator cannot reason about.</summary>
    Unknown,
    TypeParameter,
    Interface,
    Array,
    /// <summary>A value type or a sealed class.</summary>
    Sealed,
    /// <summary>A class that can have subclasses.</summary>
    Class
  }

  /// <summary>
  /// A parameter of a data portal operation method.
  /// </summary>
  internal sealed record OperationParameterModel
  {
    /// <summary>The parameter name, escaped with @ when it is a keyword.</summary>
    public required string Name { get; init; }

    /// <summary>Fully qualified type including nullable annotation.</summary>
    public required string TypeDisplay { get; init; }

    /// <summary>
    /// Type usable in an <c>is</c> pattern: nullable annotation removed and
    /// <see cref="Nullable{T}"/> unwrapped.
    /// </summary>
    public required string PatternTypeDisplay { get; init; }

    /// <summary>Type key used to compute the operation name.</summary>
    public required string TypeKey { get; init; }

    /// <summary>"ref ", "out ", "in " or empty.</summary>
    public string RefKindPrefix { get; init; } = string.Empty;

    public bool IsNullableValueType { get; init; }

    /// <summary>The runtime value may legitimately be null (reference type or Nullable).</summary>
    public bool AcceptsNull { get; init; }

    /// <summary>
    /// An <c>is</c> pattern on this type can only match that exact runtime
    /// type (value types and sealed classes).
    /// </summary>
    public bool IsExactTypeMatch { get; init; }

    public bool ContainsTypeParameter { get; init; }

    public bool IsParams { get; init; }

    /// <summary>True when the type is exactly object[] (a CSLA params-array operation).</summary>
    public bool IsObjectArray { get; init; }

    public bool IsInjected { get; init; }

    public bool AllowNull { get; init; }

    /// <summary>C# expression for the service key of a keyed [Inject], or null.</summary>
    public string? ServiceKeyExpression { get; init; }

    /// <summary>C# expression for the default value (without "= "), or null.</summary>
    public string? DefaultValueExpression { get; init; }

    public TypeVisibility Visibility { get; init; }

    /// <summary>The kind of the pattern type.</summary>
    public CriteriaTypeCategory Category { get; init; }

    /// <summary>
    /// The pattern types of the base classes and interfaces of the pattern
    /// type, which a value of this type also matches.
    /// </summary>
    public EquatableArray<string> AssignableTo { get; init; }

    /// <summary>For an array, its element type is a value type.</summary>
    public bool HasValueTypeElements { get; init; }
  }
}
