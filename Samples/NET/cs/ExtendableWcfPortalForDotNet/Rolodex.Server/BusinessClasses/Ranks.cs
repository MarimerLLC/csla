using System;
using Csla;
using Csla.Data;
using Rolodex.Business.Data;
using RolodexEF;

namespace Rolodex.Business.BusinessClasses
{
  [Serializable]
  public class Ranks : NameValueListBase<int, string>
  {
    public static void GetRanks(EventHandler<DataPortalResult<Ranks>> handler)
    {
      DataPortal<Ranks> dp = new DataPortal<Ranks>();
      dp.FetchCompleted += handler;
      dp.BeginFetch();
    }

    public static Ranks GetRanks()
    {
      return DataPortal.Fetch<Ranks>();
    }

    private void DataPortal_Fetch()
    {
      using (
        ObjectContextManager<RolodexEntities> manager =
          ObjectContextManager<RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
      {
        RaiseListChangedEvents = false;
        IsReadOnly = false;
        foreach (var item in manager.ObjectContext.Ranks)
        {
          Add(new NameValuePair(item.RankId, item.Rank));
        }
        IsReadOnly = true;
        RaiseListChangedEvents = true;
      }
    }
  }
}