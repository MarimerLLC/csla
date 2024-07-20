//-----------------------------------------------------------------------
// <copyright file="DefinitionExtractionContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Helper for definition extraction, used to optimise symbo, recognition</summary>
//-----------------------------------------------------------------------
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Csla.Generator.AutoImplementProperties.CSharp.AutoImplement.Discovery
{

  /// <summary>
  /// Helper for definition extraction, used to optimise symbol recognition
  /// </summary>
  internal class DefinitionExtractionContext(SemanticModel _semanticModel, bool _addAttributes, bool _filterPartialProperties)
  {
    public SemanticModel SemanticModel => _semanticModel;

    public bool AddAttributes => _addAttributes;

    public bool FilterPartialProperties => _filterPartialProperties;

    /// <summary>
    /// Get the namespace of the type represented by a type declaration
    /// </summary>
    /// <param name="typeDeclarationSyntax">The type declaration syntax representing the type to be tested</param>
    /// <returns>The namespace in which the type is declared, or an empty string if it is global</returns>
    public string GetTypeNamespace(TypeDeclarationSyntax typeDeclarationSyntax)
    {
      INamedTypeSymbol typeSymbol;

      typeSymbol = _semanticModel.GetDeclaredSymbol(typeDeclarationSyntax) as INamedTypeSymbol;
      if (typeSymbol is null || typeSymbol.ContainingNamespace is null) return string.Empty;
      return typeSymbol.ContainingNamespace.ToString();
    }

    /// <summary>
    /// Get the namespace of the type represented by a type declaration
    /// </summary>
    /// <param name="typeSyntax">The type syntax representing the type to be tested</param>
    /// <returns>The namespace in which the type is declared, or an empty string if it is global</returns>
    public string GetTypeNamespace(TypeSyntax typeSyntax)
    {
      INamedTypeSymbol typeSymbol;

      typeSymbol = _semanticModel.GetSymbolInfo(typeSyntax).Symbol as INamedTypeSymbol;
      if (typeSymbol is null || typeSymbol.ContainingNamespace is null) return string.Empty;
      return typeSymbol.ContainingNamespace.ToString();
    }


    #region Private Helper Methods



    /// <summary>
    /// Perform a recursive match on a namespace symbol by name
    /// </summary>
    /// <param name="namespaceSymbol">The symbol for which a match is being tested</param>
    /// <param name="desiredTypeNamespace">The desired namespace, including period separators if necessary</param>
    /// <returns>Boolean true if the namespace symbol matches that desired by name</returns>
    private bool IsMatchingNamespaceSymbol(INamespaceSymbol namespaceSymbol, string desiredTypeNamespace)
    {
      string endNamespace;
      string remainingNamespace = string.Empty;

      // Split off the end namespace section (the string after the last period) to test
      endNamespace = desiredTypeNamespace;
      int separatorPosition = desiredTypeNamespace.LastIndexOf('.');
      if (separatorPosition > -1)
      {
        endNamespace = desiredTypeNamespace.Substring(separatorPosition + 1);
        remainingNamespace = desiredTypeNamespace.Substring(0, separatorPosition);
      }
      if (!namespaceSymbol.Name.Equals(endNamespace, StringComparison.InvariantCultureIgnoreCase)) return false;

      if (string.IsNullOrWhiteSpace(remainingNamespace))
      {
        return true;
      }

      // Recurse down remaining namespace sections until it is complete
      return IsMatchingNamespaceSymbol(namespaceSymbol.ContainingNamespace, remainingNamespace);
    }

    #endregion

  }
}
