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
    private static PropertyInfo<string> NameProperty = RegisterProperty(
      typeof(HasAsyncRule),
      new PropertyInfo<string>("Name"));

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

    private ManualResetEvent _reset = new ManualResetEvent(false);
    public EventWaitHandle Reset
    {
      get { return _reset; }
    }

    public class InvalidAsyncValidationRule : BusinessRule
    {
      public InvalidAsyncValidationRule(Csla.Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      {
        IsAsync = true;
        InputProperties = new List<Core.IPropertyInfo> { primaryProperty };
      }

      protected override void Execute(RuleContext context)
      {
        throw new NotImplementedException();
      }
    }
  }
}
