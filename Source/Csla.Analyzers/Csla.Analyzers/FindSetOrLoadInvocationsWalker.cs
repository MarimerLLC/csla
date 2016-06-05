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

      if (invocationSymbol != null && invocationSymbol.ContainingType.Name == CslaMemberConstants.Types.BusinessBase &&
        (invocationSymbol.Name == CslaMemberConstants.Properties.SetProperty ||
        invocationSymbol.Name == CslaMemberConstants.Properties.SetPropertyConvert ||
        invocationSymbol.Name == CslaMemberConstants.Properties.LoadProperty ||
        invocationSymbol.Name == CslaMemberConstants.Properties.LoadPropertyAsync ||
        invocationSymbol.Name == CslaMemberConstants.Properties.LoadPropertyConvert ||
        invocationSymbol.Name == CslaMemberConstants.Properties.LoadPropertyMarkDirty))
      {
        this.Invocation = node;
      }
    }

    internal InvocationExpressionSyntax Invocation { get; private set; }
    private SemanticModel Model { get; }
  }
}
