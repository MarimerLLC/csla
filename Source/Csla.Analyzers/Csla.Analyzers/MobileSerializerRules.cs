//-----------------------------------------------------------------------
// <copyright file="MobileSerializerRules.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Rules for declarative MobileFormatter custom serializer registrations</summary>
//-----------------------------------------------------------------------
// This file is compiled into Csla.Analyzers and, as a linked file, into the
// bundled source generator, so the generator skips exactly the declarations
// that CSLA0028 reports.
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// Why a custom serializer declaration cannot be registered. The analyzer
  /// maps each value to a localized message.
  /// </summary>
  internal enum MobileSerializerProblem
  {
    None,
    SerializerNotClass,
    SerializerOpenGeneric,
    SerializerStatic,
    SerializerAbstract,
    SerializerNotMobileSerializer,
    SerializerNoPublicConstructor,
    SerializerInaccessible,
    TypeNotClassOrStruct,
    TypeGeneric,
    TypeInaccessible
  }

  /// <summary>
  /// Rules for the <c>[MobileSerializer]</c>, <c>[MobileSerializerFor]</c> and
  /// <c>[assembly: MobileSerializerRegistration]</c> attributes.
  /// </summary>
  internal static class MobileSerializerRules
  {
    public const string AttributeNamespace = "Csla.Serialization.Mobile";
    public const string MobileSerializerAttributeName = "MobileSerializerAttribute";
    public const string MobileSerializerForAttributeName = "MobileSerializerForAttribute";
    public const string MobileSerializerRegistrationAttributeName = "MobileSerializerRegistrationAttribute";
    public const string MobileSerializerInterfaceName = "IMobileSerializer";

    public const string MobileSerializerAttributeMetadataName = AttributeNamespace + "." + MobileSerializerAttributeName;
    public const string MobileSerializerForAttributeMetadataName = AttributeNamespace + "." + MobileSerializerForAttributeName;
    public const string MobileSerializerRegistrationAttributeMetadataName = AttributeNamespace + "." + MobileSerializerRegistrationAttributeName;

    /// <summary>
    /// Determines whether a type is the CSLA type with the given name in
    /// the <c>Csla.Serialization.Mobile</c> namespace.
    /// </summary>
    public static bool IsCslaMobileType(ITypeSymbol? type, string name)
      => type is not null && type.Name == name && type.ContainingType is null
        && type.ContainingNamespace?.ToDisplayString() == AttributeNamespace;

    /// <summary>
    /// Gets the serializer type of a <c>[MobileSerializer]</c> attribute.
    /// </summary>
    public static ITypeSymbol? GetTypeSideSerializer(AttributeData attribute)
      => attribute.ConstructorArguments.Length == 1 && attribute.ConstructorArguments[0].Kind == TypedConstantKind.Type
        ? attribute.ConstructorArguments[0].Value as ITypeSymbol
        : null;

    /// <summary>
    /// Gets the types listed by a <c>[MobileSerializerFor]</c> attribute.
    /// </summary>
    public static IEnumerable<ITypeSymbol> GetSerializerSideTypes(AttributeData attribute)
    {
      if (attribute.ConstructorArguments.Length != 1)
        yield break;
      var argument = attribute.ConstructorArguments[0];
      if (argument.Kind == TypedConstantKind.Array)
      {
        if (argument.IsNull)
          yield break;
        foreach (var value in argument.Values)
        {
          if (value.Value is ITypeSymbol type)
            yield return type;
        }
      }
      else if (argument.Value is ITypeSymbol single)
      {
        yield return single;
      }
    }

    /// <summary>
    /// Gets the type and serializer type of a <c>[MobileSerializerRegistration]</c> attribute.
    /// </summary>
    public static bool TryGetRegistration(AttributeData attribute, [NotNullWhen(true)] out ITypeSymbol? type, [NotNullWhen(true)] out ITypeSymbol? serializer)
    {
      type = null;
      serializer = null;
      if (attribute.ConstructorArguments.Length != 2)
        return false;
      type = attribute.ConstructorArguments[0].Value as ITypeSymbol;
      serializer = attribute.ConstructorArguments[1].Value as ITypeSymbol;
      return type is not null && serializer is not null;
    }

    /// <summary>
    /// Determines whether a type can be registered as a custom serializer:
    /// a non-abstract, closed class that implements <c>IMobileSerializer</c>,
    /// has a public constructor and can be named in an assembly attribute.
    /// </summary>
    /// <param name="serializer">The serializer type.</param>
    /// <param name="compilation">The compilation the registration is generated in.</param>
    /// <param name="problem">
    /// Why the type cannot be used, or <see cref="MobileSerializerProblem.None"/>
    /// when it is valid or when the compiler already reports an error for it.
    /// </param>
    public static bool IsValidSerializer([NotNullWhen(true)] ITypeSymbol? serializer, Compilation compilation, out MobileSerializerProblem problem)
    {
      problem = GetSerializerProblem(serializer, compilation);
      return serializer is not null && serializer.TypeKind != TypeKind.Error && problem == MobileSerializerProblem.None;
    }

    private static MobileSerializerProblem GetSerializerProblem(ITypeSymbol? serializer, Compilation compilation)
    {
      if (serializer is null || serializer.TypeKind == TypeKind.Error)
        return MobileSerializerProblem.None;
      if (serializer is not INamedTypeSymbol named || named.TypeKind != TypeKind.Class)
        return MobileSerializerProblem.SerializerNotClass;
      if (ContainsTypeParameters(named))
        return MobileSerializerProblem.SerializerOpenGeneric;
      if (named.IsStatic)
        return MobileSerializerProblem.SerializerStatic;
      if (named.IsAbstract)
        return MobileSerializerProblem.SerializerAbstract;
      if (!named.AllInterfaces.Any(i => IsCslaMobileType(i, MobileSerializerInterfaceName)))
        return MobileSerializerProblem.SerializerNotMobileSerializer;
      if (!named.InstanceConstructors.Any(c => c.DeclaredAccessibility == Accessibility.Public))
        return MobileSerializerProblem.SerializerNoPublicConstructor;
      if (!compilation.IsSymbolAccessibleWithin(named, compilation.Assembly))
        return MobileSerializerProblem.SerializerInaccessible;
      return MobileSerializerProblem.None;
    }

    /// <summary>
    /// Determines whether a type can have a registered custom serializer:
    /// a non-generic class or struct that can be named in an assembly attribute.
    /// </summary>
    /// <param name="type">The type to serialize.</param>
    /// <param name="compilation">The compilation the registration is generated in.</param>
    /// <param name="problem">
    /// Why the type cannot be used, or <see cref="MobileSerializerProblem.None"/>
    /// when it is valid or when the compiler already reports an error for it.
    /// </param>
    public static bool IsValidTargetType([NotNullWhen(true)] ITypeSymbol? type, Compilation compilation, out MobileSerializerProblem problem)
    {
      problem = GetTargetTypeProblem(type, compilation);
      return type is not null && type.TypeKind != TypeKind.Error && problem == MobileSerializerProblem.None;
    }

    private static MobileSerializerProblem GetTargetTypeProblem(ITypeSymbol? type, Compilation compilation)
    {
      if (type is null || type.TypeKind == TypeKind.Error)
        return MobileSerializerProblem.None;
      if (type is not INamedTypeSymbol named || (named.TypeKind != TypeKind.Class && named.TypeKind != TypeKind.Struct))
        return MobileSerializerProblem.TypeNotClassOrStruct;
      if (IsGeneric(named))
        return MobileSerializerProblem.TypeGeneric;
      if (!compilation.IsSymbolAccessibleWithin(named, compilation.Assembly))
        return MobileSerializerProblem.TypeInaccessible;
      return MobileSerializerProblem.None;
    }

    /// <summary>
    /// Gets the name of a type for use in generated code, e.g. <c>global::NS.Money</c>.
    /// </summary>
    public static string FullyQualifiedName(ITypeSymbol type)
      => type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    /// <summary>
    /// Gets the name of a type for use in diagnostic messages.
    /// </summary>
    public static string DisplayName(ITypeSymbol type)
      => type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);

    private static bool IsGeneric(INamedTypeSymbol type)
    {
      for (var current = type; current is not null; current = current.ContainingType)
      {
        if (current.IsGenericType)
          return true;
      }
      return false;
    }

    private static bool ContainsTypeParameters(ITypeSymbol type)
    {
      switch (type)
      {
        case ITypeParameterSymbol:
          return true;
        case IArrayTypeSymbol array:
          return ContainsTypeParameters(array.ElementType);
        case INamedTypeSymbol named:
          if (named.IsUnboundGenericType)
            return true;
          if (named.TypeArguments.Any(ContainsTypeParameters))
            return true;
          return named.ContainingType is not null && ContainsTypeParameters(named.ContainingType);
        default:
          return false;
      }
    }
  }
}
