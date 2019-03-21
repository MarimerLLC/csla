using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Csla.Analyzers
{
  internal sealed class FindGetOrReadInvocationsWalker
    : CSharpSyntaxWalker
  {
    internal FindGetOrReadInvocationsWalker(SyntaxNode node, SemanticModel model)
    {
      this.Model = model;
      base.Visit(node);
    }

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
      var symbol = this.Model.GetSymbolInfo(node);
      var methodSymbol = symbol.Symbol as IMethodSymbol;

      if (methodSymbol.IsPropertyInfoManagementMethod())
      {
        this.Invocation = node;
      }
    }

    internal InvocationExpressionSyntax Invocation { get; private set; }
    private SemanticModel Model { get; }
  }
}
