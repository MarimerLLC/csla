using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  /// <summary>
  /// Reports a <c>[CslaImplementPropertiesInterface&lt;T&gt;]</c> attribute that the source
  /// generator can't implement properties for, because it isn't on a partial class or
  /// struct, or its type argument is not an interface declared in the same project (CSLA0030).
  /// </summary>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class ImplementPropertiesInterfaceAnalyzer
    : DiagnosticAnalyzer
  {
    private const string AttributeMetadataName = "Csla.CslaImplementPropertiesInterfaceAttribute`1";

    private static readonly DiagnosticDescriptor rule =
      new(
        Constants.AnalyzerIdentifiers.InvalidImplementPropertiesInterface, ImplementPropertiesInterfaceAnalyzerConstants.Title,
        ImplementPropertiesInterfaceAnalyzerConstants.Message, Constants.Categories.Usage,
        DiagnosticSeverity.Warning, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.InvalidImplementPropertiesInterface, nameof(ImplementPropertiesInterfaceAnalyzer)));

    /// <summary>
    ///
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [rule];

    /// <summary>
    ///
    /// </summary>
    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();
      context.RegisterCompilationStartAction(startContext =>
      {
        var attributeType = startContext.Compilation.GetTypeByMetadataName(AttributeMetadataName);
        if (attributeType is null)
          return;

        startContext.RegisterSymbolAction(c => AnalyzeType(c, attributeType), SymbolKind.NamedType);
      });
    }

    private static void AnalyzeType(SymbolAnalysisContext context, INamedTypeSymbol attributeType)
    {
      var type = (INamedTypeSymbol)context.Symbol;
      foreach (var attribute in type.GetAttributes())
      {
        if (!SymbolEqualityComparer.Default.Equals(attribute.AttributeClass?.OriginalDefinition, attributeType))
          continue;

        var typeArgument = attribute.AttributeClass!.TypeArguments.FirstOrDefault();
        // An unresolved type argument already has a compiler error
        if (typeArgument is null || typeArgument.TypeKind == TypeKind.Error)
          continue;

        var attributeSyntax = attribute.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken);
        var typeArgumentName = typeArgument.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
        LocalizableResourceString? reason = null;
        if (attributeSyntax?.Parent?.Parent is TypeDeclarationSyntax declaration && !IsGeneratedTarget(declaration))
          reason = Reason(nameof(Resources.ImplementPropertiesInterfaceAnalyzer_NotPartialClassOrStruct));
        else if (typeArgument.TypeKind != TypeKind.Interface)
          reason = Reason(nameof(Resources.ImplementPropertiesInterfaceAnalyzer_NotInterface), typeArgumentName);
        else if (!IsDeclaredInCompilation(typeArgument, context.Compilation, context.CancellationToken))
          reason = Reason(nameof(Resources.ImplementPropertiesInterfaceAnalyzer_NotInSource), typeArgumentName);

        if (reason is null)
          continue;

        var location = attributeSyntax?.GetLocation() ?? type.Locations[0];
        context.ReportDiagnostic(Diagnostic.Create(rule, location, type.Name, reason));
      }
    }

    private static LocalizableResourceString Reason(string name, params string[] arguments)
      => new(name, Resources.ResourceManager, typeof(Resources), arguments);

    // These rules mirror what IncrementalAutoImplementInterfacePartialsGenerator and
    // TypeDefinitionExtractor.ExtractTypeDefinitionForInterfaces accept; keep them in step.

    /// <summary>
    /// The generator only implements properties on a partial class or struct.
    /// </summary>
    private static bool IsGeneratedTarget(TypeDeclarationSyntax declaration)
      => declaration is ClassDeclarationSyntax or StructDeclarationSyntax
        && declaration.Modifiers.Any(SyntaxKind.PartialKeyword);

    /// <summary>
    /// The generator reads the interface's properties from its declaration, so it
    /// must be declared in the compilation being built.
    /// </summary>
    private static bool IsDeclaredInCompilation(ITypeSymbol interfaceType, Compilation compilation, CancellationToken cancellationToken)
      => interfaceType.DeclaringSyntaxReferences.Any(r =>
        compilation.ContainsSyntaxTree(r.SyntaxTree) && r.GetSyntax(cancellationToken) is InterfaceDeclarationSyntax);
  }
}
