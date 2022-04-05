using System;
using System.Collections.ObjectModel;
using Csla;

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
      var portal = App.ApplicationContext.GetRequiredService<IDataPortal<Blah>>();
      Add(portal.Create("one"));
      Add(portal.Create("two"));
      Add(portal.Create("three"));
    }
  }
}
