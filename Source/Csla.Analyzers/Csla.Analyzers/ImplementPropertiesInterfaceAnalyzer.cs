using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  /// <summary>
  /// Reports a <c>[CslaImplementPropertiesInterface&lt;T&gt;]</c> attribute whose type
  /// argument is not an interface declared in the same project, so the source
  /// generator can't implement its properties (CSLA0030).
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

        string? reason = null;
        if (typeArgument.TypeKind != TypeKind.Interface)
          reason = nameof(Resources.ImplementPropertiesInterfaceAnalyzer_NotInterface);
        else if (!IsDeclaredInCompilation(typeArgument, context.Compilation, context.CancellationToken))
          reason = nameof(Resources.ImplementPropertiesInterfaceAnalyzer_NotInSource);

        if (reason is null)
          continue;

        var typeArgumentName = typeArgument.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
        var location = attribute.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken).GetLocation() ?? type.Locations[0];
        context.ReportDiagnostic(Diagnostic.Create(rule, location,
          type.Name,
          new LocalizableResourceString(reason, Resources.ResourceManager, typeof(Resources), typeArgumentName)));
      }
    }

    /// <summary>
    /// The generator reads the interface's properties from its declaration, so it
    /// must be declared in the compilation being built.
    /// </summary>
    private static bool IsDeclaredInCompilation(ITypeSymbol interfaceType, Compilation compilation, CancellationToken cancellationToken)
      => interfaceType.DeclaringSyntaxReferences.Any(r =>
        compilation.ContainsSyntaxTree(r.SyntaxTree) && r.GetSyntax(cancellationToken) is InterfaceDeclarationSyntax);
  }
}
