using System;
using Csla;

namespace CustomAuthzRules.Test
{
  [Serializable]
  public class TestRoot : BusinessBase<TestRoot>
  {
    public static readonly PropertyInfo<string> CountryProperty = RegisterProperty<string>(c => c.Country);
#pragma warning disable CSLA0007
    public string Country
    {
      get { return GetProperty(CountryProperty) ?? string.Empty; }
      set { SetProperty(CountryProperty, value); }
    }
#pragma warning restore CSLA0007

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
#pragma warning disable CSLA0007
    public string Name
    {
      get { return GetProperty(NameProperty) ?? string.Empty; }
      set { SetProperty(NameProperty, value); }
    }
#pragma warning restore CSLA0007
  }
}
