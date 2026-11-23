using Csla.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;

namespace Csla.Analyzers
{
  /// <summary>
  /// Reports a two-argument <c>RegisterNotNullProperty&lt;string&gt;</c> call whose
  /// second argument is a constant empty or whitespace string. That argument binds
  /// to <c>friendlyName</c>, so the call registers the property without a default
  /// value, although it reads as if <c>string.Empty</c> were the default.
  /// </summary>
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public sealed class NotNullPropertyEmptyFriendlyNameAnalyzer
    : DiagnosticAnalyzer
  {
    private const string FriendlyNameParameterName = "friendlyName";

    private static readonly DiagnosticDescriptor rule =
      new(
        Constants.AnalyzerIdentifiers.NotNullPropertyEmptyFriendlyName, NotNullPropertyEmptyFriendlyNameAnalyzerConstants.Title,
        NotNullPropertyEmptyFriendlyNameAnalyzerConstants.Message, Constants.Categories.Usage,
        DiagnosticSeverity.Warning, true,
        helpLinkUri: HelpUrlBuilder.Build(
          Constants.AnalyzerIdentifiers.NotNullPropertyEmptyFriendlyName, nameof(NotNullPropertyEmptyFriendlyNameAnalyzer)));

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
      if (method.Name != NotNullPropertyRegistrations.RegisterNotNullPropertyName ||
        !method.ContainingType.IsDeclaredInCsla() ||
        method.TypeArguments.Length != 1 ||
        method.TypeArguments[0].SpecialType != SpecialType.System_String ||
        invocation.Arguments.Length != 2)
      {
        return;
      }

      var friendlyName = invocation.Arguments.FirstOrDefault(_ => _.Parameter?.Name == FriendlyNameParameterName);
      if (friendlyName is not null &&
        friendlyName.Value.ConstantValue is { HasValue: true, Value: string text } &&
        string.IsNullOrWhiteSpace(text))
      {
        var propertyName = invocation.Arguments.FirstOrDefault(_ => _.Parameter?.Ordinal == 0)?.Value.ConstantValue is { HasValue: true, Value: string name }
          ? name
          : context.ContainingSymbol is IFieldSymbol field
            ? NotNullPropertyRegistrations.GetRegisteredName(field, context.CancellationToken)
            : invocation.Arguments[0].Syntax.ToString();
        context.ReportDiagnostic(Diagnostic.Create(rule, friendlyName.Syntax.GetLocation(), propertyName));
      }
    }
  }
}
