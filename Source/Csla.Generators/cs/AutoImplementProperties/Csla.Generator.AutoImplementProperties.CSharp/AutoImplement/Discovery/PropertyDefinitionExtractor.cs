//-----------------------------------------------------------------------
// <copyright file="PropertyDefinitionExtractor.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Extract the definition of a single property of a type for source generation</summary>
//-----------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Csla.Generator.AutoImplementProperties.CSharp.AutoImplement.Discovery
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
    /// <param name="propertyDeclaration">The PropertyDeclarationSyntax from which to extract the necessary data</param>
    /// <returns>A readonly list of ExtractedPropertyDefinition containing the data extracted from the syntax tree</returns>
    public static ExtractedPropertyDefinition ExtractPropertyDefinition(DefinitionExtractionContext extractionContext, PropertyDeclarationSyntax propertyDeclaration)
    {
      ExtractedPropertyDefinition propertyDefinition = new ExtractedPropertyDefinition();

      propertyDefinition.PropertyName = GetPropertyName(extractionContext, propertyDeclaration);
      propertyDefinition.Getter = HasGetter(propertyDeclaration);
      propertyDefinition.Setter = HasSetter(propertyDeclaration);
      propertyDefinition.Modifiers = GetPropertyModifiers(propertyDeclaration);
      propertyDefinition.AttributeDefinitions.AddRange(GetPropertyAttributes(propertyDeclaration));
      propertyDefinition.Partial = IsPartial(propertyDeclaration);

      propertyDefinition.TypeDefinition.TypeName = GetPropertyTypeName(extractionContext, propertyDeclaration);
      propertyDefinition.TypeDefinition.TypeNamespace = extractionContext.GetTypeNamespace(propertyDeclaration.Type);
      propertyDefinition.TypeDefinition.Nullable = GetFieldTypeNullable(extractionContext, propertyDeclaration);

      return propertyDefinition;
    }

    #region Private Helper Methods
    /// <summary>
    /// Determines whether the field type is nullable.
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed.</param>
    /// <param name="propertyDeclaration">The PropertyDeclarationSyntax representing the field declaration.</param>
    /// <returns><c>true</c> if the field type is nullable; otherwise, <c>false</c>.</returns>
    private static bool GetFieldTypeNullable(DefinitionExtractionContext extractionContext, PropertyDeclarationSyntax propertyDeclaration)
    {
      return propertyDeclaration.Type is NullableTypeSyntax;
    }

    /// <summary>
    /// Extract the name of the property for which we are building information
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed</param>
    /// <param name="propertyDeclaration">The PropertyDeclarationSyntax from which to extract the necessary information</param>
    /// <returns>The name of the property for which we are extracting information</returns>
    private static string GetPropertyName(DefinitionExtractionContext extractionContext, PropertyDeclarationSyntax propertyDeclaration)
    {
      return propertyDeclaration.Identifier.ValueText;
    }

    /// <summary>
    /// Extract the type name of the property for which we are building information
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed</param>
    /// <param name="propertyDeclaration">The PropertyDeclarationSyntax from which to extract the necessary information</param>
    /// <returns>The type name of the property for which we are extracting information</returns>
    private static string GetPropertyTypeName(DefinitionExtractionContext extractionContext, PropertyDeclarationSyntax propertyDeclaration)
    {
      return propertyDeclaration.Type.ToString();
    }

    /// <summary>
    /// Determines whether the property has a getter.
    /// </summary>
    /// <param name="propertyDeclaration">The PropertyDeclarationSyntax representing the property declaration.</param>
    /// <returns><c>true</c> if the property has a getter; otherwise, <c>false</c>.</returns>
    private static bool HasGetter(PropertyDeclarationSyntax propertyDeclaration)
    {
      return propertyDeclaration.AccessorList?.Accessors.Any(a => a.Kind() == SyntaxKind.GetAccessorDeclaration) ?? false;
    }

    /// <summary>
    /// Determines whether the property has a setter.
    /// </summary>
    /// <param name="propertyDeclaration">The PropertyDeclarationSyntax representing the property declaration.</param>
    /// <returns><c>true</c> if the property has a setter; otherwise, <c>false</c>.</returns>
    private static bool HasSetter(PropertyDeclarationSyntax propertyDeclaration)
    {
      return propertyDeclaration.AccessorList?.Accessors.Any(a => a.Kind() == SyntaxKind.SetAccessorDeclaration) ?? false;
    }

    /// <summary>
    /// Get the property modifiers as a string array.
    /// </summary>
    /// <param name="propertyDeclaration">The PropertyDeclarationSyntax representing the property declaration.</param>
    /// <returns>An array of strings representing the property modifiers.</returns>
    private static string[] GetPropertyModifiers(PropertyDeclarationSyntax propertyDeclaration)
    {
      var modifiers = propertyDeclaration.Modifiers.Select(m => m.ToString()).ToArray();
      return modifiers;
    }

    private static List<ExtractedAttributeDefinition> GetPropertyAttributes(PropertyDeclarationSyntax propertyDeclaration)
    {
      List<ExtractedAttributeDefinition> attributes = [];

      foreach (var attributeList in propertyDeclaration.AttributeLists)
      {
        foreach (var attribute in attributeList.Attributes)
        {
          ExtractedAttributeDefinition attributeDefinition = new ExtractedAttributeDefinition();
          attributeDefinition.AttributeName = attribute.Name.ToString();

          // Add constructor arguments
          foreach (var argument in attribute.ArgumentList.Arguments)
          {
            attributeDefinition.ConstructorArguments.Add(argument.Expression.ToString());
          }

          // Add named properties
          foreach (var argument in attribute.ArgumentList.Arguments)
          {
            if (argument.NameEquals != null)
            {
              attributeDefinition.NamedProperties.Add(argument.NameEquals.Name.ToString(), argument.Expression.ToString());
            }
          }

          attributes.Add(attributeDefinition);
        }
      }

      return attributes;
    }
    /// <summary>
    /// Determines whether the property is partial.
    /// </summary>
    /// <param name="propertyDeclaration">The PropertyDeclarationSyntax representing the property declaration.</param>
    /// <returns><c>true</c> if the property is partial; otherwise, <c>false</c>.</returns>
    private static bool IsPartial(PropertyDeclarationSyntax propertyDeclaration)
    {
      return propertyDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
    }

    #endregion

  }
}
