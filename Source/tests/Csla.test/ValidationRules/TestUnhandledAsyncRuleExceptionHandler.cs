//-----------------------------------------------------------------------
// <copyright file="TestUnhandledAsyncRuleExceptionHandler.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This only works on Silverlight because when run through NUnit it is not running</summary>
//-----------------------------------------------------------------------

using Csla.Rules;

namespace Csla.Test.ValidationRules
{
  internal class TestUnhandledAsyncRuleExceptionHandler : IUnhandledAsyncRuleExceptionHandler
  {
    public bool CanHandleResult { get; set; } = true;

    public Action<Exception, IBusinessRuleBase> CanHandleInspector { get; set; }

    public bool CanHandle(Exception exception, IBusinessRuleBase executingRule)
    {
      CanHandleInspector?.Invoke(exception, executingRule);
      return CanHandleResult;
    }


    public Action<Exception, IBusinessRuleBase, IRuleContext> HandleInspector { get; set; }
    public ValueTask Handle(Exception exception, IBusinessRuleBase executingRule, IRuleContext ruleContext)
    {
      HandleInspector?.Invoke(exception, executingRule, ruleContext);
      return ValueTask.CompletedTask;
    }
  }
}