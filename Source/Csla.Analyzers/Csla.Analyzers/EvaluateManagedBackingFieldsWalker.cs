using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Csla.Analyzers
{
  internal sealed class EvaluateManagedBackingFieldsWalker
    : CSharpSyntaxWalker
  {
    internal EvaluateManagedBackingFieldsWalker(SyntaxNode node, SemanticModel model, IFieldSymbol fieldSymbol)
    {
      this.FieldSymbol = fieldSymbol;
      this.Model = model;
      base.Visit(node);
    }

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
      var invocationSymbol = this.Model.GetSymbolInfo(node).Symbol as IMethodSymbol;

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
        // Check arguments.
        foreach (var xx in node.ArgumentList.Arguments)
        {
          var argumentSymbol = this.Model.GetSymbolInfo(xx.Expression).Symbol;

          if (argumentSymbol != null && argumentSymbol == this.FieldSymbol)
          {
            this.UsesField = true;
          }
        }
      }
    }

    private SemanticModel Model { get; }
    private IFieldSymbol FieldSymbol { get; }
    public bool UsesField { get; private set; }
  }
}
