using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace BusinessLibrary
{
  [Serializable]
  [Csla.Server.ObjectFactory("DataAccess.LineItemFactory, DataAccess")]
  public class LineItem : BusinessBase<LineItem>
  {
    private static PropertyInfo<int> IdProperty = RegisterProperty(new PropertyInfo<int>("Id", "Id"));
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    private static PropertyInfo<string> NameProperty = RegisterProperty(new PropertyInfo<string>("Name", "Name"));
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(Csla.Validation.CommonRules.MinValue<int>,
        new Csla.Validation.CommonRules.MinValueRuleArgs<int>(IdProperty, 1));
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, NameProperty);
    }

    private LineItem()
    { /* require use of factory methods */ }

    internal static LineItem NewItem()
    {
      return DataPortal.Create<LineItem>();
    }
  }
}
