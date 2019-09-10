using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace MVVMexperiment
{
  [Serializable]
  public class DataList : BusinessListBase<DataList, Data>
  {
    public static DataList GetList(int id)
    {
      return DataPortal.Fetch<DataList>();
    }

    public static void GetList(int id, EventHandler<DataPortalResult<DataList>> handler)
    {
      DataPortal.BeginFetch<DataList>(handler);
    }

    private void DataPortal_Fetch()
    {
      var rle = this.RaiseListChangedEvents;
      Add(DataPortal.FetchChild<Data>());
      Add(DataPortal.FetchChild<Data>());
      Add(DataPortal.FetchChild<Data>());
      Add(DataPortal.FetchChild<Data>());
      Add(DataPortal.FetchChild<Data>());
      this.RaiseListChangedEvents = rle;
    }

    public DataList()
    {
      AllowNew = true;
    }

    protected override Data AddNewCore()
    {
      var item = DataPortal.CreateChild<Data>();
      Add(item);
      return item;
    }
  }
}
