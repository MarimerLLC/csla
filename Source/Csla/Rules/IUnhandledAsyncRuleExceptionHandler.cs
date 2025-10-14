namespace Csla.Rules;

/// <summary>
/// Represents an interface for handling exceptions raised by <see cref="IBusinessRuleAsync"/>.
/// </summary>
public interface IUnhandledAsyncRuleExceptionHandler
{
  /// <summary>
  /// Checks whether the given <paramref name="exception"/> and <paramref name="executingRule"/> can be handled.
  /// </summary>
  /// <param name="exception">The unhandled <see cref="Exception"/>.</param>
  /// <param name="executingRule">The rule causing <paramref name="exception"/>.</param>
  /// <returns><see langword="true"/> if the exception can be handled. Otherwise <see langword="false"/>.</returns>
  bool CanHandle(Exception exception, IBusinessRuleBase executingRule);

  /// <summary>
  /// Handles the raised <paramref name="exception"/>.
  /// </summary>
  /// <param name="exception">The unhandled <see cref="Exception"/>.</param>
  /// <param name="executingRule">The rule causing <paramref name="exception"/>.</param>
  /// <param name="ruleContext">The associated <see cref="IRuleContext"/> to this rule run.</param>
  /// <returns><see cref="ValueTask"/></returns>
  ValueTask Handle(Exception exception, IBusinessRuleBase executingRule, IRuleContext ruleContext);
}