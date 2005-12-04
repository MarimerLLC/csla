using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Security.Principal;
using Csla;

namespace ProjectTracker.Library.Security
{

    [Serializable()]
    public class PTIdentity : ReadOnlyBase<PTIdentity>, IIdentity
    {

      #region IsInRole

      private List<string> _roles = new List<string>();

      internal bool IsInRole(string role)
      {
        return _roles.Contains(role);
      }

      #endregion

      #region IIdentity

      private bool _isAuthenticated;
      private string _name = string.Empty;

      public string AuthenticationType
      {
        get { return "Csla"; }
      }

      public bool IsAuthenticated
      {
        get { return _isAuthenticated; }
      }

      public string Name
      {
        get { return _name; }
      }

      #endregion

      #region Constructors

      private PTIdentity()
      {

      }

      #endregion

      #region Criteria

      [Serializable()]
      private class Criteria
      {

        private string _username;
        public string Username
        {
          get { return _username; }
        }

        private string _password;
        public string Password
        {
          get { return _password; }
        }

        public Criteria(string username, string password)
        {
          _username = username;
          _password = password;
        }

      }

      #endregion

      #region Factory Methods

      internal static PTIdentity UnauthenticatedIdentity()
      {
        return new PTIdentity();
      }

      internal static PTIdentity GetIdentity(string username, string password)
      {
        return DataPortal.Fetch<PTIdentity>(new Criteria(username, password));
      }

      #endregion

      #region Data Access

      private void DataPortal_Fetch(Criteria criteria)
      {
        using (SqlConnection cn = new SqlConnection(DataBase.SecurityConn))
        {
          cn.Open();
          using (SqlCommand cm = cn.CreateCommand())
          {
            cm.CommandText = "Login";
            cm.CommandType = CommandType.StoredProcedure;
            cm.Parameters.AddWithValue("@user", criteria.Username);
            cm.Parameters.AddWithValue("@pw", criteria.Password);
            using (SqlDataReader dr = cm.ExecuteReader())
            {
              if (dr.Read())
              {
                _name = criteria.Username;
                _isAuthenticated = true;
                if (dr.NextResult())
                {
                  while (dr.Read())
                  {
                    _roles.Add(dr.GetString(0));
                  }
                }
              }
              else
              {
                _name = string.Empty;
                _isAuthenticated = false;
              }
              dr.Close();
            }
          }
          cn.Close();
        }
      }

      #endregion

    }
}
