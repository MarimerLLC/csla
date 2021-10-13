//-----------------------------------------------------------------------
// <copyright file="TypeDefinitionExtractor.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Extract the definition of a type for source generation</summary>
//-----------------------------------------------------------------------
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;

namespace Csla.Generators.CSharp.AutoSerialization.Discovery
{

  /// <summary>
  /// Extract the definition of a type for which source generation is required
  /// This is used to detach the builder from the Roslyn infrastructure, to enable testing
  /// </summary>
  internal static class TypeDefinitionExtractor
  {

    /// <summary>
    /// Extract the data that will be needed for source generation from the syntax tree provided
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed</param>
    /// <param name="targetTypeDeclaration">The TypeDeclarationSyntax from which to extract the necessary data</param>
    /// <returns>ExtractedTypeDefinition containing the data extracted from the syntax tree</returns>
    public static ExtractedTypeDefinition ExtractTypeDefinition(DefinitionExtractionContext extractionContext, TypeDeclarationSyntax targetTypeDeclaration)
    {
      ExtractedTypeDefinition definition = new ExtractedTypeDefinition();
      StringBuilder fullyQualifiedNameBuilder = new StringBuilder();

      definition.TypeName = GetTypeName(extractionContext, targetTypeDeclaration);
      definition.TypeKind = GetTypeKind(extractionContext, targetTypeDeclaration);
      definition.Namespace = GetNamespace(extractionContext, targetTypeDeclaration);
      definition.Scope = GetScopeDefinition(extractionContext, targetTypeDeclaration);

      foreach (ExtractedContainerDefinition containerDefinition in ContainerDefinitionsExtractor.GetContainerDefinitions(extractionContext, targetTypeDeclaration))
      {
        definition.ContainerDefinitions.Add(containerDefinition);
        fullyQualifiedNameBuilder.Append(containerDefinition.Name);
        fullyQualifiedNameBuilder.Append(".");
      }

      foreach (ExtractedPropertyDefinition propertyDefinition in PropertyDefinitionsExtractor.ExtractPropertyDefinitions(extractionContext, targetTypeDeclaration))
      {
        definition.Properties.Add(propertyDefinition);
      }

      foreach (ExtractedFieldDefinition fieldDefinition in FieldDefinitionsExtractor.ExtractFieldDefinitions(extractionContext, targetTypeDeclaration))
      {
        definition.Fields.Add(fieldDefinition);
      }

      fullyQualifiedNameBuilder.Append(definition.TypeName);
      definition.FullyQualifiedName = fullyQualifiedNameBuilder.ToString();

      return definition;
    }

    #region Private Helper Methods

    /// <summary>
    /// Extract the namespace of the type for which we will be generating code
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed</param>
    /// <param name="targetTypeDeclaration">The TypeDeclarationSyntax from which to extract the necessary information</param>
    /// <returns>The namespace of the type for which generation is being performed</returns>
    private static string GetNamespace(DefinitionExtractionContext extractionContext, TypeDeclarationSyntax targetTypeDeclaration)
    {
      string namespaceName = string.Empty;
      NamespaceDeclarationSyntax namespaceDeclaration;
      TypeDeclarationSyntax containingTypeDeclaration;

      // Iterate through the containing types should the target type be nested inside other types
      containingTypeDeclaration = targetTypeDeclaration;
      while (containingTypeDeclaration.Parent is TypeDeclarationSyntax)
      {
        containingTypeDeclaration = (TypeDeclarationSyntax)containingTypeDeclaration.Parent;
      }

      namespaceDeclaration = containingTypeDeclaration.Parent as NamespaceDeclarationSyntax;
      if (namespaceDeclaration is not null)
      {
        namespaceName = namespaceDeclaration.Name.ToString();
      }

      return namespaceName;
    }

    /// <summary>
    /// Extract the scope of the type for which we will be generating code
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed</param>
    /// <param name="targetTypeDeclaration">The TypeDeclarationSyntax from which to extract the necessary information</param>
    /// <returns>The scope of the type for which generation is being performed</returns>
    private static string GetScopeDefinition(DefinitionExtractionContext extractionContext, TypeDeclarationSyntax targetTypeDeclaration)
    {
      StringBuilder scopeNameBuilder = new StringBuilder();

      foreach (SyntaxToken modifier in targetTypeDeclaration.Modifiers)
      {
        if (modifier.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PublicKeyword))
        {
          AppendScopeName(scopeNameBuilder, modifier.ValueText);
          continue;
        }
        if (modifier.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.InternalKeyword))
        {
          AppendScopeName(scopeNameBuilder, modifier.ValueText);
          continue;
        }
        if (modifier.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.ProtectedKeyword))
        {
          AppendScopeName(scopeNameBuilder, modifier.ValueText);
          continue;
        }
        if (modifier.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PrivateKeyword))
        {
          AppendScopeName(scopeNameBuilder, modifier.ValueText);
          continue;
        }
      }

      if (scopeNameBuilder.Length < 1)
      {
        scopeNameBuilder.Append("internal");
      }

      return scopeNameBuilder.ToString().Trim();
    }

    /// <summary>
    /// Append a scope name to a StringBuilder being used to build the scopes
    /// </summary>
    /// <param name="stringBuilder">The StringBuilder to which to append the provided scope</param>
    /// <param name="scope">The name of the scope we are to append</param>
    private static void AppendScopeName(StringBuilder stringBuilder, string scope)
    {
      stringBuilder.Append(scope);
      stringBuilder.Append(" ");
    }

    /// <summary>
    /// Extract the name of the type for which we will be generating code
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed</param>
    /// <param name="targetTypeDeclaration">The TypeDeclarationSyntax from which to extract the necessary information</param>
    /// <returns>The name of the type for which generation is being performed</returns>
    private static string GetTypeName(DefinitionExtractionContext extractionContext, TypeDeclarationSyntax targetTypeDeclaration)
    {
      return targetTypeDeclaration.Identifier.ToString();
    }

    /// <summary>
    /// Extract the textual definition of the kind that this type represents
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed</param>
    /// <param name="targetTypeDeclaration">The TypeDeclarationSyntax from which to extract the necessary information</param>
    /// <returns>The kind of the type for which generation is being performed</returns>
    private static string GetTypeKind(DefinitionExtractionContext extractionContext, TypeDeclarationSyntax targetTypeDeclaration)
    {
      return targetTypeDeclaration.Keyword.ToString();
    }

    #endregion

  }
}
