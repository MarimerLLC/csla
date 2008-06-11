using System;
using Csla;
using Csla.Serialization;

namespace Example.Business
{
  [Serializable]
  public class AddressBase : BusinessBase<AddressBase>
  {
    #region Serialization

    //protected override object GetValue(System.Reflection.FieldInfo field)
    //{
    //  if (field.DeclaringType == typeof(AddressBase))
    //    return field.GetValue(this);
    //  else
    //    return base.GetValue(field);
    //}

    //protected override void SetValue(System.Reflection.FieldInfo field, object value)
    //{
    //  if (field.DeclaringType == typeof(AddressBase))
    //    field.SetValue(this, value);
    //  else
    //    base.SetValue(field, value);
    //}

    #endregion

    public string City { get; set; }

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
