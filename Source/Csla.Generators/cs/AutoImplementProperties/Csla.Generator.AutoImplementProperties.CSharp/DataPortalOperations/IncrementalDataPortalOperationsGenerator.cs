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

      context.RegisterSourceOutput(types, static (spc, model) =>
      {
        var (_, collisions) = DataPortalOperationsBuilder.SelectNamedDispatchMethods(model.Methods);
        foreach (var (winner, loser) in collisions)
        {
          spc.ReportDiagnostic(Diagnostic.Create(
            DataPortalOperationsDiagnostics.DuplicateOperationName,
            loser.Location?.ToLocation(),
            winner.MethodDisplay, loser.MethodDisplay, model.Type.TypeName, winner.OperationName));
        }

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
        .Select(static (entries, ct) => GroupByType(entries, ct))
        .WithTrackingName(TrackingNames.GroupOperationTypes);
    }

    private static EquatableArray<OperationTypeModel> GroupByType(ImmutableArray<OperationMethodEntry> entries, CancellationToken ct)
    {
      var result = new List<OperationTypeModel>();
      foreach (var group in entries.GroupBy(e => e.Type.MetadataName).OrderBy(g => g.Key, StringComparer.Ordinal))
      {
        ct.ThrowIfCancellationRequested();
        var header = group.First().Type with { IsPartial = group.All(e => e.Type.IsPartial) };
        var methods = group
          .Select(e => e.Method)
          .OrderBy(m => m.Location?.FilePath ?? string.Empty, StringComparer.Ordinal)
          .ThenBy(m => m.Location?.TextSpan.Start ?? 0)
          .ThenBy(m => Array.IndexOf(OperationDiscovery.OperationKinds, m.Kind));
        result.Add(new OperationTypeModel(header, new EquatableArray<OperationMethodModel>(methods)));
      }
      return new EquatableArray<OperationTypeModel>(result);
    }
  }
}
