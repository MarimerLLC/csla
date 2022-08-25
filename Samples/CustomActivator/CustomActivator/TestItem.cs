using System;
using Csla;

namespace CustomActivator
{
  [Serializable]
  public class TestItem : BusinessBase<TestItem>, ITestItem
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    [Fetch]
    private void Fetch(string name)
    {
      using (BypassPropertyChecks)
      {
        Name = name;
      }
    }
  }
}
