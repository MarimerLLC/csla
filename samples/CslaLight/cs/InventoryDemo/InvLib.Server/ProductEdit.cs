using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace InvLib
{
  [Serializable]
  public partial class ProductEdit : BusinessBase<ProductEdit>
  {
    public static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      private set { SetProperty(IdProperty, value); }
    }

    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    private static PropertyInfo<float> PriceProperty = RegisterProperty<float>(c => c.Price);
    public float Price
    {
      get { return GetProperty(PriceProperty); }
      set { SetProperty(PriceProperty, value); }
    }

    private static PropertyInfo<int> CategoryIdProperty = RegisterProperty<int>(c => c.CategoryId);
    public int CategoryId
    {
      get { return GetProperty(CategoryIdProperty); }
      set { SetProperty(CategoryIdProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, NameProperty);
      ValidationRules.AddRule(Csla.Validation.CommonRules.MinValue<float>,
        new Csla.Validation.CommonRules.MinValueRuleArgs<float>(PriceProperty, 0));
    }
  }
}
