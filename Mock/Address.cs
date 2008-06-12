using System;
using Csla.Silverlight;
using Csla.Serialization;
using Csla;

namespace Example.Business
{
  [Serializable]
  public class Address : AddressBase
  {
    private static readonly PropertyInfo<string> ZipCodeProperty = RegisterProperty(
      typeof(Address),
      new PropertyInfo<string>("ZipCode"));

    public string ZipCode
    {
      get { return GetProperty<string>(ZipCodeProperty); }
      set { SetProperty<string>(ZipCodeProperty, value); }
    }

    public override bool Equals(object theOtherAddress)
    {
      if (!base.Equals(theOtherAddress))
        return false;
      Address myOtherAddress = theOtherAddress as Address;
      if (myOtherAddress == null)
        return false;
      return this.ZipCode.Equals(myOtherAddress.ZipCode);
    }
    public override int GetHashCode()
    {
      return (base.GetHashCode().ToString() + (ZipCode ?? "")).GetHashCode();
    }
  }
}
