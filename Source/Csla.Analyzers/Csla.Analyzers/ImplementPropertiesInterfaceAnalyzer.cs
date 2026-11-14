using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  /// <summary>
  /// Reports a <c>[CslaImplementProperties]</c> or <c>[CslaImplementPropertiesInterface&lt;T&gt;]</c>
  /// attribute that the source generator can't implement properties for, because it isn't on
  /// a partial class or struct, the type doesn't inherit a supported CSLA base class, or the
  /// interface attribute's type argument is not an interface declared in the same project (CSLA0030).
  /// </summary>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class ImplementPropertiesInterfaceAnalyzer
    : DiagnosticAnalyzer
  {
    private const string AttributeMetadataName = "Csla.CslaImplementPropertiesInterfaceAttribute`1";
    private const string PropertiesAttributeMetadataName = "Csla.CslaImplementPropertiesAttribute";

    // The base classes in the Csla namespace whose properties the generator implements
    // (BusinessDocumentBase inherits BusinessBase); keep in step with CslaBaseClasses in
    // Csla.Generator.AutoImplementProperties.CSharp
    private static readonly ImmutableHashSet<string> SupportedBaseClassNames =
      ImmutableHashSet.Create("BusinessBase", "ReadOnlyBase", "CommandBase");

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
        var propertiesAttributeType = startContext.Compilation.GetTypeByMetadataName(PropertiesAttributeMetadataName);
        if (attributeType is null && propertiesAttributeType is null)
          return;

        startContext.RegisterSymbolAction(c => AnalyzeType(c, attributeType, propertiesAttributeType), SymbolKind.NamedType);
      });
    }

    private static void AnalyzeType(SymbolAnalysisContext context, INamedTypeSymbol? attributeType, INamedTypeSymbol? propertiesAttributeType)
    {
      var type = (INamedTypeSymbol)context.Symbol;
      foreach (var attribute in type.GetAttributes())
      {
        var isInterfaceAttribute = attributeType is not null
          && SymbolEqualityComparer.Default.Equals(attribute.AttributeClass?.OriginalDefinition, attributeType);
        var isPropertiesAttribute = propertiesAttributeType is not null
          && SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, propertiesAttributeType);
        if (!isInterfaceAttribute && !isPropertiesAttribute)
          continue;

        var typeArgument = attribute.AttributeClass!.TypeArguments.FirstOrDefault();
        // An unresolved type argument already has a compiler error
        if (isInterfaceAttribute && (typeArgument is null || typeArgument.TypeKind == TypeKind.Error))
          continue;

        var attributeSyntax = attribute.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken);
        var typeArgumentName = typeArgument?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat) ?? string.Empty;
        LocalizableResourceString? reason = null;
        if (attributeSyntax?.Parent?.Parent is TypeDeclarationSyntax declaration && !IsGeneratedTarget(declaration))
          reason = Reason(nameof(Resources.ImplementPropertiesInterfaceAnalyzer_NotPartialClassOrStruct));
        else if (isInterfaceAttribute && typeArgument!.TypeKind != TypeKind.Interface)
          reason = Reason(nameof(Resources.ImplementPropertiesInterfaceAnalyzer_NotInterface), typeArgumentName);
        else if (isInterfaceAttribute && !IsDeclaredInCompilation(typeArgument!, context.Compilation, context.CancellationToken))
          reason = Reason(nameof(Resources.ImplementPropertiesInterfaceAnalyzer_NotInSource), typeArgumentName);
        else if (!InheritsSupportedBaseClass(type))
          reason = Reason(nameof(Resources.ImplementPropertiesInterfaceAnalyzer_UnsupportedBaseClass));

        if (reason is null)
          continue;

        var location = attributeSyntax?.GetLocation() ?? type.Locations[0];
        context.ReportDiagnostic(Diagnostic.Create(rule, location, type.Name, reason));
      }
    }

    private static LocalizableResourceString Reason(string name, params string[] arguments)
      => new(name, Resources.ResourceManager, typeof(Resources), arguments);

    // These rules mirror what the IncrementalAutoImplement*PartialsGenerator generators and
    // TypeDefinitionExtractor accept; keep them in step.

    /// <summary>
    /// The generator implements properties only for a type that inherits a supported base
    /// class in the Csla namespace, directly or through intermediate base classes. A base
    /// class that can't be resolved already has a compiler error, so it isn't reported.
    /// </summary>
    private static bool InheritsSupportedBaseClass(INamedTypeSymbol type)
    {
      for (var baseType = type.BaseType; baseType is not null; baseType = baseType.BaseType)
      {
        if (baseType.TypeKind == TypeKind.Error)
          return true;
        if (baseType.ContainingNamespace is { Name: "Csla" } containingNamespace
          && containingNamespace.ContainingNamespace?.IsGlobalNamespace == true
          && SupportedBaseClassNames.Contains(baseType.Name))
          return true;
      }
      return false;
    }

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
