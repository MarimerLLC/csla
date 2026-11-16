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

        if (attribute?.Name is GenericNameSyntax genericName)
        {
          // Get the generic argument of the attribute
          var genericArgument = genericName.TypeArgumentList?.Arguments.FirstOrDefault();

          if (genericArgument is null)
          {
            return extractedTypeDefinition;
          }

          // Get the type symbol of the generic argument
          var semanticModel = extractionContext.SemanticModel;
          var typeSymbol = semanticModel.GetTypeInfo(genericArgument).Type;

          // Check if the type symbol is an interface
          if (typeSymbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.TypeKind == TypeKind.Interface)
          {
            // Get the InterfaceDeclarationSyntax of the generic parameter; it must be in
            // this compilation so it can be bound
            var compilation = semanticModel.Compilation;
            var interfaceDeclarationSyntax = namedTypeSymbol.DeclaringSyntaxReferences
                .Where(r => compilation.ContainsSyntaxTree(r.SyntaxTree))
                .Select(r => r.GetSyntax())
                .OfType<InterfaceDeclarationSyntax>()
                .FirstOrDefault();

            if (interfaceDeclarationSyntax != null)
            {
              // The interface may be declared in another file, so bind it with its own tree's semantic model
              var interfaceContext = new DefinitionExtractionContext(
                compilation.GetSemanticModel(interfaceDeclarationSyntax.SyntaxTree), extractionContext.AddAttributes, extractionContext.FilterPartialProperties);
              foreach (ExtractedPropertyDefinition propertyDefinition in PropertyDefinitionsExtractor.ExtractPropertyDefinitions(interfaceContext, interfaceDeclarationSyntax))
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
      var typeName = GetTypeName(targetTypeDeclaration);
      var containingTypes = GetContainingTypes(targetTypeDeclaration);
      var nameSpace = GetNamespace(targetTypeDeclaration);
      ExtractedTypeDefinition definition = new ExtractedTypeDefinition
      {
        TypeName = typeName,
        TypeParameters = GetTypeParameters(targetTypeDeclaration),
        TypeKind = GetTypeKind(targetTypeDeclaration),
        Namespace = nameSpace,
        Scope = GetScopeDefinition(targetTypeDeclaration),
        BaseClassTypeName = GetBaseClassTypeName(extractionContext, targetTypeDeclaration),
        DefaultPropertyModifiers = ["public"],
        DefaultPropertySetterModifiers = [],
        ContainingTypeDeclarations = containingTypes.Select(GetPartialDeclaration).ToArray(),
        // Used as the hint name, so it must be unique in the compilation: namespace, containing
        // types and the type, each generic one with its arity (see GetHintSegment)
        FullyQualifiedName = string.Join(".", new[] { nameSpace }
          .Concat(containingTypes.Concat([targetTypeDeclaration]).Select(GetHintSegment))
          .Where(s => !string.IsNullOrEmpty(s)))
      };

      foreach (ExtractedPropertyDefinition propertyDefinition in PropertyDefinitionsExtractor.ExtractPropertyDefinitions(extractionContext, targetTypeDeclaration))
      {
        definition.Properties.Add(propertyDefinition);
      }

      return definition;
    }

    #region Private Helper Methods

    // ... existing helper methods ...

    /// <summary>
    /// Extract the name of the base class of the type for which we will be generating code
    /// </summary>
    /// <param name="extractionContext">The definition extraction context in which the extraction is being performed</param>
    /// <param name="targetTypeDeclaration">The TypeDeclarationSyntax from which to extract the necessary information</param>
    /// <returns>The name of the nearest base class in <see cref="CslaBaseClasses"/> the type inherits,
    /// through any intermediate base classes; otherwise an empty string, and nothing is generated</returns>
    private static string GetBaseClassTypeName(DefinitionExtractionContext extractionContext, TypeDeclarationSyntax targetTypeDeclaration)
    {
      var targetTypeSymbol = extractionContext.SemanticModel.GetDeclaredSymbol(targetTypeDeclaration) as INamedTypeSymbol;

      for (var baseType = targetTypeSymbol?.BaseType; baseType is not null; baseType = baseType.BaseType)
      {
        if (baseType.ContainingNamespace is { Name: "Csla" } containingNamespace
          && containingNamespace.ContainingNamespace?.IsGlobalNamespace == true
          && CslaBaseClasses.PropertyMethods.ContainsKey(baseType.Name))
        {
          return baseType.Name;
        }
      }

      return string.Empty;
    }

    /// <summary>
    /// Get the types that contain a nested type, outermost first
    /// </summary>
    /// <param name="targetTypeDeclaration">The declaration of the possibly nested type</param>
    /// <returns>The declarations of the containing types; empty for a top-level type</returns>
    private static List<TypeDeclarationSyntax> GetContainingTypes(TypeDeclarationSyntax targetTypeDeclaration)
    {
      var containingTypes = new List<TypeDeclarationSyntax>();
      for (var parent = targetTypeDeclaration.Parent as TypeDeclarationSyntax; parent is not null; parent = parent.Parent as TypeDeclarationSyntax)
      {
        containingTypes.Insert(0, parent);
      }

      return containingTypes;
    }

    /// <summary>
    /// Build the partial declaration that reopens a containing type, such as <c>partial class Outer&lt;T&gt;</c>
    /// </summary>
    /// <param name="typeDeclaration">The declaration of the containing type</param>
    /// <returns>The partial declaration, without accessibility, base list or constraints, which a partial declaration may omit</returns>
    private static string GetPartialDeclaration(TypeDeclarationSyntax typeDeclaration)
    {
      var keyword = typeDeclaration is RecordDeclarationSyntax { ClassOrStructKeyword.RawKind: not 0 } record
        ? $"record {record.ClassOrStructKeyword}"
        : typeDeclaration.Keyword.ToString();
      return $"partial {keyword} {typeDeclaration.Identifier}{GetTypeParameters(typeDeclaration)}";
    }

    /// <summary>
    /// Get the hint name segment of a type: its name, with its arity if it is generic.
    /// '&lt;' and '&gt;' aren't allowed in hint names, and the arity follows a '-', which can't occur in an
    /// identifier, so Foo&lt;T&gt; (Foo-1) collides neither with Foo nor with a class named Foo_1
    /// </summary>
    /// <param name="typeDeclaration">The declaration of the type</param>
    /// <returns>The hint name segment, such as <c>Foo</c> or <c>Foo-1</c></returns>
    private static string GetHintSegment(TypeDeclarationSyntax typeDeclaration)
    {
      var arity = typeDeclaration.TypeParameterList?.Parameters.Count ?? 0;
      return arity > 0 ? $"{typeDeclaration.Identifier}-{arity}" : typeDeclaration.Identifier.ToString();
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
      SyntaxNode? potentialNamespaceParent = targetTypeDeclaration.Parent;
    
      // Keep moving "out" of nested classes etc until we get to a namespace
      // or until we run out of parents
      while (potentialNamespaceParent != null && potentialNamespaceParent is not NamespaceDeclarationSyntax && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
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
    /// Build the type parameter list the generated partial declaration must repeat
    /// </summary>
    /// <param name="typeDeclaration">The declaration of the type</param>
    /// <returns>The type parameter names in angle brackets, such as <c>&lt;T&gt;</c>, without
    /// attributes or constraints (which a partial declaration may omit); empty for a non-generic type</returns>
    private static string GetTypeParameters(TypeDeclarationSyntax typeDeclaration)
    {
      if (typeDeclaration.TypeParameterList is not { Parameters.Count: > 0 } typeParameterList)
      {
        return string.Empty;
      }

      return $"<{string.Join(", ", typeParameterList.Parameters.Select(p => p.Identifier.ToString()))}>";
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
