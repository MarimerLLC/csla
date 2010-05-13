using Csla;
using Csla.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Csla.Security;
using Csla.Serialization;
#if !SILVERLIGHT
using ProjectTracker.DalLinq.Security;
#endif

namespace ProjectTracker.Library
{
  namespace Security
  {
    [Serializable()]
    public class PTIdentity : CslaIdentity
    {
#if SILVERLIGHT
      #region Factory Methods

      internal static void GetPTIdentity(string username, string password, EventHandler<DataPortalResult<PTIdentity>> callback)
      {
        var dp = new DataPortal<PTIdentity>();
        dp.FetchCompleted += callback;
        dp.BeginFetch(new UsernameCriteria(username, password));
      }

      #endregion
#else
      #region  Factory Methods

      internal static PTIdentity GetIdentity(string username, string password)
      {
        return DataPortal.Fetch<PTIdentity>(new UsernameCriteria(username, password));
      }

      internal static PTIdentity GetIdentity(string username)
      {
        return DataPortal.Fetch<PTIdentity>(new LoadOnlyCriteria(username));
      }

      private PTIdentity()
      { /* require use of factory methods */ }

      #endregion

      #region  Data Access

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

      private void DataPortal_Fetch(UsernameCriteria criteria)
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
          Name = user.Username;
          IsAuthenticated = true;
          var roleList = new Csla.Core.MobileList<string>();
          var roles = from r in user.Roles select r;
          foreach (var role in roles)
            roleList.Add(role.Role1);
          Roles = roleList;
        }
        else
        {
          Name = "";
          IsAuthenticated = false;
          Roles = new Csla.Core.MobileList<string>();
        }
      }

      #endregion
#endif
    }
  }
}
