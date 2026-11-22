using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  /// <summary>
  /// Reports <c>PropertyInfo&lt;T&gt;</c> registrations, in a context with nullable
  /// annotations enabled, that can be migrated to <c>RegisterNotNullProperty</c>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// <c>MigrateToNotNullProperty</c> (Fix All): a value type, a <c>string</c> (the fix
  /// supplies <c>string.Empty</c> as its default value), or a reference type with an
  /// explicit non-null default value. The migration has compile-time consequences only,
  /// unless the app already writes null into a property it declares non-null.
  /// </para>
  /// <para>
  /// <c>MigrateToNotNullPropertyWithReview</c> (no Fix All): any other non-nullable
  /// reference type. Without a default value, every Create and Fetch must load it.
  /// </para>
  /// <para>
  /// The CLR property of the registered name decides, by the rule the source generator
  /// uses: a property declared <c>T?</c>, or with <c>[AllowNull]</c> or <c>[MaybeNull]</c>,
  /// can be null, and isn't reported. Not reported either: <c>Nullable&lt;T&gt;</c>,
  /// lazy-load and private-field registrations, info-object forms, properties used with
  /// a delayed-load method, and a non-serialized reference-type property without a default.
  /// </para>
  /// </remarks>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class MigrateToNotNullPropertyAnalyzer
    : DiagnosticAnalyzer
  {
    private static readonly DiagnosticDescriptor rule =
      new(
        Constants.AnalyzerIdentifiers.MigrateToNotNullProperty, MigrateToNotNullPropertyAnalyzerConstants.Title,
        MigrateToNotNullPropertyAnalyzerConstants.Message, Constants.Categories.Refactoring,
        DiagnosticSeverity.Info, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.MigrateToNotNullProperty, nameof(MigrateToNotNullPropertyAnalyzer)));

    private static readonly DiagnosticDescriptor reviewRule =
      new(
        Constants.AnalyzerIdentifiers.MigrateToNotNullPropertyWithReview, MigrateToNotNullPropertyAnalyzerConstants.ReviewTitle,
        MigrateToNotNullPropertyAnalyzerConstants.ReviewMessage, Constants.Categories.Refactoring,
        DiagnosticSeverity.Info, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.MigrateToNotNullPropertyWithReview, nameof(MigrateToNotNullPropertyAnalyzer)));

    /// <summary>
    ///
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [rule, reviewRule];

    /// <summary>
    ///
    /// </summary>
    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();
      context.RegisterSymbolStartAction(AnalyzeTypeStart, SymbolKind.NamedType);
    }

    private static void AnalyzeTypeStart(SymbolStartAnalysisContext context)
    {
      var type = (INamedTypeSymbol)context.Symbol;
      if (type.TypeKind != TypeKind.Class)
      {
        return;
      }

      var candidates = new ConcurrentDictionary<IFieldSymbol, Diagnostic>(SymbolEqualityComparer.Default);
      var delayedLoad = new ConcurrentDictionary<IFieldSymbol, bool>(SymbolEqualityComparer.Default);

      context.RegisterOperationAction(c =>
      {
        var initializer = (IFieldInitializerOperation)c.Operation;
        if (initializer.InitializedFields.Length == 1 &&
          Analyze(initializer, initializer.InitializedFields[0], type, c.CancellationToken) is { } diagnostic)
        {
          candidates[initializer.InitializedFields[0]] = diagnostic;
        }
      }, OperationKind.FieldInitializer);

      context.RegisterOperationAction(c =>
      {
        var invocation = (IInvocationOperation)c.Operation;
        if (!NotNullPropertyDelayedLoadAnalyzer.delayedLoadMethods.Contains(invocation.TargetMethod.Name) ||
          !invocation.TargetMethod.ContainingType.IsDeclaredInCsla())
        {
          return;
        }
        foreach (var argument in invocation.Arguments)
        {
          if (NotNullPropertyRegistrations.GetReferencedField(argument.Value) is { } field)
          {
            delayedLoad[field.OriginalDefinition] = true;
          }
        }
      }, OperationKind.Invocation);

      context.RegisterSymbolEndAction(end =>
      {
        foreach (var candidate in candidates)
        {
          if (!delayedLoad.ContainsKey(candidate.Key.OriginalDefinition))
          {
            end.ReportDiagnostic(candidate.Value);
          }
        }
      });
    }

    private static Diagnostic? Analyze(IFieldInitializerOperation initializer, IFieldSymbol field, INamedTypeSymbol type,
      CancellationToken cancellationToken)
    {
      if (!field.IsStatic || !NotNullMigration.IsPropertyInfo(field.Type) ||
        field.DeclaringSyntaxReferences.Length != 1 ||
        field.DeclaringSyntaxReferences[0].GetSyntax(cancellationToken) is not VariableDeclaratorSyntax
        {
          Parent: VariableDeclarationSyntax { Variables.Count: 1 }
        } declarator ||
        initializer.SemanticModel is not { } semanticModel ||
        !NotNullPropertyRules.IsAnnotationsEnabled(semanticModel, declarator.SpanStart) ||
        !NotNullMigration.TryRead(initializer.Value, out var registration))
      {
        return null;
      }

      // the property's declaration decides, by the generator's rule; without one, the type argument
      var property = NotNullMigration.FindProperty(type, registration);
      if (!NotNullPropertyRules.QualifiesForNotNull(registration.ValueType) ||
        !NotNullPropertyRules.QualifiesForNotNull(((INamedTypeSymbol)field.Type).TypeArguments[0]) ||
        (property is not null && NotNullPropertyRules.GetRegistration(property) == NotNullRegistration.None))
      {
        return null;
      }

      var name = registration.PropertyName ?? field.Name;
      var location = declarator.Identifier.GetLocation();

      if (registration.ValueType.IsValueType)
      {
        return Diagnostic.Create(rule, location, name);
      }

      var hasNonNullDefault = registration.DefaultValue is { } defaultValue && !registration.HasNullDefault &&
        defaultValue.Syntax is ArgumentSyntax { Expression: var expression } &&
        semanticModel.GetTypeInfo(expression, cancellationToken).Nullability.FlowState == NullableFlowState.NotNull;

      if (registration.IsString)
      {
        // the fix supplies string.Empty in place of a missing or null default;
        // a default that might be null can't be replaced without changing behaviour
        return registration.DefaultValue is null || registration.HasNullDefault || hasNonNullDefault
          ? Diagnostic.Create(rule, location, name)
          : null;
      }

      if (hasNonNullDefault)
      {
        return Diagnostic.Create(rule, location, name);
      }

      // a non-serialized property without a default would be null after deserialization (CSLA0032)
      if (property is not null && property.GetAttributes().Any(_ =>
        _.AttributeClass.IsDoNotSerializeAttribute() || _.AttributeClass.IsCalculatedPropertyAttribute()))
      {
        return null;
      }

      return Diagnostic.Create(reviewRule, location, name);
    }
  }
}
