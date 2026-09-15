//-----------------------------------------------------------------------
// <copyright file="IncrementalDataPortalExtensionsGenerator.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Source generator for strongly typed data portal extension methods</summary>
//-----------------------------------------------------------------------
// Portions adapted from Csla.DataPortalExtensions by Stefan Ossendorf
// (https://github.com/StefanOssendorf/Csla.DataPortalExtensions),
// licensed under the MIT License. Copyright (c) 2023 Stefan Ossendorf.
//-----------------------------------------------------------------------

using System.Text;
using Csla.Generator.AutoImplementProperties.CSharp.DataPortalOperations;
using Csla.Generator.AutoImplementProperties.CSharp.DataPortalOperations.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Csla.Generator.AutoImplementProperties.CSharp.DataPortalExtensions
{
  /// <summary>
  /// Incremental source generator that emits strongly typed extension methods
  /// on <c>IDataPortal&lt;T&gt;</c> and <c>IChildDataPortal&lt;T&gt;</c> for each
  /// data portal operation method of a business class marked with
  /// <c>[DataPortalExtensions]</c>. Diagnostics about the generated methods
  /// are reported by <see cref="DataPortalExtensionsAnalyzer"/>.
  /// </summary>
  [Generator(LanguageNames.CSharp)]
  public class IncrementalDataPortalExtensionsGenerator : IIncrementalGenerator
  {
    internal const string DataPortalExtensionsAttributeName = "Csla.DataPortalExtensionsAttribute";

    /// <summary>
    /// MSBuild property that controls whether synchronous extension methods
    /// are generated. Set it to <c>false</c> to generate only the async methods.
    /// </summary>
    internal const string GenerateSyncProperty = "build_property.CslaGenerateSyncDataPortalExtensions";

    /// <summary>
    /// Operation kinds that have a client-side data portal method.
    /// </summary>
    internal static readonly string[] SupportedKinds = ["Create", "Fetch", "Execute", "Delete", "CreateChild", "FetchChild"];

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
      var extensionTypes = context.SyntaxProvider.ForAttributeWithMetadataName(
          DataPortalExtensionsAttributeName,
          predicate: static (node, _) => node is ClassDeclarationSyntax,
          transform: static (ctx, ct) => ExtractExtensionType(ctx, ct))
        .Where(static t => t is not null)
        .Select(static (t, _) => t!)
        .WithTrackingName(TrackingNames.ExtractExtensionTypes);

      var operationTypes = IncrementalDataPortalOperationsGenerator.GetOperationTypes(context);

      var generateSync = context.AnalyzerConfigOptionsProvider
        .Select(static (provider, _) => GetGenerateSync(provider.GlobalOptions))
        .WithTrackingName(TrackingNames.ExtensionOptions);

      var models = extensionTypes
        .Combine(operationTypes)
        .Combine(generateSync)
        .Select(static (pair, _) => new ExtensionGenerationModel(
          pair.Left.Left,
          GetExtensionMethods(pair.Left.Right.Where(t => t.Type.MetadataName == pair.Left.Left.MetadataName)),
          pair.Right))
        .WithTrackingName(TrackingNames.ExtensionTypes);

      context.RegisterSourceOutput(models, static (spc, model) =>
      {
        var source = DataPortalExtensionsBuilder.Build(model);
        if (source is not null)
          spc.AddSource($"{model.Type.HintName}.DataPortalExtensions.g.cs", SourceText.From(source, Encoding.UTF8));
      });
    }

    /// <summary>
    /// Whether synchronous extension methods are generated. They are unless
    /// the <c>CslaGenerateSyncDataPortalExtensions</c> MSBuild property is <c>false</c>.
    /// </summary>
    internal static bool GetGenerateSync(AnalyzerConfigOptions options)
      => !(options.TryGetValue(GenerateSyncProperty, out var value)
        && string.Equals(value?.Trim(), "false", StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// The operation methods of the business type that have a client-side data portal method.
    /// </summary>
    internal static EquatableArray<OperationMethodModel> GetExtensionMethods(IEnumerable<OperationTypeModel> operationTypes)
      => new(operationTypes
        .SelectMany(t => t.Methods)
        .Where(m => SupportedKinds.Contains(m.Kind)));

    private static ExtensionTypeModel? ExtractExtensionType(GeneratorAttributeSyntaxContext context, CancellationToken ct)
    {
      if (context.TargetSymbol is not INamedTypeSymbol type)
        return null;

      ct.ThrowIfCancellationRequested();

      return CreateExtensionType(type, context.Attributes.FirstOrDefault(), context.SemanticModel.Compilation);
    }

    /// <summary>
    /// Build the model for a business type marked with <c>[DataPortalExtensions]</c>.
    /// </summary>
    internal static ExtensionTypeModel CreateExtensionType(INamedTypeSymbol type, AttributeData? attribute, Compilation compilation)
    {
      var prefix = attribute?.NamedArguments
        .Where(a => a.Key == "Prefix")
        .Select(a => a.Value.Value as string)
        .FirstOrDefault() ?? string.Empty;

      var containers = new List<INamedTypeSymbol>();
      for (var container = type.ContainingType; container is not null; container = container.ContainingType)
        containers.Insert(0, container);

      var @namespace = type.ContainingNamespace is { IsGlobalNamespace: false } ns ? ns.ToDisplayString() : string.Empty;
      var hintParts = new List<string>();
      if (@namespace.Length > 0)
        hintParts.Add(@namespace);
      hintParts.AddRange(containers.Select(c => c.Name));
      hintParts.Add(type.Name);

      return new ExtensionTypeModel
      {
        MetadataName = OperationDiscovery.GetFullMetadataName(type),
        Namespace = @namespace,
        FullyQualifiedName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
        ExtensionClassName = string.Concat(containers.Select(c => c.Name + "_")) + type.Name + "DataPortalExtensions",
        TypeName = type.Name,
        HintName = string.Join(".", hintParts),
        Prefix = prefix,
        IsGeneric = type.TypeParameters.Length > 0 || containers.Any(c => c.TypeParameters.Length > 0),
        IsAbstract = type.IsAbstract,
        IsCslaObject = type.AllInterfaces.Any(i => i.ToDisplayString() == "Csla.Core.ICslaObject"),
        Visibility = OperationDiscovery.GetVisibility(type),
        Location = LocationInfo.From(type.Locations.FirstOrDefault()),
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
}
