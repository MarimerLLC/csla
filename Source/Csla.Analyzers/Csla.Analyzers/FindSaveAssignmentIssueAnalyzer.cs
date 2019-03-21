using Csla.Analyzers.Extensions;
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
      Constants.AnalyzerIdentifiers.FindSaveAssignmentIssue, FindSaveAssignmentIssueAnalyzerConstants.Title,
      FindSaveAssignmentIssueAnalyzerConstants.Message, Constants.Categories.Usage,
      DiagnosticSeverity.Error, true);
    private static DiagnosticDescriptor saveAsyncResultIsNotAssignedRule = new DiagnosticDescriptor(
      Constants.AnalyzerIdentifiers.FindSaveAsyncAssignmentIssue, FindSaveAsyncAssignmentIssueAnalyzerConstants.Title,
      FindSaveAsyncAssignmentIssueAnalyzerConstants.Message, Constants.Categories.Usage,
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

      if (!invocationNode.ContainsDiagnostics)
      {
        var symbol = context.SemanticModel.GetSymbolInfo(invocationNode.Expression);
        var invocationSymbol = symbol.Symbol;

        if ((invocationSymbol?.ContainingType?.IsBusinessBase() ?? false))
        {
          context.CancellationToken.ThrowIfCancellationRequested();
          var expressionStatementNode = invocationNode.FindParent<ExpressionStatementSyntax>();

          if (invocationSymbol?.Name == Constants.SaveMethodNames.Save)
          {
            FindSaveAssignmentIssueAnalyzer.CheckForCondition(context, invocationNode,
              expressionStatementNode, FindSaveAssignmentIssueAnalyzer.saveResultIsNotAssignedRule);
          }
          else if (invocationSymbol?.Name == Constants.SaveMethodNames.SaveAsync)
          {
            FindSaveAssignmentIssueAnalyzer.CheckForCondition(context, invocationNode,
              expressionStatementNode, FindSaveAssignmentIssueAnalyzer.saveAsyncResultIsNotAssignedRule);
          }
        }
      }
    }

    private static void CheckForCondition(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocationNode,
      ExpressionStatementSyntax expressionStatementParent, DiagnosticDescriptor descriptor)
    {
      // Make sure the invocation's containing type is not the same as the class that contains it
      if ((invocationNode.DescendantNodesAndTokens().Any(_ => _.IsKind(SyntaxKind.DotToken)) &&
        !invocationNode.DescendantNodesAndTokens().Any(_ => _.IsKind(SyntaxKind.ThisExpression) || _.IsKind(SyntaxKind.BaseExpression))) &&
        (!expressionStatementParent?.DescendantNodesAndTokens()?.Any(_ => _.IsKind(SyntaxKind.EqualsToken)) ?? false) &&
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

      while (parentNode != null && !(parentNode is BlockSyntax))
      {
        foundReturn = parentNode is ReturnStatementSyntax ||
          parentNode is ParenthesizedLambdaExpressionSyntax;

        if (foundReturn)
        {
          break;
        }

        parentNode = parentNode.Parent;
      }

      return foundReturn;
    }
  }
}