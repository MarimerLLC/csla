using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Csla.Analyzers.Extensions
{
  internal static class ISymbolExtensions
  {
    internal static string GetFullNamespace(this ISymbol @this)
    {
      var namespaces = new List<string>();
      var @namespace = @this.ContainingNamespace;

      while (@namespace != null)
      {
        if (!string.IsNullOrWhiteSpace(@namespace.Name))
        {
          namespaces.Add(@namespace.Name);
        }

        @namespace = @namespace.ContainingNamespace;
      }

      namespaces.Reverse();

      return string.Join(".", namespaces);
    }
  }
}