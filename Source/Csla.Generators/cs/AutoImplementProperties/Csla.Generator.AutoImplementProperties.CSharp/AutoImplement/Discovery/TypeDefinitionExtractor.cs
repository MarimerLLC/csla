//-----------------------------------------------------------------------
// <copyright file="TypeDefinitionExtractor.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Extract the definition of a type for source generation</summary>
//-----------------------------------------------------------------------
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Csla.Generator.AutoImplementProperties.CSharp.AutoImplement.Discovery
{

  /// <summary>
  /// Extract the definition of a type for which source generation is required
  /// This is used to detach the builder from the Roslyn infrastructure, to enable testing
  /// </summary>
  internal static class TypeDefinitionExtractor
  {
    public static ExtractedTypeDefinition ExtractTypeDefinitionForInterfaces(DefinitionExtractionContext extractionContext, TypeDeclarationSyntax typeDeclarationSyntax)
    {
      var extractedTypeDefinition = ExtractTypeDefinition(extractionContext, typeDeclarationSyntax);

      // Assuming you have the typeDeclarationSyntax of the class
      if (typeDeclarationSyntax.AttributeLists.Count > 0)
      {
        // Find the AutoImplementPropertiesInterfaceAttribute attribute
        var attribute = typeDeclarationSyntax.AttributeLists
            .SelectMany(al => al.Attributes)
            .FirstOrDefault(a => a.Name.ToString().StartsWith(DefinitionExtractionContext.CslaImplementPropertiesAttribute));

        if (attribute != null)
        {
          var genericName = attribute.Name as GenericNameSyntax;
          // Get the generic argument of the attribute
          var genericArgument = genericName?.TypeArgumentList?.Arguments.FirstOrDefault();


          // Get the type symbol of the generic argument
          var semanticModel = extractionContext.SemanticModel;
          var typeSymbol = semanticModel.GetTypeInfo(genericArgument).Type;

          // Check if the type symbol is an interface
          if (typeSymbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.TypeKind == TypeKind.Interface)
          {
            // Get the InterfaceDeclarationSyntax of the generic parameter
            var interfaceDeclarationSyntax = namedTypeSymbol.DeclaringSyntaxReferences
                .Select(r => r.GetSyntax())
                .OfType<InterfaceDeclarationSyntax>()
                .FirstOrDefault();

            if (interfaceDeclarationSyntax != null)
            {
              foreach (ExtractedPropertyDefinition propertyDefinition in PropertyDefinitionsExtractor.ExtractPropertyDefinitions(extractionContext, interfaceDeclarationSyntax))
              {
                if (!propertyDefinition.Modifiers.Any())
                {
                  propertyDefinition.Modifiers = extractedTypeDefinition.DefaultPropertyModifiers;
                }
                if (!propertyDefinition.Setter)
                {
                  propertyDefinition.SetterModifiers = ["private"];
                  propertyDefinition.Setter = true;
                }
                extractedTypeDefinition.Properties.Add(propertyDefinition);
              }
            }
          }

        }
      }
      return extractedTypeDefinition;
    }


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

      definition.TypeName = GetTypeName(targetTypeDeclaration);
      definition.TypeKind = GetTypeKind(targetTypeDeclaration);
      definition.Namespace = GetNamespace(targetTypeDeclaration);
      definition.Scope = GetScopeDefinition(targetTypeDeclaration);
      definition.BaseClassTypeName = GetBaseClassTypeName(extractionContext, targetTypeDeclaration);
      definition.DefaultPropertyModifiers = ["public"];
      definition.DefaultPropertySetterModifiers = [];

      foreach (ExtractedPropertyDefinition propertyDefinition in PropertyDefinitionsExtractor.ExtractPropertyDefinitions(extractionContext, targetTypeDeclaration))
      {
        definition.Properties.Add(propertyDefinition);
      }

      fullyQualifiedNameBuilder.Append(definition.TypeName);
      definition.FullyQualifiedName = fullyQualifiedNameBuilder.ToString();

      return definition;
    }

    #region Private Helper Methods

    // ... existing helper methods ...

    /// <summary>
    /// Extract the name of the base class of the type for which we will be generating code
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed</param>
    /// <param name="targetTypeDeclaration">The TypeDeclarationSyntax from which to extract the necessary information</param>
    /// <returns>The name of the base class of the type for which generation is being performed</returns>
    private static string GetBaseClassTypeName(DefinitionExtractionContext extractionContext, TypeDeclarationSyntax targetTypeDeclaration)
    {
      var targetTypeSymbol = extractionContext.SemanticModel.GetDeclaredSymbol(targetTypeDeclaration) as INamedTypeSymbol;
      var baseTypeSymbol = targetTypeSymbol?.BaseType;

      if (baseTypeSymbol != null)
      {
        return baseTypeSymbol.Name;
      }

      return null;
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Extract the namespace of the type for which we will be generating code
    /// </summary>
    /// <param name="targetTypeDeclaration">The TypeDeclarationSyntax from which to extract the necessary information</param>
    /// <returns>The namespace of the type for which generation is being performed</returns>
    private static string GetNamespace(TypeDeclarationSyntax targetTypeDeclaration)
    {
      // If we don't have a namespace at all we'll return an empty string
      // This accounts for the "default namespace" case
      string nameSpace = string.Empty;

      // Get the containing syntax node for the type declaration
      // (could be a nested type, for example)
      SyntaxNode potentialNamespaceParent = targetTypeDeclaration.Parent;
    
      // Keep moving "out" of nested classes etc until we get to a namespace
      // or until we run out of parents
      while (potentialNamespaceParent != null &&
             potentialNamespaceParent is not NamespaceDeclarationSyntax
             && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
      {
        potentialNamespaceParent = potentialNamespaceParent.Parent;
      }

      // Build up the final namespace by looping until we no longer have a namespace declaration
      if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
      {
        // We have a namespace. Use that as the type
        nameSpace = namespaceParent.Name.ToString();
        
        // Keep moving "out" of the namespace declarations until we 
        // run out of nested namespace declarations
        while (true)
        {
          if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
          {
            break;
          }

          // Add the outer namespace as a prefix to the final namespace
          nameSpace = $"{namespaceParent.Name}.{nameSpace}";
          namespaceParent = parent;
        }
      }

      // return the final namespace
      return nameSpace;
    }

    /// <summary>
    /// Extract the scope of the type for which we will be generating code
    /// </summary>
    /// <param name="targetTypeDeclaration">The TypeDeclarationSyntax from which to extract the necessary information</param>
    /// <returns>The scope of the type for which generation is being performed</returns>
    private static string GetScopeDefinition(TypeDeclarationSyntax targetTypeDeclaration)
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
      stringBuilder.Append(' ');
    }

    /// <summary>
    /// Extract the name of the type for which we will be generating code
    /// </summary>
    /// <param name="targetTypeDeclaration">The TypeDeclarationSyntax from which to extract the necessary information</param>
    /// <returns>The name of the type for which generation is being performed</returns>
    private static string GetTypeName(TypeDeclarationSyntax targetTypeDeclaration)
    {
      return targetTypeDeclaration.Identifier.ToString();
    }

    /// <summary>
    /// Extract the textual definition of the kind that this type represents
    /// </summary>
    /// <param name="targetTypeDeclaration">The TypeDeclarationSyntax from which to extract the necessary information</param>
    /// <returns>The kind of the type for which generation is being performed</returns>
    private static string GetTypeKind(TypeDeclarationSyntax targetTypeDeclaration)
    {
      return targetTypeDeclaration.Keyword.ToString();
    }

    #endregion

  }
}
