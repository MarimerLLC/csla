//-----------------------------------------------------------------------
// <copyright file="FieldDefinitionsExtractor.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Extract the definitions of all fields for source generation</summary>
//-----------------------------------------------------------------------
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Generators.AutoSerialization.Discovery
{

  /// <summary>
  /// Extract the definition of all fields for a type for which source generation is required
  /// This is used to detach the builder from the Roslyn infrastructure, to enable testing
  /// </summary>
  /// <remarks>Only the fields to be included in serialization are extracted; those manually excluded
  /// from serialization through use of the [AutoNonSerialized] attribute are not returned</remarks>
  internal static class FieldDefinitionsExtractor
  {

    /// <summary>
    /// Extract information about the fields which must be serialized from a part of the syntax tree
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed</param>
    /// <param name="targetTypeDeclaration">The TypeDeclarationSyntax from which to extract the necessary data</param>
    /// <returns>A readonly list of ExtractedFieldDefinition containing the data extracted from the syntax tree</returns>
    public static IReadOnlyList<ExtractedFieldDefinition> ExtractFieldDefinitions(DefinitionExtractionContext extractionContext, TypeDeclarationSyntax targetTypeDeclaration)
    {
      List<ExtractedFieldDefinition> propertyDefinitions = new List<ExtractedFieldDefinition>();
      ExtractedFieldDefinition fieldDefinition;
      IReadOnlyList<FieldDeclarationSyntax> serializableFields;

      serializableFields = GetSerializableFieldDeclarations(extractionContext, targetTypeDeclaration);
      foreach (FieldDeclarationSyntax fieldDeclaration in serializableFields)
      {
        fieldDefinition = FieldDefinitionExtractor.ExtractFieldDefinition(extractionContext, fieldDeclaration);
        propertyDefinitions.Add(fieldDefinition);
      }

      return propertyDefinitions;
    }

    #region Private Helper Methods

    /// <summary>
    /// Get the property declarations for all fields which are to be serialized
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed</param>
    /// <param name="targetTypeDeclaration">The TypeDeclarationSyntax from which to extract the necessary data</param>
    /// <returns>A readonly list of field declarations to be included in serialization</returns>
    private static IReadOnlyList<FieldDeclarationSyntax> GetSerializableFieldDeclarations(DefinitionExtractionContext extractionContext, TypeDeclarationSyntax targetTypeDeclaration)
    {
      List<FieldDeclarationSyntax> serializableFields;
      List<FieldDeclarationSyntax> optedInSerializableFields;

      // Get all fields that are not specifically opted out with the [AutoNonSerialized] attribute
      serializableFields = GetPublicNonExcludedFields(extractionContext, targetTypeDeclaration);

      // Add any pfields that are opted in with the use of the [AutoSerialized] attribute
      optedInSerializableFields = GetNonPublicIncludedFields(extractionContext, targetTypeDeclaration);
      serializableFields.AddRange(optedInSerializableFields);
      // serializableFields = GetAllIncludedFields(extractionContext, targetTypeDeclaration);

      return serializableFields;
    }

    /// <summary>
    /// Get the field declarations for all public fields which are not explicitly excluded from serialization
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed</param>
    /// <param name="targetTypeDeclaration">The TypeDeclarationSyntax from which to extract the necessary data</param>
    /// <returns>A readonly list of field declarations to be included in serialization</returns>
    private static List<FieldDeclarationSyntax> GetPublicNonExcludedFields(DefinitionExtractionContext extractionContext, TypeDeclarationSyntax targetTypeDeclaration)
    {
      List<FieldDeclarationSyntax> serializableFields;

      // Get all fields that are not specifically opted out with the [AutoNonSerialized] attribute
      serializableFields = targetTypeDeclaration.Members.Where(
        m => m is FieldDeclarationSyntax fieldDeclaration &&
        HasOneOfScopes(extractionContext, fieldDeclaration, "public") &&
        !extractionContext.IsFieldDecoratedWithAutoNonSerialized(fieldDeclaration))
        .Cast<FieldDeclarationSyntax>()
        .ToList();

      return serializableFields;
    }

    /// <summary>
    /// Get the field declarations for all non-public fields which have been explicitly included in serialization
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed</param>
    /// <param name="targetTypeDeclaration">The TypeDeclarationSyntax from which to extract the necessary data</param>
    /// <returns>A readonly list of field declarations to be included in serialization</returns>
    private static List<FieldDeclarationSyntax> GetNonPublicIncludedFields(DefinitionExtractionContext extractionContext, TypeDeclarationSyntax targetTypeDeclaration)
    {
      List<FieldDeclarationSyntax> serializableFields;

      // Get any private or protected fields that are opted in with the use of the [AutoSerialized] attribute
      serializableFields = targetTypeDeclaration.Members.Where(
        m => m is FieldDeclarationSyntax fieldDeclaration &&
        !HasOneOfScopes(extractionContext, fieldDeclaration, "public") &&
        extractionContext.IsFieldDecoratedWithAutoSerialized(fieldDeclaration))
        .Cast<FieldDeclarationSyntax>()
        .ToList();

      return serializableFields;
    }

    /// <summary>
    /// Get the field declarations for all fields which have been explicitly included in serialization
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed</param>
    /// <param name="targetTypeDeclaration">The TypeDeclarationSyntax from which to extract the necessary data</param>
    /// <returns>A readonly list of field declarations to be included in serialization</returns>
    private static List<FieldDeclarationSyntax> GetAllIncludedFields(DefinitionExtractionContext extractionContext, TypeDeclarationSyntax targetTypeDeclaration)
    {
      List<FieldDeclarationSyntax> serializableFields;

      // Get any private or protected fields that are opted in with the use of the [AutoSerialized] attribute
      serializableFields = targetTypeDeclaration.Members.Where(
        m => m is FieldDeclarationSyntax fieldDeclaration &&
        extractionContext.IsFieldDecoratedWithAutoSerialized(fieldDeclaration))
        .Cast<FieldDeclarationSyntax>()
        .ToList();

      return serializableFields;
    }

    /// <summary>
    /// Determine if a field has one of the scopes requested by a caller
    /// </summary>
    /// <param name="context">The definition extraction context for this extraction</param>
    /// <param name="fieldDeclaration">The declaration of the field being tested</param>
    /// <param name="scopes">The list of scopes in which the caller is interested</param>
    /// <returns>Boolean true if the field has one of the scopes requested by the caller, else false</returns>
    private static bool HasOneOfScopes(DefinitionExtractionContext context, FieldDeclarationSyntax fieldDeclaration, params string[] scopes)
    {
      foreach (string scope in scopes)
      {
        if (fieldDeclaration.Modifiers.Any(m => m.ValueText.Equals(scope, StringComparison.InvariantCultureIgnoreCase)))
        {
          return true;
        }
      }

      return false;
    }

    #endregion

  }
}
