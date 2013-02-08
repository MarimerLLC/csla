using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;
using Csla.Silverlight;
using Csla.Validation;

#if!SILVERLIGHT
using System.Data.SqlClient;
using Rolodex.Business.Data;
using RolodexEF;
using Csla.Data;
#endif


namespace Rolodex.Business.BusinessClasses
{
  [Serializable]
  public class RankList : EditableRootListBase<Rank>
  {

#if SILVERLIGHT
    public RankList() { }

    protected override void AddNewCore()
    {
      Add(Rank.NewRank());
    }

#else
    private RankList() { }
#endif

#if SILVERLIGHT
    public static void GetRankList(EventHandler<DataPortalResult<RankList>> handler)
    {
      DataPortal<RankList> dp = new DataPortal<RankList>();
      dp.FetchCompleted += handler;
      dp.BeginFetch();
    }
#else

    protected void DataPortal_Fetch()
    {
      RaiseListChangedEvents = false;
      using (ObjectContextManager<RolodexEntities> manager = ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
      {
        foreach (var item in manager.ObjectContext.Ranks)
        {
          Add(Rank.GetRank(item));
        }
      }
      RaiseListChangedEvents = true;
    }
#endif

  }
}
