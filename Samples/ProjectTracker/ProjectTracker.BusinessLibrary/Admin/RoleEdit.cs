using Csla;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Csla.Rules;
using ProjectTracker.Dal;

namespace ProjectTracker.Library.Admin
{
  [CslaImplementProperties]
  public partial class RoleEdit : BusinessBase<RoleEdit>
  {
    public RoleEdit()
    {
      MarkAsChild();
    }

    public partial int Id { get; private set; }

    [Required]
    public partial string Name { get; set; }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public partial byte[] TimeStamp { get; set; }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();

      BusinessRules.AddRule(new NoDuplicates { PrimaryProperty = IdProperty });

      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(AuthorizationActions.WriteProperty, IdProperty, Security.Roles.Administrator));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(AuthorizationActions.WriteProperty, NameProperty, Security.Roles.Administrator));
    }

    private class NoDuplicates : BusinessRule
    {
      protected override void Execute(Csla.Rules.IRuleContext context)
      {
        if (context.Target is not RoleEdit target)
          return;
        if (target.Parent is not RoleEditList parent)
          return;

        foreach (RoleEdit item in parent)
          if (item.Id == target.ReadProperty(IdProperty) && !ReferenceEquals(item, target))
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
        Name = data.Name ?? string.Empty;
        TimeStamp = data.LastChanged ?? [];
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
        TimeStamp = item.LastChanged ?? [];
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
        TimeStamp = item.LastChanged ?? [];
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
