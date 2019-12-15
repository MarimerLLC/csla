using System;
using Csla;
using Csla.Security;
using System.Threading.Tasks;

namespace ProjectTracker.Library.Security
{
  [Serializable]
  public class PTIdentity : CslaIdentityBase<PTIdentity>
  {
    public static async Task<PTIdentity> GetPTIdentityAsync(string username, string password)
    {
      return await DataPortal.FetchAsync<PTIdentity>(new UsernameCriteria(username, password));
    }

    public static PTIdentity GetPTIdentity(string username, string password)
    {
      return DataPortal.Fetch<PTIdentity>(new UsernameCriteria(username, password));
    }

    internal static PTIdentity GetPTIdentity(string username)
    {
      return DataPortal.Fetch<PTIdentity>(username);
    }

    internal static async Task<PTIdentity> GetPTIdentityAsync(string username)
    {
      return await DataPortal.FetchAsync<PTIdentity>(username);
    }

    [Fetch]
    private void Fetch(string username)
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

    [Fetch]
    private void Fetch(UsernameCriteria criteria)
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
        Name = data.Username;
        IsAuthenticated = true;
        AuthenticationType = "Membership";
        Roles = new Csla.Core.MobileList<string>(data.Roles);
      }
      else
      {
        Name = string.Empty;
        IsAuthenticated = false;
        AuthenticationType = string.Empty;
        Roles = new Csla.Core.MobileList<string>();
      }
    }
  }
}
