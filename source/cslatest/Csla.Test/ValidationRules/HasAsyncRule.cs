using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Validation;
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
      AsyncRuleArgs args = new AsyncRuleArgs(NameProperty, ResetProperty);
      ValidationRules.AddRule(HasAsyncRule.Rule1, args);
      ValidationRules.AddRule(HasAsyncRule.Rule2, args);
      ValidationRules.AddRule(HasAsyncRule.Rule3, args);
      ValidationRules.AddRule(SingletonHasAsyncRule.Instance.Rule1, args);
      ValidationRules.AddRule(SingletonHasAsyncRule.Instance.Rule2, args);
      ValidationRules.AddRule(SingletonHasAsyncRule.Instance.Rule3, args);
    }

    private static void Rule1(AsyncValidationRuleContext context)
    {
      Random r = new Random();
      BackgroundWorker worker = new BackgroundWorker();
      // Using closures to access the context would be easier but this is not possible
      // in all languages. Below is an example of how to use the context without closures

      worker.DoWork += (s, e) =>
      {
        AsyncValidationRuleContext avrc = (AsyncValidationRuleContext)e.Argument;
        int sleep = r.Next(0, 20);
        System.Threading.Thread.Sleep(sleep);
        ((ManualResetEvent)avrc.PropertyValues["Reset"]).WaitOne();
        avrc.OutArgs.Result = (string)avrc.PropertyValues["Name"] != "error";
        e.Result = avrc;
      };
      worker.RunWorkerCompleted += (s, e) =>
      {
        AsyncValidationRuleContext avrc = (AsyncValidationRuleContext)e.Result;
        avrc.OutArgs.Description = "error detected";
        avrc.Complete();
      };

      // simulating an asynchronous process.
      worker.RunWorkerAsync(context);
    }
    private static void Rule2(AsyncValidationRuleContext context)
    {
      Rule1(context);
    }
    private static void Rule3(AsyncValidationRuleContext context)
    {
      Rule1(context);
    }

    public class SingletonHasAsyncRule
    {
      private SingletonHasAsyncRule() { }

      private static SingletonHasAsyncRule _instance;
      public static SingletonHasAsyncRule Instance
      {
        get
        {
          if (_instance == null)
            _instance = new SingletonHasAsyncRule();
          return _instance;
        }
      }

      public void Rule1(AsyncValidationRuleContext context)
      {
        HasAsyncRule.Rule1(context);
      }

      public void Rule2(AsyncValidationRuleContext context)
      {
        Rule1(context);
      }

      public void Rule3(AsyncValidationRuleContext context)
      {
        Rule1(context);
      }
    }
  }
}
