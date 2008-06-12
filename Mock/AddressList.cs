using System;
using Csla;
using Csla.Serialization;

namespace Example.Business
{
  [Serializable]
  public class AddressList : BusinessListBase<AddressList, Address>
  {

    public override bool Equals(object theOtherAddressList)
    {
      AddressList myOtherAddressList = theOtherAddressList as AddressList;
      if (myOtherAddressList == null)
        return false;
      if (this.Count != myOtherAddressList.Count)
        return false;
      bool IsMatched = false;
      foreach (Address myAddress in this)
      {
        IsMatched = false;
        foreach (Address myOtherAddress in myOtherAddressList)
        {
          if (myAddress.Equals(myOtherAddress))
            IsMatched = true;
        }
        if (!IsMatched)
          return false;
      }
      return true;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}
