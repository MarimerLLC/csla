using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace TestApp
{
  public class CustomIdentityFactory : Csla.Server.ObjectFactory
  {
    public CustomIdentity Fetch(Csla.Security.UsernameCriteria criteria)
    {
      if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["CslaIsInRoleProvider"]))
        return DoFetch(criteria);
      else
        return PermissionFetch(criteria);
    }

    private CustomIdentity DoFetch(Csla.Security.UsernameCriteria criteria)
    {
      CustomIdentity obj;
      using (var ctx = Csla.Data.ObjectContextManager<SecurityEntities>.GetManager("SecurityEntities"))
      {
        var q = (from r in ctx.ObjectContext.Users
                 where r.Username == criteria.Username
                 select r).FirstOrDefault();
        if (q != null)
        {
          q.Roles.Load();
          var roles = new List<string>();
          foreach (var r in q.Roles)
          {
            r.Permissions.Load();
            foreach (var p in r.Permissions)
              if (!roles.Contains(p.Name))
                roles.Add(p.Name);
          }
          obj = new CustomIdentity(q.Username, roles);
        }
        else
        {
          obj = new CustomIdentity();
        }
      }
      return obj;
    }

    private CustomIdentity PermissionFetch(Csla.Security.UsernameCriteria criteria)
    {
      CustomIdentity obj;
      using (var ctx = Csla.Data.ObjectContextManager<SecurityEntities>.GetManager("SecurityEntities"))
      {
        var q = (from r in ctx.ObjectContext.Users
                 where r.Username == criteria.Username
                 select r).FirstOrDefault();
        if (q != null)
        {
          q.Roles.Load();
          var roles = new List<string>();
          var permissions = new List<string>();
          foreach (var r in q.Roles)
          {
            roles.Add(r.Name);
            r.Permissions.Load();
            foreach (var p in r.Permissions)
              if (!permissions.Contains(p.Name))
                permissions.Add(p.Name);
          }
          obj = new CustomIdentity(q.Username, roles, permissions);
        }
        else
        {
          obj = new CustomIdentity();
        }
      }
      return obj;
    }
  }
}
