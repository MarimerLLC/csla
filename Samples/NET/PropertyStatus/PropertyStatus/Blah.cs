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
  public class Blah : BusinessBase<Blah>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(nameof(Data));
    public string Data
    {
      get => GetProperty(DataProperty);
      set => SetProperty(DataProperty, value);
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new AsyncRule(DataProperty));
    }

    [Fetch]
    private void Fetch()
    { }
  }

  public class BlahCollection : ObservableCollection<Blah>
  {
    public BlahCollection()
    {
      Add(DataPortal.Create<Blah>("one"));
      Add(DataPortal.Create<Blah>("two"));
      Add(DataPortal.Create<Blah>("three"));
    }
  }
}
