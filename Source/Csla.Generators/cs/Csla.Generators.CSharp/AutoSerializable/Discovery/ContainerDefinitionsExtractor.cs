//-----------------------------------------------------------------------
// <copyright file="ContainerDefinitionsExtractor.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Extract the definitions of the containers of a type</summary>
//-----------------------------------------------------------------------
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Generators.CSharp.AutoSerialization.Discovery
{
  internal static class ContainerDefinitionsExtractor
  {

    /// <summary>
    /// Extract the definitions of the containers of the type for which we will be generating code
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed</param>
    /// <param name="targetTypeDeclaration">The TypeDeclarationSyntax from which to extract the necessary information</param>
    /// <returns>The definitions of all of the containers of the type for which generation is being performed</returns>
    public static IReadOnlyList<ExtractedContainerDefinition> GetContainerDefinitions(DefinitionExtractionContext extractionContext, TypeDeclarationSyntax targetTypeDeclaration)
    {
      NamespaceDeclarationSyntax namespaceDeclaration;
      TypeDeclarationSyntax containingTypeDeclaration;
      List<ExtractedContainerDefinition> containers = new List<ExtractedContainerDefinition>();

      // Iterate through the containing types should the target type be nested inside other types
      containingTypeDeclaration = targetTypeDeclaration;
      while (containingTypeDeclaration.Parent is TypeDeclarationSyntax)
      {
        containingTypeDeclaration = (TypeDeclarationSyntax)containingTypeDeclaration.Parent;
        containers.Add(GetContainerDefinition(extractionContext, containingTypeDeclaration));
      }

      namespaceDeclaration = containingTypeDeclaration.Parent as NamespaceDeclarationSyntax;
      if (namespaceDeclaration is not null)
      {
        containers.Add(GetContainerDefinition(extractionContext, namespaceDeclaration));
      }

      containers.Reverse();

      return containers;
    }

    private static ExtractedContainerDefinition GetContainerDefinition(DefinitionExtractionContext extractionContext, TypeDeclarationSyntax typeDeclarationSyntax)
    {
      StringBuilder containerDefinitionBuilder = new StringBuilder();
      ExtractedContainerDefinition containerDefinition;

      foreach (SyntaxToken modifier in typeDeclarationSyntax.Modifiers)
      {
        containerDefinitionBuilder.Append(modifier.ToString());
        containerDefinitionBuilder.Append(" ");
      }

      containerDefinitionBuilder.Append(typeDeclarationSyntax.Keyword.ToString());
      containerDefinitionBuilder.Append(" ");
      containerDefinitionBuilder.Append(typeDeclarationSyntax.Identifier.ToString());

      containerDefinition = new ExtractedContainerDefinition()
      {
        Name = typeDeclarationSyntax.Identifier.ToString(),
        FullDefinition = containerDefinitionBuilder.ToString()

      };

      return containerDefinition;
    }

    private static ExtractedContainerDefinition GetContainerDefinition(DefinitionExtractionContext extractionContext, NamespaceDeclarationSyntax namespaceDeclarationSyntax)
    {
      StringBuilder containerDefinitionBuilder = new StringBuilder();
      ExtractedContainerDefinition containerDefinition;

      foreach (SyntaxToken modifier in namespaceDeclarationSyntax.Modifiers)
      {
        containerDefinitionBuilder.Append(modifier.ToString());
        containerDefinitionBuilder.Append(" ");
      }

      containerDefinitionBuilder.Append("namespace ");
      containerDefinitionBuilder.Append(namespaceDeclarationSyntax.Name.ToString());

      containerDefinition = new ExtractedContainerDefinition()
      {
        Name = namespaceDeclarationSyntax.Name.ToString(),
        FullDefinition = containerDefinitionBuilder.ToString()

      };

      return containerDefinition;
    }



  }
}
