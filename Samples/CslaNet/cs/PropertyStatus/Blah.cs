using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Csla.Core;
using Csla;
using Csla.Validation;
using System.Threading;

namespace Example
{
  public class Blah : BusinessBase
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty(
      typeof(Blah),
      new PropertyInfo<string>("Data"));
    
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(DoAsyncRule, DataProperty);
    }

    private static void DoAsyncRule(AsyncValidationRuleContext context)
    {
      BackgroundWorker worker = new BackgroundWorker();
      worker.DoWork += (o, e) => Thread.Sleep(3000);
      worker.RunWorkerCompleted += (o, e) =>
      {
        string val = (string)context.PropertyValues["Data"];
        if (val == "Error")
        {
          context.OutArgs.Result = false;
          context.OutArgs.Severity = RuleSeverity.Error;
          context.OutArgs.Description = "Invalid data!";
        }
        else if (val == "Warning")
        {
          context.OutArgs.Result = false;
          context.OutArgs.Severity = RuleSeverity.Warning;
          context.OutArgs.Description = "This might not be a great idea!";
        }
        else if (val == "Information")
        {
          context.OutArgs.Result = false;
          context.OutArgs.Severity = RuleSeverity.Information;
          context.OutArgs.Description = "Just an FYI!";
        }

        context.Complete();
      };
      worker.RunWorkerAsync();
    }
  }

  public class BlahCollection : ObservableCollection<Blah>
  {
    public BlahCollection()
    {
      Add(new Blah { Data = "one" });
      Add(new Blah { Data = "two" });
      Add(new Blah { Data = "three" });
    }
  }
}
