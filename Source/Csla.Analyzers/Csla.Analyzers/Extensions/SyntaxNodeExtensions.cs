using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Csla.Analyzers.Extensions
{
  internal static class SyntaxNodeExtensions
  {
    internal static bool HasUsing(this SyntaxNode @this, string qualifiedName)
    {
      if (@this == null)
      {
        return false;
      }

      if (@this.Kind() == SyntaxKind.UsingDirective)
      {
        var usingNode = @this as UsingDirectiveSyntax;

        if (usingNode.Name.ToFullString() == qualifiedName)
        {
          return true;
        }
      }

      return @this.ChildNodes().Where(_ => _.HasUsing(qualifiedName)).Any();
    }

    internal static T FindParent<T>(this SyntaxNode @this)
      where T : SyntaxNode
    {
      var parentNode = @this.Parent;

      while (parentNode != null)
      {
        if (parentNode is T parentAsTypeNode)
        {
          return parentAsTypeNode;
        }

        parentNode = parentNode.Parent;
      }

      return null;
    }
  }
}