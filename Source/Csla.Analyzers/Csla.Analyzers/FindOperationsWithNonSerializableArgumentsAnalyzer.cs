using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class FindOperationsWithNonSerializableArgumentsAnalyzer
    : DiagnosticAnalyzer
  {
    private static DiagnosticDescriptor shouldUseSerializableTypesRule = new DiagnosticDescriptor(
      Constants.AnalyzerIdentifiers.PublicNoArgumentConstructorIsMissing, PublicNoArgumentConstructorIsMissingConstants.Title,
      PublicNoArgumentConstructorIsMissingConstants.Message, Constants.Categories.Design,
      DiagnosticSeverity.Error, true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
      get
      {
        return ImmutableArray.Create(
          FindOperationsWithNonSerializableArgumentsAnalyzer.shouldUseSerializableTypesRule);
      }
    }

    public override void Initialize(AnalysisContext context)
    { }
  }
}
