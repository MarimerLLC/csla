//-----------------------------------------------------------------------
// <copyright file="DefinitionExtractionContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Wraps a SemanticModel for definition extraction</summary>
//-----------------------------------------------------------------------

using Microsoft.CodeAnalysis;

namespace Csla.Generator.DataPortalInterfaces.CSharp.Discovery
{

  /// <summary>
  /// Wraps a SemanticModel for definition extraction,
  /// providing helper methods for symbol resolution
  /// </summary>
  internal class DefinitionExtractionContext
  {
    private readonly SemanticModel _semanticModel;

    /// <summary>
    /// Create a new DefinitionExtractionContext
    /// </summary>
    /// <param name="semanticModel">The Roslyn SemanticModel to use</param>
    public DefinitionExtractionContext(SemanticModel semanticModel)
    {
      _semanticModel = semanticModel;
    }

    /// <summary>
    /// Gets the SemanticModel
    /// </summary>
    public SemanticModel SemanticModel => _semanticModel;

    /// <summary>
    /// Get the symbol for a syntax node
    /// </summary>
    public ISymbol? GetDeclaredSymbol(SyntaxNode node)
    {
      return _semanticModel.GetDeclaredSymbol(node);
    }

    /// <summary>
    /// Get the type info for a syntax node
    /// </summary>
    public TypeInfo GetTypeInfo(SyntaxNode node)
    {
      return _semanticModel.GetTypeInfo(node);
    }
  }
}
