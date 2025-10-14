namespace Csla.Rules;

internal class DontObserveUnhandledAsyncRuleExceptionHandler : IUnhandledAsyncRuleExceptionHandler
{
  public bool CanHandle(Exception exception, IBusinessRuleBase executingRule) => false;
  public ValueTask Handle(Exception exception, IBusinessRuleBase executingRule, IRuleContext ruleContext) => throw new NotSupportedException();
}
