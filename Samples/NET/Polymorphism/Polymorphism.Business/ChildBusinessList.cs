using System;
using System.Collections.Generic;
using Csla;

namespace Polymorphism.Business
{
  [Serializable]
  public class ChildBusinessList :
    BusinessListBase<ChildBusinessList, IChild>
  {
    public static ChildBusinessList GetEditableRootList(int id)
    {
      return DataPortal.Fetch<ChildBusinessList>(id);
    }

    private void DataPortal_Fetch(int criteria)
    {
      RaiseListChangedEvents = false;

      this.Add(new ChildType1(1, "Rocky", "MVP"));
      this.Add(new ChildType2(2, "Jonny", "Norway"));

      RaiseListChangedEvents = true;
    }
  }
}
