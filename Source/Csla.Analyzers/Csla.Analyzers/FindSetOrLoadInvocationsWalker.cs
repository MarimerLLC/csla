using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Csla.Analyzers
{
  internal sealed class FindSetOrLoadInvocationsWalker
    : CSharpSyntaxWalker
  {
    internal FindSetOrLoadInvocationsWalker(SyntaxNode node, SemanticModel model)
    {
      Model = model;
      Visit(node);
    }

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
      var symbol = Model.GetSymbolInfo(node);
      var methodSymbol = symbol.Symbol as IMethodSymbol;

      if (methodSymbol.IsPropertyInfoManagementMethod())
      {
        Invocation = node;
      }
    }

    internal InvocationExpressionSyntax Invocation { get; private set; }
    private SemanticModel Model { get; }
  }
}
