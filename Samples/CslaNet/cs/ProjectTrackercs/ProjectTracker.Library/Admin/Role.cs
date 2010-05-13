using Csla;
using Csla.Data;
using System;
using System.Linq;
using Csla.Serialization;

namespace ProjectTracker.Library.Admin
{
  [Serializable]
  public class Role : BusinessBase<Role>
  {
    #region  Business Methods

    public static PropertyInfo<bool> IdSetProperty = RegisterProperty<bool>(c => c.IdSet);
    private bool IdSet
    {
      get { return ReadProperty(IdSetProperty); }
      set { LoadProperty(IdSetProperty, value); }
    }

    public static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      private set { LoadProperty(IdProperty, value); }
    }

    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(r => r.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public static PropertyInfo<byte[]> TimeStampProperty = RegisterProperty<byte[]>(c => c.TimeStamp);
    public byte[] TimeStamp
    {
      get { return GetProperty(TimeStampProperty); }
      set { SetProperty(TimeStampProperty, value); }
    }

    #endregion

    #region  Business Rules

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new NextId());

      BusinessRules.AddRule(new NoDuplicates { PrimaryProperty = IdProperty });
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(NameProperty));

      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, IdProperty, "Administrator"));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, NameProperty, "Administrator"));
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

    private class NextId : Csla.Rules.BusinessRule
    {
      protected override void Execute(Csla.Rules.RuleContext context)
      {
        var target = (Role)context.Target;
        if (!target.IdSet)
        {
          target.IdSet = true;
          Roles parent = (Roles)target.Parent;
          var max = parent.Max(c => c.Id);
          target.Id = max + 1;
        }
      }
    }

    #endregion

    #region Factory Methods

    internal static Role NewRole()
    {
      return DataPortal.CreateChild<Role>();
    }

    #endregion
#if !SILVERLIGHT
    #region  Factory Methods

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
#endif
  }
}