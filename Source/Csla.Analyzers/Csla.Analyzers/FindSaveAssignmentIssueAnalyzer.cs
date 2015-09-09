using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using static Csla.Analyzers.Extensions.ITypeSymbolExtensions;

namespace Csla.Analyzers
{
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class FindSaveAssignmentIssueAnalyzer
    : DiagnosticAnalyzer
  {
    private static DiagnosticDescriptor saveResultIsNotAssignedRule = new DiagnosticDescriptor(
      FindSaveAssignmentIssueAnalyzerConstants.DiagnosticId, FindSaveAssignmentIssueAnalyzerConstants.Title,
      FindSaveAssignmentIssueAnalyzerConstants.Message, FindSaveAssignmentIssueAnalyzerConstants.Category,
      DiagnosticSeverity.Error, true);
    private static DiagnosticDescriptor saveAsyncResultIsNotAssignedRule = new DiagnosticDescriptor(
      FindSaveAsyncAssignmentIssueAnalyzerConstants.DiagnosticId, FindSaveAsyncAssignmentIssueAnalyzerConstants.Title,
      FindSaveAsyncAssignmentIssueAnalyzerConstants.Message, FindSaveAsyncAssignmentIssueAnalyzerConstants.Category,
      DiagnosticSeverity.Error, true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
      get
      {
        return ImmutableArray.Create(
          FindSaveAssignmentIssueAnalyzer.saveResultIsNotAssignedRule,
          FindSaveAssignmentIssueAnalyzer.saveAsyncResultIsNotAssignedRule);
      }
    }

    public override void Initialize(AnalysisContext context)
    {
      context.RegisterSyntaxNodeAction<SyntaxKind>(
        FindSaveAssignmentIssueAnalyzer.AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
      // http://stackoverflow.com/questions/29614112/how-to-get-invoked-method-name-in-roslyn
      var invocationNode = (InvocationExpressionSyntax)context.Node;
      var invocationSymbol = context.SemanticModel.GetSymbolInfo(invocationNode.Expression).Symbol;

      if ((invocationSymbol?.ContainingType?.IsBusinessBase() ?? false))
      {
        if (context.CancellationToken.IsCancellationRequested)
        {
          return;
        }

        if (invocationSymbol?.Name == "Save")
        {
          FindSaveAssignmentIssueAnalyzer.CheckForCondition(context, invocationNode, 
            invocationNode.Parent, FindSaveAssignmentIssueAnalyzer.saveResultIsNotAssignedRule);
        }
        else if (invocationSymbol?.Name == "SaveAsync")
        {
          FindSaveAssignmentIssueAnalyzer.CheckForCondition(context, invocationNode,
            invocationNode.Parent?.Parent, FindSaveAssignmentIssueAnalyzer.saveAsyncResultIsNotAssignedRule);
        }
      }
    }

    private static void CheckForCondition(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationNode, 
      SyntaxNode expressionStatementParent, DiagnosticDescriptor descriptor)
    {
      if ((!expressionStatementParent?.DescendantNodesAndTokens()?.Any(_ => _.IsKind(SyntaxKind.EqualsToken)) ?? false) &&
        !(invocationNode.DescendantNodes()?.Any(_ => new ContainsInvocationExpressionWalker(_).HasIssue) ?? false) &&
        !FindSaveAssignmentIssueAnalyzer.IsReturnValue(invocationNode))
      {
        context.ReportDiagnostic(Diagnostic.Create(descriptor, invocationNode.GetLocation()));
      }
    }

    private static bool IsReturnValue(InvocationExpressionSyntax invocationNode)
    {
      var parentNode = invocationNode?.Parent;
      var foundReturn = false;

      while(parentNode != null && !(parentNode is BlockSyntax))
      {
        foundReturn = parentNode is ReturnStatementSyntax ||
          parentNode is ParenthesizedLambdaExpressionSyntax;

        if(foundReturn)
        {
          break;
        }

        parentNode = parentNode.Parent;
      }

      return foundReturn;
    }
  }
}