using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Csla.Core;
using Csla;
using System.Threading;

namespace PropertyStatus
{
  [Serializable]
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
      BusinessRules.AddRule(new DoAsyncRule(DataProperty));
    }

    private class DoAsyncRule : Csla.Rules.BusinessRule
    {
      public DoAsyncRule(Csla.Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      {
        InputProperties = new List<IPropertyInfo> { PrimaryProperty };
        IsAsync = true;
      }

      protected override void Execute(Csla.Rules.IRuleContext context)
      {
        BackgroundWorker worker = new BackgroundWorker();
        worker.DoWork += (o, e) => Thread.Sleep(3000);
        worker.RunWorkerCompleted += (o, e) =>
        {
          string val = (string)context.InputPropertyValues[PrimaryProperty];
          if (val == "Error")
          {
            context.AddErrorResult("Invalid data!");
          }
          else if (val == "Warning")
          {
            context.AddWarningResult("This might not be a great idea!");
          }
          else if (val == "Information")
          {
            context.AddInformationResult("Just an FYI!");
          }
          context.Complete();
        };
        worker.RunWorkerAsync();
      }
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
