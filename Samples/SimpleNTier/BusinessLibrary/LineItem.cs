using System;
using System.ComponentModel.DataAnnotations;
using Csla;

namespace BusinessLibrary
{
  [Csla.Server.ObjectFactory("DataAccess.LineItemFactory, DataAccess")]
  [CslaImplementProperties]
  public partial class LineItem : BusinessBase<LineItem>
  {
    [Range(1, 9999)]
    public partial int Id { get; set; }

    [Required]
    public partial string Name { get; set; }

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
