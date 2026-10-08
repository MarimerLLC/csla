using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Csla.Analyzers
{
  /// <summary>
  /// Replaces a call to an untyped, criteria-based data portal method with
  /// the generated extension method, adding a using directive for the
  /// extension method's namespace when it is not in scope.
  /// </summary>
  [ExportCodeFixProvider(LanguageNames.CSharp)]
  [Shared]
  public sealed class UseGeneratedDataPortalExtensionCodeFix
    : CodeFixProvider
  {
    /// <summary>
    ///
    /// </summary>
    public override ImmutableArray<string> FixableDiagnosticIds => [Constants.AnalyzerIdentifiers.UseGeneratedDataPortalExtension];

    /// <summary>
    ///
    /// </summary>
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <summary>
    ///
    /// </summary>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
      var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
      if (root is null)
      {
        return;
      }

      var diagnostic = context.Diagnostics.First();
      if (!diagnostic.Properties.TryGetValue(UseGeneratedDataPortalExtensionAnalyzer.ExtensionNamesProperty, out var extensionNames) ||
        string.IsNullOrEmpty(extensionNames))
      {
        return;
      }

      if (root.FindToken(diagnostic.Location.SourceSpan.Start).Parent is not SimpleNameSyntax name ||
        name.FirstAncestorOrSelf<InvocationExpressionSyntax>() is not { } invocation)
      {
        return;
      }

      var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
      if (semanticModel?.GetSymbolInfo(invocation, context.CancellationToken).Symbol is not IMethodSymbol { ContainingType.TypeArguments: [INamedTypeSymbol businessType] })
      {
        return;
      }

      foreach (var extensionName in extensionNames!.Split([';'], StringSplitOptions.RemoveEmptyEntries))
      {
        var title = string.Format(UseGeneratedDataPortalExtensionCodeFixConstants.UseExtensionDescription, extensionName);
        context.RegisterCodeFix(
          CodeAction.Create(title,
            _ => Task.FromResult(context.Document.WithSyntaxRoot(
              ReplaceCall(root, semanticModel, invocation, name, extensionName, businessType))),
            title),
          diagnostic);
      }
    }

    private static SyntaxNode ReplaceCall(SyntaxNode root, SemanticModel semanticModel, InvocationExpressionSyntax invocation,
      SimpleNameSyntax name, string extensionName, INamedTypeSymbol businessType)
    {
      // Generic type arguments of the untyped method do not apply to the extension method.
      var newName = SyntaxFactory.IdentifierName(extensionName).WithTriviaFrom(name);
      var newInvocation = invocation.ReplaceNode(name, newName);
      var newRoot = root.ReplaceNode(invocation, newInvocation);

      // The extension class is declared in the business type's namespace.
      var @namespace = businessType.ContainingNamespace;
      if (@namespace is null || @namespace.IsGlobalNamespace ||
        semanticModel.GetSpeculativeSymbolInfo(invocation.SpanStart, newInvocation, SpeculativeBindingOption.BindAsExpression).Symbol is not null ||
        newRoot is not CompilationUnitSyntax compilationUnit)
      {
        return newRoot;
      }

      var namespaceName = @namespace.ToDisplayString();
      if (compilationUnit.Usings.Any(u => u.Name?.ToString() == namespaceName))
      {
        return newRoot;
      }

      return compilationUnit.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(namespaceName)));
    }
  }
}
