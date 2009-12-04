using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;
using Csla.DataPortalClient;
using System.ComponentModel;

#if!SILVERLIGHT
using System.Data.SqlClient;
using Rolodex.Business.Data;
#endif

namespace Rolodex.Business.Security
{
  [Serializable()]
  public class RolodexIdentity : CslaIdentity
  {

    private static PropertyInfo<int> UserIdProperty = 
      RegisterProperty<int>(typeof(RolodexIdentity), new PropertyInfo<int>("UserId", "User Id", 0));
    public int UserId
    {
      get
      {
        return GetProperty<int>(UserIdProperty);
      }
    }

#if SILVERLIGHT

    public RolodexIdentity() {}

    public static void GetIdentity(string username, string password, EventHandler<DataPortalResult<RolodexIdentity>> completed)
    {
      GetCslaIdentity<RolodexIdentity>(completed, new CredentialsCriteria(username, password));
    }

#else
    public static void GetIdentity(string username, string password, string roles)
    {
      GetCslaIdentity<RolodexIdentity>(new CredentialsCriteria(username, password));
    }

    private void DataPortal_Fetch(CredentialsCriteria criteria)
    {
      using (SqlConnection connection = new SqlConnection(DataConnection.ConnectionString))
      {
        connection.Open();
        using (SqlCommand command = new SqlCommand("GetUser", connection))
        {
          command.CommandType = System.Data.CommandType.StoredProcedure;
          command.Parameters.Add(new SqlParameter("@userName",criteria.Username));
          using (Csla.Data.SafeDataReader reader =  new Csla.Data.SafeDataReader(command.ExecuteReader()))
          {
            if (reader.Read())
            {
              if (criteria.Password == reader.GetString("Password"))
              {
                LoadProperty<int>(UserIdProperty, reader.GetInt32(0));
                Name = reader.GetString(1);
                Roles = new MobileList<string>(new string[] { reader.GetString("Role") });
                IsAuthenticated = true;
              }
            }
          }
        }
        connection.Close();
      }

     
    }

    
#endif
  }
}
