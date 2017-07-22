using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;
using System.Threading.Tasks;

namespace Library
{
  [Csla.Server.ObjectFactory("DataAccess.DataListDal, DataAccess")]
  [Serializable]
  public class DataList : BusinessListBase<DataList, DataItem>
  {
    public int TotalRowCount { get; set; }

    public static void GetListPaged(EventHandler<DataPortalResult<DataList>> callback)
    {
      GetFirstPage((o, e) =>
        {
          if (e.Error == null && e.Object != null && e.Object.Count > 0)
            GetNextPage(0, e.Object);
          callback(o, e);
        });
    }

    private static void GetNextPage(int page, DataList targetList)
    {
      page++;
      var dp = new DataPortal<DataList>();
      dp.FetchCompleted += (o, e) =>
        {
          if (e.Error == null && e.Object != null && e.Object.Count > 0)
          {
            targetList.AddRange(e.Object);
            GetNextPage(page, targetList);
          }
        };
      dp.BeginFetch(new PagedCriteria(page, 10));
    }

    public static void GetFirstPage(EventHandler<DataPortalResult<DataList>> callback)
    {
      DataPortal.BeginFetch<DataList>(new PagedCriteria(0, 10, true), callback);
    }

    public static void GetPage(int page, EventHandler<DataPortalResult<DataList>> callback)
    {
      DataPortal.BeginFetch<DataList>(new PagedCriteria(page, 10, false), callback);
    }

    #region MobileFormatter

    protected override void OnGetState(Csla.Serialization.Mobile.SerializationInfo info)
    {
      base.OnGetState(info);
      info.AddValue("TotalRowCount", TotalRowCount);
    }

    protected override void OnSetState(Csla.Serialization.Mobile.SerializationInfo info)
    {
      base.OnSetState(info);
      TotalRowCount = info.GetValue<int>("TotalRowCount");
    }

    #endregion
  }
}
