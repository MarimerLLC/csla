//-----------------------------------------------------------------------
// <copyright file="TypeDefinitionExtractor.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Extract the definition of a type for source generation</summary>
//-----------------------------------------------------------------------

using System.Text;
using Csla.Generator.DataPortalInterfaces.CSharp.Extractors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Csla.Generator.DataPortalInterfaces.CSharp.Discovery
{

  /// <summary>
  /// Extract the definition of a type for which data portal interface
  /// source generation is required
  /// </summary>
  internal static class TypeDefinitionExtractor
  {

    /// <summary>
    /// Extract the data needed for source generation from the syntax tree
    /// </summary>
    public static ExtractedTypeDefinition ExtractTypeDefinition(
      DefinitionExtractionContext extractionContext,
      ClassDeclarationSyntax targetTypeDeclaration)
    {
      var fullyQualifiedNameBuilder = new StringBuilder();

      var definition = new ExtractedTypeDefinition
      {
        TypeName = GetTypeName(targetTypeDeclaration),
        TypeParameters = GetTypeParameters(targetTypeDeclaration),
        Namespace = GetNamespace(targetTypeDeclaration),
        Scope = GetScopeDefinition(targetTypeDeclaration),
        IsPartial = targetTypeDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword)
      };

      foreach (var containerDefinition in ContainerDefinitionsExtractor.GetContainerDefinitions(extractionContext, targetTypeDeclaration))
      {
        definition.ContainerDefinitions.Add(containerDefinition);
        fullyQualifiedNameBuilder.Append(containerDefinition.Name).Append('.');
      }

      foreach (var operationMethod in OperationMethodExtractor.ExtractOperationMethods(extractionContext, targetTypeDeclaration))
      {
        definition.OperationMethods.Add(operationMethod);
      }

      fullyQualifiedNameBuilder.Append(definition.TypeName);
      definition.FullyQualifiedName = fullyQualifiedNameBuilder.ToString();

      return definition;
    }

    private static string GetNamespace(TypeDeclarationSyntax targetTypeDeclaration)
    {
      string nameSpace = string.Empty;
      var potentialNamespaceParent = targetTypeDeclaration.Parent;

      while (potentialNamespaceParent != null &&
             potentialNamespaceParent is not NamespaceDeclarationSyntax &&
             potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
      {
        potentialNamespaceParent = potentialNamespaceParent.Parent;
      }

      if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
      {
        nameSpace = namespaceParent.Name.ToString();

        while (true)
        {
          if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
          {
            break;
          }

          nameSpace = $"{parent.Name}.{nameSpace}";
          namespaceParent = parent;
        }
      }

      return nameSpace;
    }

    private static string GetScopeDefinition(TypeDeclarationSyntax targetTypeDeclaration)
    {
      var scopeNameBuilder = new StringBuilder();

      foreach (var modifier in targetTypeDeclaration.Modifiers)
      {
        if (modifier.IsKind(SyntaxKind.PublicKeyword) ||
            modifier.IsKind(SyntaxKind.InternalKeyword) ||
            modifier.IsKind(SyntaxKind.ProtectedKeyword) ||
            modifier.IsKind(SyntaxKind.PrivateKeyword))
        {
          scopeNameBuilder.Append(modifier.ValueText);
          scopeNameBuilder.Append(' ');
        }
      }

      if (scopeNameBuilder.Length < 1)
      {
        scopeNameBuilder.Append("internal ");
      }

      return scopeNameBuilder.ToString().TrimEnd();
    }

    private static string GetTypeName(TypeDeclarationSyntax targetTypeDeclaration)
    {
      return targetTypeDeclaration.Identifier.ToString();
    }

    private static string GetTypeParameters(TypeDeclarationSyntax targetTypeDeclaration)
    {
      if (targetTypeDeclaration.TypeParameterList != null)
      {
        return targetTypeDeclaration.TypeParameterList.ToString();
      }
      return string.Empty;
    }
  }
}
