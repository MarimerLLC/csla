using System;
using Csla;

namespace CustomAuthzRules.Test
{
  [Serializable]
  public partial class TestRoot : BusinessBase<TestRoot>
  {
    [Create]
    private void Create()
    { }

    public static readonly PropertyInfo<string> CountryProperty = RegisterProperty<string>(c => c.Country);
    public string Country
    {
      get { return GetProperty(CountryProperty)!; }
      set { SetProperty(CountryProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty)!; }
      set { SetProperty(NameProperty, value); }
    }
  }
}
