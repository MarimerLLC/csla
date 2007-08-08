using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using System.Security.Principal;

namespace ProjectTracker.Library.Security
{
  [Serializable()]
  public class PTPrincipal : Csla.Security.BusinessPrincipalBase
  {
    private PTPrincipal(IIdentity identity)
      : base(identity) { }

    public static bool VerifyCredentials(string username, string password)
    {
      return UsernamePasswordValidator.Validate(username, password);
    }

    public static bool Login(string username, string password)
    {
      return SetPrincipal(PTIdentity.GetIdentity(username, password));
    }

    public static void LoadPrincipal(string username)
    {
      SetPrincipal(PTIdentity.GetIdentity(username));
    }

    private static bool SetPrincipal(PTIdentity identity)
    {
      if (identity.IsAuthenticated)
      {
        PTPrincipal principal = new PTPrincipal(identity);
        Csla.ApplicationContext.User = principal;
      }
      return identity.IsAuthenticated;
    }

    public static void Logout()
    {
      PTIdentity identity = PTIdentity.UnauthenticatedIdentity();
      PTPrincipal principal = new PTPrincipal(identity);
      Csla.ApplicationContext.User = principal;
    }

    public override bool IsInRole(string role)
    {
      PTIdentity identity = (PTIdentity)this.Identity;
      return identity.IsInRole(role);
    }

    #region UsernamePasswordValidator

    [Serializable]
    private class UsernamePasswordValidator : ReadOnlyBase<UsernamePasswordValidator>
    {
      #region Business Methods

      private bool _validUser;
      public bool ValidUser
      {
        get { return _validUser; }
      }

      Guid _id = Guid.NewGuid();
      protected override object GetIdValue()
      {
        return _id;
      }

      #endregion

      #region Factory Methods

      public static bool Validate(string username, string password)
      {
        UsernamePasswordValidator obj = DataPortal.Fetch<UsernamePasswordValidator>(new Criteria(username, password));
        return obj.ValidUser;
      }

      private UsernamePasswordValidator()
      { /* require use of factory methods */ }

      #endregion 

      #region Data Access 

      [Serializable]
      private class Criteria
      {
        string _username;
        public string Username
        {
          get { return _username; }
        }

        string _password;
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

      private void DataPortal_Fetch(Criteria criteria)
      {
        _validUser = false;
        using (SqlConnection cn =
          new SqlConnection(Database.SecurityConnection))
        {
          cn.Open();
          using (SqlCommand cm = cn.CreateCommand())
          {
            cm.CommandText = "VerifyCredentials";
            cm.CommandType = CommandType.StoredProcedure;
            cm.Parameters.AddWithValue("@user", criteria.Username);
            cm.Parameters.AddWithValue("@pw", criteria.Password);
            using (SqlDataReader dr = cm.ExecuteReader())
            {
              if (dr.Read())
                _validUser = true;
            }
          }
        }
      }

      #endregion

    }

    #endregion
  }
}
