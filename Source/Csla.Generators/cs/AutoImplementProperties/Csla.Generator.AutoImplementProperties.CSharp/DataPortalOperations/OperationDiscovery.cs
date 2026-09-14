//-----------------------------------------------------------------------
// <copyright file="OperationDiscovery.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Extracts data portal operation models from Roslyn symbols</summary>
//-----------------------------------------------------------------------

using System.Globalization;
using System.Text;
using Csla.Generator.AutoImplementProperties.CSharp.DataPortalOperations.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Csla.Generator.AutoImplementProperties.CSharp.DataPortalOperations
{
  /// <summary>
  /// Extracts equatable data portal operation models from Roslyn symbols.
  /// </summary>
  internal static class OperationDiscovery
  {
    /// <summary>
    /// The data portal operation kinds, in the order used for generated dispatch.
    /// </summary>
    public static readonly string[] OperationKinds =
    [
      "Create",
      "Fetch",
      "Insert",
      "Update",
      "Execute",
      "Delete",
      "DeleteSelf",
      "CreateChild",
      "FetchChild",
      "InsertChild",
      "UpdateChild",
      "DeleteSelfChild",
      "ExecuteChild"
    ];

    public const string InjectAttributeName = "Csla.InjectAttribute";
    public const string RunLocalAttributeName = "Csla.RunLocalAttribute";
    public const string NoDataPortalExtensionAttributeName = "Csla.NoDataPortalExtensionAttribute";
    public const string OperationsInterfaceName = "IDataPortalOperations";

    private const string OperationMappingName = "Csla.Server.IDataPortalOperationMapping";
    private const string NamedOperationMappingName = "Csla.Server.IDataPortalOperationNamedMapping";

    /// <summary>
    /// Fully qualified type with nullable reference annotations.
    /// </summary>
    internal static readonly SymbolDisplayFormat TypeFormat = SymbolDisplayFormat.FullyQualifiedFormat
      .AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

    /// <summary>
    /// Fully qualified type without nullable reference annotations, with tuples
    /// expanded so the name can be used in an <c>is</c> pattern.
    /// </summary>
    internal static readonly SymbolDisplayFormat PatternFormat = SymbolDisplayFormat.FullyQualifiedFormat
      .AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.ExpandValueTuple);

    private static readonly SymbolDisplayFormat MethodDisplayFormat = new(
      memberOptions: SymbolDisplayMemberOptions.IncludeParameters,
      parameterOptions: SymbolDisplayParameterOptions.IncludeType,
      miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

    public static string GetAttributeMetadataName(string kind) => $"Csla.{kind}Attribute";

    /// <summary>
    /// Extract the model for one operation method, as seen through one operation attribute.
    /// </summary>
    public static OperationMethodEntry? ExtractMethod(GeneratorAttributeSyntaxContext context, string kind, CancellationToken ct)
    {
      if (context.TargetSymbol is not IMethodSymbol method || !IsCandidateMethod(method))
        return null;

      var type = method.ContainingType;
      if (type is null || type.TypeKind != TypeKind.Class)
        return null;

      ct.ThrowIfCancellationRequested();

      var compilation = context.SemanticModel.Compilation;
      var header = BuildHeader(type, context.TargetNode, compilation);
      var model = BuildMethod(method, kind);
      return new OperationMethodEntry(header, model);
    }

    /// <summary>
    /// A method that the generator includes in the operations interface.
    /// </summary>
    internal static bool IsCandidateMethod(IMethodSymbol method)
      => method.MethodKind == MethodKind.Ordinary && !method.IsStatic && !method.IsGenericMethod;

    private static OperationTypeHeader BuildHeader(INamedTypeSymbol type, SyntaxNode methodNode, Compilation compilation)
    {
      var containers = new List<INamedTypeSymbol>();
      for (var container = type.ContainingType; container is not null; container = container.ContainingType)
        containers.Insert(0, container);

      var hintParts = new List<string>();
      var @namespace = type.ContainingNamespace is { IsGlobalNamespace: false } ns ? ns.ToDisplayString() : string.Empty;
      if (@namespace.Length > 0)
        hintParts.Add(@namespace);
      foreach (var container in containers)
        hintParts.Add(GetHintSegment(container));
      hintParts.Add(GetHintSegment(type));

      var isGeneric = type.TypeParameters.Length > 0 || containers.Any(c => c.TypeParameters.Length > 0);

      return new OperationTypeHeader
      {
        MetadataName = GetFullMetadataName(type),
        Namespace = @namespace,
        ContainerDeclarations = new EquatableArray<string>(containers.Select(GetPartialDeclaration)),
        ContainerNames = new EquatableArray<string>(containers.Select(c => c.Name)),
        TypeDeclaration = GetPartialDeclaration(type),
        FullyQualifiedName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
        TypeName = type.Name,
        HintName = string.Join(".", hintParts),
        IsPartial = IsPartialAllTheWay(methodNode),
        IsAbstract = type.IsAbstract,
        IsGeneric = isGeneric,
        HidesInheritedOperationsInterface = HidesInheritedOperationsInterface(type, compilation),
        ImplementsOperationMapping = type.Interfaces.Any(i => i.ToDisplayString() == OperationMappingName),
        ImplementsNamedOperationMapping = type.Interfaces.Any(i => i.ToDisplayString() == NamedOperationMappingName)
      };
    }

    private static OperationMethodModel BuildMethod(IMethodSymbol method, string kind)
    {
      var parameters = method.Parameters.Select(BuildParameter).ToArray();
      var criteriaKeys = parameters.Where(p => !p.IsInjected).Select(p => p.TypeKey).ToArray();
      var operationName = criteriaKeys.Length == 0 ? kind : kind + "__" + string.Join("_", criteriaKeys);

      return new OperationMethodModel
      {
        MethodName = EscapeIdentifier(method.Name),
        MethodDisplay = method.ToDisplayString(MethodDisplayFormat),
        Kind = kind,
        ReturnTypeDisplay = method.ReturnType.ToDisplayString(TypeFormat),
        IsAsync = IsAwaitable(method.ReturnType),
        Parameters = new EquatableArray<OperationParameterModel>(parameters),
        OperationName = operationName,
        RunLocal = HasAttribute(method, RunLocalAttributeName),
        NoExtension = HasAttribute(method, NoDataPortalExtensionAttributeName),
        Location = LocationInfo.From(method.Locations.FirstOrDefault())
      };
    }

    private static OperationParameterModel BuildParameter(IParameterSymbol parameter)
    {
      var type = parameter.Type;
      var isNullableValueType = type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T };
      var patternType = isNullableValueType ? ((INamedTypeSymbol)type).TypeArguments[0] : type;

      var isInjected = false;
      var allowNull = false;
      string? serviceKey = null;
      foreach (var attribute in parameter.GetAttributes())
      {
        if (!IsOrDerivesFrom(attribute.AttributeClass, InjectAttributeName))
          continue;
        isInjected = true;
        foreach (var namedArgument in attribute.NamedArguments)
        {
          if (namedArgument.Key == "AllowNull" && namedArgument.Value.Value is bool value)
            allowNull = value;
          else if (namedArgument.Key == "Key" && !namedArgument.Value.IsNull)
            serviceKey = FormatTypedConstant(namedArgument.Value);
        }
      }

      // Like reflection-based dispatch, a nullable parameter type makes the service optional.
      if (isInjected && (isNullableValueType || (type.IsReferenceType && parameter.NullableAnnotation == NullableAnnotation.Annotated)))
        allowNull = true;

      return new OperationParameterModel
      {
        Name = EscapeIdentifier(parameter.Name),
        TypeDisplay = type.ToDisplayString(TypeFormat),
        PatternTypeDisplay = patternType.ToDisplayString(PatternFormat),
        TypeKey = GetOperationTypeKey(type),
        RefKindPrefix = parameter.RefKind switch
        {
          RefKind.Ref => "ref ",
          RefKind.Out => "out ",
          RefKind.In => "in ",
          _ => string.Empty
        },
        IsNullableValueType = isNullableValueType,
        AcceptsNull = type.IsReferenceType || isNullableValueType,
        IsExactTypeMatch = patternType.IsValueType || (patternType.IsSealed && patternType.TypeKind != TypeKind.Array),
        ContainsTypeParameter = ContainsTypeParameter(type),
        IsParams = parameter.IsParams,
        IsObjectArray = type is IArrayTypeSymbol { Rank: 1, ElementType.SpecialType: SpecialType.System_Object },
        IsInjected = isInjected,
        AllowNull = allowNull,
        ServiceKeyExpression = serviceKey,
        DefaultValueExpression = parameter.HasExplicitDefaultValue ? FormatDefaultValue(type, parameter.ExplicitDefaultValue) : null,
        Visibility = GetVisibility(type)
      };
    }

    /// <summary>
    /// Computes a deterministic type key for use in operation names.
    /// Must produce the same result as DataPortalOperationNameHelper.GetTypeKey(Type) at runtime.
    /// Arrays: elementType + "Array" (e.g. "Int32Array").
    /// Generic types: MetadataName with backtick replaced, then all type
    /// arguments including those of containing types (e.g. "List_1_Int32").
    /// Other types: MetadataName (e.g. "Int32", "String").
    /// </summary>
    internal static string GetOperationTypeKey(ITypeSymbol typeSymbol)
    {
      if (typeSymbol is IArrayTypeSymbol arrayType)
        return GetOperationTypeKey(arrayType.ElementType) + "Array";

      if (typeSymbol.TypeKind == TypeKind.Dynamic)
        return "Object";

      if (typeSymbol is INamedTypeSymbol namedType && namedType.IsGenericType)
      {
        var typeArguments = new List<ITypeSymbol>();
        CollectTypeArguments(namedType, typeArguments);
        var baseName = namedType.MetadataName.Replace('`', '_');
        return baseName + "_" + string.Join("_", typeArguments.Select(GetOperationTypeKey));
      }

      return typeSymbol.MetadataName;
    }

    private static void CollectTypeArguments(INamedTypeSymbol type, List<ITypeSymbol> typeArguments)
    {
      if (type.ContainingType is not null)
        CollectTypeArguments(type.ContainingType, typeArguments);
      typeArguments.AddRange(type.TypeArguments);
    }

    /// <summary>
    /// Returns true when the type will expose a generated operations interface.
    /// </summary>
    internal static bool WillGenerateOperationsInterface(INamedTypeSymbol type)
    {
      if (type.TypeKind != TypeKind.Class)
        return false;

      var hasOperations = type.GetMembers()
        .OfType<IMethodSymbol>()
        .Any(m => IsCandidateMethod(m) && m.GetAttributes().Any(a => IsOperationAttribute(a.AttributeClass)));
      if (!hasOperations)
        return false;

      return type.DeclaringSyntaxReferences.All(r => r.GetSyntax() is TypeDeclarationSyntax declaration && IsPartialAllTheWay(declaration));
    }

    internal static bool IsOperationAttribute(INamedTypeSymbol? attributeClass)
    {
      if (attributeClass is null || attributeClass.ContainingNamespace?.ToDisplayString() != "Csla")
        return false;
      var name = attributeClass.Name;
      return name.EndsWith("Attribute") && OperationKinds.Contains(name.Substring(0, name.Length - "Attribute".Length));
    }

    private static bool HidesInheritedOperationsInterface(INamedTypeSymbol type, Compilation compilation)
    {
      for (var baseType = type.BaseType; baseType is not null && baseType.SpecialType != SpecialType.System_Object; baseType = baseType.BaseType)
      {
        var definition = baseType.OriginalDefinition;
        if (definition.GetTypeMembers(OperationsInterfaceName).Any(t => compilation.IsSymbolAccessibleWithin(t, compilation.Assembly)))
          return true;
        if (SymbolEqualityComparer.Default.Equals(definition.ContainingAssembly, compilation.Assembly) && WillGenerateOperationsInterface(definition))
          return true;
      }
      return false;
    }

    /// <summary>
    /// The type declaration containing the node and all of its
    /// containing type declarations have the partial modifier.
    /// </summary>
    internal static bool IsPartialAllTheWay(SyntaxNode node)
    {
      var found = false;
      for (var current = node; current is not null; current = current.Parent)
      {
        if (current is TypeDeclarationSyntax typeDeclaration)
        {
          found = true;
          if (!typeDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
            return false;
        }
      }
      return found;
    }

    private static string GetPartialDeclaration(INamedTypeSymbol type)
    {
      var keyword = type switch
      {
        { IsRecord: true, TypeKind: TypeKind.Struct } => "record struct",
        { IsRecord: true } => "record",
        { TypeKind: TypeKind.Struct } => "struct",
        { TypeKind: TypeKind.Interface } => "interface",
        _ => "class"
      };
      var builder = new StringBuilder("partial ").Append(keyword).Append(' ').Append(EscapeIdentifier(type.Name));
      if (type.TypeParameters.Length > 0)
        builder.Append('<').Append(string.Join(", ", type.TypeParameters.Select(t => EscapeIdentifier(t.Name)))).Append('>');
      return builder.ToString();
    }

    private static string GetHintSegment(INamedTypeSymbol type)
      => type.TypeParameters.Length > 0 ? $"{type.Name}_{type.TypeParameters.Length}" : type.Name;

    internal static string GetFullMetadataName(INamedTypeSymbol type)
    {
      var name = type.MetadataName;
      for (var container = type.ContainingType; container is not null; container = container.ContainingType)
        name = container.MetadataName + "+" + name;
      if (type.ContainingNamespace is { IsGlobalNamespace: false } ns)
        name = ns.ToDisplayString() + "." + name;
      return name;
    }

    private static bool IsAwaitable(ITypeSymbol returnType)
    {
      if (returnType is not INamedTypeSymbol named)
        return false;
      var definition = named.OriginalDefinition;
      if (definition.ContainingNamespace?.ToDisplayString() != "System.Threading.Tasks")
        return false;
      return definition.MetadataName is "Task" or "Task`1" or "ValueTask" or "ValueTask`1";
    }

    private static bool HasAttribute(ISymbol symbol, string attributeName)
      => symbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == attributeName);

    private static bool IsOrDerivesFrom(INamedTypeSymbol? type, string fullName)
    {
      for (var current = type; current is not null; current = current.BaseType)
      {
        if (current.ToDisplayString() == fullName)
          return true;
      }
      return false;
    }

    private static bool ContainsTypeParameter(ITypeSymbol type) => type switch
    {
      ITypeParameterSymbol => true,
      IArrayTypeSymbol array => ContainsTypeParameter(array.ElementType),
      INamedTypeSymbol named => named.TypeArguments.Any(ContainsTypeParameter)
        || (named.ContainingType is not null && ContainsTypeParameter(named.ContainingType)),
      _ => false
    };

    internal static TypeVisibility GetVisibility(ITypeSymbol type)
    {
      switch (type)
      {
        case IArrayTypeSymbol array:
          return GetVisibility(array.ElementType);
        case INamedTypeSymbol named:
          var result = TypeVisibility.Public;
          for (var current = named; current is not null; current = current.ContainingType)
            result = Min(result, FromAccessibility(current.DeclaredAccessibility));
          foreach (var typeArgument in named.TypeArguments)
            result = Min(result, GetVisibility(typeArgument));
          return result;
        default:
          return TypeVisibility.Public;
      }

      static TypeVisibility Min(TypeVisibility left, TypeVisibility right) => (TypeVisibility)Math.Max((int)left, (int)right);

      static TypeVisibility FromAccessibility(Accessibility accessibility) => accessibility switch
      {
        Accessibility.Public => TypeVisibility.Public,
        Accessibility.Internal or Accessibility.ProtectedOrInternal => TypeVisibility.Internal,
        _ => TypeVisibility.Private
      };
    }

    internal static string EscapeIdentifier(string identifier)
      => SyntaxFacts.GetKeywordKind(identifier) != SyntaxKind.None ? "@" + identifier : identifier;

    private static string FormatTypedConstant(TypedConstant constant)
    {
      if (constant.Kind == TypedConstantKind.Type && constant.Value is ITypeSymbol typeValue)
        return $"typeof({typeValue.ToDisplayString(PatternFormat)})";
      if (constant.Kind == TypedConstantKind.Enum && constant.Type is ITypeSymbol enumType)
        return FormatEnumValue(enumType, constant.Value);
      return FormatPrimitive(constant.Value) ?? constant.ToCSharpString();
    }

    private static string FormatDefaultValue(ITypeSymbol type, object? value)
    {
      var isNullableValueType = type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T };
      var targetType = isNullableValueType ? ((INamedTypeSymbol)type).TypeArguments[0] : type;

      if (value is null)
        return type.IsReferenceType || isNullableValueType ? "null" : "default";

      if (targetType.TypeKind == TypeKind.Enum)
        return FormatEnumValue(targetType, value);

      return FormatPrimitive(value) ?? "default";
    }

    private static string FormatEnumValue(ITypeSymbol enumType, object? value)
    {
      var enumName = enumType.ToDisplayString(PatternFormat);
      var member = enumType.GetMembers()
        .OfType<IFieldSymbol>()
        .FirstOrDefault(f => f.HasConstantValue && Equals(f.ConstantValue, value));
      return member is not null
        ? $"{enumName}.{EscapeIdentifier(member.Name)}"
        : $"({enumName})({Convert.ToString(value, CultureInfo.InvariantCulture)})";
    }

    private static string? FormatPrimitive(object? value) => value switch
    {
      null => "null",
      string s => SymbolDisplay.FormatLiteral(s, quote: true),
      char c => SymbolDisplay.FormatLiteral(c, quote: true),
      bool b => b ? "true" : "false",
      float f when float.IsNaN(f) => "float.NaN",
      float f when float.IsPositiveInfinity(f) => "float.PositiveInfinity",
      float f when float.IsNegativeInfinity(f) => "float.NegativeInfinity",
      float f => f.ToString("R", CultureInfo.InvariantCulture) + "F",
      double d when double.IsNaN(d) => "double.NaN",
      double d when double.IsPositiveInfinity(d) => "double.PositiveInfinity",
      double d when double.IsNegativeInfinity(d) => "double.NegativeInfinity",
      double d => d.ToString("R", CultureInfo.InvariantCulture) + "D",
      decimal m => m.ToString(CultureInfo.InvariantCulture) + "M",
      long l => l.ToString(CultureInfo.InvariantCulture) + "L",
      ulong ul => ul.ToString(CultureInfo.InvariantCulture) + "UL",
      uint ui => ui.ToString(CultureInfo.InvariantCulture) + "U",
      int or short or ushort or byte or sbyte => Convert.ToString(value, CultureInfo.InvariantCulture),
      _ => null
    };
  }
}
