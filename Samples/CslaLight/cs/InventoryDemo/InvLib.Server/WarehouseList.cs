using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace InvLib
{
  [Serializable]
  public partial class WarehouseList : ReadOnlyListBase<WarehouseList, WarehouseInfo>
  {
    #region Factory Methods

    public static void GetWarehouseList(EventHandler<DataPortalResult<WarehouseList>> callback)
    {
      var dp = new DataPortal<WarehouseList>();
      dp.FetchCompleted += callback;
      dp.BeginFetch();
    }

    #endregion
  }
}
