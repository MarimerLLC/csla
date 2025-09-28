//-----------------------------------------------------------------------
// <copyright file="DelayedAsynRuleExceptionRoot.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This only works on Silverlight because when run through NUnit it is not running</summary>
//-----------------------------------------------------------------------

using Csla.Core;
using Csla.Rules;

namespace Csla.Test.ValidationRules
{
  [CslaImplementProperties]
  internal partial class DelayedAsynRuleExceptionRoot : BusinessBase<DelayedAsynRuleExceptionRoot>
  {
    public partial TimeSpan ExceptionDelay { get; set; }

    [Create, RunLocal]
    private void Create()
    {
      _ = this;
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new RaiseExceptionRuleAsync(ExceptionDelayProperty));
    }


    private class RaiseExceptionRuleAsync : BusinessRuleAsync
    {
      public RaiseExceptionRuleAsync(IPropertyInfo primaryProperty) : base(primaryProperty)
      {
        InputProperties.Add(primaryProperty);
      }

      protected override async Task ExecuteAsync(IRuleContext context)
      {
        var delay = context.GetInputValue<TimeSpan>(PrimaryProperty);

        await Task.Delay(delay);

        throw new InvalidOperationException("This is a test exception");
      }
    }
  }
}