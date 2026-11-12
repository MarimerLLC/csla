//-----------------------------------------------------------------------
// <copyright file="IncrementalMobileSerializerRegistrationGenerator.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Source generator for assembly-level custom serializer registrations</summary>
//-----------------------------------------------------------------------

using System.Collections.Immutable;
using System.Text;
using Csla.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Csla.Generator.AutoImplementProperties.CSharp.MobileSerializers
{
  /// <summary>
  /// Incremental source generator that turns <c>[MobileSerializer]</c> on a type
  /// and <c>[MobileSerializerFor]</c> on a serializer into assembly-level
  /// <c>[MobileSerializerRegistration]</c> attributes. The registrations are read
  /// at run time by <c>MobileFormatterOptions.AddCustomSerializers</c> and at
  /// compile time by the CSLA analyzers. Invalid declarations are skipped; the
  /// CSLA0028 analyzer reports them.
  /// </summary>
  [Generator(LanguageNames.CSharp)]
  public class IncrementalMobileSerializerRegistrationGenerator : IIncrementalGenerator
  {
    /// <summary>
    /// Name of the generated file.
    /// </summary>
    public const string HintName = "CslaMobileSerializerRegistrations.g.cs";

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
      var typeSide = context.SyntaxProvider.ForAttributeWithMetadataName(
          MobileSerializerRules.MobileSerializerAttributeMetadataName,
          predicate: static (node, _) => node is TypeDeclarationSyntax,
          transform: static (ctx, ct) => ExtractTypeSide(ctx, ct))
        .WithTrackingName(TrackingNames.ExtractTypeSideRegistrations)
        .Collect();

      var serializerSide = context.SyntaxProvider.ForAttributeWithMetadataName(
          MobileSerializerRules.MobileSerializerForAttributeMetadataName,
          predicate: static (node, _) => node is ClassDeclarationSyntax,
          transform: static (ctx, ct) => ExtractSerializerSide(ctx, ct))
        .WithTrackingName(TrackingNames.ExtractSerializerSideRegistrations)
        .Collect();

      var registrations = typeSide
        .Combine(serializerSide)
        .Select(static (pair, _) => Merge(pair.Left, pair.Right))
        .WithTrackingName(TrackingNames.MergeRegistrations);

      context.RegisterSourceOutput(registrations, static (spc, model) =>
      {
        if (model.IsEmpty)
          return;
        spc.AddSource(HintName, SourceText.From(Build(model), Encoding.UTF8));
      });
    }

    private static EquatableArray<SerializerRegistration> ExtractTypeSide(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
      if (context.TargetSymbol is not INamedTypeSymbol type)
        return EquatableArray<SerializerRegistration>.Empty;

      var compilation = context.SemanticModel.Compilation;
      var result = new List<SerializerRegistration>();
      foreach (var attribute in context.Attributes)
      {
        cancellationToken.ThrowIfCancellationRequested();
        var serializer = MobileSerializerRules.GetTypeSideSerializer(attribute);
        if (TryCreate(type, serializer, compilation, out var registration))
          result.Add(registration);
      }
      return new EquatableArray<SerializerRegistration>(result);
    }

    private static EquatableArray<SerializerRegistration> ExtractSerializerSide(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
      if (context.TargetSymbol is not INamedTypeSymbol serializer)
        return EquatableArray<SerializerRegistration>.Empty;

      var compilation = context.SemanticModel.Compilation;
      var result = new List<SerializerRegistration>();
      foreach (var attribute in context.Attributes)
      {
        foreach (var type in MobileSerializerRules.GetSerializerSideTypes(attribute))
        {
          cancellationToken.ThrowIfCancellationRequested();
          if (TryCreate(type, serializer, compilation, out var registration))
            result.Add(registration);
        }
      }
      return new EquatableArray<SerializerRegistration>(result);
    }

    private static bool TryCreate(ITypeSymbol? type, ITypeSymbol? serializer, Compilation compilation, out SerializerRegistration registration)
    {
      registration = null!;
      if (type is null || serializer is null)
        return false;
      if (!MobileSerializerRules.IsValidTargetType(type, compilation, out _)
        || !MobileSerializerRules.IsValidSerializer(serializer, compilation, out _))
        return false;

      registration = new SerializerRegistration(
        MobileSerializerRules.FullyQualifiedName(type),
        MobileSerializerRules.FullyQualifiedName(serializer));
      return true;
    }

    /// <summary>
    /// Combines the registrations from both attributes, without duplicates,
    /// in a stable order so the generated file is deterministic.
    /// </summary>
    internal static EquatableArray<SerializerRegistration> Merge(
      ImmutableArray<EquatableArray<SerializerRegistration>> typeSide,
      ImmutableArray<EquatableArray<SerializerRegistration>> serializerSide)
    {
      var all = new SortedSet<SerializerRegistration>(SerializerRegistrationComparer.Instance);
      foreach (var group in typeSide)
        all.UnionWith(group);
      foreach (var group in serializerSide)
        all.UnionWith(group);
      return new EquatableArray<SerializerRegistration>(all);
    }

    /// <summary>
    /// Builds the generated source.
    /// </summary>
    internal static string Build(EquatableArray<SerializerRegistration> registrations)
    {
      var builder = new StringBuilder();
      builder.AppendLine("// <auto-generated/>");
      builder.AppendLine("// Custom serializers declared with [MobileSerializer] and [MobileSerializerFor].");
      builder.AppendLine("// Register them with MobileFormatterOptions.AddCustomSerializers(assembly).");
      builder.AppendLine();
      foreach (var registration in registrations)
      {
        builder.Append("[assembly: global::Csla.Serialization.Mobile.MobileSerializerRegistration(typeof(")
          .Append(registration.Type)
          .Append("), typeof(")
          .Append(registration.Serializer)
          .AppendLine("))]");
      }
      return builder.ToString();
    }

    private sealed class SerializerRegistrationComparer : IComparer<SerializerRegistration>
    {
      public static readonly SerializerRegistrationComparer Instance = new();

      public int Compare(SerializerRegistration? x, SerializerRegistration? y)
      {
        if (ReferenceEquals(x, y))
          return 0;
        if (x is null)
          return -1;
        if (y is null)
          return 1;
        var result = string.CompareOrdinal(x.Type, y.Type);
        return result != 0 ? result : string.CompareOrdinal(x.Serializer, y.Serializer);
      }
    }
  }

  /// <summary>
  /// A type and the serializer registered for it, as fully qualified names.
  /// </summary>
  internal sealed record SerializerRegistration(string Type, string Serializer);
}
