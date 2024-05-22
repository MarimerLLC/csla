﻿using Csla.Core;
using Csla.Rules;

namespace Csla.Test.ValidationRules
{
  [Serializable]
  public class AsyncRuleRoot : BusinessBase<AsyncRuleRoot>
  {
    public static readonly PropertyInfo<string> CustomerNumberProperty = RegisterProperty<string>(c => c.CustomerNumber);

    public string CustomerNumber
    {
      get { return GetProperty(CustomerNumberProperty); }

      set { SetProperty(CustomerNumberProperty, value); }
    }

    public static readonly PropertyInfo<string> CustomerNameProperty = RegisterProperty<string>(c => c.CustomerName);

    public string CustomerName
    {
      get { return GetProperty(CustomerNameProperty); }

      set { SetProperty(CustomerNameProperty, value); }
    }

    public static readonly PropertyInfo<string> AsyncAwaitProperty = RegisterProperty<string>(c => c.AsyncAwait);
    public string AsyncAwait
    {
      get { return GetProperty(AsyncAwaitProperty); }
      set { SetProperty(AsyncAwaitProperty, value); }
    }

    public static AsyncRuleRoot NewRoot(IDataPortal<AsyncRuleRoot> dataPortal)
    {
      return dataPortal.Create();
    }

    public AsyncRuleRoot()
    {
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Rules.CommonRules.Required(CustomerNumberProperty));
      BusinessRules.AddRule(new Rules.CommonRules.Required(CustomerNameProperty));

      // async rule will only run when CustomerNumber has value
      BusinessRules.AddRule(new LookupCustomerRule(CustomerNumberProperty, CustomerNameProperty) { Priority = 10 });

      BusinessRules.AddRule(new AsyncAwaitRule(AsyncAwaitProperty));
    }

    private class LookupCustomerRule : BusinessRule
    {
      private IPropertyInfo _nameProperty;

      public LookupCustomerRule(IPropertyInfo primaryProperty, IPropertyInfo nameProperty)

        : base(primaryProperty)
      {
        _nameProperty = nameProperty;
        AffectedProperties.Add(nameProperty);
        InputProperties.Add(primaryProperty);

        IsAsync = true;
      }

      protected override void Execute(IRuleContext context)
      {
        var cn = (string)context.InputPropertyValues[PrimaryProperty];

        var bw = new System.ComponentModel.BackgroundWorker();

        bw.RunWorkerCompleted += (_, _) =>
        {
          context.AddOutValue(_nameProperty, $"customer name {cn}");

          context.Complete();
        };
        bw.DoWork += (_, _) =>
        {
          Thread.Sleep(50);
        };
        bw.RunWorkerAsync();

      }
    }

    private class AsyncAwaitRule : BusinessRuleAsync
    {
      public AsyncAwaitRule(IPropertyInfo primaryProperty)
        : base(primaryProperty)
      { }

      protected override async Task ExecuteAsync(IRuleContext context)
      {
        await Task.Delay(0);
        context.AddOutValue("abc");
      }
    }

    [Create]
    private void Create()
    {
      BusinessRules.CheckRules();
    }

    [Create]
    private void Create(string asyncAwaitValue) 
    {
      using (BypassPropertyChecks) {
        AsyncAwait = asyncAwaitValue;
      }
    }

    public Task<List<string>> CheckRulesForPropertyAsyncAwait() => BusinessRules.CheckRulesAsync(AsyncAwaitProperty);
  }
}