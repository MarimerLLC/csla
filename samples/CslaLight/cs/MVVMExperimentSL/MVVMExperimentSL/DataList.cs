using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace MVVMexperiment
{
  [Serializable]
  public class DataList : BusinessListBase<DataList, Data>
  {
    public static void GetList(int id, EventHandler<DataPortalResult<DataList>> handler)
    {
      DataPortal.BeginFetch<DataList>(handler);
    }

    public void DataPortal_Fetch(Csla.DataPortalClient.LocalProxy<DataList>.CompletedHandler handler)
    {
      var rle = this.RaiseListChangedEvents;
      Add(DataPortal.FetchChild<Data>());
      Add(DataPortal.FetchChild<Data>());
      Add(DataPortal.FetchChild<Data>());
      Add(DataPortal.FetchChild<Data>());
      Add(DataPortal.FetchChild<Data>());
      this.RaiseListChangedEvents = rle;
      handler(this, null);
    }

    public DataList()
    {
      AllowNew = true;
    }

    protected override void AddNewCore()
    {
      var item = DataPortal.CreateChild<Data>();
      Add(item);
      OnAddedNew(item);
    }
  }
}
