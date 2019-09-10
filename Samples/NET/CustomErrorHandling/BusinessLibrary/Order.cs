using System;
using System.ComponentModel.DataAnnotations;
using Csla;

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
      private set => LoadProperty(IdProperty, value);
    }

    public static readonly PropertyInfo<string> CustomerNameProperty = RegisterProperty<string>(nameof(CustomerName));
    [Required]
    public string CustomerName
    {
      get => GetProperty(CustomerNameProperty);
      set => SetProperty(CustomerNameProperty, value);
    }

    public static readonly PropertyInfo<LineItems> LineItemsProperty = RegisterProperty<LineItems>(nameof(LineItems));
    public LineItems LineItems
    {
      get => LazyGetProperty(LineItemsProperty, () => DataPortal.CreateChild<LineItems>());
    }
  }
}
