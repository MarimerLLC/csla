using System;
using System.ComponentModel.DataAnnotations;
using Csla;
using Csla.Rules.CommonRules;

namespace BusinessLibrary
{
  [Csla.Server.ObjectFactory("DataAccess.OrderFactory, DataAccess")]
  [CslaImplementProperties]
  public partial class Order : BusinessBase<Order>
  {
    public partial int Id { get; private set; }

    public partial string LastName { get; set; }

    [Display(Name = "Customer name")]
    [Required]
    public partial string CustomerName { get; set; }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new Required(CustomerNameProperty));
    }
    
    public static readonly PropertyInfo<LineItems> LineItemsProperty = RegisterProperty<LineItems>(p => p.LineItems, RelationshipTypes.LazyLoad);
#nullable disable
    public LineItems LineItems
    {
      get =>
        LazyGetProperty(LineItemsProperty, 
          () => ApplicationContext.GetRequiredService<IChildDataPortal<LineItems>>().CreateChild());
    }
#nullable enable
  }
}