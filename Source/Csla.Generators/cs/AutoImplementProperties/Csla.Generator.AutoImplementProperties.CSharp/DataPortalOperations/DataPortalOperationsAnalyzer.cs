//-----------------------------------------------------------------------
// <copyright file="DataPortalOperationsAnalyzer.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Reports data portal operation methods that share an operation name</summary>
//-----------------------------------------------------------------------

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Csla.Generator.AutoImplementProperties.CSharp.DataPortalOperations
{
  /// <summary>
  /// Reports operation methods of a partial class that map to the same
  /// data portal operation name, and how <see cref="IncrementalDataPortalOperationsGenerator"/>
  /// dispatches them.
  /// </summary>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class DataPortalOperationsAnalyzer : DiagnosticAnalyzer
  {
    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
      [DataPortalOperationsDiagnostics.DuplicateOperationName, DataPortalOperationsDiagnostics.AmbiguousOperationName];

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();
      context.RegisterCompilationStartAction(compilationContext =>
      {
        var attributeKinds = GetOperationAttributeKinds(compilationContext.Compilation);
        if (attributeKinds.Count == 0)
          return;

        compilationContext.RegisterSymbolAction(symbolContext => AnalyzeType(symbolContext, attributeKinds), SymbolKind.NamedType);
      });
    }

    /// <summary>
    /// The operation attribute types in the compilation, mapped to their operation kind.
    /// </summary>
    internal static Dictionary<INamedTypeSymbol, string> GetOperationAttributeKinds(Compilation compilation)
    {
      var result = new Dictionary<INamedTypeSymbol, string>(SymbolEqualityComparer.Default);
      foreach (var kind in OperationDiscovery.OperationKinds)
      {
        if (compilation.GetTypeByMetadataName(OperationDiscovery.GetAttributeMetadataName(kind)) is { } attribute)
          result[attribute] = kind;
      }
      return result;
    }

    private static void AnalyzeType(SymbolAnalysisContext context, Dictionary<INamedTypeSymbol, string> attributeKinds)
    {
      var type = (INamedTypeSymbol)context.Symbol;
      var model = OperationDiscovery.GetOperationType(type, attributeKinds, context.Compilation, context.CancellationToken);
      if (model is null || !model.Type.IsPartial)
        return;

      var (_, collisions) = DataPortalOperationsBuilder.SelectNamedDispatchMethods(model.Methods);
      foreach (var collision in collisions)
      {
        // A collision between two inherited methods is reported on the type
        // declaring them. One that involves a method the type declares is
        // reported at that method, even when the other method wins.
        var declared = !collision.Loser.IsInherited ? collision.Loser
          : !collision.Winner.IsInherited ? collision.Winner
          : null;
        if (declared is null)
          continue;

        context.ReportDiagnostic(Diagnostic.Create(
          collision.IsAmbiguous ? DataPortalOperationsDiagnostics.AmbiguousOperationName : DataPortalOperationsDiagnostics.DuplicateOperationName,
          declared.Location?.ToLocation(context.Compilation),
          collision.Winner.MethodDisplay, collision.Loser.MethodDisplay, model.Type.TypeName, collision.Winner.OperationName));
      }
    }
  }
}
