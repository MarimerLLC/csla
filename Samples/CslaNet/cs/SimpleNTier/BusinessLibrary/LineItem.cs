using System;
using Csla;

namespace BusinessLibrary
{
  [Serializable]
  [Csla.Server.ObjectFactory("DataAccess.LineItemFactory, DataAccess")]
  public class LineItem : BusinessBase<LineItem>
  {
    private static PropertyInfo<int> IdProperty = RegisterProperty<int>(p => p.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(p => p.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Csla.Rules.CommonRules.MinValue<int>(IdProperty, 1));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(NameProperty));
    }

    private LineItem()
    {
    }

    internal static LineItem NewItem()
    {
      return DataPortal.Create<LineItem>();
    }
  }
}
