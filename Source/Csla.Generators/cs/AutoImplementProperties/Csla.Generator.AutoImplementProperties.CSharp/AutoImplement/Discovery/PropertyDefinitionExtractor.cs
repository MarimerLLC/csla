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
      ExtractedPropertyDefinition propertyDefinition = new ExtractedPropertyDefinition
      {
        PropertyName = GetPropertyName(propertyDeclaration),
        Getter = HasGetter(propertyDeclaration),
        Setter = HasSetter(propertyDeclaration),
        SetterModifiers = GetSetterModifiers(propertyDeclaration),
        Modifiers = GetPropertyModifiers(propertyDeclaration),
        Partial = IsPartial(propertyDeclaration),
        TypeDefinition = new ExtractedMemberTypeDefinition
        {
          Nullable = GetFieldTypeNullable(propertyDeclaration),
          IsValueType = extractionContext.IsValueType(propertyDeclaration.Type),
          FullyQualifiedType = extractionContext.GetFullyQualifiedType(propertyDeclaration.Type)
        }
      };
      propertyDefinition.AttributeDefinitions.AddRange(GetPropertyAttributes(propertyDeclaration, extractionContext));

      return propertyDefinition;
    }

    #region Private Helper Methods
    /// <summary>
    /// Determines whether the field type is nullable.
    /// </summary>
    /// <param name="propertyDeclaration">The PropertyDeclarationSyntax representing the field declaration.</param>
    /// <returns><c>true</c> if the field type is nullable; otherwise, <c>false</c>.</returns>
    private static bool GetFieldTypeNullable(PropertyDeclarationSyntax propertyDeclaration)
    {
      return propertyDeclaration.Type is NullableTypeSyntax;
    }

    /// <summary>
    /// Extract the name of the property for which we are building information
    /// </summary>
    /// <param name="propertyDeclaration">The PropertyDeclarationSyntax from which to extract the necessary information</param>
    /// <returns>The name of the property for which we are extracting information</returns>
    private static string GetPropertyName(PropertyDeclarationSyntax propertyDeclaration)
    {
      return propertyDeclaration.Identifier.ValueText;
    }

    /// <summary>
    /// Extract the type name of the property for which we are building information
    /// </summary>
    /// <param name="propertyDeclaration">The PropertyDeclarationSyntax from which to extract the necessary information</param>
    /// <returns>The type name of the property for which we are extracting information</returns>
    private static string GetPropertyTypeName(PropertyDeclarationSyntax propertyDeclaration)
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

    private static List<ExtractedAttributeDefinition> GetPropertyAttributes(PropertyDeclarationSyntax propertyDeclaration, DefinitionExtractionContext extractionContext)
    {
      List<ExtractedAttributeDefinition> attributes = [];
      if (!extractionContext.AddAttributes)
      {
        return attributes;
      }
      // The attribute is copied into a generated file that doesn't have the using directives
      // of the interface's file, so its type and the names in its arguments are fully qualified
      var qualifier = new AttributeArgumentQualifier(extractionContext.SemanticModel);
      foreach (var attributeList in propertyDeclaration.AttributeLists)
      {
        foreach (var attribute in attributeList.Attributes)
        {
          ExtractedAttributeDefinition attributeDefinition = new ExtractedAttributeDefinition
          {
            AttributeName = GetAttributeTypeName(attribute, extractionContext.SemanticModel)
          };
          // Add named properties
          foreach (var argument in attribute.ArgumentList?.Arguments ?? [])
          {
            var expression = qualifier.Qualify(argument.Expression);
            if (argument.NameEquals != null)
            {
              attributeDefinition.NamedProperties.Add(argument.NameEquals.Name.ToString(), expression);
            }
            else
            {
              // Keep a named constructor argument's name, since it can be out of order
              attributeDefinition.ConstructorArguments.Add(argument.NameColon is null ? expression : $"{argument.NameColon.Name}: {expression}");
            }
          }

          attributes.Add(attributeDefinition);
        }
      }

      return attributes;
    }

    /// <summary>
    /// Get the fully qualified name of an attribute's type, however its name is written
    /// (short, namespace-qualified, aliased or nested)
    /// </summary>
    private static string GetAttributeTypeName(AttributeSyntax attribute, SemanticModel semanticModel)
    {
      var symbolInfo = semanticModel.GetSymbolInfo(attribute);
      // When the arguments don't match a constructor there is no symbol, but the
      // candidates still identify the attribute type
      var constructor = symbolInfo.Symbol ?? symbolInfo.CandidateSymbols.FirstOrDefault();
      return constructor?.ContainingType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? attribute.Name.ToString();
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

    /// <summary>
    /// Get the setter modifiers for a property declaration.
    /// </summary>
    /// <param name="propertyDeclaration">The PropertyDeclarationSyntax representing the property declaration.</param>
    /// <returns>An array of strings representing the setter modifiers.</returns>
    private static string[] GetSetterModifiers(PropertyDeclarationSyntax propertyDeclaration)
    {
      var setterModifiers = propertyDeclaration.AccessorList?.Accessors
          .Where(a => a.Kind() == SyntaxKind.SetAccessorDeclaration)
          .SelectMany(a => a.Modifiers)
          .Select(m => m.ToString())
          .ToArray();

      return setterModifiers ?? [];
    }

    #endregion

  }
}
