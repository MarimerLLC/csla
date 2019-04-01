using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Csla.Analyzers
{
  internal sealed class ContainsInvocationExpressionWalker
    : CSharpSyntaxWalker
  {
    internal ContainsInvocationExpressionWalker(SyntaxNode node) => Visit(node);

    public override void VisitInvocationExpression(InvocationExpressionSyntax node) => HasIssue = true;

    internal bool HasIssue { get; private set; }
  }
}
