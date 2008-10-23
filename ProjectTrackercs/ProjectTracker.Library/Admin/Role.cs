using Csla;
using Csla.Data;
using System;

namespace ProjectTracker.Library
{
  namespace Admin
  {

    [Serializable()]
    public class Role : BusinessBase<Role>
    {
      #region  Business Methods

      private static PropertyInfo<int> IdProperty = 
        RegisterProperty(new PropertyInfo<int>("Id"));
      private bool _idSet;
      public int Id
      {
        get
        {
          if (!_idSet)
          {
            _idSet = true;
            SetProperty(IdProperty, GetMax() + 1);
          }
          return GetProperty(IdProperty);
        }
        set
        {
          _idSet = true;
          SetProperty(IdProperty, value);
        }
      }

      private int GetMax()
      {
        // generate a default id value
        Roles parent = (Roles)this.Parent;
        int max = 0;
        foreach (Role item in parent)
        {
          if (item.Id > max)
          {
            max = item.Id;
          }
        }
        return max;

      }

      private static PropertyInfo<string> NameProperty = RegisterProperty(typeof(Role), new PropertyInfo<string>("Name"));
      public string Name
      {
        get
        {
          return GetProperty(NameProperty);
        }
        set
        {
          SetProperty(NameProperty, value);
        }
      }

      private byte[] _timestamp = new byte[8];

      #endregion

      #region  Validation Rules

      protected override void AddBusinessRules()
      {
        ValidationRules.AddRule<Role>(NoDuplicates, IdProperty);
        ValidationRules.AddRule(
          Csla.Validation.CommonRules.StringRequired, NameProperty);
      }

      private static bool NoDuplicates<T>(
        T target, Csla.Validation.RuleArgs e) where T : Role
      {
        Roles parent = (Roles)target.Parent;
        if (parent != null)
        {
          foreach (Role item in parent)
          {
            if (item.Id == target.ReadProperty(IdProperty) && 
              !(ReferenceEquals(item, target)))
            {
              e.Description = "Role Id must be unique";
              return false;
            }
          }
        }
        return true;
      }

      #endregion

      #region  Authorization Rules

      protected override void AddAuthorizationRules()
      {
        AuthorizationRules.AllowWrite(IdProperty, "Administrator");
        AuthorizationRules.AllowWrite(NameProperty, "Administrator");
      }

      #endregion

      #region  Factory Methods

      internal static Role NewRole()
      {
        return DataPortal.CreateChild<Role>();
      }

      internal static Role GetRole(ProjectTracker.DalLinq.getRolesResult data)
      {
        return DataPortal.FetchChild<Role>(data);
      }

      private Role()
      { /* require use of factory methods */ }

      #endregion

      #region  Data Access

      private void Child_Fetch(ProjectTracker.DalLinq.getRolesResult data)
      {
        LoadProperty(IdProperty, data.Id);
        _idSet = true;
        LoadProperty(NameProperty, data.Name);
        _timestamp = data.LastChanged.ToArray();
      }

      private void Child_Insert()
      {
        using (var mgr = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(ProjectTracker.DalLinq.Database.PTracker))
        {
          System.Data.Linq.Binary lastChanged = _timestamp;
          mgr.DataContext.addRole(ReadProperty(IdProperty), ReadProperty(NameProperty), ref lastChanged);
          _timestamp = lastChanged.ToArray();
        }
      }

      private void Child_Update()
      {
        using (var mgr = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(ProjectTracker.DalLinq.Database.PTracker))
        {
          System.Data.Linq.Binary lastChanged = null;
          mgr.DataContext.updateRole(ReadProperty(IdProperty), ReadProperty(NameProperty), _timestamp, ref lastChanged);
          _timestamp = lastChanged.ToArray();
        }
      }

      private void Child_DeleteSelf()
      {
        using (var mgr = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(ProjectTracker.DalLinq.Database.PTracker))
        {
          mgr.DataContext.deleteRole(Id);
        }
      }

      #endregion

    }
  }
}