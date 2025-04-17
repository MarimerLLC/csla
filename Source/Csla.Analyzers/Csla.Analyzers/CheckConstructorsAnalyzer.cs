using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using static Csla.Analyzers.Extensions.ITypeSymbolExtensions;

namespace Csla.Analyzers
{
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class CheckConstructorsAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor publicNoArgumentConstructorIsMissingRule = 
      new(
        Constants.AnalyzerIdentifiers.PublicNoArgumentConstructorIsMissing, PublicNoArgumentConstructorIsMissingConstants.Title,
        PublicNoArgumentConstructorIsMissingConstants.Message, Constants.Categories.Usage,
        DiagnosticSeverity.Error, true, 
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.PublicNoArgumentConstructorIsMissing, nameof(CheckConstructorsAnalyzer)));
    private static readonly DiagnosticDescriptor constructorHasParametersRule = 
      new(
        Constants.AnalyzerIdentifiers.ConstructorHasParameters, ConstructorHasParametersConstants.Title,
        ConstructorHasParametersConstants.Message, Constants.Categories.Usage,
        DiagnosticSeverity.Warning, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.ConstructorHasParameters, nameof(CheckConstructorsAnalyzer)));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
    [
        publicNoArgumentConstructorIsMissingRule,
        constructorHasParametersRule
    ];

    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
      context.EnableConcurrentExecution();
      context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
    }

    private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
    {
      var hasPublicNoArgumentConstructor = false;
      var hasNonPublicNoArgumentConstructor = false;
      var classNode = (ClassDeclarationSyntax)context.Node;
      var classSymbol = context.SemanticModel.GetDeclaredSymbol(classNode);

      if (classSymbol.IsStereotype() && !classSymbol.IsAbstract)
      {
        foreach (var constructor in classSymbol.Constructors)
        {
          if (!constructor.IsStatic)
          {
            if (constructor.DeclaredAccessibility == Accessibility.Public)
            {
              if (constructor.Parameters.Length == 0)
              {
                hasPublicNoArgumentConstructor = true;
              }
              else if(classSymbol.IsEditableStereotype())
              {
                foreach (var location in constructor.Locations)
                {
                  context.ReportDiagnostic(Diagnostic.Create(
                    constructorHasParametersRule, location));
                }
              }
            }
            else if (constructor.Parameters.Length == 0)
            {
              hasNonPublicNoArgumentConstructor = true;
            }
          }
        }

        if (!hasPublicNoArgumentConstructor)
        {
          var properties = new Dictionary<string, string?>
          {
            [PublicNoArgumentConstructorIsMissingConstants.HasNonPublicNoArgumentConstructor] = hasNonPublicNoArgumentConstructor.ToString()
          }.ToImmutableDictionary();

          context.ReportDiagnostic(Diagnostic.Create(
            publicNoArgumentConstructorIsMissingRule,
            classNode.Identifier.GetLocation(), properties));
        }
      }
    }
  }
}