using System.ComponentModel.DataAnnotations;
using Csla;
using Csla.Rules;

namespace Csla.Blazor.Test
{
  [Serializable]
  public class PersonEdit : BusinessBase<PersonEdit>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public static readonly PropertyInfo<int> NameLengthProperty = RegisterProperty<int>(nameof(NameLength));
    public int NameLength
    {
      get => GetProperty(NameLengthProperty);
      set => SetProperty(NameLengthProperty, value);
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [ObjectAuthorizationRules]
    public static void AddObjectAuthorizationRules()
    {
      //Csla.Rules.BusinessRules.AddRule(typeof(PersonEdit),
      //  new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.CreateObject, "Admin"));
      //Csla.Rules.BusinessRules.AddRule(typeof(PersonEdit),
      //  new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.EditObject, "Admin"));
      //Csla.Rules.BusinessRules.AddRule(typeof(PersonEdit),
      //  new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.DeleteObject, "Admin"));
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new CheckCase(NameProperty));
      BusinessRules.AddRule(new NoZAllowed(NameProperty));
    }

    [Create]
    private void Create()
    {
      Id = -1;
      base.Child_Create();
    }

    public class CheckCase : BusinessRule
    {
      public CheckCase(Csla.Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      { }

      protected override void Execute(IRuleContext context)
      {
        var text = (string)ReadProperty(context.Target, PrimaryProperty);
        if (string.IsNullOrWhiteSpace(text)) return;
        var ideal = text.Substring(0, 1).ToUpper();
        ideal += text.Substring(1).ToLower();
        if (text != ideal)
          context.AddErrorResult("Check capitalization");
      }
    }

    public class NoZAllowed : BusinessRule
    {
      public NoZAllowed(Csla.Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      { }

      protected override void Execute(IRuleContext context)
      {
        var text = (string)ReadProperty(context.Target, PrimaryProperty);
        if (text.ToLower().Contains("z"))
          context.AddErrorResult("No letter Z allowed");
      }
    }

  }
}
