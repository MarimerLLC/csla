using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Csla.Core;
using Csla.Rules;
using Csla.Serialization;

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

    public static AsyncRuleRoot NewRoot()
    {
      return Csla.DataPortal.Create<AsyncRuleRoot>();
    }

    public AsyncRuleRoot()
    {
      BusinessRules.CheckRules();
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(CustomerNumberProperty));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(CustomerNameProperty));

      // async rule will only run when CustomerNumber has value
      BusinessRules.AddRule(new LookupCustomerRule(CustomerNumberProperty, CustomerNameProperty) { Priority = 10 });

      BusinessRules.AddRule(new AsyncAwaitRule(AsyncAwaitProperty));
    }

    private class LookupCustomerRule : Csla.Rules.BusinessRule
    {
      private IPropertyInfo _nameProperty;

      public LookupCustomerRule(IPropertyInfo primaryProperty, IPropertyInfo nameProperty)

        : base(primaryProperty)
      {
        _nameProperty = nameProperty;
        AffectedProperties.Add(nameProperty);
        InputProperties = new List<IPropertyInfo>() { primaryProperty };

        IsAsync = true;
      }

      protected override void Execute(IRuleContext context)
      {
        var cn = (string)context.InputPropertyValues[PrimaryProperty];

        var bw = new System.ComponentModel.BackgroundWorker();

        bw.RunWorkerCompleted += (o, e) =>
        {
          context.AddOutValue(_nameProperty, string.Format("customer name {0}", cn));

          context.Complete();
        };
        bw.DoWork += (o, e) =>
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
  }
}