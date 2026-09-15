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
using Microsoft.CodeAnalysis.Diagnostics;

namespace Csla.Generator.AutoImplementProperties.CSharp.DataPortalExtensions
{
  /// <summary>
  /// Reports business types marked with <c>[DataPortalExtensions]</c> for which
  /// <see cref="IncrementalDataPortalExtensionsGenerator"/> does not generate some
  /// or all extension methods, and why.
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
      DataPortalOperationsDiagnostics.InvalidExtensionPrefix
    ];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();
      context.RegisterCompilationStartAction(compilationContext =>
      {
        var compilation = compilationContext.Compilation;
        if (compilation.GetTypeByMetadataName(IncrementalDataPortalExtensionsGenerator.DataPortalExtensionsAttributeName) is not { } extensionsAttribute)
          return;

        var attributeKinds = DataPortalOperationsAnalyzer.GetOperationAttributeKinds(compilation);
        var generateSync = IncrementalDataPortalExtensionsGenerator.GetGenerateSync(compilationContext.Options.AnalyzerConfigOptionsProvider.GlobalOptions);

        compilationContext.RegisterSymbolAction(
          symbolContext => AnalyzeType(symbolContext, extensionsAttribute, attributeKinds, generateSync), SymbolKind.NamedType);
      });
    }

    private static void AnalyzeType(SymbolAnalysisContext context, INamedTypeSymbol extensionsAttribute,
      Dictionary<INamedTypeSymbol, string> attributeKinds, bool generateSync)
    {
      var type = (INamedTypeSymbol)context.Symbol;
      if (type.TypeKind != TypeKind.Class || type.IsRecord)
        return;

      var attribute = type.GetAttributes().FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, extensionsAttribute));
      if (attribute is null)
        return;

      var compilation = context.Compilation;
      var extensionType = IncrementalDataPortalExtensionsGenerator.CreateExtensionType(type, attribute, compilation);
      var operationType = OperationDiscovery.GetOperationType(type, attributeKinds, compilation, context.CancellationToken);
      var methods = IncrementalDataPortalExtensionsGenerator.GetExtensionMethods(
        operationType is null ? [] : [operationType]);

      var (_, diagnostics) = DataPortalExtensionsBuilder.Analyze(new ExtensionGenerationModel(extensionType, methods, generateSync));
      foreach (var diagnostic in diagnostics)
        context.ReportDiagnostic(diagnostic.ToDiagnostic(compilation));
    }
  }
}
