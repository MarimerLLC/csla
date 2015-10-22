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
      var invocationSymbol = this.Model.GetSymbolInfo(node).Symbol;

      if(invocationSymbol != null && invocationSymbol.ContainingType.Name == CslaMemberConstants.CslaTypeNames.BusinessBase &&
        (invocationSymbol.Name == CslaMemberConstants.CslaPropertyMethods.GetProperty ||
        invocationSymbol.Name == CslaMemberConstants.CslaPropertyMethods.GetPropertyConvert ||
        invocationSymbol.Name == CslaMemberConstants.CslaPropertyMethods.ReadProperty ||
        invocationSymbol.Name == CslaMemberConstants.CslaPropertyMethods.ReadPropertyConvert ||
        invocationSymbol.Name == CslaMemberConstants.CslaPropertyMethods.LazyGetProperty ||
        invocationSymbol.Name == CslaMemberConstants.CslaPropertyMethods.LazyGetPropertyAsync ||
        invocationSymbol.Name == CslaMemberConstants.CslaPropertyMethods.LazyReadProperty ||
        invocationSymbol.Name == CslaMemberConstants.CslaPropertyMethods.LazyReadPropertyAsync))
      {
        this.Invocation = node;
      }
    }

    internal InvocationExpressionSyntax Invocation { get; private set; }
    private SemanticModel Model { get; }
  }
}
