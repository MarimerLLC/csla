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
  public class RanksFactory
  {

#if!SILVERLIGHT

    public Ranks FetchRanks()
    {
      Ranks returnValue = new Ranks();
      returnValue.RaiseListChangedEvents = false;
      returnValue.SetReadOnlyFlag(false);
      returnValue.Add(new Ranks.NameValuePair(0, ""));

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
              returnValue.Add(new Ranks.NameValuePair(reader.GetInt32("RankID"), reader.GetString("Rank")));
            }
          }
        }
        connection.Close();
      }

      returnValue.SetReadOnlyFlag(true);
      returnValue.RaiseListChangedEvents = true;
      return returnValue;
    }
#endif
  }
}
