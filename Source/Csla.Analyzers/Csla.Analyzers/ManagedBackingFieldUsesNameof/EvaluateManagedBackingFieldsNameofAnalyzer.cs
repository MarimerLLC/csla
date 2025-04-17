using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Reflection;
using static Csla.Analyzers.Extensions.ITypeSymbolExtensions;

namespace Csla.Analyzers.ManagedBackingFieldUsesNameof
{
  /// <summary>
  /// 
  /// </summary>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class EvaluateManagedBackingFieldsNameofAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor shouldUseNameofRule =
      new(
        Constants.AnalyzerIdentifiers.EvaluateManagedBackingFieldsNameof,
        EvaluateManagedBackingFieldsNameofAnalyzerConstants.Title,
        EvaluateManagedBackingFieldsNameofAnalyzerConstants.Message,
        Constants.Categories.Refactoring, DiagnosticSeverity.Info, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.EvaluateManagedBackingFieldsNameof, nameof(EvaluateManagedBackingFieldsNameofAnalyzer)));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [shouldUseNameofRule];

    /// <param name="context"></param>
    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
      context.EnableConcurrentExecution();
      context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.FieldDeclaration);
    }

    // Extend the AnalyzeFieldDeclaration method or create a new method to analyze invocation expressions
    private static void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
    {
      var fieldNode = (FieldDeclarationSyntax)context.Node;
      if (!fieldNode.ContainsDiagnostics)
      {
        foreach (var variable in fieldNode.Declaration.Variables)
        {
          context.CancellationToken.ThrowIfCancellationRequested();

          if (context.SemanticModel.GetDeclaredSymbol(variable) is IFieldSymbol fieldSymbol && variable.Initializer?.Value is InvocationExpressionSyntax invocation)
          {
            if (fieldSymbol.Type.IsIPropertyInfo() && invocation.Expression is GenericNameSyntax invocationName)
            {
              var methodName = invocationName.Identifier.Text;
              if (methodName == "RegisterProperty" && invocation.ArgumentList.Arguments.Count > 0)
              {
                var firstArgument = invocation.ArgumentList.Arguments[0];
                if (firstArgument.Expression is SimpleLambdaExpressionSyntax propertyLambda)
                {
                  // Report diagnostic to suggest using nameof
                  var propertyName = propertyLambda.Body.ToString();
                  var diagnostic = Diagnostic.Create(shouldUseNameofRule, firstArgument.GetLocation(), propertyName);
                  context.ReportDiagnostic(diagnostic);
                }
              }
            }
          }
        }
      }
    }
  }
}