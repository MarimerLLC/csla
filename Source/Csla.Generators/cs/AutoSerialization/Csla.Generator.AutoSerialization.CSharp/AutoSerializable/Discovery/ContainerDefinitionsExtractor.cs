//-----------------------------------------------------------------------
// <copyright file="ContainerDefinitionsExtractor.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Extract the definitions of the containers of a type</summary>
//-----------------------------------------------------------------------
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace Csla.Generator.AutoSerialization.CSharp.AutoSerialization.Discovery
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
      List<ExtractedContainerDefinition> containers = [];

      // Iterate through the containing types should the target type be nested inside other types
      var containingTypeDeclaration = targetTypeDeclaration;
      while (containingTypeDeclaration.Parent is TypeDeclarationSyntax syntax)
      {
        containingTypeDeclaration = syntax;
        containers.Add(GetContainerDefinition(containingTypeDeclaration));
      }

      var namespaceDeclaration = containingTypeDeclaration.Parent as NamespaceDeclarationSyntax;
      if (namespaceDeclaration is not null)
      {
        containers.Add(GetContainerDefinition(namespaceDeclaration));
      }

      containers.Reverse();

      return containers;
    }

    private static ExtractedContainerDefinition GetContainerDefinition(TypeDeclarationSyntax typeDeclarationSyntax)
    {
      StringBuilder containerDefinitionBuilder = new StringBuilder();
      ExtractedContainerDefinition containerDefinition;

      foreach (SyntaxToken modifier in typeDeclarationSyntax.Modifiers)
      {
        containerDefinitionBuilder.Append(modifier.ToString());
        containerDefinitionBuilder.Append(' ');
      }

      containerDefinitionBuilder.Append(typeDeclarationSyntax.Keyword.ToString());
      containerDefinitionBuilder.Append(' ');
      containerDefinitionBuilder.Append(typeDeclarationSyntax.Identifier.ToString());

      containerDefinition = new ExtractedContainerDefinition
      {
        Name = typeDeclarationSyntax.Identifier.ToString(),
        FullDefinition = containerDefinitionBuilder.ToString()
      };

      return containerDefinition;
    }

    private static ExtractedContainerDefinition GetContainerDefinition(NamespaceDeclarationSyntax namespaceDeclarationSyntax)
    {
      StringBuilder containerDefinitionBuilder = new StringBuilder();
      ExtractedContainerDefinition containerDefinition;

      foreach (SyntaxToken modifier in namespaceDeclarationSyntax.Modifiers)
      {
        containerDefinitionBuilder.Append(modifier.ToString());
        containerDefinitionBuilder.Append(' ');
      }

      var namespaceValue = namespaceDeclarationSyntax.Name.ToString();
      if (!string.IsNullOrWhiteSpace(namespaceValue))
      {
        containerDefinitionBuilder.Append("namespace ");
        containerDefinitionBuilder.Append(namespaceValue);
      }

      containerDefinition = new ExtractedContainerDefinition
      {
        Name = namespaceDeclarationSyntax.Name.ToString(),
        FullDefinition = containerDefinitionBuilder.ToString()

      };

      return containerDefinition;
    }



  }
}
