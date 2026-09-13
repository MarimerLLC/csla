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
using Microsoft.CodeAnalysis.Text;

namespace Csla.Generator.AutoImplementProperties.CSharp.DataPortalExtensions
{
  /// <summary>
  /// Incremental source generator that emits strongly typed extension methods
  /// on <c>IDataPortal&lt;T&gt;</c> and <c>IChildDataPortal&lt;T&gt;</c> for each
  /// data portal operation method of a business class marked with
  /// <c>[DataPortalExtensions]</c>.
  /// </summary>
  [Generator(LanguageNames.CSharp)]
  public class IncrementalDataPortalExtensionsGenerator : IIncrementalGenerator
  {
    private const string DataPortalExtensionsAttributeName = "Csla.DataPortalExtensionsAttribute";

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

      var models = extensionTypes
        .Combine(operationTypes)
        .Select(static (pair, _) => new ExtensionGenerationModel(
          pair.Left,
          new EquatableArray<OperationMethodModel>(
            pair.Right
              .Where(t => t.Type.MetadataName == pair.Left.MetadataName)
              .SelectMany(t => t.Methods)
              .Where(m => SupportedKinds.Contains(m.Kind)))))
        .WithTrackingName(TrackingNames.ExtensionTypes);

      context.RegisterSourceOutput(models, static (spc, model) =>
      {
        var (source, diagnostics) = DataPortalExtensionsBuilder.Build(model);
        foreach (var diagnostic in diagnostics)
          spc.ReportDiagnostic(diagnostic.ToDiagnostic());
        if (source is not null)
          spc.AddSource($"{model.Type.HintName}.DataPortalExtensions.g.cs", SourceText.From(source, Encoding.UTF8));
      });
    }

    private static ExtensionTypeModel? ExtractExtensionType(GeneratorAttributeSyntaxContext context, CancellationToken ct)
    {
      if (context.TargetSymbol is not INamedTypeSymbol type)
        return null;

      ct.ThrowIfCancellationRequested();

      var attribute = context.Attributes.FirstOrDefault();
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

      var compilation = context.SemanticModel.Compilation;
      var hiddenNames = new SortedSet<string>(StringComparer.Ordinal);
      foreach (var portalInterface in new[] { "Csla.IDataPortal`1", "Csla.IChildDataPortal`1" })
      {
        var symbol = compilation.GetTypeByMetadataName(portalInterface);
        if (symbol is null)
          continue;
        foreach (var member in symbol.GetMembers())
          hiddenNames.Add(member.Name);
      }

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
        HiddenNames = new EquatableArray<string>(hiddenNames)
      };
    }
  }
}
