using Csla;

namespace NullablePropertiesExample
{
  public partial class LineItemEdit : BusinessBase<LineItemEdit>
  {
    public static readonly NotNullPropertyInfo<string> ProductProperty =
      RegisterNotNullProperty<string>(nameof(Product), null, string.Empty);
    public string Product
    {
      get => GetProperty(ProductProperty);
      set => SetProperty(ProductProperty, value);
    }

    public static readonly NotNullPropertyInfo<int> QuantityProperty = RegisterNotNullProperty<int>(nameof(Quantity));
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
