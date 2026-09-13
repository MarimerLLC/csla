//-----------------------------------------------------------------------
// <copyright file="OperationMethodModel.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>A data portal operation method for one operation attribute</summary>
//-----------------------------------------------------------------------

namespace Csla.Generator.AutoImplementProperties.CSharp.DataPortalOperations.Models
{
  /// <summary>
  /// A data portal operation method as seen through one operation
  /// attribute. A method carrying several operation attributes
  /// yields one model per attribute.
  /// </summary>
  internal sealed record OperationMethodModel
  {
    public required string MethodName { get; init; }

    /// <summary>Method signature for diagnostics, e.g. "Fetch(int)".</summary>
    public string MethodDisplay { get; init; } = string.Empty;

    /// <summary>Operation kind, e.g. "Fetch" or "FetchChild".</summary>
    public required string Kind { get; init; }

    /// <summary>Fully qualified return type, e.g. "void".</summary>
    public required string ReturnTypeDisplay { get; init; }

    /// <summary>Returns Task, Task&lt;T&gt;, ValueTask or ValueTask&lt;T&gt;.</summary>
    public bool IsAsync { get; init; }

    /// <summary>All parameters, criteria and injected, in declaration order.</summary>
    public required EquatableArray<OperationParameterModel> Parameters { get; init; }

    /// <summary>Deterministic operation name, e.g. "Fetch__Int32".</summary>
    public required string OperationName { get; init; }

    public bool RunLocal { get; init; }

    public bool NoExtension { get; init; }

    public LocationInfo? Location { get; init; }

    public IEnumerable<OperationParameterModel> CriteriaParameters => Parameters.Where(p => !p.IsInjected);

    public int CriteriaCount => Parameters.Count(p => !p.IsInjected);

    public int InjectCount => Parameters.Count(p => p.IsInjected);

    /// <summary>
    /// The method can be invoked from generated dispatch code. Methods
    /// taking a single object[] (which receive the whole criteria array)
    /// or ref/out parameters are left to the reflection-based path.
    /// </summary>
    public bool CanDispatch =>
      !Parameters.Any(p => p.RefKindPrefix is "ref " or "out ")
      && !(Parameters.Count == 1 && Parameters[0].IsObjectArray);

    /// <summary>
    /// The method can take part in name-based dispatch. When criteria
    /// reference a type parameter the generated name cannot match the
    /// runtime-computed name.
    /// </summary>
    public bool CanDispatchByName => CanDispatch && !CriteriaParameters.Any(p => p.ContainsTypeParameter);
  }
}
