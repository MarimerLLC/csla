using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  /// <summary>
  /// Reports a read authorization rule for a not-null reference-type property
  /// without a default value: an authorization rule created with
  /// <c>AuthorizationActions.ReadProperty</c> and the property's field as
  /// arguments, by a constructor or by a method that returns the rule.
  /// Permission queries such as <c>BusinessRules.HasPermission</c> aren't
  /// reported, because they create no rule. A denied read with <c>NoAccessBehavior.SuppressException</c>
  /// returns the default value, which is null, through a non-nullable getter.
  /// Best-effort: a rule built elsewhere (a helper, a variable) isn't detected.
  /// </summary>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class NotNullPropertyReadAuthorizationAnalyzer
    : DiagnosticAnalyzer
  {
    private const string ReadPropertyMemberName = "ReadProperty";

    private static readonly DiagnosticDescriptor rule =
      new(
        Constants.AnalyzerIdentifiers.NotNullPropertyWithReadAuthorization, NotNullPropertyReadAuthorizationAnalyzerConstants.Title,
        NotNullPropertyReadAuthorizationAnalyzerConstants.Message, Constants.Categories.Usage,
        DiagnosticSeverity.Warning, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.NotNullPropertyWithReadAuthorization, nameof(NotNullPropertyReadAuthorizationAnalyzer)));

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
      context.RegisterOperationAction(c =>
      {
        var creation = (IObjectCreationOperation)c.Operation;
        if (creation.Type.IsAuthorizationRule())
          Analyze(c, creation.Arguments);
      }, OperationKind.ObjectCreation);
      context.RegisterOperationAction(c =>
      {
        // a method that returns a rule (a factory); permission queries such
        // as HasPermission return bool and create no rule
        var invocation = (IInvocationOperation)c.Operation;
        if (invocation.TargetMethod.ReturnType.IsAuthorizationRule())
          Analyze(c, invocation.Arguments);
      }, OperationKind.Invocation);
    }

    private static void Analyze(OperationAnalysisContext context, ImmutableArray<IArgumentOperation> arguments)
    {
      if (arguments.Length < 2 || !arguments.Any(_ => IsReadPropertyAction(_.Value)))
      {
        return;
      }

      foreach (var argument in arguments)
      {
        if (NotNullPropertyRegistrations.GetReferencedField(argument.Value) is { IsStatic: true } field &&
          NotNullPropertyRegistrations.IsNoDefaultReference(field, context.CancellationToken))
        {
          context.ReportDiagnostic(Diagnostic.Create(rule, context.Operation.Syntax.GetLocation(),
            NotNullPropertyRegistrations.GetRegisteredName(field, context.CancellationToken)));
        }
      }
    }

    private static bool IsReadPropertyAction(IOperation value) =>
      NotNullPropertyRegistrations.GetReferencedField(value) is { Name: ReadPropertyMemberName } field &&
      field.ContainingType.Name == CslaMemberConstants.Types.AuthorizationActions &&
      field.ContainingType.IsDeclaredInCsla();
  }
}
