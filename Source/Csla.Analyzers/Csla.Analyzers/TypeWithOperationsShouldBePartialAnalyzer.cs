using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  /// <summary>
  /// Reports classes that declare data portal operation methods
  /// (methods with <c>[Create]</c>, <c>[Fetch]</c>, etc.) when the
  /// class or any of its containing types is not <c>partial</c>.
  /// The CSLA source generator needs to add a partial declaration
  /// to such types to implement their operations interface.
  /// </summary>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class TypeWithOperationsShouldBePartialAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly string[] OperationAttributeMetadataNames =
    [
      "Csla.CreateAttribute",
      "Csla.FetchAttribute",
      "Csla.InsertAttribute",
      "Csla.UpdateAttribute",
      "Csla.ExecuteAttribute",
      "Csla.DeleteAttribute",
      "Csla.DeleteSelfAttribute",
      "Csla.CreateChildAttribute",
      "Csla.FetchChildAttribute",
      "Csla.InsertChildAttribute",
      "Csla.UpdateChildAttribute",
      "Csla.DeleteSelfChildAttribute",
      "Csla.ExecuteChildAttribute"
    ];

    private static readonly DiagnosticDescriptor shouldBePartialRule =
      new(
        Constants.AnalyzerIdentifiers.TypeWithOperationsShouldBePartial, TypeWithOperationsShouldBePartialAnalyzerConstants.Title,
        TypeWithOperationsShouldBePartialAnalyzerConstants.Message, Constants.Categories.Usage,
        DiagnosticSeverity.Warning, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.TypeWithOperationsShouldBePartial, nameof(TypeWithOperationsShouldBePartialAnalyzer)));

    /// <summary>
    ///
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [shouldBePartialRule];

    /// <summary>
    ///
    /// </summary>
    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();
      context.RegisterCompilationStartAction(compilationContext =>
      {
        var operationAttributes = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        foreach (var metadataName in OperationAttributeMetadataNames)
        {
          var attributeType = compilationContext.Compilation.GetTypeByMetadataName(metadataName);
          if (attributeType is not null)
          {
            operationAttributes.Add(attributeType);
          }
        }

        if (operationAttributes.Count == 0)
        {
          return;
        }

        compilationContext.RegisterSymbolAction(
          symbolContext => AnalyzeNamedType(symbolContext, operationAttributes), SymbolKind.NamedType);
      });
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context, HashSet<INamedTypeSymbol> operationAttributes)
    {
      var typeSymbol = (INamedTypeSymbol)context.Symbol;
      if (typeSymbol.TypeKind != TypeKind.Class || typeSymbol.IsRecord)
      {
        return;
      }

      if (!HasOperationMethods(typeSymbol, operationAttributes))
      {
        return;
      }

      foreach (var reference in typeSymbol.DeclaringSyntaxReferences)
      {
        context.CancellationToken.ThrowIfCancellationRequested();

        if (reference.GetSyntax(context.CancellationToken) is ClassDeclarationSyntax classNode &&
          GetNonPartialTypeDeclarations(classNode).Count > 0)
        {
          context.ReportDiagnostic(Diagnostic.Create(
            shouldBePartialRule, classNode.Identifier.GetLocation(), typeSymbol.Name));
          return;
        }
      }
    }

    private static bool HasOperationMethods(INamedTypeSymbol typeSymbol, HashSet<INamedTypeSymbol> operationAttributes)
    {
      foreach (var member in typeSymbol.GetMembers())
      {
        if (member is IMethodSymbol method)
        {
          foreach (var attribute in method.GetAttributes())
          {
            if (attribute.AttributeClass is not null && operationAttributes.Contains(attribute.AttributeClass))
            {
              return true;
            }
          }
        }
      }

      return false;
    }

    /// <summary>
    /// Gets the given type declaration and all of its containing
    /// type declarations that are not declared <c>partial</c>.
    /// </summary>
    internal static List<TypeDeclarationSyntax> GetNonPartialTypeDeclarations(TypeDeclarationSyntax typeNode)
    {
      var result = new List<TypeDeclarationSyntax>();
      foreach (var node in typeNode.AncestorsAndSelf().OfType<TypeDeclarationSyntax>())
      {
        if (!node.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
          result.Add(node);
        }
      }

      return result;
    }
  }
}
