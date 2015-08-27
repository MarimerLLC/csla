using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;
using Csla.Security;
using System.Threading.Tasks;

namespace ProjectTracker.Library.Security
{
  [Serializable]
  public class PTIdentity : CslaIdentityBase<PTIdentity>
  {
    public static void GetPTIdentity(string username, string password, EventHandler<DataPortalResult<PTIdentity>> callback)
    {
      DataPortal.BeginFetch<PTIdentity>(new UsernameCriteria(username, password), callback);
    }

    public static async Task<PTIdentity> GetPTIdentityAsync(string username, string password)
    {
      return await DataPortal.FetchAsync<PTIdentity>(new UsernameCriteria(username, password));
    }

#if !WINDOWS_PHONE && !NETFX_CORE

    public static PTIdentity GetPTIdentity(string username, string password)
    {
      return DataPortal.Fetch<PTIdentity>(new UsernameCriteria(username, password));
    }

    internal static PTIdentity GetPTIdentity(string username)
    {
      return DataPortal.Fetch<PTIdentity>(username);
    }

    private void DataPortal_Fetch(string username)
    {
      ProjectTracker.Dal.UserDto data = null;
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IUserDal>();
        try
        {
          data = dal.Fetch(username);
        }
        catch (ProjectTracker.Dal.DataNotFoundException)
        {
          data = null;
        }
        LoadUser(data);
      }
    }

    private void DataPortal_Fetch(UsernameCriteria criteria)
    {
      ProjectTracker.Dal.UserDto data = null;
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IUserDal>();
        try
        {
          data = dal.Fetch(criteria.Username, criteria.Password);
        }
        catch (ProjectTracker.Dal.DataNotFoundException)
        {
          data = null;
        }
        LoadUser(data);
      }
    }

    private void LoadUser(ProjectTracker.Dal.UserDto data)
    {
      if (data != null)
      {
        base.Name = data.Username;
        base.IsAuthenticated = true;
        base.AuthenticationType = "Membership";
        base.Roles = new Csla.Core.MobileList<string>(data.Roles);
      }
      else
      {
        base.Name = string.Empty;
        base.IsAuthenticated = false;
        base.AuthenticationType = string.Empty;
        base.Roles = new Csla.Core.MobileList<string>();
      }
    }
#endif
  }
}
