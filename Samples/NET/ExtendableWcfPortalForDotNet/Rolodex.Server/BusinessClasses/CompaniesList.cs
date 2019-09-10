using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;
using Csla.Silverlight;
using System.ComponentModel;

#if!SILVERLIGHT
using System.Data.SqlClient;
using Rolodex.Business.Data;
#endif

namespace Rolodex.Business.BusinessClasses
{
  [Serializable]
  public class CompaniesList : NameValueListBase<int, string>
  {

    public static void GetCompaniesList(EventHandler<DataPortalResult<CompaniesList>> handler)
    {
      int customerID = (new Random()).Next(1, 10);
      DataPortal<CompaniesList> dp = new DataPortal<CompaniesList>();
      dp.FetchCompleted += handler;
      dp.BeginFetch();
    }


#if SILVERLIGHT

    public CompaniesList() {}

#else
   
    private void DataPortal_Fetch()
    {
      RaiseListChangedEvents = false;
      IsReadOnly = false;
      using (SqlConnection connection = new SqlConnection(DataConnection.ConnectionString))
      {
        connection.Open();
        using (SqlCommand command = new SqlCommand("GetCompanies", connection))
        {
          command.CommandType = System.Data.CommandType.StoredProcedure;
          using (Csla.Data.SafeDataReader reader = new Csla.Data.SafeDataReader(command.ExecuteReader()))
          {
            while (reader.Read())
            {
              Add(new NameValueListBase<int, string>.NameValuePair(reader.GetInt32("CompanyId"), reader.GetString("CompanyName")));
            }
          }
        }
        connection.Close();
      }
      IsReadOnly = true;
      RaiseListChangedEvents = true;
    }


#endif
  }
}
