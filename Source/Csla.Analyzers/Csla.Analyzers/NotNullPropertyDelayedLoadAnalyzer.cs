using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  /// <summary>
  /// Reports a not-null property passed to one of the delayed-load methods
  /// (<c>LazyGetProperty</c>, <c>LazyGetPropertyAsync</c>, <c>LazyReadProperty</c>,
  /// <c>LazyReadPropertyAsync</c>, <c>LoadPropertyAsync</c>), which throw
  /// <see cref="NotSupportedException"/> for it. Value types and generated
  /// registrations are included.
  /// </summary>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class NotNullPropertyDelayedLoadAnalyzer
    : DiagnosticAnalyzer
  {
    /// <summary>
    /// The delayed-load methods, which not-null properties don't support.
    /// </summary>
    internal static readonly ImmutableHashSet<string> delayedLoadMethods =
    [
      CslaMemberConstants.Properties.LazyGetProperty,
      CslaMemberConstants.Properties.LazyGetPropertyAsync,
      CslaMemberConstants.Properties.LazyReadProperty,
      CslaMemberConstants.Properties.LazyReadPropertyAsync,
      CslaMemberConstants.Properties.LoadPropertyAsync
    ];

    private static readonly DiagnosticDescriptor rule =
      new(
        Constants.AnalyzerIdentifiers.NotNullPropertyDelayedLoad, NotNullPropertyDelayedLoadAnalyzerConstants.Title,
        NotNullPropertyDelayedLoadAnalyzerConstants.Message, Constants.Categories.Usage,
        DiagnosticSeverity.Error, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.NotNullPropertyDelayedLoad, nameof(NotNullPropertyDelayedLoadAnalyzer)));

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
      context.RegisterOperationAction(AnalyzeInvocation, OperationKind.Invocation);
    }

    private static void AnalyzeInvocation(OperationAnalysisContext context)
    {
      var invocation = (IInvocationOperation)context.Operation;
      var method = invocation.TargetMethod;
      if (!delayedLoadMethods.Contains(method.Name) || !method.ContainingType.IsDeclaredInCsla())
      {
        return;
      }

      foreach (var argument in invocation.Arguments)
      {
        var value = NotNullPropertyRegistrations.Unwrap(argument.Value);
        if (value is not null && NotNullPropertyRegistrations.IsNotNullPropertyInfo(value.Type))
        {
          var propertyName = NotNullPropertyRegistrations.GetReferencedField(value) is { } field
            ? NotNullPropertyRegistrations.GetRegisteredName(field, context.CancellationToken)
            : argument.Syntax.ToString();
          context.ReportDiagnostic(Diagnostic.Create(rule, argument.Syntax.GetLocation(), propertyName, method.Name));
        }
      }
    }
  }
}
