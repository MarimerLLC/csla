using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace Csla.Analyzers
{
  /// <summary>
  /// Rewrites <c>PropertyInfo&lt;T&gt;</c> registrations reported by
  /// <see cref="MigrateToNotNullPropertyAnalyzer"/> as not-null registrations.
  /// </summary>
  /// <remarks>
  /// Per field: the field type becomes <c>NotNullPropertyInfo&lt;T&gt;</c>, the call
  /// becomes <c>RegisterNotNullProperty</c> without an explicit <c>RelationshipTypes.None</c>,
  /// a <c>string</c> gets <c>string.Empty</c> as its default value (in place of a missing or
  /// null default), and another reference type drops a null default. In the same document,
  /// the <c>!</c> after <c>GetProperty(field)</c> and <c>ReadProperty(field)</c> is removed;
  /// the typed not-null overloads return a non-null value.
  /// </remarks>
  internal static class MigrateToNotNullPropertyRewriter
  {
    private const string NotNullPropertyInfoName = "NotNullPropertyInfo";
    private const string DefaultValueParameter = "defaultValue";

    /// <summary>
    /// Migrates the fields reported by <paramref name="diagnostics"/> in a document.
    /// </summary>
    internal static async Task<Document> MigrateAsync(Document document, IEnumerable<Diagnostic> diagnostics,
      CancellationToken cancellationToken)
    {
      var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
      var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
      if (root is null || semanticModel is null)
      {
        return document;
      }

      var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
      var fields = new HashSet<IFieldSymbol>(SymbolEqualityComparer.Default);
      var declarators = new List<VariableDeclaratorSyntax>();
      foreach (var diagnostic in diagnostics)
      {
        if (root.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<VariableDeclaratorSyntax>() is { } declarator &&
          semanticModel.GetDeclaredSymbol(declarator, cancellationToken) is IFieldSymbol field &&
          !fields.Contains(field) &&
          MigrateField(editor, semanticModel, declarator, cancellationToken))
        {
          fields.Add(field);
          declarators.Add(declarator);
        }
      }

      RemoveSuppressions(editor, root, semanticModel, fields, declarators, cancellationToken);
      return editor.GetChangedDocument();
    }

    private static bool MigrateField(DocumentEditor editor, SemanticModel semanticModel, VariableDeclaratorSyntax declarator,
      CancellationToken cancellationToken)
    {
      if (declarator is not { Parent: VariableDeclarationSyntax declaration, Initializer.Value: { } value } ||
        GetGenericName(declaration.Type) is not { Identifier.ValueText: NotNullMigration.PropertyInfoName } typeName ||
        Unwrap(value) is not InvocationExpressionSyntax invocation ||
        !NotNullMigration.TryRead(semanticModel.GetOperation(invocation, cancellationToken), out var registration))
      {
        return false;
      }

      editor.ReplaceNode(typeName, typeName.WithIdentifier(
        SyntaxFactory.Identifier(NotNullPropertyInfoName).WithTriviaFrom(typeName.Identifier)));
      editor.ReplaceNode(invocation, RewriteInvocation(invocation, registration));
      return true;
    }

    private static InvocationExpressionSyntax RewriteInvocation(InvocationExpressionSyntax invocation, MigrationRegistration registration)
    {
      var arguments = invocation.ArgumentList.Arguments;
      // indexes into the original list: nodes of an edited list are new nodes
      var relationshipIndex = registration.Relationship?.Syntax is ArgumentSyntax relationship ? arguments.IndexOf(relationship) : -1;
      var defaultIndex = registration.DefaultValue?.Syntax is ArgumentSyntax defaultValue ? arguments.IndexOf(defaultValue) : -1;

      var removals = new List<int>();
      if (relationshipIndex >= 0)
      {
        removals.Add(relationshipIndex);
      }
      if (defaultIndex >= 0 && registration.HasNullDefault)
      {
        if (registration.IsString)
        {
          var current = arguments[defaultIndex];
          arguments = arguments.Replace(current, current.WithExpression(StringEmpty().WithTriviaFrom(current.Expression)));
        }
        else
        {
          removals.Add(defaultIndex);
        }
      }
      foreach (var index in removals.OrderByDescending(_ => _))
      {
        arguments = arguments.RemoveAt(index);
      }

      if (defaultIndex < 0 && registration.IsString)
      {
        if (arguments.Any(_ => _.NameColon is not null))
        {
          arguments = arguments.Add(SyntaxFactory.Argument(
            SyntaxFactory.NameColon(DefaultValueParameter), default, StringEmpty()));
        }
        else
        {
          if (registration.FriendlyName is null)
          {
            arguments = arguments.Add(SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
          }
          arguments = arguments.Add(SyntaxFactory.Argument(StringEmpty()));
        }
      }

      return invocation
        .WithExpression(RenameMethod(invocation.Expression))
        .WithArgumentList(invocation.ArgumentList.WithArguments(arguments));
    }

    private static void RemoveSuppressions(DocumentEditor editor, SyntaxNode root, SemanticModel semanticModel,
      HashSet<IFieldSymbol> fields, List<VariableDeclaratorSyntax> declarators, CancellationToken cancellationToken)
    {
      if (fields.Count == 0)
      {
        return;
      }

      foreach (var suppression in root.DescendantNodes().OfType<PostfixUnaryExpressionSyntax>())
      {
        if (!suppression.IsKind(SyntaxKind.SuppressNullableWarningExpression) ||
          Unwrap(suppression.Operand) is not InvocationExpressionSyntax { ArgumentList.Arguments.Count: 1 } call ||
          GetMethodName(call.Expression) is not (CslaMemberConstants.Properties.GetProperty or CslaMemberConstants.Properties.ReadProperty) ||
          declarators.Any(_ => _.Span.Contains(suppression.Span)) ||
          semanticModel.GetSymbolInfo(call.ArgumentList.Arguments[0].Expression, cancellationToken).Symbol is not IFieldSymbol field ||
          !fields.Contains(field.OriginalDefinition))
        {
          continue;
        }

        editor.ReplaceNode(suppression, suppression.Operand.WithTrailingTrivia(suppression.GetTrailingTrivia()));
      }
    }

    private static ExpressionSyntax StringEmpty() => SyntaxFactory.ParseExpression("string.Empty");

    private static ExpressionSyntax RenameMethod(ExpressionSyntax expression) =>
      expression switch
      {
        MemberAccessExpressionSyntax memberAccess => memberAccess.WithName(Rename(memberAccess.Name)),
        SimpleNameSyntax name => Rename(name),
        _ => expression
      };

    private static SimpleNameSyntax Rename(SimpleNameSyntax name) =>
      name.WithIdentifier(SyntaxFactory.Identifier(NotNullPropertyRegistrations.RegisterNotNullPropertyName).WithTriviaFrom(name.Identifier));

    private static GenericNameSyntax? GetGenericName(TypeSyntax type) =>
      type switch
      {
        GenericNameSyntax generic => generic,
        QualifiedNameSyntax qualified => GetGenericName(qualified.Right),
        AliasQualifiedNameSyntax aliasQualified => GetGenericName(aliasQualified.Name),
        _ => null
      };

    private static string? GetMethodName(ExpressionSyntax expression) =>
      expression switch
      {
        MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.ValueText,
        SimpleNameSyntax name => name.Identifier.ValueText,
        _ => null
      };

    private static ExpressionSyntax Unwrap(ExpressionSyntax expression)
    {
      while (expression is ParenthesizedExpressionSyntax parenthesized)
      {
        expression = parenthesized.Expression;
      }
      return expression;
    }
  }
}
