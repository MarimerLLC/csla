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
      using (SqlConnection connection = new SqlConnection(DataConnection.ConnectionString))
      {
        connection.Open();
        using (SqlCommand command = new SqlCommand("RanksSelect", connection))
        {
          command.CommandType = System.Data.CommandType.StoredProcedure;
          using (Csla.Data.SafeDataReader reader = new Csla.Data.SafeDataReader(command.ExecuteReader()))
          {
            while (reader.Read())
            {
              Add(Rank.GetRank(reader));
            }
          }
        }
        connection.Close();
      }
      RaiseListChangedEvents = true;
    }
#endif

  }
}
