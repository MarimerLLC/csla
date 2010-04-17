using Csla;
using Csla.Data;
using System;
using System.Linq;

namespace ProjectTracker.Library.Admin
{
  [Serializable]
  public class Role : BusinessBase<Role>
  {
    #region  Business Methods

    private static PropertyInfo<int> IdProperty = RegisterProperty<int>(r => r.Id);
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
      Roles parent = (Roles)Parent;
      return parent.Max(c => c.Id);
    }

    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(r => r.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    private static PropertyInfo<byte[]> TimeStampProperty = RegisterProperty<byte[]>(c => c.TimeStamp);
    public byte[] TimeStamp
    {
      get { return GetProperty(TimeStampProperty); }
      set { SetProperty(TimeStampProperty, value); }
    }

    #endregion

    #region  Validation Rules

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new NoDuplicates { PrimaryProperty = IdProperty });
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(NameProperty));
    }

    private class NoDuplicates : Csla.Rules.BusinessRule
    {
      protected override void Execute(Csla.Rules.RuleContext context)
      {
        var target = (Role)context.Target;
        Roles parent = (Roles)target.Parent;
        if (parent != null)
          foreach (Role item in parent)
            if (item.Id == target.ReadProperty(IdProperty) && !(ReferenceEquals(item, target)))
            {
              context.AddErrorResult("Role Id must be unique");
              break;
            }
      }
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
      using (BypassPropertyChecks)
      {
        Id = data.Id;
        Name = data.Name;
        TimeStamp = data.LastChanged.ToArray();
      }
    }

    private void Child_Insert()
    {
      using (var mgr = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(ProjectTracker.DalLinq.Database.PTracker))
      {
        using (BypassPropertyChecks)
        {
          System.Data.Linq.Binary lastChanged = TimeStamp;
          mgr.DataContext.addRole(Id, Name, ref lastChanged);
          TimeStamp = lastChanged.ToArray();
        }
      }
    }

    private void Child_Update()
    {
      using (var mgr = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(ProjectTracker.DalLinq.Database.PTracker))
      {
        using (BypassPropertyChecks)
        {
          System.Data.Linq.Binary lastChanged = null;
          mgr.DataContext.updateRole(Id, Name, TimeStamp, ref lastChanged);
          TimeStamp = lastChanged.ToArray();
        }
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