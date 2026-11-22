using Csla;

namespace NullablePropertiesExample
{
  public partial class OrderHistory : ReadOnlyBase<OrderHistory>
  {
    public static readonly NotNullPropertyInfo<string> SummaryProperty =
      RegisterNotNullProperty<string>(nameof(Summary), null, string.Empty);
    public string Summary
    {
      get => GetProperty(SummaryProperty);
      private set => LoadProperty(SummaryProperty, value);
    }

    [FetchChild]
    private void FetchChild(int orderId)
    {
      Console.WriteLine($"  (fetching history for order {orderId})");
      Summary = "Placed, packed, shipped";
    }
  }
}
