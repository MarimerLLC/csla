//-----------------------------------------------------------------------
// <copyright file="ContainerDefinitionsExtractor.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Extract container definitions for a type</summary>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Text;
using Csla.Generator.DataPortalInterfaces.CSharp.Extractors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Csla.Generator.DataPortalInterfaces.CSharp.Discovery
{

  /// <summary>
  /// Extract the container definitions for a type declaration
  /// </summary>
  internal static class ContainerDefinitionsExtractor
  {

    /// <summary>
    /// Get the container definitions for a type declaration,
    /// from outermost to innermost
    /// </summary>
    public static IReadOnlyList<ExtractedContainerDefinition> GetContainerDefinitions(
      DefinitionExtractionContext context,
      TypeDeclarationSyntax typeDeclaration)
    {
      var containers = new List<ExtractedContainerDefinition>();
      var parent = typeDeclaration.Parent;

      while (parent != null)
      {
        if (parent is TypeDeclarationSyntax parentType)
        {
          containers.Insert(0, BuildTypeContainer(parentType));
        }
        else if (parent is BaseNamespaceDeclarationSyntax namespaceDecl)
        {
          containers.Insert(0, BuildNamespaceContainer(namespaceDecl));
          break;
        }
        parent = parent.Parent;
      }

      return containers;
    }

    private static ExtractedContainerDefinition BuildNamespaceContainer(BaseNamespaceDeclarationSyntax namespaceDecl)
    {
      var fullNamespace = GetFullNamespace(namespaceDecl);
      return new ExtractedContainerDefinition
      {
        Name = fullNamespace,
        FullDefinition = $"namespace {fullNamespace}"
      };
    }

    private static string GetFullNamespace(BaseNamespaceDeclarationSyntax namespaceDecl)
    {
      var parts = new List<string>();
      var current = namespaceDecl;

      while (current != null)
      {
        parts.Insert(0, current.Name.ToString());
        current = current.Parent as BaseNamespaceDeclarationSyntax;
      }

      return string.Join(".", parts);
    }

    private static ExtractedContainerDefinition BuildTypeContainer(TypeDeclarationSyntax typeDecl)
    {
      var sb = new StringBuilder();

      foreach (var modifier in typeDecl.Modifiers)
      {
        if (modifier.IsKind(SyntaxKind.PublicKeyword) ||
            modifier.IsKind(SyntaxKind.InternalKeyword) ||
            modifier.IsKind(SyntaxKind.ProtectedKeyword) ||
            modifier.IsKind(SyntaxKind.PrivateKeyword))
        {
          sb.Append(modifier.ValueText);
          sb.Append(' ');
        }
      }

      sb.Append("partial ");
      sb.Append(typeDecl.Keyword.ValueText);
      sb.Append(' ');
      sb.Append(typeDecl.Identifier.Text);

      if (typeDecl.TypeParameterList != null)
      {
        sb.Append(typeDecl.TypeParameterList.ToString());
      }

      return new ExtractedContainerDefinition
      {
        Name = typeDecl.Identifier.Text,
        FullDefinition = sb.ToString()
      };
    }
  }
}
