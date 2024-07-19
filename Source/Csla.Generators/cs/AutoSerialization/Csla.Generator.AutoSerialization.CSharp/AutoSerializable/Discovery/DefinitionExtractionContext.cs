﻿//-----------------------------------------------------------------------
// <copyright file="DefinitionExtractionContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Helper for definition extraction, used to optimise symbo, recognition</summary>
//-----------------------------------------------------------------------
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Csla.Generator.AutoSerialization.CSharp.AutoSerialization.Discovery
{

  /// <summary>
  /// Helper for definition extraction, used to optimise symbol recognition
  /// </summary>
  internal class DefinitionExtractionContext
  {

    private readonly SemanticModel _semanticModel;
    private const string _serializationNamespace = "Csla.Serialization";
    private const string _autoSerializableAttributeName = "AutoSerializableAttribute";
    private const string _autoSerializedAttributeName = "AutoSerializedAttribute";
    private const string _autoNonSerializedAttributeName = "AutoNonSerializedAttribute";
    private const string _iMobileObjectInterfaceNamespace = "Csla.Serialization.Mobile";
    private const string _iMobileObjectInterfaceName = "IMobileObject";

    public DefinitionExtractionContext(SemanticModel semanticModel)
    {
      _semanticModel = semanticModel;
    }


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

    /// <summary>
    /// Determine if a type declaration represents a type that is auto serializable
    /// </summary>
    /// <param name="typeSymbol">The declaration representing the type to be tested</param>
    /// <returns>Boolean true if the type is decorated with the AutoSerializable attribute, otherwise false</returns>
    public bool IsTypeAutoSerializable(TypeDeclarationSyntax typeDeclarationSyntax)
    {
      INamedTypeSymbol typeSymbol;

      typeSymbol = _semanticModel.GetDeclaredSymbol(typeDeclarationSyntax) as INamedTypeSymbol;
      return IsTypeDecoratedBy(typeSymbol, _autoSerializableAttributeName, _serializationNamespace);
    }

    /// <summary>
    /// Determine if a type declaration represents a type that is auto serializable
    /// </summary>
    /// <param name="typeSymbol">The declaration representing the type to be tested</param>
    /// <returns>Boolean true if the type is decorated with the AutoSerializable attribute, otherwise false</returns>
    public bool IsTypeAutoSerializable(TypeSyntax typeSyntax)
    {
      INamedTypeSymbol typeSymbol;

      typeSymbol = _semanticModel.GetSymbolInfo(typeSyntax).Symbol as INamedTypeSymbol;
      if (typeSymbol is null) return false;
      return IsTypeDecoratedBy(typeSymbol, _autoSerializableAttributeName, _serializationNamespace);
    }

    /// <summary>
    /// Determine if a type declaration represents a type that implements the IMobileObject interface
    /// </summary>
    /// <param name="typeSymbol">The declaration representing the type to be tested</param>
    /// <remarks>Determines if the type either implements the interface directly or via inheritance</remarks>
    /// <returns>Boolean true if the type implements the IMobileObject interface, otherwise false</returns>
    public bool DoesTypeImplementIMobileObject(TypeSyntax typeSyntax)
    {
      INamedTypeSymbol typeSymbol;

      typeSymbol = _semanticModel.GetSymbolInfo(typeSyntax).Symbol as INamedTypeSymbol;
      if (typeSymbol is null) return false;

      foreach (ITypeSymbol interfaceSymbol in typeSymbol.AllInterfaces)
      {
        if (IsMatchingTypeSymbol(interfaceSymbol as INamedTypeSymbol, _iMobileObjectInterfaceName, _iMobileObjectInterfaceNamespace))
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Determine if a property declaration is marked as included in serialization
    /// </summary>
    /// <param name="propertyDeclaration">The declaration of the property being inspected</param>
    /// <returns>Boolean true if the property is decorated with the AutoSerialized attribute, otherwise false</returns>
    public bool IsPropertyDecoratedWithAutoSerialized(PropertyDeclarationSyntax propertyDeclaration)
    {
      return IsPropertyDecoratedWith(propertyDeclaration, _autoSerializedAttributeName, _serializationNamespace);
    }

    /// <summary>
    /// Determine if a property declaration is marked as excluded from serialization
    /// </summary>
    /// <param name="propertyDeclaration">The declaration of the property being inspected</param>
    /// <returns>Boolean true if the property is decorated with the AutoNonSerialized attribute, otherwise false</returns>
    public bool IsPropertyDecoratedWithAutoNonSerialized(PropertyDeclarationSyntax propertyDeclaration)
    {
      return IsPropertyDecoratedWith(propertyDeclaration, _autoNonSerializedAttributeName, _serializationNamespace);
    }

    /// <summary>
    /// Determine if a field declaration is marked as included in serialization
    /// </summary>
    /// <param name="fieldDeclaration">The declaration of the field being inspected</param>
    /// <returns>Boolean true if the field is decorated with the AutoSerialized attribute, otherwise false</returns>
    public bool IsFieldDecoratedWithAutoSerialized(FieldDeclarationSyntax fieldDeclaration)
    {
      return IsFieldDecoratedWith(fieldDeclaration, _autoSerializedAttributeName, _serializationNamespace);
    }

    /// <summary>
    /// Determine if a field declaration is marked as excluded from serialization
    /// </summary>
    /// <param name="fieldDeclaration">The declaration of the field being inspected</param>
    /// <returns>Boolean true if the field is decorated with the AutoNonSerialized attribute, otherwise false</returns>
    public bool IsFieldDecoratedWithAutoNonSerialized(FieldDeclarationSyntax fieldDeclaration)
    {
      return IsFieldDecoratedWith(fieldDeclaration, _autoNonSerializedAttributeName, _serializationNamespace);
    }

    #region Private Helper Methods

    /// <summary>
    /// Determine if the type symbol represents a type decorated by an attribute of interest
    /// </summary>
    /// <param name="typeSymbol">The symbol representing the type</param>
    /// <param name="desiredAttributeTypeName">The name of the type of attribute of interest</param>
    /// <param name="desiredAttributeTypeNamespace">The namespace of the type of attribute of interest</param>
    /// <returns>Boolean true if the type is decorated with the attribute, otherwise false</returns>
    private bool IsTypeDecoratedBy(INamedTypeSymbol typeSymbol, string desiredAttributeTypeName, string desiredAttributeTypeNamespace)
    {
      return typeSymbol.GetAttributes().Any(
        attr => IsMatchingTypeSymbol(attr.AttributeClass, desiredAttributeTypeName, desiredAttributeTypeNamespace));
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

    /// <summary>
    /// Determine if a field declaration syntax is decorated with an attribute of interest
    /// </summary>
    /// <param name="fieldDeclaration">The syntax node representing the field being investigated</param>
    /// <param name="desiredAttributeTypeName">The name of the type of attribute of interest</param>
    /// <param name="desiredAttributeTypeNamespace">The namespace of the type of attribute of interest</param>
    /// <returns>Boolean true if the type is decorated with the attribute, otherwise false</returns>
    private bool IsFieldDecoratedWith(FieldDeclarationSyntax fieldDeclaration, string desiredAttributeTypeName, string desiredAttributeTypeNamespace)
    {
      INamedTypeSymbol appliedAttributeSymbol;

      foreach (AttributeSyntax attributeSyntax in fieldDeclaration.AttributeLists.SelectMany(al => al.Attributes))
      {
        appliedAttributeSymbol = _semanticModel.GetTypeInfo(attributeSyntax).Type as INamedTypeSymbol;
        if (IsMatchingTypeSymbol(appliedAttributeSymbol, desiredAttributeTypeName, desiredAttributeTypeNamespace))
        {
          return true;
        }
      }
      return false;
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
