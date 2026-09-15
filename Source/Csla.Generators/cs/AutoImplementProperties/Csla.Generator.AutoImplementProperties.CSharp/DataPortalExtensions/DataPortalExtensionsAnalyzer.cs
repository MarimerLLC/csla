//-----------------------------------------------------------------------
// <copyright file="DataPortalExtensionsAnalyzer.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Reports data portal extension methods that are not generated</summary>
//-----------------------------------------------------------------------

using System.Collections.Immutable;
using Csla.Generator.AutoImplementProperties.CSharp.DataPortalOperations;
using Csla.Generator.AutoImplementProperties.CSharp.DataPortalOperations.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Csla.Generator.AutoImplementProperties.CSharp.DataPortalExtensions
{
  /// <summary>
  /// Reports business types for which <see cref="IncrementalDataPortalExtensionsGenerator"/>
  /// does not generate some or all of the requested extension methods, and
  /// why, along with invalid extension settings.
  /// </summary>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class DataPortalExtensionsAnalyzer : DiagnosticAnalyzer
  {
    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
      DataPortalOperationsDiagnostics.GenericExtensionsNotGenerated,
      DataPortalOperationsDiagnostics.InvalidExtensionsTarget,
      DataPortalOperationsDiagnostics.ExtensionNameHidden,
      DataPortalOperationsDiagnostics.InaccessibleParameterType,
      DataPortalOperationsDiagnostics.DuplicateExtensionMethod,
      DataPortalOperationsDiagnostics.InvalidExtensionPrefix,
      DataPortalOperationsDiagnostics.InvalidAsyncSuffix,
      DataPortalOperationsDiagnostics.SyncExtensionsNotGenerated
    ];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();
      context.RegisterCompilationStartAction(compilationContext =>
      {
        var compilation = compilationContext.Compilation;
        if (compilation.GetTypeByMetadataName(OperationDiscovery.DataPortalExtensionsAttributeName) is not { } extensionsAttribute)
          return;

        var options = ExtensionOptions.Create(compilation, compilationContext.Options.AnalyzerConfigOptionsProvider.GlobalOptions);
        var attributeKinds = DataPortalOperationsAnalyzer.GetOperationAttributeKinds(compilation);
        var state = new AnalysisState();

        compilationContext.RegisterSymbolAction(symbolContext =>
        {
          if (AnalyzeType(symbolContext, extensionsAttribute, attributeKinds, options))
            state.ExtensionsRequested = true;
        }, SymbolKind.NamedType);

        if (options.AssemblyEnabled && options.AssemblyPrefix.Length > 0 && !SyntaxFacts.IsValidIdentifier(options.AssemblyPrefix))
        {
          compilationContext.RegisterSyntaxNodeAction(
            syntaxContext => AnalyzeAssemblyAttributes(syntaxContext, extensionsAttribute, options), SyntaxKind.AttributeList);
        }

        if (options.InvalidAsyncSuffix is not null || (options.GenerateSyncRequested && !options.GenerateSync))
        {
          compilationContext.RegisterCompilationEndAction(endContext =>
          {
            if (!state.ExtensionsRequested)
              return;
            if (options.InvalidAsyncSuffix is not null)
              endContext.ReportDiagnostic(Diagnostic.Create(DataPortalOperationsDiagnostics.InvalidAsyncSuffix, Location.None, options.InvalidAsyncSuffix));
            if (options.GenerateSyncRequested && !options.GenerateSync)
              endContext.ReportDiagnostic(Diagnostic.Create(DataPortalOperationsDiagnostics.SyncExtensionsNotGenerated, Location.None));
          });
        }
      });
    }

    /// <summary>
    /// Reports why extension methods are not generated for a type. Returns
    /// true when extension methods are requested for the type.
    /// </summary>
    private static bool AnalyzeType(SymbolAnalysisContext context, INamedTypeSymbol extensionsAttribute,
      Dictionary<INamedTypeSymbol, string> attributeKinds, ExtensionOptions options)
    {
      var type = (INamedTypeSymbol)context.Symbol;
      if (type.TypeKind != TypeKind.Class)
        return false;

      var compilation = context.Compilation;
      var operationType = OperationDiscovery.GetOperationType(type, attributeKinds, compilation, context.CancellationToken);
      if (operationType is null)
      {
        // A type marked with the attribute is reported even without operation methods.
        if (!type.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, extensionsAttribute))
          || OperationDiscovery.BuildTypeHeader(type, compilation, context.CancellationToken) is not { } header)
          return false;
        operationType = new OperationTypeModel(header, new EquatableArray<OperationMethodModel>(Array.Empty<OperationMethodModel>()));
      }

      var model = IncrementalDataPortalExtensionsGenerator.CreateModel(operationType, options, LocationInfo.From(type.Locations.FirstOrDefault()));
      if (model is null)
        return false;

      DataPortalExtensionsBuilder.Analyze(model, (descriptor, location, messageArgs)
        => context.ReportDiagnostic(Diagnostic.Create(descriptor, location?.ToLocation(compilation), messageArgs)));
      return true;
    }

    private static void AnalyzeAssemblyAttributes(SyntaxNodeAnalysisContext context, INamedTypeSymbol extensionsAttribute, ExtensionOptions options)
    {
      var attributeList = (AttributeListSyntax)context.Node;
      if (attributeList.Target?.Identifier.IsKind(SyntaxKind.AssemblyKeyword) != true)
        return;

      foreach (var attribute in attributeList.Attributes)
      {
        if (context.SemanticModel.GetSymbolInfo(attribute, context.CancellationToken).Symbol?.ContainingType is { } attributeType
          && SymbolEqualityComparer.Default.Equals(attributeType, extensionsAttribute))
        {
          context.ReportDiagnostic(Diagnostic.Create(
            DataPortalOperationsDiagnostics.InvalidExtensionPrefix, attribute.GetLocation(), context.Compilation.AssemblyName, options.AssemblyPrefix));
        }
      }
    }

    private sealed class AnalysisState
    {
      private volatile bool _extensionsRequested;

      public bool ExtensionsRequested
      {
        get => _extensionsRequested;
        set => _extensionsRequested = value;
      }
    }
  }
}
