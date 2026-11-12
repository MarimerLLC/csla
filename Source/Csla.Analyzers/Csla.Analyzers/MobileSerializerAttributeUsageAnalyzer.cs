using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  /// <summary>
  /// Verifies custom serializer declarations: <c>[MobileSerializer]</c> on a type,
  /// <c>[MobileSerializerFor]</c> on a serializer and
  /// <c>[assembly: MobileSerializerRegistration]</c>. Reports declarations the
  /// source generator cannot register (CSLA0028) and types that are mapped to
  /// more than one serializer (CSLA0029).
  /// </summary>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class MobileSerializerAttributeUsageAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor invalidDeclarationRule =
      new(
        Constants.AnalyzerIdentifiers.InvalidCustomSerializerDeclaration, MobileSerializerAttributeUsageAnalyzerConstants.InvalidDeclarationTitle,
        MobileSerializerAttributeUsageAnalyzerConstants.InvalidDeclarationMessage, Constants.Categories.Usage,
        DiagnosticSeverity.Error, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.InvalidCustomSerializerDeclaration, nameof(MobileSerializerAttributeUsageAnalyzer)));
    private static readonly DiagnosticDescriptor conflictRule =
      new(
        Constants.AnalyzerIdentifiers.ConflictingCustomSerializers, MobileSerializerAttributeUsageAnalyzerConstants.ConflictTitle,
        MobileSerializerAttributeUsageAnalyzerConstants.ConflictMessage, Constants.Categories.Usage,
        DiagnosticSeverity.Warning, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.ConflictingCustomSerializers, nameof(MobileSerializerAttributeUsageAnalyzer)));

    /// <summary>
    ///
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [invalidDeclarationRule, conflictRule];

    /// <summary>
    ///
    /// </summary>
    public override void Initialize(AnalysisContext context)
    {
      // Generated registrations are valid by construction, and reporting
      // conflicts on them would repeat the diagnostics on the user's attributes.
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();
      context.RegisterCompilationStartAction(startContext =>
      {
        var symbols = MobileSerializerSymbols.Create(startContext.Compilation);
        if (symbols?.MobileSerializerAttribute is null || symbols.MobileSerializerForAttribute is null || symbols.RegistrationAttribute is null)
          return;

        startContext.RegisterSymbolAction(c => AnalyzeType(c, symbols), SymbolKind.NamedType);
        startContext.RegisterSyntaxNodeAction(c => AnalyzeAssemblyAttributes(c, symbols), SyntaxKind.AttributeList);
      });
    }

    private static void AnalyzeType(SymbolAnalysisContext context, MobileSerializerSymbols symbols)
    {
      var type = (INamedTypeSymbol)context.Symbol;
      var compilation = context.Compilation;
      foreach (var attribute in type.GetAttributes())
      {
        if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, symbols.MobileSerializerAttribute))
        {
          var location = GetLocation(attribute, context.CancellationToken) ?? type.Locations[0];
          var serializer = MobileSerializerRules.GetTypeSideSerializer(attribute);
          var typeValid = Validate(context.ReportDiagnostic, MobileSerializerRules.IsValidTargetType(type, compilation, symbols, out var typeProblem), typeProblem, type, location);
          var serializerValid = Validate(context.ReportDiagnostic, MobileSerializerRules.IsValidSerializer(serializer, compilation, symbols, out var serializerProblem), serializerProblem, serializer, location);
          if (typeValid && serializerValid && serializer is not null)
            ReportConflict(context.ReportDiagnostic, GetRegistry(compilation, symbols, context.CancellationToken), type, serializer, location);
        }
        else if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, symbols.MobileSerializerForAttribute))
        {
          var syntax = attribute.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken) as AttributeSyntax;
          var attributeLocation = syntax?.GetLocation() ?? type.Locations[0];
          var serializerValid = Validate(context.ReportDiagnostic, MobileSerializerRules.IsValidSerializer(type, compilation, symbols, out var serializerProblem), serializerProblem, type, attributeLocation);

          var arguments = syntax?.ArgumentList?.Arguments;
          var targets = MobileSerializerRules.GetSerializerSideTypes(attribute).ToList();
          for (var i = 0; i < targets.Count; i++)
          {
            var target = targets[i];
            // Point at the typeof() argument when the types are listed one per argument.
            var location = arguments is { } list && list.Count == targets.Count
              ? list[i].GetLocation()
              : attributeLocation;
            var typeValid = Validate(context.ReportDiagnostic, MobileSerializerRules.IsValidTargetType(target, compilation, symbols, out var typeProblem), typeProblem, target, location);
            if (typeValid && serializerValid)
              ReportConflict(context.ReportDiagnostic, GetRegistry(compilation, symbols, context.CancellationToken), target, type, location);
          }
        }
      }
    }

    private static void AnalyzeAssemblyAttributes(SyntaxNodeAnalysisContext context, MobileSerializerSymbols symbols)
    {
      var list = (AttributeListSyntax)context.Node;
      if (list.Target is null || !list.Target.Identifier.IsKind(SyntaxKind.AssemblyKeyword))
        return;

      ImmutableArray<AttributeData>? assemblyAttributes = null;
      foreach (var attributeSyntax in list.Attributes)
      {
        // Bind just the attribute name first, so the assembly's attributes are
        // only searched for registrations.
        var constructor = context.SemanticModel.GetSymbolInfo(attributeSyntax, context.CancellationToken).Symbol;
        if (!SymbolEqualityComparer.Default.Equals(constructor?.ContainingType, symbols.RegistrationAttribute))
          continue;

        assemblyAttributes ??= context.Compilation.Assembly.GetAttributes();
        var attribute = assemblyAttributes.Value.FirstOrDefault(a =>
          a.ApplicationSyntaxReference is { } reference
          && reference.SyntaxTree == attributeSyntax.SyntaxTree
          && reference.Span == attributeSyntax.Span);
        if (attribute is null || !MobileSerializerRules.TryGetRegistration(attribute, out var type, out var serializer))
          continue;

        var location = attributeSyntax.GetLocation();
        var typeValid = Validate(context.ReportDiagnostic, MobileSerializerRules.IsValidTargetType(type, context.Compilation, symbols, out var typeProblem), typeProblem, type, location);
        var serializerValid = Validate(context.ReportDiagnostic, MobileSerializerRules.IsValidSerializer(serializer, context.Compilation, symbols, out var serializerProblem), serializerProblem, serializer, location);
        if (typeValid && serializerValid)
          ReportConflict(context.ReportDiagnostic, GetRegistry(context.Compilation, symbols, context.CancellationToken), type, serializer, location);
      }
    }

    private static CustomSerializerRegistry GetRegistry(Compilation compilation, MobileSerializerSymbols symbols, CancellationToken cancellationToken)
      => CustomSerializerRegistry.Get(compilation, symbols, cancellationToken);

    private static bool Validate(Action<Diagnostic> report, bool valid, MobileSerializerProblem problem, ITypeSymbol? type, Location location)
    {
      if (!valid && problem != MobileSerializerProblem.None && type is not null)
        report(Diagnostic.Create(invalidDeclarationRule, location, GetProblemMessage(problem, type)));
      return valid;
    }

    /// <summary>
    /// Gets the localized reason for the CSLA0028 message. It is formatted
    /// in the culture the diagnostic message is requested in.
    /// </summary>
    private static LocalizableResourceString GetProblemMessage(MobileSerializerProblem problem, ITypeSymbol type)
    {
      var name = problem switch
      {
        MobileSerializerProblem.SerializerNotClass => nameof(Resources.MobileSerializerAttributeUsageAnalyzer_SerializerNotClass),
        MobileSerializerProblem.SerializerOpenGeneric => nameof(Resources.MobileSerializerAttributeUsageAnalyzer_SerializerOpenGeneric),
        MobileSerializerProblem.SerializerStatic => nameof(Resources.MobileSerializerAttributeUsageAnalyzer_SerializerStatic),
        MobileSerializerProblem.SerializerAbstract => nameof(Resources.MobileSerializerAttributeUsageAnalyzer_SerializerAbstract),
        MobileSerializerProblem.SerializerNotMobileSerializer => nameof(Resources.MobileSerializerAttributeUsageAnalyzer_SerializerNotMobileSerializer),
        MobileSerializerProblem.SerializerNoPublicConstructor => nameof(Resources.MobileSerializerAttributeUsageAnalyzer_SerializerNoPublicConstructor),
        MobileSerializerProblem.SerializerInaccessible => nameof(Resources.MobileSerializerAttributeUsageAnalyzer_SerializerInaccessible),
        MobileSerializerProblem.TypeNotClassOrStruct => nameof(Resources.MobileSerializerAttributeUsageAnalyzer_TypeNotClassOrStruct),
        MobileSerializerProblem.TypeGeneric => nameof(Resources.MobileSerializerAttributeUsageAnalyzer_TypeGeneric),
        MobileSerializerProblem.TypeInaccessible => nameof(Resources.MobileSerializerAttributeUsageAnalyzer_TypeInaccessible),
        MobileSerializerProblem.TypeSerializedNatively => nameof(Resources.MobileSerializerAttributeUsageAnalyzer_TypeSerializedNatively),
        MobileSerializerProblem.TypeIsMobileObject => nameof(Resources.MobileSerializerAttributeUsageAnalyzer_TypeIsMobileObject),
        _ => throw new ArgumentOutOfRangeException(nameof(problem), problem, null)
      };
      return new LocalizableResourceString(name, Resources.ResourceManager, typeof(Resources), MobileSerializerRules.DisplayName(type));
    }

    private static void ReportConflict(Action<Diagnostic> report, CustomSerializerRegistry registry, ITypeSymbol type, ITypeSymbol serializer, Location location)
    {
      var serializers = registry.GetSerializers(type);
      if (!serializers.Any(s => !SymbolEqualityComparer.Default.Equals(s, serializer)))
        return;

      var names = serializers
        .Append(serializer)
        .Select(s => s.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat))
        .Distinct(StringComparer.Ordinal)
        .OrderBy(n => n, StringComparer.Ordinal);
      report(Diagnostic.Create(conflictRule, location,
        type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
        string.Join(", ", names.Select(n => $"'{n}'"))));
    }

    private static Location? GetLocation(AttributeData attribute, CancellationToken cancellationToken)
      => attribute.ApplicationSyntaxReference?.GetSyntax(cancellationToken).GetLocation();
  }
}
