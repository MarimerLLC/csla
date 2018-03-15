using Csla;
using Csla.Data;
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Csla.Serialization;
using System.ComponentModel;

namespace ProjectTracker.Library.Admin
{
  [Serializable]
  public class RoleEdit : BusinessBase<RoleEdit>
  {
    public RoleEdit()
    {
      MarkAsChild();
    }

    public readonly static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      private set { LoadProperty(IdProperty, value); }
    }

    public readonly static PropertyInfo<string> NameProperty = RegisterProperty<string>(r => r.Name);
    [Required]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public readonly static PropertyInfo<byte[]> TimeStampProperty = RegisterProperty<byte[]>(c => c.TimeStamp);
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public byte[] TimeStamp
    {
      get { return GetProperty(TimeStampProperty); }
      set { SetProperty(TimeStampProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();

      BusinessRules.AddRule(new NoDuplicates { PrimaryProperty = IdProperty });

      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, IdProperty, "Administrator"));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, NameProperty, "Administrator"));
    }

    private class NoDuplicates : Csla.Rules.BusinessRule
    {
      protected override void Execute(Csla.Rules.RuleContext context)
      {
        var target = (RoleEdit)context.Target;
        RoleEditList parent = (RoleEditList)target.Parent;
        if (parent != null)
          foreach (RoleEdit item in parent)
            if (item.Id == target.ReadProperty(IdProperty) && !(ReferenceEquals(item, target)))
            {
              context.AddErrorResult("Role Id must be unique");
              break;
            }
      }
    }

#if !FULL_DOTNET 
    public static RoleEdit NewRoleEdit()
    {
      return DataPortal.CreateChild<RoleEdit>();
    }
#else
    public static RoleEdit GetRole(int id)
    {
      return DataPortal.Fetch<RoleEdit>();
    }
    
    internal static RoleEdit GetRole(object data)
    {
      return DataPortal.FetchChild<RoleEdit>(data);
    }

    private void Child_Fetch(ProjectTracker.Dal.RoleDto data)
    {
      using (BypassPropertyChecks)
      {
        Id = data.Id;
        Name = data.Name;
        TimeStamp = data.LastChanged;
      }
    }

    private void Child_Insert()
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IRoleDal>();
        using (BypassPropertyChecks)
        {
          var item = new ProjectTracker.Dal.RoleDto
          {
            Name = this.Name
          };
          dal.Insert(item);
          Id = item.Id;
          TimeStamp = item.LastChanged;
        }
      }
   }

    private void Child_Update()
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IRoleDal>();
        using (BypassPropertyChecks)
        {
          var item = new ProjectTracker.Dal.RoleDto
          {
            Id = this.Id,
            Name = this.Name,
            LastChanged = this.TimeStamp
          };
          dal.Update(item);
          Id = item.Id;
          TimeStamp = item.LastChanged;
        }
      }
    }

    private void Child_DeleteSelf()
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IRoleDal>();
        using (BypassPropertyChecks)
        {
          dal.Delete(this.Id);
        }
      }
    }
#endif
  }
}