using System;
using System.Collections;
using System.Security.Principal;
using System.Data;
using System.Data.SqlClient;

namespace CSLA.Security
{
  /// <summary>
  /// Implements a custom Identity class that supports
  /// CSLA .NET data access via the DataPortal.
  /// </summary>
  [Serializable()]
  public class BusinessIdentity : ReadOnlyBase, IIdentity
  {
    string _username = string.Empty;
    ArrayList _roles = new ArrayList();

    #region IIdentity

    /// <summary>
    /// Implements the IsAuthenticated property defined by IIdentity.
    /// </summary>
    bool IIdentity.IsAuthenticated
    {
      get
      {
        return (_username.Length > 0);
      }
    }

    /// <summary>
    /// Implements the AuthenticationType property defined by IIdentity.
    /// </summary>
    string IIdentity.AuthenticationType
    {
      get
      {
        return "CSLA";
      }
    }

    /// <summary>
    /// Implements the Name property defined by IIdentity.
    /// </summary>
    string IIdentity.Name
    {
      get
      {
        return _username;
      }
    }

    #endregion

    internal bool IsInRole(string role)
    {
      return _roles.Contains(role);
    }

    #region Create and Load

    internal static BusinessIdentity LoadIdentity(
                              string userName, string password)
    {
      return (BusinessIdentity)DataPortal.Fetch(
        new Criteria(userName, password));
    }

    [Serializable()]
    private class Criteria
    {
      public string Username;
      public string Password;

      public Criteria(string username, string password)
      {
        Username = username;
        Password = password;
      }
    }

    private BusinessIdentity() {} // prevent direct creation

    #endregion

    #region Data access

    /// <summary>
    /// Retrieves the identity data for a specific user.
    /// </summary>
    protected override void DataPortal_Fetch(object criteria)
    {
      Criteria crit = (Criteria)criteria;

      _roles.Clear();

      SqlConnection cn = new SqlConnection(DB("Security"));
      SqlCommand cm = new SqlCommand();
      cn.Open();
      try
      {
        cm.Connection = cn;
        cm.CommandText = "Login";
        cm.CommandType = CommandType.StoredProcedure;
        cm.Parameters.Add("@user", crit.Username);
        cm.Parameters.Add("@pw", crit.Password);
        SqlDataReader dr = cm.ExecuteReader();
        try
        {
          if(dr.Read())
          {
            _username = crit.Username;

            if(dr.NextResult())
              while(dr.Read())
                _roles.Add(dr.GetString(0));
          }
          else
            _username = string.Empty;
        }
        finally
        {
          dr.Close();
        }
      }
      finally
      {
        cn.Close();
      }
    }

    #endregion

  }
}
