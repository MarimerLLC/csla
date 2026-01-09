using Csla;
using System;
using System.Collections.Generic;
using ProjectTracker.Dal;

namespace ProjectTracker.Library
{
  namespace Admin
  {
    /// <summary>
    /// Used to maintain the list of roles
    /// in the system.
    /// </summary>
    [Serializable]
    public class RoleEditList : BusinessListBase<RoleEditList, RoleEdit>
    {
      /// <summary>
      /// Remove a role based on the role's
      /// id value.
      /// </summary>
      /// <param name="id">Id value of the role to remove.</param>
      public void Remove(int id)
      {
        foreach (RoleEdit item in this)
        {
          if (item.Id == id)
          {
            Remove(item);
            break;
          }
        }
      }

      /// <summary>
      /// Get a role based on its id value.
      /// </summary>
      /// <param name="id">Id value of the role to return.</param>
      public RoleEdit GetRoleById(int id)
      {
        foreach (RoleEdit item in this)
        {
          if (item.Id == id)
            return item;
        }
        return null!;
      }

      [ObjectAuthorizationRules]
      public static void AddObjectAuthorizationRules()
      {
        Csla.Rules.BusinessRules.AddRule(typeof(RoleEditList), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.CreateObject, Security.Roles.Administrator));
        Csla.Rules.BusinessRules.AddRule(typeof(RoleEditList), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.EditObject, Security.Roles.Administrator));
        Csla.Rules.BusinessRules.AddRule(typeof(RoleEditList), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.DeleteObject, Security.Roles.Administrator));
      }

      [Fetch]
      private void Fetch([Inject] IRoleDal dal, [Inject] IChildDataPortal<RoleEdit> roleEditPortal)
      {
        using (LoadListMode)
        {
          List<ProjectTracker.Dal.RoleDto> list = dal.Fetch();
          foreach (var item in list)
            Add(roleEditPortal.FetchChild(item));
        }
      }

      [Update]
      private void Update()
      {
        using (LoadListMode)
        {
          Child_Update();
        }
      }
    }
  }
}