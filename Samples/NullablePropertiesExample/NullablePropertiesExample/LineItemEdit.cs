using Csla;

namespace NullablePropertiesExample
{
  public partial class LineItemEdit : BusinessBase<LineItemEdit>
  {
    public static readonly PropertyInfo<string> ProductProperty = RegisterProperty<string>(nameof(Product));
    public string Product
    {
      get => GetProperty(ProductProperty)!;
      set => SetProperty(ProductProperty, value);
    }

    public static readonly PropertyInfo<int> QuantityProperty = RegisterProperty<int>(nameof(Quantity));
    public int Quantity
    {
      get => GetProperty(QuantityProperty);
      set => SetProperty(QuantityProperty, value);
    }

    [FetchChild]
    private void FetchChild(string product, int quantity)
    {
      using (BypassPropertyChecks)
      {
        Product = product;
        Quantity = quantity;
      }
    }
  }
}
