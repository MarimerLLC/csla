using System;
using Csla;
using Csla.Serialization;

namespace SilverlightApplication9
{
  [Serializable]
  public class AddressEditList : BusinessListBase<AddressEditList, AddressEdit>
  {
    public static AddressEditList GetList()
    {
      return DataPortal.FetchChild<AddressEditList>();
    }

    public void Child_Fetch()
    {
      RaiseListChangedEvents = false;
      Add(AddressEdit.GetAddress("Eden Prairie"));
      Add(AddressEdit.GetAddress("Aitkin"));
      Add(AddressEdit.GetAddress("Minneapolis"));
      RaiseListChangedEvents = true;
    }
  }
}
