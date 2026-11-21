//-----------------------------------------------------------------------
// <copyright file="NotNullPropertyRules.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Which managed properties are registered as not-null properties</summary>
//-----------------------------------------------------------------------
// This file is compiled into Csla.Analyzers and, as a linked file, into the
// bundled source generator. The generator uses it today; it lives here so the
// not-null analyzers and migration fixes planned in #5047 can share the same
// rule rather than a copy of it.
//-----------------------------------------------------------------------

using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// How a managed property is registered.
  /// </summary>
  internal enum NotNullRegistration
  {
    /// <summary><c>RegisterProperty</c>: the type is nullable, or nullability is unknown.</summary>
    None,
    /// <summary><c>RegisterNotNullProperty</c> without a default value.</summary>
    NotNull,
    /// <summary><c>RegisterNotNullProperty&lt;string&gt;</c> with <c>string.Empty</c> as its default value.</summary>
    NotNullString
  }

  /// <summary>
  /// The rule that decides whether a managed property is a not-null property.
  /// </summary>
  internal static class NotNullPropertyRules
  {
    /// <summary>
    /// Gets a value indicating whether nullable annotations are enabled at
    /// a position: the project setting, changed by any <c>#nullable</c> directive.
    /// </summary>
    public static bool IsAnnotationsEnabled(SemanticModel semanticModel, int position)
      => (semanticModel.GetNullableContext(position) & NullableContext.AnnotationsEnabled) == NullableContext.AnnotationsEnabled;

    /// <summary>
    /// Gets how a property is registered, when it is declared in a context
    /// with nullable annotations enabled. A property marked <c>[AllowNull]</c>
    /// or <c>[MaybeNull]</c> lets null in or out, so it is not a not-null
    /// property whatever its type: a not-null registration would make its
    /// setter stricter than the declaration (and than an interface it implements).
    /// </summary>
    public static NotNullRegistration GetRegistration(IPropertySymbol property)
    {
      foreach (var attribute in property.GetAttributes())
      {
        if (attribute.AttributeClass is { Name: "AllowNullAttribute" or "MaybeNullAttribute" } attributeClass
          && attributeClass.ContainingNamespace?.ToDisplayString() == "System.Diagnostics.CodeAnalysis")
          return NotNullRegistration.None;
      }
      return GetRegistration(property.Type);
    }

    /// <summary>
    /// Gets how a property of the given declared type is registered, when it
    /// is declared in a context with nullable annotations enabled.
    /// </summary>
    /// <param name="type">The declared type of the property, from the semantic model.</param>
    public static NotNullRegistration GetRegistration(ITypeSymbol? type)
    {
      if (!QualifiesForNotNull(type))
        return NotNullRegistration.None;
      return type!.SpecialType == SpecialType.System_String
        ? NotNullRegistration.NotNullString
        : NotNullRegistration.NotNull;
    }

    /// <summary>
    /// Gets a value indicating whether a property of the given declared type
    /// can never hold null and satisfies the <c>notnull</c> constraint of
    /// <c>RegisterNotNullProperty&lt;P&gt;</c>.
    /// </summary>
    public static bool QualifiesForNotNull(ITypeSymbol? type)
    {
      switch (type)
      {
        case null:
        case IErrorTypeSymbol:
          return false;
        case ITypeParameterSymbol typeParameter:
          return SatisfiesNotNull(typeParameter);
      }

      if (type.IsValueType)
        return type.OriginalDefinition.SpecialType != SpecialType.System_Nullable_T;

      return type.NullableAnnotation == NullableAnnotation.NotAnnotated;
    }

    /// <summary>
    /// A type parameter satisfies <c>notnull</c> only through its constraints:
    /// an unconstrained <c>T</c> can be a nullable type, and using it with
    /// <c>RegisterNotNullProperty&lt;T&gt;</c> would warn (CS8714).
    /// </summary>
    private static bool SatisfiesNotNull(ITypeParameterSymbol typeParameter)
    {
      if (typeParameter.NullableAnnotation == NullableAnnotation.Annotated)
        return false;
      if (typeParameter.HasNotNullConstraint || typeParameter.HasValueTypeConstraint || typeParameter.HasUnmanagedTypeConstraint)
        return true;
      if (typeParameter.HasReferenceTypeConstraint && typeParameter.ReferenceTypeConstraintNullableAnnotation == NullableAnnotation.NotAnnotated)
        return true;
      foreach (var constraint in typeParameter.ConstraintTypes)
      {
        if (QualifiesForNotNull(constraint))
          return true;
      }
      return false;
    }
  }
}
