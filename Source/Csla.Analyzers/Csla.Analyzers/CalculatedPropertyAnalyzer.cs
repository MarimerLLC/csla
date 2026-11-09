using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  /// <summary>
  /// Verifies that properties marked with <c>[CalculatedProperty]</c>
  /// are read-only (no setter, or a private setter) and are declared
  /// in a <c>BusinessBase</c> type.
  /// </summary>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class CalculatedPropertyAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor mustBeReadOnlyRule =
      new(
        Constants.AnalyzerIdentifiers.CalculatedPropertyMustBeReadOnly, CalculatedPropertyAnalyzerConstants.MustBeReadOnlyTitle,
        CalculatedPropertyAnalyzerConstants.MustBeReadOnlyMessage, Constants.Categories.Usage,
        DiagnosticSeverity.Warning, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.CalculatedPropertyMustBeReadOnly, nameof(CalculatedPropertyAnalyzer)));
    private static readonly DiagnosticDescriptor requiresBusinessBaseRule =
      new(
        Constants.AnalyzerIdentifiers.CalculatedPropertyRequiresBusinessBase, CalculatedPropertyAnalyzerConstants.RequiresBusinessBaseTitle,
        CalculatedPropertyAnalyzerConstants.RequiresBusinessBaseMessage, Constants.Categories.Usage,
        DiagnosticSeverity.Warning, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.CalculatedPropertyRequiresBusinessBase, nameof(CalculatedPropertyAnalyzer)));

    /// <summary>
    ///
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [mustBeReadOnlyRule, requiresBusinessBaseRule];

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
      var propertySymbol = (IPropertySymbol)context.Symbol;

      if (!propertySymbol.GetAttributes().Any(a => a.AttributeClass.IsCalculatedPropertyAttribute()))
      {
        return;
      }

      if (!propertySymbol.ContainingType.IsBusinessBase())
      {
        context.ReportDiagnostic(Diagnostic.Create(
          requiresBusinessBaseRule, propertySymbol.Locations[0], propertySymbol.Name));
        return;
      }

      var setter = propertySymbol.SetMethod;
      if (setter is not null &&
        (setter.IsInitOnly || setter.DeclaredAccessibility != Accessibility.Private))
      {
        context.ReportDiagnostic(Diagnostic.Create(
          mustBeReadOnlyRule, propertySymbol.Locations[0], propertySymbol.Name));
      }
    }
  }
}
