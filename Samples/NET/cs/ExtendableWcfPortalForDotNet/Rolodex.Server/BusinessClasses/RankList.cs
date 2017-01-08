using System;
using Csla;
using Csla.Data;
using Rolodex.Business.Data;
using RolodexEF;

namespace Rolodex.Business.BusinessClasses
{
  [Serializable]
  public class RankList : DynamicListBase<Rank>
  {
    public static void GetRankList(EventHandler<DataPortalResult<RankList>> handler)
    {
      DataPortal<RankList> dp = new DataPortal<RankList>();
      dp.FetchCompleted += handler;
      dp.BeginFetch();
    }

    protected void DataPortal_Fetch()
    {
      RaiseListChangedEvents = false;
      using (
        ObjectContextManager<RolodexEntities> manager =
          ObjectContextManager<RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
      {
        foreach (var item in manager.ObjectContext.Ranks)
        {
          Add(Rank.GetRank(item));
        }
      }
      RaiseListChangedEvents = true;
    }
  }
}