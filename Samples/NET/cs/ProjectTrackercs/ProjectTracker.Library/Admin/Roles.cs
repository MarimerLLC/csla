using Csla;
using Csla.Data;
using Csla.Security;
using System;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  namespace Admin
  {
    /// <summary>
    /// Used to maintain the list of roles
    /// in the system.
    /// </summary>
    [Serializable()]
    public class Roles : BusinessListBase<Roles, Role>
    {
      #region  Business Methods

      /// <summary>
      /// Remove a role based on the role's
      /// id value.
      /// </summary>
      /// <param name="id">Id value of the role to remove.</param>
      public void Remove(int id)
      {
        foreach (Role item in this)
        {
          if (item.Id == id)
          {
            Remove(item);
            break;
          }
        }
      }

      /// <summary>
      /// Get a role bsaed on its id value.
      /// </summary>
      /// <param name="id">Id value of the role to return.</param>
      public Role GetRoleById(int id)
      {
        foreach (Role item in this)
        {
          if (item.Id == id)
          {
            return item;
          }
        }
        return null;
      }

#if SILVERLIGHT
      protected override void AddNewCore()
      {
        var item = Role.NewRole();
        Add(item);
        OnAddedNew(item);
      }
#else
      protected override Role AddNewCore()
      {
        Role item = Role.NewRole();
        Add(item);
        return item;
      }
#endif
      #endregion

      #region  Business Rules

      protected static void AddObjectAuthorizationRules()
      {
        Csla.Rules.BusinessRules.AddRule(typeof(Roles), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.CreateObject, "Administrator"));
        Csla.Rules.BusinessRules.AddRule(typeof(Roles), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.EditObject, "Administrator"));
        Csla.Rules.BusinessRules.AddRule(typeof(Roles), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.DeleteObject, "Administrator"));
      }

      #endregion

#if !SILVERLIGHT
      #region  Factory Methods

      public static Roles GetRoles()
      {
        return DataPortal.Fetch<Roles>();
      }

      private Roles()
      {
        this.Saved += Roles_Saved;
        this.AllowNew = true;
      }

      protected override void OnDeserialized()
      {
        base.OnDeserialized();
        this.Saved += Roles_Saved;
      }

      #endregion

      #region  Data Access

      private void Roles_Saved(object sender, Csla.Core.SavedEventArgs e)
      {
        // this runs on the client and invalidates
        // the RoleList cache
        RoleList.InvalidateCache();
      }

      protected override void DataPortal_OnDataPortalInvokeComplete(Csla.DataPortalEventArgs e)
      {
        if (ApplicationContext.ExecutionLocation == ApplicationContext.ExecutionLocations.Server &&
            e.Operation == DataPortalOperations.Update)
        {
          // this runs on the server and invalidates
          // the RoleList cache
          RoleList.InvalidateCache();
        }
      }

      private void DataPortal_Fetch()
      {
        this.RaiseListChangedEvents = false;
        using (var ctx = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(ProjectTracker.DalLinq.Database.PTracker))
        {
          foreach (var value in ctx.DataContext.getRoles())
            this.Add(Role.GetRole(value));
        }
        this.RaiseListChangedEvents = true;
      }

      [Transactional(TransactionalTypes.TransactionScope)]
      protected override void DataPortal_Update()
      {
        this.RaiseListChangedEvents = false;
        using (var ctx = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(ProjectTracker.DalLinq.Database.PTracker))
        {
          Child_Update();
        }
        this.RaiseListChangedEvents = true;
      }

      #endregion
#endif
    }
  }
}