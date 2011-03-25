using Csla;
using Csla.Data;
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Csla.Serialization;

namespace ProjectTracker.Library.Admin
{
  [Serializable]
  public class RoleEdit : BusinessBase<RoleEdit>
  {
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
    [Required]
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

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();

      BusinessRules.AddRule(new NextId());
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

    private class NextId : Csla.Rules.BusinessRule
    {
      protected override void Execute(Csla.Rules.RuleContext context)
      {
        var target = (RoleEdit)context.Target;
        if (!target.IdSet)
        {
          target.IdSet = true;
          RoleEditList parent = (RoleEditList)target.Parent;
          var max = parent.Max(c => c.Id);
          target.Id = max + 1;
        }
      }
    }

    internal static RoleEdit NewRole()
    {
      return DataPortal.CreateChild<RoleEdit>();
    }
#if !SILVERLIGHT
    internal static RoleEdit GetRole(object data)
    {
      return DataPortal.FetchChild<RoleEdit>(data);
    }

    private void Child_Fetch(object data)
    {
    }

    private void Child_Insert()
    {
    }

    private void Child_Update()
    {
    }

    private void Child_DeleteSelf()
    {
    }

#endif
  }
}