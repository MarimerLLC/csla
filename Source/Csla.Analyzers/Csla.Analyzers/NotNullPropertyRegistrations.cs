//-----------------------------------------------------------------------
// <copyright file="NotNullPropertyRegistrations.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Reads not-null property registrations for the not-null analyzers</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Csla.Analyzers
{
  /// <summary>
  /// Reads <c>NotNullPropertyInfo&lt;T&gt;</c> fields and their registrations
  /// for the not-null analyzers.
  /// </summary>
  /// <remarks>
  /// Whether a registration has a default value is read from the syntax of the
  /// field's initializer, not from a semantic model: the field can be declared in
  /// another file, in a base type, or in source generator output, which the
  /// analyzers don't analyze. The registration methods are CSLA's own API, so the
  /// shapes are known: every <c>RegisterNotNullProperty</c> overload with three
  /// arguments takes a default value, and so does the four-argument
  /// <c>NotNullPropertyInfo&lt;T&gt;</c> constructor and <c>CreateNotNull</c>
  /// factory method. Any other shape is unknown, and the analyzers stay silent.
  /// </remarks>
  internal static class NotNullPropertyRegistrations
  {
    internal const string NotNullPropertyInfoName = "NotNullPropertyInfo";
    internal const string RegisterNotNullPropertyName = "RegisterNotNullProperty";
    private const string CreateNotNullName = "CreateNotNull";
    private const string DefaultValueParameterName = "defaultValue";
    private const string PropertySuffix = "Property";
    // RegisterNotNullProperty(string propertyName, ...) and (Expression propertyLambdaExpression, ...)
    private static readonly string[] RegisterNameParameterNames = ["propertyName", "propertyLambdaExpression"];
    // new NotNullPropertyInfo<T>(string name, ...) and CreateNotNull<T>(Type containingType, string name, ...)
    private static readonly string[] InfoNameParameterNames = ["name"];

    /// <summary>
    /// Gets whether a type is <c>Csla.NotNullPropertyInfo&lt;T&gt;</c> or derives from it.
    /// </summary>
    internal static bool IsNotNullPropertyInfo(ITypeSymbol? type, [NotNullWhen(true)] out ITypeSymbol? valueType)
    {
      for (var current = type as INamedTypeSymbol; current is not null; current = current.BaseType)
      {
        if (current.Name == NotNullPropertyInfoName && current.TypeArguments.Length == 1 &&
          current.ContainingAssembly?.Name == CslaMemberConstants.AssemblyName)
        {
          valueType = current.TypeArguments[0];
          return true;
        }
      }
      valueType = null;
      return false;
    }

    /// <summary>
    /// Gets whether a type is <c>Csla.NotNullPropertyInfo&lt;T&gt;</c> or derives from it.
    /// </summary>
    internal static bool IsNotNullPropertyInfo(ITypeSymbol? type) => IsNotNullPropertyInfo(type, out _);

    /// <summary>
    /// Gets whether a field is a <c>NotNullPropertyInfo&lt;T&gt;</c> of a reference type
    /// whose registration is known to have no default value: a property that must be
    /// loaded, and whose getter returns null when it isn't. A registration whose
    /// shape isn't known returns <see langword="false"/>.
    /// </summary>
    internal static bool IsNoDefaultReference(IFieldSymbol field, CancellationToken cancellationToken) =>
      IsNotNullPropertyInfo(field.Type, out var valueType) &&
      valueType.IsReferenceType &&
      TryGetHasDefault(field, cancellationToken, out var hasDefault) &&
      !hasDefault;

    /// <summary>
    /// Reads whether the registration in a field's initializer has a default value.
    /// </summary>
    /// <returns><see langword="false"/> when the registration's shape isn't known.</returns>
    internal static bool TryGetHasDefault(IFieldSymbol field, CancellationToken cancellationToken, out bool hasDefault)
    {
      hasDefault = false;
      return TryGetInitializer(field, cancellationToken, out var initializer) &&
        TryGetHasDefault(initializer, out hasDefault);
    }

    /// <summary>
    /// Gets the name a field's registration gives its property: the
    /// <c>nameof(X)</c>, <c>"X"</c> or <c>x =&gt; x.X</c> argument, or else the
    /// field name without its <c>Property</c> suffix.
    /// </summary>
    internal static string GetRegisteredName(IFieldSymbol field, CancellationToken cancellationToken)
    {
      if (TryGetInitializer(field, cancellationToken, out var initializer) &&
        TryGetRegisteredName(initializer) is { } name)
      {
        return name;
      }
      return field.Name.EndsWith(PropertySuffix, StringComparison.Ordinal) && field.Name.Length > PropertySuffix.Length
        ? field.Name.Substring(0, field.Name.Length - PropertySuffix.Length)
        : field.Name;
    }

    /// <summary>
    /// Gets the field a property-info argument references, ignoring conversions.
    /// </summary>
    internal static IFieldSymbol? GetReferencedField(IOperation? operation) =>
      Unwrap(operation) is IFieldReferenceOperation fieldReference ? fieldReference.Field : null;

    /// <summary>
    /// Removes implicit and explicit conversions and parentheses from an operation.
    /// </summary>
    internal static IOperation? Unwrap(IOperation? operation)
    {
      while (true)
      {
        switch (operation)
        {
          case IConversionOperation conversion:
            operation = conversion.Operand;
            break;
          case IParenthesizedOperation parenthesized:
            operation = parenthesized.Operand;
            break;
          default:
            return operation;
        }
      }
    }

    private static bool TryGetInitializer(IFieldSymbol field, CancellationToken cancellationToken, [NotNullWhen(true)] out ExpressionSyntax? initializer)
    {
      initializer = null;
      if (field.DeclaringSyntaxReferences.Length == 1 &&
        field.DeclaringSyntaxReferences[0].GetSyntax(cancellationToken) is VariableDeclaratorSyntax { Initializer.Value: { } value })
      {
        initializer = value;
      }
      return initializer is not null;
    }

    private static bool TryGetHasDefault(ExpressionSyntax expression, out bool hasDefault)
    {
      hasDefault = false;
      switch (Unwrap(expression))
      {
        case InvocationExpressionSyntax invocation when GetMethodName(invocation.Expression) == RegisterNotNullPropertyName:
          {
            var arguments = invocation.ArgumentList.Arguments;
            if (GetInfoArgument(arguments) is { } info)
            {
              return TryGetHasDefault(info, out hasDefault);
            }
            if (HasNamedArgument(arguments, DefaultValueParameterName) || arguments.Count >= 3)
            {
              hasDefault = true;
              return true;
            }
            // (name) and (name, friendlyName): only when the name argument is
            // clearly a name, not a variable holding a property info.
            return GetNameArgument(arguments, 0, RegisterNameParameterNames) is { } nameArgument &&
              IsNameArgument(Unwrap(nameArgument));
          }
        case InvocationExpressionSyntax invocation when GetMethodName(invocation.Expression) == CreateNotNullName:
          hasDefault = HasNamedArgument(invocation.ArgumentList.Arguments, DefaultValueParameterName) ||
            invocation.ArgumentList.Arguments.Count >= 4;
          return true;
        case BaseObjectCreationExpressionSyntax creation when IsNotNullPropertyInfoCreation(creation):
          var creationArguments = creation.ArgumentList?.Arguments ?? default;
          hasDefault = HasNamedArgument(creationArguments, DefaultValueParameterName) || creationArguments.Count >= 4;
          return true;
        default:
          return false;
      }
    }

    private static string? TryGetRegisteredName(ExpressionSyntax expression)
    {
      ExpressionSyntax? nameArgument;
      switch (Unwrap(expression))
      {
        case InvocationExpressionSyntax invocation when GetMethodName(invocation.Expression) == RegisterNotNullPropertyName:
          var arguments = invocation.ArgumentList.Arguments;
          if (GetInfoArgument(arguments) is { } info)
          {
            return TryGetRegisteredName(info);
          }
          nameArgument = GetNameArgument(arguments, 0, RegisterNameParameterNames);
          break;
        case InvocationExpressionSyntax invocation when GetMethodName(invocation.Expression) == CreateNotNullName:
          // CreateNotNull(containingType, name, ...)
          nameArgument = GetNameArgument(invocation.ArgumentList.Arguments, 1, InfoNameParameterNames);
          break;
        case BaseObjectCreationExpressionSyntax creation when IsNotNullPropertyInfoCreation(creation):
          nameArgument = GetNameArgument(creation.ArgumentList?.Arguments ?? default, 0, InfoNameParameterNames);
          break;
        default:
          return null;
      }
      return nameArgument is null ? null : GetName(Unwrap(nameArgument));
    }

    /// <summary>
    /// Gets the argument that holds the property name: the one named after the
    /// name parameter when named arguments are used (they can come in any
    /// order), or else the one at the name parameter's position.
    /// </summary>
    private static ExpressionSyntax? GetNameArgument(SeparatedSyntaxList<ArgumentSyntax> arguments, int position, string[] parameterNames)
    {
      foreach (var argument in arguments)
      {
        if (argument.NameColon is { } nameColon && Array.IndexOf(parameterNames, nameColon.Name.Identifier.ValueText) >= 0)
        {
          return argument.Expression;
        }
      }
      // positional arguments come before named ones
      return position < arguments.Count && arguments[position].NameColon is null
        ? arguments[position].Expression
        : null;
    }

    private static string? GetName(ExpressionSyntax expression)
    {
      switch (expression)
      {
        case LiteralExpressionSyntax literal when literal.IsKind(SyntaxKind.StringLiteralExpression):
          return literal.Token.ValueText;
        case InvocationExpressionSyntax { Expression: IdentifierNameSyntax { Identifier.ValueText: "nameof" } } nameOf
          when nameOf.ArgumentList.Arguments.Count == 1:
          return GetLastName(nameOf.ArgumentList.Arguments[0].Expression);
        case LambdaExpressionSyntax { Body: ExpressionSyntax body }:
          return GetLastName(Unwrap(body));
        default:
          return null;
      }
    }

    private static string? GetLastName(ExpressionSyntax expression) =>
      expression switch
      {
        IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
        MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.ValueText,
        _ => null
      };

    private static bool IsNameArgument(ExpressionSyntax expression) =>
      expression is LiteralExpressionSyntax or InterpolatedStringExpressionSyntax or LambdaExpressionSyntax ||
      expression is InvocationExpressionSyntax { Expression: IdentifierNameSyntax { Identifier.ValueText: "nameof" } };

    /// <summary>
    /// Gets the property info created in place for <c>RegisterNotNullProperty(info)</c> and
    /// <c>RegisterNotNullProperty(typeof(T), info)</c>. Other positions hold a name, a
    /// friendly name or a default value, and a default value can be a <c>new</c> expression too.
    /// </summary>
    private static ExpressionSyntax? GetInfoArgument(SeparatedSyntaxList<ArgumentSyntax> arguments)
    {
      var candidate = arguments.Count switch
      {
        1 => arguments[0].Expression,
        2 when Unwrap(arguments[0].Expression) is TypeOfExpressionSyntax => arguments[1].Expression,
        _ => null
      };
      return candidate is not null && IsInfoCreation(Unwrap(candidate)) ? candidate : null;
    }

    private static bool IsInfoCreation(ExpressionSyntax expression) =>
      expression is BaseObjectCreationExpressionSyntax ||
      expression is InvocationExpressionSyntax invocation && GetMethodName(invocation.Expression) == CreateNotNullName;

    private static bool IsNotNullPropertyInfoCreation(BaseObjectCreationExpressionSyntax creation) =>
      creation switch
      {
        ImplicitObjectCreationExpressionSyntax => true,
        ObjectCreationExpressionSyntax { Type: var type } => GetTypeName(type) == NotNullPropertyInfoName,
        _ => false
      };

    private static string? GetTypeName(TypeSyntax type) =>
      type switch
      {
        QualifiedNameSyntax qualified => GetTypeName(qualified.Right),
        AliasQualifiedNameSyntax aliasQualified => GetTypeName(aliasQualified.Name),
        SimpleNameSyntax simple => simple.Identifier.ValueText,
        _ => null
      };

    private static string? GetMethodName(ExpressionSyntax expression) =>
      expression switch
      {
        MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.ValueText,
        MemberBindingExpressionSyntax memberBinding => memberBinding.Name.Identifier.ValueText,
        SimpleNameSyntax simple => simple.Identifier.ValueText,
        _ => null
      };

    private static bool HasNamedArgument(SeparatedSyntaxList<ArgumentSyntax> arguments, string name)
    {
      foreach (var argument in arguments)
      {
        if (argument.NameColon?.Name.Identifier.ValueText == name)
        {
          return true;
        }
      }
      return false;
    }

    private static ExpressionSyntax Unwrap(ExpressionSyntax expression)
    {
      while (true)
      {
        switch (expression)
        {
          case ParenthesizedExpressionSyntax parenthesized:
            expression = parenthesized.Expression;
            break;
          case PostfixUnaryExpressionSyntax postfix when postfix.IsKind(SyntaxKind.SuppressNullableWarningExpression):
            expression = postfix.Operand;
            break;
          case CastExpressionSyntax cast:
            expression = cast.Expression;
            break;
          default:
            return expression;
        }
      }
    }
  }
}
