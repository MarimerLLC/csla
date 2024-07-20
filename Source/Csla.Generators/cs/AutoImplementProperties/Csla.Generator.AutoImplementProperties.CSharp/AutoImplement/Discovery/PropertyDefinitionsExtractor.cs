//-----------------------------------------------------------------------
// <copyright file="PropertyDefinitionsExtractor.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Extract the definitions of all properties of a type for source generation</summary>
//-----------------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Csla.Generator.AutoImplementProperties.CSharp.AutoSerialization.Discovery
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
      List<ExtractedPropertyDefinition> propertyDefinitions = [];
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

      // Get public or internal properties that are not specifically opted out with the [AutoNonSerialized] attribute
      serializableProperties = GetPublicNonExcludedProperties(extractionContext, targetTypeDeclaration);

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
        HasOneOfScopes(extractionContext, propertyDeclaration, "partial"))
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



    #endregion

  }
}
