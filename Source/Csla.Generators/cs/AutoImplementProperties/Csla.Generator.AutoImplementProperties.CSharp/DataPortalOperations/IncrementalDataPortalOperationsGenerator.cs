//-----------------------------------------------------------------------
// <copyright file="IncrementalDataPortalOperationsGenerator.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Source generator for data portal operation interfaces and dispatch</summary>
//-----------------------------------------------------------------------

using System.Collections.Immutable;
using System.Text;
using Csla.Generator.AutoImplementProperties.CSharp.DataPortalOperations.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Csla.Generator.AutoImplementProperties.CSharp.DataPortalOperations
{
  /// <summary>
  /// Incremental source generator that, for every partial class declaring
  /// data portal operation methods, generates a nested internal
  /// <c>IDataPortalOperations</c> interface naming each operation method and,
  /// for concrete classes, implementations of <c>IDataPortalOperationMapping</c>
  /// and <c>IDataPortalOperationNamedMapping</c>.
  /// </summary>
  [Generator(LanguageNames.CSharp)]
  public class IncrementalDataPortalOperationsGenerator : IIncrementalGenerator
  {
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
      var types = GetOperationTypes(context)
        .SelectMany(static (types, _) => types)
        .Where(static t => t.Type.IsPartial)
        .WithTrackingName(TrackingNames.OperationTypes);

      // Diagnostics for these types are reported by DataPortalOperationsAnalyzer.
      context.RegisterSourceOutput(types, static (spc, model) =>
      {
        spc.AddSource(
          $"{model.Type.HintName}.DataPortalOperations.g.cs",
          SourceText.From(DataPortalOperationsBuilder.Build(model), Encoding.UTF8));
      });
    }

    /// <summary>
    /// Discovers every type declaring data portal operation methods, grouped
    /// by type so a partial class spread across several files yields one model.
    /// </summary>
    internal static IncrementalValueProvider<EquatableArray<OperationTypeModel>> GetOperationTypes(IncrementalGeneratorInitializationContext context)
    {
      IncrementalValueProvider<ImmutableArray<OperationMethodEntry>>? all = null;
      foreach (var kind in OperationDiscovery.OperationKinds)
      {
        var entries = context.SyntaxProvider.ForAttributeWithMetadataName(
            OperationDiscovery.GetAttributeMetadataName(kind),
            predicate: static (node, _) => node is MethodDeclarationSyntax,
            transform: (ctx, ct) => OperationDiscovery.ExtractMethod(ctx, kind, ct))
          .Where(static e => e is not null)
          .Select(static (e, _) => e!)
          .WithTrackingName(TrackingNames.ExtractOperationMethods)
          .Collect();

        all = all is null
          ? entries
          : all.Value.Combine(entries).Select(static (pair, _) => pair.Left.AddRange(pair.Right));
      }

      return all!.Value
        .Select(static (entries, ct) => OperationDiscovery.GroupByType(entries, ct))
        .WithTrackingName(TrackingNames.GroupOperationTypes);
    }
  }
}
