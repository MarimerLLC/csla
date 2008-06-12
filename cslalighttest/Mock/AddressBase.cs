using System;
using Csla;
using Csla.Serialization;

namespace Example.Business
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

    public override bool Equals(object theOtherAddressBase)
    {
      AddressBase myOtherAddressBase = theOtherAddressBase as AddressBase;
      if (myOtherAddressBase == null)
        return false;
      if (this.City == null && myOtherAddressBase.City == null)
        return true;
      if (this.City == null)
        return false;
      return this.City.Equals(myOtherAddressBase.City);
    }
    public override int GetHashCode()
    {
      return this.City.GetHashCode();
    }
  }
}
