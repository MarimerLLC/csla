//-----------------------------------------------------------------------
// <copyright file="ExtensionTypeModel.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>A business type that gets data portal extension methods</summary>
//-----------------------------------------------------------------------
// Portions adapted from Csla.DataPortalExtensions by Stefan Ossendorf
// (https://github.com/StefanOssendorf/Csla.DataPortalExtensions),
// licensed under the MIT License. Copyright (c) 2023 Stefan Ossendorf.
//-----------------------------------------------------------------------

using Csla.Generator.AutoImplementProperties.CSharp.DataPortalOperations;
using Csla.Generator.AutoImplementProperties.CSharp.DataPortalOperations.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Csla.Generator.AutoImplementProperties.CSharp.DataPortalExtensions
{
  /// <summary>
  /// A business type that gets data portal extension methods, because it
  /// is marked with <c>[DataPortalExtensions]</c> or its assembly is.
  /// </summary>
  internal sealed record ExtensionTypeModel
  {
    public required string Namespace { get; init; }

    /// <summary>Fully qualified business type name, e.g. "global::NS.PersonEdit".</summary>
    public required string FullyQualifiedName { get; init; }

    /// <summary>Name of the generated static class, e.g. "PersonEditDataPortalExtensions".</summary>
    public required string ExtensionClassName { get; init; }

    public required string TypeName { get; init; }

    /// <summary>File name friendly unique name.</summary>
    public required string HintName { get; init; }

    public string Prefix { get; init; } = string.Empty;

    /// <summary>
    /// The type is not marked with <c>[DataPortalExtensions]</c> itself;
    /// extensions are requested by the assembly attribute.
    /// </summary>
    public bool FromAssembly { get; init; }

    public bool IsGeneric { get; init; }

    public bool IsAbstract { get; init; }

    public bool IsCslaObject { get; init; }

    public TypeVisibility Visibility { get; init; }

    /// <summary>Location of the type, set only for diagnostics reported by the analyzer.</summary>
    public LocationInfo? Location { get; init; }

    /// <summary>
    /// Creates the model for a type, or returns null when no extension
    /// methods are requested for it. Types covered only by the assembly
    /// attribute that cannot have extension methods are skipped; types
    /// marked with the attribute are returned so the reason can be reported.
    /// </summary>
    public static ExtensionTypeModel? Create(OperationTypeHeader header, ExtensionOptions options, LocationInfo? location = null)
    {
      if (header.NoExtensions || header.IsRecord)
        return null;

      var fromAssembly = !header.HasExtensionsAttribute;
      if (fromAssembly)
      {
        if (!options.AssemblyEnabled)
          return null;
        if (header.IsGeneric || header.IsAbstract || !header.IsCslaObject || header.Visibility == TypeVisibility.Private)
          return null;
      }

      return new ExtensionTypeModel
      {
        Namespace = header.Namespace,
        FullyQualifiedName = header.FullyQualifiedName,
        ExtensionClassName = string.Concat(header.ContainerNames.Select(c => c + "_")) + header.TypeName + "DataPortalExtensions",
        TypeName = header.TypeName,
        HintName = header.HintName,
        Prefix = header.ExtensionsPrefix ?? options.AssemblyPrefix,
        FromAssembly = fromAssembly,
        IsGeneric = header.IsGeneric,
        IsAbstract = header.IsAbstract,
        IsCslaObject = header.IsCslaObject,
        Visibility = header.Visibility,
        Location = location
      };
    }
  }

  /// <summary>
  /// Compilation-wide settings for data portal extension generation.
  /// </summary>
  internal sealed record ExtensionOptions
  {
    /// <summary>
    /// MSBuild property that controls whether synchronous extension methods
    /// are generated. Set it to <c>false</c> to generate only the async methods.
    /// </summary>
    public const string GenerateSyncProperty = "build_property.CslaGenerateSyncDataPortalExtensions";

    /// <summary>
    /// MSBuild property that sets the suffix of async extension method names.
    /// Defaults to <c>Async</c>; set it to <c>none</c> for no suffix.
    /// </summary>
    public const string AsyncSuffixProperty = "build_property.CslaDataPortalExtensionsAsyncSuffix";

    public const string DefaultAsyncSuffix = "Async";

    /// <summary>The assembly is marked with <c>[DataPortalExtensions]</c>.</summary>
    public bool AssemblyEnabled { get; init; }

    /// <summary>The Prefix set by the assembly attribute.</summary>
    public string AssemblyPrefix { get; init; } = string.Empty;

    /// <summary>The CslaGenerateSyncDataPortalExtensions property is not false.</summary>
    public bool GenerateSyncRequested { get; init; } = true;

    /// <summary>Suffix of async method names.</summary>
    public string AsyncSuffix { get; init; } = DefaultAsyncSuffix;

    /// <summary>The configured suffix, when it is not a valid identifier part and is ignored.</summary>
    public string? InvalidAsyncSuffix { get; init; }

    /// <summary>
    /// Synchronous methods are generated. They are not when they are turned off,
    /// or when there is no async suffix and they would have the same names.
    /// </summary>
    public bool GenerateSync => GenerateSyncRequested && AsyncSuffix.Length > 0;

    /// <summary>
    /// Names of instance members of IDataPortal&lt;T&gt;, which would hide
    /// root extension methods with the same name.
    /// </summary>
    public EquatableArray<string> RootHiddenNames { get; init; } = new(Array.Empty<string>());

    /// <summary>
    /// Names of instance members of IChildDataPortal&lt;T&gt;, which would hide
    /// child extension methods with the same name.
    /// </summary>
    public EquatableArray<string> ChildHiddenNames { get; init; } = new(Array.Empty<string>());

    public static ExtensionOptions Create(Compilation compilation, AnalyzerConfigOptions globalOptions)
    {
      var assemblyAttribute = compilation.Assembly.GetAttributes()
        .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == OperationDiscovery.DataPortalExtensionsAttributeName);

      var generateSync = !(globalOptions.TryGetValue(GenerateSyncProperty, out var syncValue)
        && string.Equals(syncValue?.Trim(), "false", StringComparison.OrdinalIgnoreCase));

      var asyncSuffix = DefaultAsyncSuffix;
      string? invalidAsyncSuffix = null;
      if (globalOptions.TryGetValue(AsyncSuffixProperty, out var suffixValue) && suffixValue?.Trim() is { Length: > 0 } suffix)
      {
        if (string.Equals(suffix, "none", StringComparison.OrdinalIgnoreCase))
          asyncSuffix = string.Empty;
        else if (SyntaxFacts.IsValidIdentifier("_" + suffix))
          asyncSuffix = suffix;
        else
          invalidAsyncSuffix = suffix;
      }

      return new ExtensionOptions
      {
        AssemblyEnabled = assemblyAttribute is not null,
        AssemblyPrefix = OperationDiscovery.GetPrefix(assemblyAttribute) ?? string.Empty,
        GenerateSyncRequested = generateSync,
        AsyncSuffix = asyncSuffix,
        InvalidAsyncSuffix = invalidAsyncSuffix,
        RootHiddenNames = GetMemberNames(compilation, "Csla.IDataPortal`1"),
        ChildHiddenNames = GetMemberNames(compilation, "Csla.IChildDataPortal`1")
      };
    }

    private static EquatableArray<string> GetMemberNames(Compilation compilation, string metadataName)
    {
      var names = new SortedSet<string>(StringComparer.Ordinal);
      if (compilation.GetTypeByMetadataName(metadataName) is { } symbol)
      {
        foreach (var member in symbol.GetMembers())
          names.Add(member.Name);
      }
      return new EquatableArray<string>(names);
    }
  }

  /// <summary>
  /// A business type that gets data portal extension methods, its operation
  /// methods, and the compilation-wide settings.
  /// </summary>
  internal sealed record ExtensionGenerationModel(ExtensionTypeModel Type, EquatableArray<OperationMethodModel> Methods, ExtensionOptions Options);
}
