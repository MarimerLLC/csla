//-----------------------------------------------------------------------
// <copyright file="NotNullMigration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Reads PropertyInfo registrations that can migrate to RegisterNotNullProperty</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Csla.Analyzers
{
  /// <summary>
  /// A <c>RegisterProperty&lt;T&gt;</c> call that has an exact
  /// <c>RegisterNotNullProperty&lt;T&gt;</c> twin, read by parameter name.
  /// </summary>
  internal sealed class MigrationRegistration
  {
    internal MigrationRegistration(IInvocationOperation invocation, ITypeSymbol valueType, string? propertyName,
      IArgumentOperation? friendlyName, IArgumentOperation? defaultValue, IArgumentOperation? relationship)
    {
      Invocation = invocation;
      ValueType = valueType;
      PropertyName = propertyName;
      FriendlyName = friendlyName;
      DefaultValue = defaultValue;
      Relationship = relationship;
    }

    /// <summary>The <c>RegisterProperty</c> call.</summary>
    public IInvocationOperation Invocation { get; }
    /// <summary>The property type <c>T</c>.</summary>
    public ITypeSymbol ValueType { get; }
    /// <summary>The registered property name, when it is a constant or a lambda member.</summary>
    public string? PropertyName { get; }
    /// <summary>The <c>friendlyName</c> argument, if any.</summary>
    public IArgumentOperation? FriendlyName { get; }
    /// <summary>The <c>defaultValue</c> argument, if any.</summary>
    public IArgumentOperation? DefaultValue { get; }
    /// <summary>The <c>relationship</c> argument (always <c>RelationshipTypes.None</c>), if any.</summary>
    public IArgumentOperation? Relationship { get; }

    /// <summary>Gets whether the default value argument is the constant <see langword="null"/>.</summary>
    public bool HasNullDefault => DefaultValue?.Value.ConstantValue is { HasValue: true, Value: null };

    /// <summary>Gets whether <c>T</c> is <see cref="string"/>.</summary>
    public bool IsString => ValueType.SpecialType == SpecialType.System_String;
  }

  /// <summary>
  /// The rules for migrating <c>PropertyInfo&lt;T&gt;</c> registrations to
  /// <c>RegisterNotNullProperty</c>, shared by the analyzer and its code fixes.
  /// </summary>
  internal static class NotNullMigration
  {
    internal const string PropertyInfoName = "PropertyInfo";
    internal const string RegisterPropertyName = "RegisterProperty";
    private const string PropertyNameParameter = "propertyName";
    private const string PropertyLambdaParameter = "propertyLambdaExpression";
    private const string FriendlyNameParameter = "friendlyName";
    private const string DefaultValueParameter = "defaultValue";
    private const string RelationshipParameter = "relationship";

    // the base classes whose RegisterProperty overloads all have a RegisterNotNullProperty twin
    private static readonly string[] MigratableBaseTypes =
      ["BusinessBase", "ReadOnlyBase", "CommandBase", "BusinessDocumentBase"];

    /// <summary>
    /// Gets whether a type is exactly <c>Csla.PropertyInfo&lt;T&gt;</c>.
    /// </summary>
    internal static bool IsPropertyInfo(ITypeSymbol? type) =>
      type is INamedTypeSymbol { Name: PropertyInfoName, TypeArguments.Length: 1 } named && named.IsDeclaredInCsla();

    /// <summary>
    /// Reads a field initializer value that is a <c>RegisterProperty&lt;T&gt;(...)</c> call
    /// in a name or lambda form, whose relationship (if any) is <c>RelationshipTypes.None</c>.
    /// </summary>
    internal static bool TryRead(IOperation? value, [NotNullWhen(true)] out MigrationRegistration? registration)
    {
      registration = null;
      if (NotNullPropertyRegistrations.Unwrap(value) is not IInvocationOperation invocation)
      {
        return false;
      }

      var method = invocation.TargetMethod;
      if (method.Name != RegisterPropertyName || method.TypeArguments.Length != 1 ||
        !method.ContainingType.IsDeclaredInCsla() ||
        Array.IndexOf(MigratableBaseTypes, method.ContainingType.Name) < 0)
      {
        return false;
      }

      IArgumentOperation? name = null, friendlyName = null, defaultValue = null, relationship = null;
      string? propertyName = null;
      foreach (var argument in invocation.Arguments)
      {
        switch (argument.Parameter?.Name)
        {
          case PropertyNameParameter:
            name = argument;
            propertyName = argument.Value.ConstantValue is { HasValue: true, Value: string text } ? text : null;
            break;
          case PropertyLambdaParameter:
            name = argument;
            propertyName = GetLambdaMemberName(argument);
            break;
          case FriendlyNameParameter:
            friendlyName = argument;
            break;
          case DefaultValueParameter:
            defaultValue = argument;
            break;
          case RelationshipParameter:
            // LazyLoad and PrivateField have no not-null form
            if (argument.Value.ConstantValue is not { HasValue: true, Value: 0 })
              return false;
            relationship = argument;
            break;
          default:
            return false; // an info-object form
        }
      }
      if (name is null)
      {
        return false;
      }

      registration = new MigrationRegistration(invocation, method.TypeArguments[0], propertyName, friendlyName, defaultValue, relationship);
      return true;
    }

    /// <summary>
    /// Gets the CLR property a registration belongs to: the property of the
    /// registered name declared in the type, if there is one.
    /// </summary>
    internal static IPropertySymbol? FindProperty(INamedTypeSymbol type, MigrationRegistration registration) =>
      registration.PropertyName is { } name
        ? type.GetMembers(name).OfType<IPropertySymbol>().FirstOrDefault()
        : null;

    private static string? GetLambdaMemberName(IArgumentOperation argument)
    {
      if (argument.Syntax is ArgumentSyntax { Expression: LambdaExpressionSyntax { Body: ExpressionSyntax body } })
      {
        while (body is CastExpressionSyntax cast)
          body = cast.Expression;
        return body switch
        {
          MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.ValueText,
          IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
          _ => null
        };
      }
      return null;
    }
  }
}
