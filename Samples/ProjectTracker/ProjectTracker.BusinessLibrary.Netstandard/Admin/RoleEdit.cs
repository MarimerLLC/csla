using Csla;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Csla.Rules;
using ProjectTracker.Dal;

namespace ProjectTracker.Library.Admin
{
  [Serializable]
  public class RoleEdit : BusinessBase<RoleEdit>
  {
    public RoleEdit()
    {
      MarkAsChild();
    }

    public readonly static PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get { return GetProperty(IdProperty); }
      private set { LoadProperty(IdProperty, value); }
    }

    public readonly static PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    [Required]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public readonly static PropertyInfo<byte[]> TimeStampProperty = RegisterProperty<byte[]>(nameof(TimeStamp));
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

      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, IdProperty, Security.Roles.Administrator));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, NameProperty, Security.Roles.Administrator));
    }

    private class NoDuplicates : BusinessRule
    {
      protected override void Execute(Csla.Rules.IRuleContext context)
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

    [CreateChild]
    private void Create()
    {
      BusinessRules.CheckRules();
    }

    [FetchChild]
    private void Fetch(ProjectTracker.Dal.RoleDto data)
    {
      using (BypassPropertyChecks)
      {
        Id = data.Id;
        Name = data.Name;
        TimeStamp = data.LastChanged;
      }
    }

    [InsertChild]
    private void Insert([Inject] IRoleDal dal)
    {
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

    [UpdateChild]
    private void Update([Inject] IRoleDal dal)
    {
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

    [DeleteSelfChild]
    private void DeleteSelf([Inject] IRoleDal dal)
    {
      using (BypassPropertyChecks)
      {
        dal.Delete(this.Id);
      }
    }
  }
}