using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  /// <summary>
  /// Reports a property marked <c>[DoNotSerialize]</c> or <c>[CalculatedProperty]</c>
  /// whose registration, found by name in the same type, is a not-null
  /// reference-type property without a default value. The property is not
  /// serialized, so it arrives null after deserialization and the data portal
  /// check fails. Generated registrations are included.
  /// </summary>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class NotNullPropertyNotSerializedAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor rule =
      new(
        Constants.AnalyzerIdentifiers.NotNullPropertyNotSerializedWithoutDefault, NotNullPropertyNotSerializedAnalyzerConstants.Title,
        NotNullPropertyNotSerializedAnalyzerConstants.Message, Constants.Categories.Usage,
        DiagnosticSeverity.Error, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.NotNullPropertyNotSerializedWithoutDefault, nameof(NotNullPropertyNotSerializedAnalyzer)));

    /// <summary>
    ///
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [rule];

    /// <summary>
    ///
    /// </summary>
    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();
      context.RegisterSymbolAction(AnalyzeProperty, SymbolKind.Property);
    }

    private static void AnalyzeProperty(SymbolAnalysisContext context)
    {
      var property = (IPropertySymbol)context.Symbol;
      if (!property.GetAttributes().Any(_ => _.AttributeClass.IsDoNotSerializeAttribute() || _.AttributeClass.IsCalculatedPropertyAttribute()))
      {
        return;
      }

      foreach (var member in property.ContainingType.GetMembers())
      {
        if (member is IFieldSymbol { IsStatic: true } field &&
          NotNullPropertyRegistrations.IsNotNullPropertyInfo(field.Type) &&
          NotNullPropertyRegistrations.GetRegisteredName(field, context.CancellationToken) == property.Name &&
          NotNullPropertyRegistrations.IsNoDefaultReference(field, context.CancellationToken))
        {
          context.ReportDiagnostic(Diagnostic.Create(rule, property.Locations[0], property.Name));
          return;
        }
      }
    }
  }
}
