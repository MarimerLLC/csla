//-----------------------------------------------------------------------
// <copyright file="PropertyDefinitionExtractor.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Extract the definition of a single property of a type for source generation</summary>
//-----------------------------------------------------------------------
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Generators.CSharp.AutoSerialization.Discovery
{

  /// <summary>
  /// Extract the definition of a single property of a type for which source generation is being performed
  /// This is used to detach the builder from the Roslyn infrastructure, to enable testing
  /// </summary>
  internal static class PropertyDefinitionExtractor
  {

    /// <summary>
    /// Extract information about a single property from its declaration in the syntax tree
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed</param>
    /// <param name="targetTypeDeclaration">The PropertyDeclarationSyntax from which to extract the necessary data</param>
    /// <returns>A readonly list of ExtractedPropertyDefinition containing the data extracted from the syntax tree</returns>
    public static ExtractedPropertyDefinition ExtractPropertyDefinition(DefinitionExtractionContext extractionContext, PropertyDeclarationSyntax propertyDeclaration)
    {
      ExtractedPropertyDefinition propertyDefinition = new ExtractedPropertyDefinition();

      propertyDefinition.PropertyName = GetPropertyName(extractionContext, propertyDeclaration);
      propertyDefinition.TypeDefinition.TypeName = GetPropertyTypeName(extractionContext, propertyDeclaration);
      propertyDefinition.TypeDefinition.TypeNamespace = extractionContext.GetTypeNamespace(propertyDeclaration.Type);
      propertyDefinition.TypeDefinition.IsAutoSerializable = extractionContext.IsTypeAutoSerializable(propertyDeclaration.Type);
      propertyDefinition.TypeDefinition.ImplementsIMobileObject = extractionContext.DoesTypeImplementIMobileObject(propertyDeclaration.Type);

      return propertyDefinition;
    }

    #region Private Helper Methods

    /// <summary>
    /// Extract the name of the property for which we are building information
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed</param>
    /// <param name="targetTypeDeclaration">The PropertyDeclarationSyntax from which to extract the necessary information</param>
    /// <returns>The name of the property for which we are extracting information</returns>
    private static string GetPropertyName(DefinitionExtractionContext extractionContext, PropertyDeclarationSyntax propertyDeclaration)
    {
      return propertyDeclaration.Identifier.ValueText;
    }

    /// <summary>
    /// Extract the type name of the property for which we are building information
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed</param>
    /// <param name="targetTypeDeclaration">The PropertyDeclarationSyntax from which to extract the necessary information</param>
    /// <returns>The type name of the property for which we are extracting information</returns>
    private static string GetPropertyTypeName(DefinitionExtractionContext extractionContext, PropertyDeclarationSyntax propertyDeclaration)
    {
      return propertyDeclaration.Type.ToString();
    }

    #endregion

  }
}
