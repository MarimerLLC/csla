using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class RoleList : NameValueListBase<int, string>
  {

    #region Business Methods

    public static int DefaultRole()
    {
      return GetList().Items[0].Key;
    }

    /// <summary>
    /// Clears the in-memory RoleList cache
    /// so the list of roles is reloaded on
    /// next request.
    /// </summary>
    public static void InvalidateCache()
    {
      _list = null;
    }

    #endregion

    #region Constructors

    private RoleList()
    {

    }

    #endregion

    #region Factory Methods

    private static RoleList _list;

    public static RoleList GetList()
    {
      if (_list == null)
        _list = DataPortal.Fetch<RoleList>(new Criteria(typeof(RoleList)));
      return _list;
    }

    #endregion

    #region Data Access

    private void DataPortal_Fetch(Criteria criteria)
    {
      using (SqlConnection cn = new SqlConnection(DataBase.DbConn))
      {
        cn.Open();
        using (SqlCommand cm = cn.CreateCommand())
        {
          cm.CommandType = CommandType.StoredProcedure;
          cm.CommandText = "getRoles";

          using (SafeDataReader dr = new SafeDataReader(cm.ExecuteReader()))
          {
            IsReadOnly = false;
            while (dr.Read())
            {
              this.Add(new NameValuePair(dr.GetInt32("id"), dr.GetString("name")));
            }
            IsReadOnly = true;
          }
        }
        cn.Close();
      }
    }

    #endregion
  }
}
