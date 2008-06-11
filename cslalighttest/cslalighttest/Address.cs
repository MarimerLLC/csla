using System;
using Csla.Silverlight;
using Csla.Serialization;

namespace Example.Business
{
  [Serializable]
  public class Address : AddressBase
  {
    #region Serialization

    //protected override object GetValue(System.Reflection.FieldInfo field)
    //{
    //  if (field.DeclaringType == typeof(Address))
    //    return field.GetValue(this);
    //  else
    //    return base.GetValue(field);
    //}

    //protected override void SetValue(System.Reflection.FieldInfo field, object value)
    //{
    //  if (field.DeclaringType == typeof(Address))
    //    field.SetValue(this, value);
    //  else
    //    base.SetValue(field, value);
    //}

    #endregion

    public string ZipCode { get; set; }

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
