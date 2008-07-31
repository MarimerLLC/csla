using Csla;
using Csla.Security;
using Csla.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using ProjectTracker.DalLinq.Security;

namespace ProjectTracker.Library
{
  namespace Security
  {
    [Serializable()]
    public class PTIdentity : CslaIdentity
    {
      #region  Factory Methods

      internal static PTIdentity GetIdentity(string username, string password)
      {
        return DataPortal.Fetch<PTIdentity>(new UsernameCriteria(username, password));
      }

      internal static PTIdentity GetIdentity(string username)
      {
        return DataPortal.Fetch<PTIdentity>(new SingleCriteria<PTIdentity, string>(username));
      }

      private PTIdentity()
      { /* require use of factory methods */ }

      #endregion

      #region  Data Access

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

      private void DataPortal_Fetch(SingleCriteria<PTIdentity, string> criteria)
      {
        using (var ctx = ContextManager<SecurityDataContext>.GetManager(ProjectTracker.DalLinq.Database.Security))
        {
          var data = from u in ctx.DataContext.Users
                     where u.Username == criteria.Value
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
          base.Name = user.Username;
          base.IsAuthenticated = true;
          var userRoles = new Csla.Core.MobileList<string>();
          var roles = from r in user.Roles select r;
          foreach (var role in roles)
            userRoles.Add(role.Role1);
          base.Roles = userRoles;
        }
        else
        {
          base.Name = "";
          base.IsAuthenticated = false;
          base.Roles = null;
        }
      }

      #endregion
    }
  }
}