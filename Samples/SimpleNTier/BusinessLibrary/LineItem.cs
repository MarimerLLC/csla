using System;
using System.ComponentModel.DataAnnotations;
using Csla;

namespace BusinessLibrary
{
  [Serializable]
  [Csla.Server.ObjectFactory("DataAccess.LineItemFactory, DataAccess")]
  public class LineItem : BusinessBase<LineItem>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(p => p.Id);
    [Range(1, 9999)]

    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(p => p.Name);
    [Required]
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
      protected override void Execute(Csla.Rules.IRuleContext context)
      {
        if (context.IsPropertyChangedContext)
        {
          var target = (LineItem)context.Target;
          if (!target.IsNew)
            context.AddErrorResult("Value may only be changed if item is new");
        }
      }
    }
  }
}
