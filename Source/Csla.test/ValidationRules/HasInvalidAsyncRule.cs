using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Rules;
using System.ComponentModel;
using System.Threading;

namespace Csla.Test.ValidationRules
{
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  public partial class HasInvalidAsyncRule : BusinessBase<HasInvalidAsyncRule>
  {
    public HasInvalidAsyncRule()
    {
      Reset = new ManualResetEvent(false);
    }

    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new InvalidAsyncValidationRule(NameProperty));
      base.AddBusinessRules();
    }

    public static PropertyInfo<ManualResetEvent> ResetProperty = RegisterProperty<ManualResetEvent>(c => c.Reset);
    public ManualResetEvent Reset
    {
      get { return GetProperty(ResetProperty); }
      set { SetProperty(ResetProperty, value); }
    }

    public Rules.BrokenRulesCollection GetBrokenRules()
    {
      return BusinessRules.GetBrokenRules();
    }

    public void Validate()
    {
      BusinessRules.CheckRules();
    }

    public class InvalidAsyncValidationRule : BusinessRule
    {
      public InvalidAsyncValidationRule(Csla.Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      {
        IsAsync = true;
        InputProperties = new List<Core.IPropertyInfo> { primaryProperty, ResetProperty };
      }

      protected override void Execute(RuleContext context)
      {
        var bw = new System.ComponentModel.BackgroundWorker();
        bw.DoWork += (o, e) =>
          {
            throw new InvalidOperationException();
          };
        bw.RunWorkerCompleted += (o, e) =>
          {
            if (e.Error != null)
              context.AddErrorResult(e.Error.Message);
            context.Complete();
            ((ManualResetEvent)context.InputPropertyValues[ResetProperty]).Set();
          };
        bw.RunWorkerAsync();
      }
    }
  }
}
