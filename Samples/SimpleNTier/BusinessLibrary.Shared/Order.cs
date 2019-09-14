using System;
using System.ComponentModel.DataAnnotations;
using Csla;
using Csla.Rules.CommonRules;

namespace BusinessLibrary
{
  [Serializable]
  [Csla.Server.ObjectFactory("DataAccess.OrderFactory, DataAccess")]
  public class Order : BusinessBase<Order>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get => GetProperty(IdProperty);
      private set => SetProperty(IdProperty, value);
    }

    public static readonly PropertyInfo<string> LastNameProperty = RegisterProperty<string>(nameof(LastName));
    public string LastName
    {
      get => GetProperty(LastNameProperty);
      set => SetProperty(LastNameProperty, value);
    }

    public static readonly PropertyInfo<string> CustomerNameProperty = RegisterProperty<string>(nameof(CustomerName));
    [Display(Name = "Customer name")]
    [Required]
    public string CustomerName
    {
      get => GetProperty(CustomerNameProperty);
      set => SetProperty(CustomerNameProperty, value);
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new Required(CustomerNameProperty));
    }
    
    public static readonly PropertyInfo<LineItems> LineItemsProperty = RegisterProperty<LineItems>(p => p.LineItems, RelationshipTypes.LazyLoad);
    public LineItems LineItems
    {
      get =>
        LazyGetProperty(LineItemsProperty, () => DataPortal.CreateChild<LineItems>());
    }
  }
}