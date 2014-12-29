using System;
#if !WINDOWS_PHONE
using System.ComponentModel.DataAnnotations;
#endif
using Csla;
using Csla.Serialization;

namespace BusinessLibrary
{
  [Serializable]
  [Csla.Server.ObjectFactory("DataAccess.LineItemFactory, DataAccess")]
  public class LineItem : BusinessBase<LineItem>
  {
    public static PropertyInfo<int> IdProperty = RegisterProperty<int>(p => p.Id);
#if !WINDOWS_PHONE
    [Range(1, 9999)]
#endif
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(p => p.Name);
#if !WINDOWS_PHONE
    [Required]
#endif
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();

      BusinessRules.AddRule(new ChangeIfNew { PrimaryProperty = IdProperty });
    }

    private class ChangeIfNew : Csla.Rules.BusinessRule
    {
      protected override void Execute(Csla.Rules.RuleContext context)
      {
        var target = (LineItem)context.Target;
        if (!target.IsNew)
          context.AddErrorResult("Value may only be changed if item is new");
      }
    }
  }
}
