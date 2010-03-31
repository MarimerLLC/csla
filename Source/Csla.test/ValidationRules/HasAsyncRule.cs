using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Rules;
using System.ComponentModel;
using System.Threading;
using Csla.Core;

namespace Csla.Test.ValidationRules
{
  public partial class HasAsyncRule : BusinessBase<HasAsyncRule>
  {
    private static PropertyInfo<string> NameProperty = RegisterProperty(
      typeof(HasAsyncRule),
      new PropertyInfo<string>("Name"));

    private static PropertyInfo<ManualResetEvent> ResetProperty = RegisterProperty(
      new PropertyInfo<ManualResetEvent>("Reset", "Reset", new ManualResetEvent(false)));

    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public ManualResetEvent Reset
    {
      get { return GetProperty(ResetProperty); }
      set { SetProperty(ResetProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Rule1(NameProperty));
      BusinessRules.AddRule(new Rule2(NameProperty));
      BusinessRules.AddRule(new Rule3(NameProperty));
    }

    public class Rule1 : BusinessRule
    {
      public Rule1(Csla.Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      {
        IsAsync = true;
        InputProperties = new List<IPropertyInfo> { primaryProperty, ResetProperty };
      }

      protected override void Execute(RuleContext context)
      {
        Random r = new Random();
        BackgroundWorker worker = new BackgroundWorker();
        // Using closures to access the context would be easier but this is not possible
        // in all languages. Below is an example of how to use the context without closures

        worker.DoWork += (s, e) =>
        {
          var avrc = (RuleContext)e.Argument;
          int sleep = r.Next(0, 20);
          System.Threading.Thread.Sleep(sleep);
          ((ManualResetEvent)avrc.InputPropertyValues[avrc.Rule.PrimaryProperty]).WaitOne();
          var name = avrc.InputPropertyValues[NameProperty];
          if (name != null || name.ToString() == "error")
            avrc.AddErrorResult("error detected");
          e.Result = avrc;
        };
        worker.RunWorkerCompleted += (s, e) =>
        {
          var avrc = (RuleContext)e.Result;
          avrc.Complete();
        };

        // simulating an asynchronous process.
        worker.RunWorkerAsync(context);
      }
    }

    public class Rule2 : BusinessRule
    {
      private Rule1 _innerRule;

      public Rule2(Csla.Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      {
        IsAsync = true;
        _innerRule = new Rule1(primaryProperty);
        InputProperties = _innerRule.InputProperties;
      }

      protected override void Execute(RuleContext context)
      {
        if (context.Target != null)
          throw new ArgumentOutOfRangeException("context.Target must be null");

        ((IBusinessRule)_innerRule).Execute(context);
      }
    }

    public class Rule3 : BusinessRule
    {
      private Rule1 _innerRule;

      public Rule3(Csla.Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      {
        IsAsync = true;
        _innerRule = new Rule1(primaryProperty);
        InputProperties = _innerRule.InputProperties;
      }

      protected override void Execute(RuleContext context)
      {
        ((IBusinessRule)_innerRule).Execute(context);
      }
    }
  }
}
