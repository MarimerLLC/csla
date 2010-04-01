using System;
using Csla;
using Csla.Serialization;

namespace cslalighttest.Serialization
{
  [Serializable]
  public class AddressBase : BusinessBase<AddressBase>
  {
    private static readonly PropertyInfo<string> CityProperty = RegisterProperty<string>(
      typeof(AddressBase),
      new PropertyInfo<string>("City"));

    public string City
    {
      get { return GetProperty<string>(CityProperty); }
      set { SetProperty<string>(CityProperty, value); }
    }
  }
}
