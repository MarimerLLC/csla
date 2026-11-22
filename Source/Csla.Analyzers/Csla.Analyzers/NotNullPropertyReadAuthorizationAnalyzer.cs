using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  /// <summary>
  /// Reports a read authorization rule for a not-null reference-type property
  /// without a default value: a rule created or added with
  /// <c>AuthorizationActions.ReadProperty</c> and the property's field as
  /// arguments. A denied read with <c>NoAccessBehavior.SuppressException</c>
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
      context.RegisterOperationAction(
        c => Analyze(c, ((IObjectCreationOperation)c.Operation).Arguments), OperationKind.ObjectCreation);
      context.RegisterOperationAction(
        c => Analyze(c, ((IInvocationOperation)c.Operation).Arguments), OperationKind.Invocation);
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
