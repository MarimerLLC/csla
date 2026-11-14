//-----------------------------------------------------------------------
// <copyright file="AttributeArgumentQualifier.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Qualify the names in an attribute argument so it binds in generated code</summary>
//-----------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Csla.Generator.AutoImplementProperties.CSharp.AutoImplement.Discovery
{
  /// <summary>
  /// Rewrites an attribute argument expression so that the types, namespaces and static
  /// members it names are fully qualified. The argument is copied into a generated file
  /// that doesn't have the using directives of the file it was written in.
  /// </summary>
  internal sealed class AttributeArgumentQualifier(SemanticModel semanticModel) : CSharpSyntaxRewriter
  {
    /// <summary>
    /// Get the text of an attribute argument expression with its names fully qualified
    /// </summary>
    public string Qualify(ExpressionSyntax expression) => Visit(expression).ToString();

    /// <inheritdoc />
    public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
    {
      // A nameof expression is replaced by its value: qualifying its operand could change
      // the value, as nameof(Alias) is the alias name, not the name of the aliased type
      if (node.Expression is IdentifierNameSyntax { Identifier.Text: "nameof" }
        && semanticModel.GetConstantValue(node) is { HasValue: true, Value: string value })
      {
        return SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(value)).WithTriviaFrom(node);
      }
      return base.VisitInvocationExpression(node);
    }

    /// <inheritdoc />
    public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
      => QualifyName(node, allowStaticMember: true) ?? node;

    /// <inheritdoc />
    public override SyntaxNode? VisitGenericName(GenericNameSyntax node)
      => QualifyName(node, allowStaticMember: true) ?? base.VisitGenericName(node);

    /// <inheritdoc />
    public override SyntaxNode? VisitQualifiedName(QualifiedNameSyntax node)
      // The name on the right is resolved relative to the left, so only the left is rewritten
      => QualifyName(node, allowStaticMember: false) ?? node.WithLeft((NameSyntax)Visit(node.Left));

    /// <inheritdoc />
    public override SyntaxNode? VisitAliasQualifiedName(AliasQualifiedNameSyntax node)
      => QualifyName(node, allowStaticMember: false) ?? node;

    /// <inheritdoc />
    public override SyntaxNode? VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
      // The member name on the right is resolved relative to the expression on the left
      => node.WithExpression((ExpressionSyntax)Visit(node.Expression));

    private SyntaxNode? QualifyName(NameSyntax node, bool allowStaticMember)
    {
      var symbolInfo = semanticModel.GetSymbolInfo(node);
      var symbol = symbolInfo.Symbol ?? (symbolInfo.CandidateSymbols.Length == 1 ? symbolInfo.CandidateSymbols[0] : null);
      ExpressionSyntax? qualified = symbol switch
      {
        INamespaceSymbol { IsGlobalNamespace: false } ns => SyntaxFactory.ParseName($"global::{ns.ToDisplayString()}"),
        ITypeSymbol type when type.TypeKind is not (TypeKind.Error or TypeKind.TypeParameter) => SyntaxFactory.ParseTypeName(GetTypeName(type)),
        // A static member named without its type, such as a constant declared in the interface
        { IsStatic: true, ContainingType: { } containingType } when allowStaticMember && symbol is IFieldSymbol or IPropertySymbol or IMethodSymbol or IEventSymbol
          => SyntaxFactory.ParseExpression($"{GetTypeName(containingType)}.{((SimpleNameSyntax)node).Identifier.Text}"),
        _ => null
      };
      return qualified?.WithTriviaFrom(node);
    }

    private static string GetTypeName(ITypeSymbol type)
      => type is INamedTypeSymbol named && IsUnbound(named)
        ? GetUnboundTypeName(named)
        : type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    /// <summary>
    /// Whether the type is, or is nested in, an unbound generic type, as in
    /// typeof(List&lt;&gt;) or typeof(Outer&lt;&gt;.Inner)
    /// </summary>
    private static bool IsUnbound(INamedTypeSymbol type)
      => type.IsUnboundGenericType || (type.ContainingType is { } containingType && IsUnbound(containingType));

    /// <summary>
    /// Build the name of an unbound generic type one level at a time, since the display
    /// format would show its type parameters, such as List&lt;T&gt;, where it needs List&lt;&gt;
    /// </summary>
    private static string GetUnboundTypeName(INamedTypeSymbol type)
    {
      var prefix = type.ContainingType is { } containingType
        ? $"{GetUnboundTypeName(containingType)}."
        : type.ContainingNamespace is { IsGlobalNamespace: false } ns ? $"global::{ns.ToDisplayString()}." : "global::";
      var name = SyntaxFacts.GetKeywordKind(type.Name) == SyntaxKind.None ? type.Name : $"@{type.Name}";
      var typeArguments = type.Arity > 0 ? $"<{new string(',', type.Arity - 1)}>" : string.Empty;
      return $"{prefix}{name}{typeArguments}";
    }
  }
}
