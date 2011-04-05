using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;
using Csla.Security;

namespace ProjectTracker.Library.Security
{
  [Serializable]
  public class PTIdentity : CslaIdentity
  {
    public static void GetPTIdentity(string username, string password, EventHandler<DataPortalResult<PTIdentity>> callback)
    {
      DataPortal.BeginFetch<PTIdentity>(new UsernameCriteria(username, password), callback);
    }

#if !SILVERLIGHT
    public static PTIdentity GetPTIdentity(string username, string password)
    {
      return DataPortal.Fetch<PTIdentity>(new UsernameCriteria(username, password));
    }

    private void DataPortal_Fetch(UsernameCriteria criteria)
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IUserDal>();
        try
        {
          var data = dal.Fetch(criteria.Username, criteria.Password);
          base.Name = data.Username;
          base.IsAuthenticated = true;
          base.AuthenticationType = "Membership";
          base.Roles = new Csla.Core.MobileList<string>(data.Roles);
        }
        catch (ProjectTracker.Dal.DataNotFoundException ex)
        {
          base.Name = string.Empty;
          base.IsAuthenticated = false;
          base.AuthenticationType = string.Empty;
          base.Roles = new Csla.Core.MobileList<string>();
        }
      }
    }
#endif
  }
}
