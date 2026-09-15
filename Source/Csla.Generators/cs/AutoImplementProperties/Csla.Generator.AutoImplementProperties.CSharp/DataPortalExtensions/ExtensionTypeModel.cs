//-----------------------------------------------------------------------
// <copyright file="ExtensionTypeModel.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>A business type marked with DataPortalExtensions</summary>
//-----------------------------------------------------------------------
// Portions adapted from Csla.DataPortalExtensions by Stefan Ossendorf
// (https://github.com/StefanOssendorf/Csla.DataPortalExtensions),
// licensed under the MIT License. Copyright (c) 2023 Stefan Ossendorf.
//-----------------------------------------------------------------------

using Csla.Generator.AutoImplementProperties.CSharp.DataPortalOperations.Models;

namespace Csla.Generator.AutoImplementProperties.CSharp.DataPortalExtensions
{
  /// <summary>
  /// A business type marked with <c>[DataPortalExtensions]</c>.
  /// </summary>
  internal sealed record ExtensionTypeModel
  {
    /// <summary>Fully qualified metadata name, matching the operations model.</summary>
    public required string MetadataName { get; init; }

    public required string Namespace { get; init; }

    /// <summary>Fully qualified business type name, e.g. "global::NS.PersonEdit".</summary>
    public required string FullyQualifiedName { get; init; }

    /// <summary>Name of the generated static class, e.g. "PersonEditDataPortalExtensions".</summary>
    public required string ExtensionClassName { get; init; }

    public required string TypeName { get; init; }

    /// <summary>File name friendly unique name.</summary>
    public required string HintName { get; init; }

    public string Prefix { get; init; } = string.Empty;

    public bool IsGeneric { get; init; }

    public bool IsAbstract { get; init; }

    public bool IsCslaObject { get; init; }

    public TypeVisibility Visibility { get; init; }

    public LocationInfo? Location { get; init; }

    /// <summary>
    /// Names of instance members of IDataPortal&lt;T&gt;, which would hide
    /// root extension methods with the same name.
    /// </summary>
    public required EquatableArray<string> RootHiddenNames { get; init; }

    /// <summary>
    /// Names of instance members of IChildDataPortal&lt;T&gt;, which would hide
    /// child extension methods with the same name.
    /// </summary>
    public required EquatableArray<string> ChildHiddenNames { get; init; }
  }

  /// <summary>
  /// A business type marked with <c>[DataPortalExtensions]</c> and its operation methods.
  /// When <paramref name="GenerateSync"/> is false only async extension methods are generated.
  /// </summary>
  internal sealed record ExtensionGenerationModel(ExtensionTypeModel Type, EquatableArray<OperationMethodModel> Methods, bool GenerateSync);
}
