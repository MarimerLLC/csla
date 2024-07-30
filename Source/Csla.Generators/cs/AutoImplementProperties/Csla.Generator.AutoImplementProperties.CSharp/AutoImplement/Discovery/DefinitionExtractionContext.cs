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
    private const string _serializationNamespace = "Csla.Serialization";

    private const string _ignorePropertyAttributeName = "CslaIgnorePropertyAttribute";
    public const string CslaImplementPropertiesAttribute = "CslaImplementPropertiesInterface";
    public const string CslaImplementPropertiesAttributeFullName = "Csla.Serialization.CslaImplementPropertiesAttribute";
    public const string CslaImplementPropertiesInterfaceAttributeFullName = "Csla.Serialization.CslaImplementPropertiesInterfaceAttribute`1";
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

    /// <summary>
    /// Determine if a property declaration is marked as excluded from auto implementation
    /// </summary>
    /// <param name="propertyDeclaration">The declaration of the property being inspected</param>
    /// <returns>Boolean true if the property is decorated with the CslaIgnorePropertyAttribute attribute, otherwise false</returns>
    public bool IsPropertyDecoratedWithIgnoreProperty(PropertyDeclarationSyntax propertyDeclaration)
    {
      return IsPropertyDecoratedWith(propertyDeclaration, _ignorePropertyAttributeName, _serializationNamespace);
    }

    /// <summary>
    /// Determine if two symbols represent the same attribute
    /// </summary>
    /// <param name="appliedAttributeSymbol">The attribute applied to the type we are testing</param>
    /// <param name="desiredTypeName">The name of the attribute whose presence we are testing for</param>
    /// <param name="desiredTypeNamespace">The namespace of the attribute whose presence we are testing for</param>
    /// <returns>Boolean true if the symbol seems to represent the desired type by name and namespace</returns>
    private bool IsMatchingTypeSymbol(INamedTypeSymbol appliedAttributeSymbol, string desiredTypeName, string desiredTypeNamespace)
    {
      INamespaceSymbol namespaceSymbol;

      // Match on the type name
      if (!appliedAttributeSymbol.Name.Equals(desiredTypeName, StringComparison.InvariantCultureIgnoreCase)) return false;

      // Match on the namespace of the type
      namespaceSymbol = appliedAttributeSymbol.ContainingNamespace;
      if (namespaceSymbol is null) return false;
      return IsMatchingNamespaceSymbol(namespaceSymbol, desiredTypeNamespace);
    }


    /// <summary>
    /// Determine if a property declaration syntax is decorated with an attribute of interest
    /// </summary>
    /// <param name="propertyDeclaration">The syntax node representing the property being investigated</param>
    /// <param name="desiredAttributeTypeName">The name of the type of attribute of interest</param>
    /// <param name="desiredAttributeTypeNamespace">The namespace of the type of attribute of interest</param>
    /// <returns>Boolean true if the type is decorated with the attribute, otherwise false</returns>
    private bool IsPropertyDecoratedWith(PropertyDeclarationSyntax propertyDeclaration, string desiredAttributeTypeName, string desiredAttributeTypeNamespace)
    {
      INamedTypeSymbol appliedAttributeSymbol;

      foreach (AttributeSyntax attributeSyntax in propertyDeclaration.AttributeLists.SelectMany(al => al.Attributes))
      {
        appliedAttributeSymbol = _semanticModel.GetTypeInfo(attributeSyntax).Type as INamedTypeSymbol;
        if (IsMatchingTypeSymbol(appliedAttributeSymbol, desiredAttributeTypeName, desiredAttributeTypeNamespace))
        {
          return true;
        }
      }
      return false;
    }

    #endregion

  }
}
