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
      this.Model = model;
      base.Visit(node);
    }

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
      var invocationSymbol = this.Model.GetSymbolInfo(node).Symbol;

      if (invocationSymbol != null && invocationSymbol.ContainingType.Name == CslaMemberConstants.CslaTypeNames.BusinessBase &&
        (invocationSymbol.Name == CslaMemberConstants.CslaPropertyMethods.SetProperty ||
        invocationSymbol.Name == CslaMemberConstants.CslaPropertyMethods.SetPropertyConvert ||
        invocationSymbol.Name == CslaMemberConstants.CslaPropertyMethods.LoadProperty ||
        invocationSymbol.Name == CslaMemberConstants.CslaPropertyMethods.LoadPropertyAsync ||
        invocationSymbol.Name == CslaMemberConstants.CslaPropertyMethods.LoadPropertyConvert ||
        invocationSymbol.Name == CslaMemberConstants.CslaPropertyMethods.LoadPropertyMarkDirty))
      {
        this.Invocation = node;
      }
    }

    internal InvocationExpressionSyntax Invocation { get; private set; }
    private SemanticModel Model { get; }
  }
}
