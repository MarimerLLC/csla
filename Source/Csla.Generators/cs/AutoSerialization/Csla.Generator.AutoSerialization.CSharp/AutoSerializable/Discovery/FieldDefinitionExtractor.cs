//-----------------------------------------------------------------------
// <copyright file="FieldDefinitionExtractor.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Extract the definition of a single field for source generation</summary>
//-----------------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Csla.Generator.AutoSerialization.CSharp.AutoSerialization.Discovery
{

  /// <summary>
  /// Extract the definition of a single field of a type for which source generation is being performed
  /// This is used to detach the builder from the Roslyn infrastructure, to enable testing
  /// </summary>
  internal static class FieldDefinitionExtractor
  {

    /// <summary>
    /// Extract information about a single field from its declaration in the syntax tree
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed</param>
    /// <param name="fieldDeclaration">The FieldDeclarationSyntax from which to extract the necessary data</param>
    /// <returns>A readonly list of ExtractedFieldDefinition containing the data extracted from the syntax tree</returns>
    public static ExtractedFieldDefinition ExtractFieldDefinition(DefinitionExtractionContext extractionContext, FieldDeclarationSyntax fieldDeclaration)
    {
      var fieldDefinition = new ExtractedFieldDefinition
      {
        FieldName = GetFieldName(fieldDeclaration),
        TypeDefinition = new ExtractedMemberTypeDefinition
        {
          TypeName = GetFieldTypeName(fieldDeclaration),
          TypeNamespace = extractionContext.GetTypeNamespace(fieldDeclaration.Declaration.Type),
          IsAutoSerializable = extractionContext.IsTypeAutoSerializable(fieldDeclaration.Declaration.Type),
          ImplementsIMobileObject = extractionContext.DoesTypeImplementIMobileObject(fieldDeclaration.Declaration.Type),
          Nullable = GetFieldTypeNullable(fieldDeclaration)
        }
      };
      return fieldDefinition;
    }

    #region Private Helper Methods

    /// <summary>
    /// Determines whether the field type is nullable.
    /// </summary>
    /// <param name="fieldDeclaration">The FieldDeclarationSyntax representing the field declaration.</param>
    /// <returns><c>true</c> if the field type is nullable; otherwise, <c>false</c>.</returns>
    private static bool GetFieldTypeNullable(FieldDeclarationSyntax fieldDeclaration)
    {
      return fieldDeclaration.Declaration.Type is NullableTypeSyntax;
    }


    /// <summary>
    /// Extract the name of the field for which we are building information
    /// </summary>
    /// <param name="fieldDeclaration">The FieldDeclarationSyntax from which to extract the necessary information</param>
    /// <returns>The name of the field for which we are extracting information</returns>
    private static string GetFieldName(FieldDeclarationSyntax fieldDeclaration)
    {
      return fieldDeclaration.Declaration.Variables[0].Identifier.ToString();
    }

    /// <summary>
    /// Extract the type name of the field for which we are building information
    /// </summary>
    /// <param name="fieldDeclaration">The FieldDeclarationSyntax from which to extract the necessary information</param>
    /// <returns>The type name of the field for which we are extracting information</returns>
    private static string GetFieldTypeName(FieldDeclarationSyntax fieldDeclaration)
    {
      return fieldDeclaration.Declaration.Type.ToString();
    }

    #endregion

  }
}
