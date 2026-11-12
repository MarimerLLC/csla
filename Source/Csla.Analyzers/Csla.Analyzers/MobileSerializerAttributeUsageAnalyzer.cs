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
        var compilation = startContext.Compilation;
        var attributes = AttributeTypes.Create(compilation);
        if (attributes is null)
          return;

        var registry = new Lazy<CustomSerializerRegistry>(() => CustomSerializerRegistry.Create(compilation));
        startContext.RegisterSymbolAction(c => AnalyzeType(c, attributes, registry), SymbolKind.NamedType);
        startContext.RegisterSyntaxNodeAction(c => AnalyzeAssemblyAttributes(c, attributes, registry), SyntaxKind.AttributeList);
      });
    }

    private static void AnalyzeType(SymbolAnalysisContext context, AttributeTypes attributes, Lazy<CustomSerializerRegistry> registry)
    {
      var type = (INamedTypeSymbol)context.Symbol;
      foreach (var attribute in type.GetAttributes())
      {
        if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributes.MobileSerializer))
        {
          var location = GetLocation(attribute, context.CancellationToken) ?? type.Locations[0];
          var serializer = MobileSerializerRules.GetTypeSideSerializer(attribute);
          var typeValid = Validate(context.ReportDiagnostic, MobileSerializerRules.IsValidTargetType(type, context.Compilation, out var typeProblem), typeProblem, type, location);
          var serializerValid = Validate(context.ReportDiagnostic, MobileSerializerRules.IsValidSerializer(serializer, context.Compilation, out var serializerProblem), serializerProblem, serializer, location);
          if (typeValid && serializerValid && serializer is not null)
            ReportConflict(context.ReportDiagnostic, registry.Value, type, serializer, location);
        }
        else if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributes.MobileSerializerFor))
        {
          var syntax = attribute.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken) as AttributeSyntax;
          var attributeLocation = syntax?.GetLocation() ?? type.Locations[0];
          var serializerValid = Validate(context.ReportDiagnostic, MobileSerializerRules.IsValidSerializer(type, context.Compilation, out var serializerProblem), serializerProblem, type, attributeLocation);

          var arguments = syntax?.ArgumentList?.Arguments;
          var targets = MobileSerializerRules.GetSerializerSideTypes(attribute).ToList();
          for (var i = 0; i < targets.Count; i++)
          {
            var target = targets[i];
            // Point at the typeof() argument when the types are listed one per argument.
            var location = arguments is { } list && list.Count == targets.Count
              ? list[i].GetLocation()
              : attributeLocation;
            var typeValid = Validate(context.ReportDiagnostic, MobileSerializerRules.IsValidTargetType(target, context.Compilation, out var typeProblem), typeProblem, target, location);
            if (typeValid && serializerValid)
              ReportConflict(context.ReportDiagnostic, registry.Value, target, type, location);
          }
        }
      }
    }

    private static void AnalyzeAssemblyAttributes(SyntaxNodeAnalysisContext context, AttributeTypes attributes, Lazy<CustomSerializerRegistry> registry)
    {
      var list = (AttributeListSyntax)context.Node;
      if (list.Target is null || !list.Target.Identifier.IsKind(SyntaxKind.AssemblyKeyword))
        return;

      var assemblyAttributes = context.Compilation.Assembly.GetAttributes();
      foreach (var attributeSyntax in list.Attributes)
      {
        var attribute = assemblyAttributes.FirstOrDefault(a =>
          a.ApplicationSyntaxReference is { } reference
          && reference.SyntaxTree == attributeSyntax.SyntaxTree
          && reference.Span == attributeSyntax.Span);
        if (attribute is null || !SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributes.MobileSerializerRegistration))
          continue;
        if (!MobileSerializerRules.TryGetRegistration(attribute, out var type, out var serializer))
          continue;

        var location = attributeSyntax.GetLocation();
        var typeValid = Validate(context.ReportDiagnostic, MobileSerializerRules.IsValidTargetType(type, context.Compilation, out var typeProblem), typeProblem, type, location);
        var serializerValid = Validate(context.ReportDiagnostic, MobileSerializerRules.IsValidSerializer(serializer, context.Compilation, out var serializerProblem), serializerProblem, serializer, location);
        if (typeValid && serializerValid)
          ReportConflict(context.ReportDiagnostic, registry.Value, type, serializer, location);
      }
    }

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

    private sealed class AttributeTypes
    {
      public required INamedTypeSymbol MobileSerializer { get; init; }
      public required INamedTypeSymbol MobileSerializerFor { get; init; }
      public required INamedTypeSymbol MobileSerializerRegistration { get; init; }

      public static AttributeTypes? Create(Compilation compilation)
      {
        var mobileSerializer = compilation.GetTypeByMetadataName(MobileSerializerRules.MobileSerializerAttributeMetadataName);
        var mobileSerializerFor = compilation.GetTypeByMetadataName(MobileSerializerRules.MobileSerializerForAttributeMetadataName);
        var registration = compilation.GetTypeByMetadataName(MobileSerializerRules.MobileSerializerRegistrationAttributeMetadataName);
        if (mobileSerializer is null || mobileSerializerFor is null || registration is null)
          return null;
        return new AttributeTypes
        {
          MobileSerializer = mobileSerializer,
          MobileSerializerFor = mobileSerializerFor,
          MobileSerializerRegistration = registration
        };
      }
    }
  }
}
