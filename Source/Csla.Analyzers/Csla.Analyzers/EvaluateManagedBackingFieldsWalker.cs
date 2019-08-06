using Csla.Analyzers.Extensions;
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
      (FieldSymbol, Model) = (fieldSymbol, model);
      Visit(node);
    }

    public override void VisitInvocationExpression(InvocationExpressionSyntax node)
    {
      var invocationSymbol = Model.GetSymbolInfo(node).Symbol as IMethodSymbol;

      if (invocationSymbol.IsPropertyInfoManagementMethod())
      {
        foreach (var argument in node.ArgumentList.Arguments)
        {
          var argumentSymbol = Model.GetSymbolInfo(argument.Expression).Symbol;
          UsesField = argumentSymbol != null && Equals(argumentSymbol, FieldSymbol);
        }
      }
    }

    private SemanticModel Model { get; }
    private IFieldSymbol FieldSymbol { get; }
    public bool UsesField { get; private set; }
  }
}