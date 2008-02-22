using Csla;
using Csla.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using ProjectTracker.DalLinq.Security;

namespace ProjectTracker.Library
{
  namespace Security
  {
    [Serializable()]
    public class PTIdentity : ReadOnlyBase<PTIdentity>, IIdentity
    {

      #region  Business Methods

      protected override object GetIdValue()
      {
        return _name;
      }

      #region  IsInRole

      private List<string> _roles = new List<string>();

      internal bool IsInRole(string role)
      {
        return _roles.Contains(role);
      }

      #endregion

      #region  IIdentity

      private bool _isAuthenticated;
      private string _name = "";

      public string AuthenticationType
      {
        get
        {
          return "Csla";
        }
      }

      public bool IsAuthenticated
      {
        get
        {
          return _isAuthenticated;
        }
      }

      public string Name
      {
        get
        {
          return _name;
        }
      }

      #endregion

      #endregion

      #region  Factory Methods

      internal static PTIdentity UnauthenticatedIdentity()
      {
        return new PTIdentity();
      }

      internal static PTIdentity GetIdentity(string username, string password)
      {
        return DataPortal.Fetch<PTIdentity>(new CredentialsCriteria(username, password));
      }

      internal static PTIdentity GetIdentity(string username)
      {
        return DataPortal.Fetch<PTIdentity>(new LoadOnlyCriteria(username));
      }

      private PTIdentity()
      {
        // require use of factory methods
      }

      #endregion

      #region  Data Access

      [Serializable()]
      private class CredentialsCriteria
      {

        private string _username;
        private string _password;

        public string Username
        {
          get
          {
            return _username;
          }
        }

        public string Password
        {
          get
          {
            return _password;
          }
        }

        public CredentialsCriteria(string username, string password)
        {
          _username = username;
          _password = password;
        }
      }

      [Serializable()]
      private class LoadOnlyCriteria
      {

        private string mUsername;

        public string Username
        {
          get
          {
            return mUsername;
          }
        }

        public LoadOnlyCriteria(string username)
        {
          mUsername = username;
        }
      }

      private void DataPortal_Fetch(CredentialsCriteria criteria)
      {
        using (var ctx = ContextManager<SecurityDataContext>.GetManager(ProjectTracker.DalLinq.Database.Security))
        {
          var data = from u in ctx.DataContext.Users
                     where u.Username == criteria.Username && u.Password == criteria.Password
                     select u;
          if (data.Count() > 0)
            Fetch(data.Single());
          else
            Fetch(null);
        }
      }

      private void DataPortal_Fetch(LoadOnlyCriteria criteria)
      {
        using (var ctx = ContextManager<SecurityDataContext>.GetManager(ProjectTracker.DalLinq.Database.Security))
        {
          var data = from u in ctx.DataContext.Users
                     where u.Username == criteria.Username
                     select u;
          if (data.Count() > 0)
            Fetch(data.Single());
          else
            Fetch(null);
        }
      }

      private void Fetch(User user)
      {
        if (user != null)
        {
          _name = user.Username;
          _isAuthenticated = true;
          var roles = from r in user.Roles select r;
          foreach (var role in roles)
            _roles.Add(role.Role1);
        }
        else
        {
          _name = "";
          _isAuthenticated = false;
          _roles.Clear();
        }
      }

      #endregion

    }
  }
}