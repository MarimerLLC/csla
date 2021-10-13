//-----------------------------------------------------------------------
// <copyright file="PropertyDefinitionsExtractor.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Extract the definitions of all properties of a type for source generation</summary>
//-----------------------------------------------------------------------
using Csla.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Generators.CSharp.AutoSerialization.Discovery
{

  /// <summary>
  /// Extract the definition of all properties for a type for which source generation is required
  /// This is used to detach the builder from the Roslyn infrastructure, to enable testing
  /// </summary>
  /// <remarks>Only the properties to be included in serialization are extracted; those manually excluded
  /// from serialization through use of the [AutoNonSerialized] attribute are not returned</remarks>
  internal static class PropertyDefinitionsExtractor
  {

    /// <summary>
    /// Extract information about the properties which must be serialized from a part of the syntax tree
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed</param>
    /// <param name="targetTypeDeclaration">The TypeDeclarationSyntax from which to extract the necessary data</param>
    /// <returns>A readonly list of ExtractedPropertyDefinition containing the data extracted from the syntax tree</returns>
    public static IReadOnlyList<ExtractedPropertyDefinition> ExtractPropertyDefinitions(DefinitionExtractionContext extractionContext, TypeDeclarationSyntax targetTypeDeclaration)
    {
      List<ExtractedPropertyDefinition> propertyDefinitions = new List<ExtractedPropertyDefinition>();
      ExtractedPropertyDefinition propertyDefinition;
      IReadOnlyList<PropertyDeclarationSyntax> serializableProperties;

      serializableProperties = GetSerializablePropertyDeclarations(extractionContext, targetTypeDeclaration);
      foreach (PropertyDeclarationSyntax propertyDeclaration in serializableProperties)
      {
        propertyDefinition = PropertyDefinitionExtractor.ExtractPropertyDefinition(extractionContext, propertyDeclaration);
        propertyDefinitions.Add(propertyDefinition);
      }

      return propertyDefinitions;
    }

    #region Private Helper Methods

    /// <summary>
    /// Get the property declarations for all properties which are to be serialized
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed</param>
    /// <param name="targetTypeDeclaration">The TypeDeclarationSyntax from which to extract the necessary data</param>
    /// <returns>A readonly list of property declarations to be included in serialization</returns>
    private static IReadOnlyList<PropertyDeclarationSyntax> GetSerializablePropertyDeclarations(DefinitionExtractionContext extractionContext, TypeDeclarationSyntax targetTypeDeclaration)
    {
      List<PropertyDeclarationSyntax> serializableProperties;
      List<PropertyDeclarationSyntax> optedInSerializableProperties;

      // Get public or internal properties that are not specifically opted out with the [AutoNonSerialized] attribute
      serializableProperties = GetPublicNonExcludedProperties(extractionContext, targetTypeDeclaration);

      // Add any private or protected properties that are opted in with the use of the [AutoSerialized] attribute
      optedInSerializableProperties = GetNonPublicIncludedProperties(extractionContext, targetTypeDeclaration);
      serializableProperties.AddRange(optedInSerializableProperties);

      return serializableProperties;
    }

    /// <summary>
    /// Get the property declarations for all public properties which are not explicitly excluded from serialization
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed</param>
    /// <param name="targetTypeDeclaration">The TypeDeclarationSyntax from which to extract the necessary data</param>
    /// <returns>A readonly list of property declarations to be included in serialization</returns>
    private static List<PropertyDeclarationSyntax> GetPublicNonExcludedProperties(DefinitionExtractionContext extractionContext, TypeDeclarationSyntax targetTypeDeclaration)
    {
      List<PropertyDeclarationSyntax> serializableProperties;

      // Get public or internal properties that are not specifically opted out with the [AutoNonSerialized] attribute
      serializableProperties = targetTypeDeclaration.Members.Where(
        m => m is PropertyDeclarationSyntax propertyDeclaration &&
        HasOneOfScopes(extractionContext, propertyDeclaration, "public") &&
        HasGetterAndSetter(extractionContext, propertyDeclaration) &&
        !extractionContext.IsPropertyDecoratedWithAutoNonSerialized(propertyDeclaration))
        .Cast<PropertyDeclarationSyntax>()
        .ToList();

      return serializableProperties;
    }

    /// <summary>
    /// Get the property declarations for all non-public properties which have been explicitly included in serialization
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed</param>
    /// <param name="targetTypeDeclaration">The TypeDeclarationSyntax from which to extract the necessary data</param>
    /// <returns>A readonly list of property declarations to be included in serialization</returns>
    private static List<PropertyDeclarationSyntax> GetNonPublicIncludedProperties(DefinitionExtractionContext extractionContext, TypeDeclarationSyntax targetTypeDeclaration)
    {
      List<PropertyDeclarationSyntax> serializableProperties;

      // Get private or protected properties that are specifically opted in with the [AutoSerialized] attribute
      serializableProperties = targetTypeDeclaration.Members.Where(
        m => m is PropertyDeclarationSyntax propertyDeclaration &&
        !HasOneOfScopes(extractionContext, propertyDeclaration, "public") &&
        HasGetterAndSetter(extractionContext, propertyDeclaration) &&
        extractionContext.IsPropertyDecoratedWithAutoSerialized(propertyDeclaration))
        .Cast<PropertyDeclarationSyntax>()
        .ToList();

      return serializableProperties;
    }

    /// <summary>
    /// Determine if a property has one of the scopes requested by a caller
    /// </summary>
    /// <param name="context">The definition extraction context for this extraction</param>
    /// <param name="propertyDeclaration">The declaration of the property being tested</param>
    /// <param name="scopes">The list of scopes in which the caller is interested</param>
    /// <returns>Boolean true if the property has one of the scopes requested by the caller, else false</returns>
    private static bool HasOneOfScopes(DefinitionExtractionContext context, PropertyDeclarationSyntax propertyDeclaration, params string[] scopes)
    {
      foreach (string scope in scopes)
      {
        if (propertyDeclaration.Modifiers.Any(m => m.ValueText.Equals(scope, StringComparison.InvariantCultureIgnoreCase)))
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Determine if a property has both a getter and a setter
    /// </summary>
    /// <param name="context">The definition extraction context for this extraction</param>
    /// <param name="propertyDeclaration">The declaration of the property being tested</param>
    /// <returns>Boolean true if the property has both a getter and setter (of any scope), else false</returns>
    private static bool HasGetterAndSetter(DefinitionExtractionContext context, PropertyDeclarationSyntax propertyDeclaration)
    {
      bool hasGetter = false;
      bool hasSetter = false;

      if (propertyDeclaration.AccessorList is null) return false;

      foreach (AccessorDeclarationSyntax accessorDeclaration in propertyDeclaration.AccessorList.Accessors)
      {
        if (accessorDeclaration.Kind() == Microsoft.CodeAnalysis.CSharp.SyntaxKind.GetAccessorDeclaration)
        {
          hasGetter = true;
        }
        if (accessorDeclaration.Kind() == Microsoft.CodeAnalysis.CSharp.SyntaxKind.SetAccessorDeclaration)
        {
          hasSetter = true;
        }
      }

      return hasGetter && hasSetter;
    }

    #endregion

  }
}
