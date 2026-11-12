using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  /// <summary>
  /// Reports root data portal operation arguments whose types MobileFormatter
  /// cannot serialize. Types with a custom serializer, declared with
  /// <c>[MobileSerializer]</c> or registered with
  /// <c>[assembly: MobileSerializerRegistration]</c> in the compilation or a
  /// referenced assembly, are serializable.
  /// </summary>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class FindOperationsWithNonSerializableArgumentsAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor shouldUseSerializableTypesRule =
      new(
        Constants.AnalyzerIdentifiers.FindOperationsWithNonSerializableArguments, FindOperationsWithNonSerializableArgumentsConstants.Title,
        FindOperationsWithNonSerializableArgumentsConstants.Message, Constants.Categories.Design,
        DiagnosticSeverity.Warning, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.FindOperationsWithNonSerializableArguments, nameof(FindOperationsWithNonSerializableArgumentsAnalyzer)));

    /// <summary>
    ///
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [shouldUseSerializableTypesRule];

    /// <summary>
    ///
    /// </summary>
    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
      context.EnableConcurrentExecution();
      context.RegisterCompilationStartAction(startContext =>
      {
        var compilation = startContext.Compilation;
        var registry = new Lazy<CustomSerializerRegistry>(() => CustomSerializerRegistry.Create(compilation));
        startContext.RegisterSyntaxNodeAction(c => AnalyzeMethodDeclaration(c, registry), SyntaxKind.MethodDeclaration);
      });
    }

    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context, Lazy<CustomSerializerRegistry> registry)
    {
      var methodNode = (MethodDeclarationSyntax)context.Node;
      var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodNode);
      if (methodSymbol is null)
      {
        return;
      }
      var typeSymbol = methodSymbol.ContainingType;

      if (typeSymbol.IsStereotype() && methodSymbol.IsRootDataPortalOperation())
      {
        foreach(var argument in methodSymbol.Parameters)
        {
          var argumentType = argument.Type;

          if (!argumentType.IsMobileObject() && !argumentType.IsSpecialTypeSerializable() &&
              !argumentType.IsSerializableByMobileFormatter(context.Compilation) &&
              !argument.GetAttributes().Any(_ => _.AttributeClass.IsInjectable()) &&
              argumentType is not { ContainingNamespace.Name: "System", Name: "Nullable" } &&
              argumentType is INamedTypeSymbol &&
              !HasCustomSerializer(argumentType, registry))
          {
            context.ReportDiagnostic(Diagnostic.Create(
              shouldUseSerializableTypesRule, argument.Locations[0]));
          }
        }
      }
    }

    /// <summary>
    /// The type itself (not a base type) is marked with <c>[MobileSerializer]</c>
    /// or has a registered custom serializer, matching the exact-type
    /// matching MobileFormatter does at run time.
    /// </summary>
    private static bool HasCustomSerializer(ITypeSymbol type, Lazy<CustomSerializerRegistry> registry)
      => type.GetAttributes().Any(a => MobileSerializerRules.IsCslaMobileType(a.AttributeClass, MobileSerializerRules.MobileSerializerAttributeName))
        || registry.Value.IsRegistered(type);
  }
}
