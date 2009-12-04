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
using Csla.Data;
using System.Data.SqlClient;
using Rolodex.Business.Data;
#endif

namespace Rolodex.Business.BusinessClasses
{
  [Serializable]
  public class ReadOnlyCompanyList : ReadOnlyListBase<ReadOnlyCompanyList,ReadOnlyCompany>
  {
#if SILVERLIGHT
    public ReadOnlyCompanyList() { }
#else
    private ReadOnlyCompanyList() { }
#endif

    public static void GetCompanyList(EventHandler<DataPortalResult<ReadOnlyCompanyList>> handler)
    {
      DataPortal<ReadOnlyCompanyList> dp = new DataPortal<ReadOnlyCompanyList>();
      dp.FetchCompleted += handler;
      dp.BeginFetch();
    }

#if !SILVERLIGHT

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
              Add(ReadOnlyCompany.GetReadOnlyCompany(reader));
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
