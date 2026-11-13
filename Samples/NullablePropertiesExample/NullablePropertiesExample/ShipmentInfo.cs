using Csla;

namespace NullablePropertiesExample
{
  public partial class ShipmentInfo : ReadOnlyBase<ShipmentInfo>
  {
    public static readonly PropertyInfo<string> CarrierProperty = RegisterProperty<string>(nameof(Carrier));
    public string Carrier
    {
      get => GetProperty(CarrierProperty)!;
      private set => LoadProperty(CarrierProperty, value);
    }

    [FetchChild]
    private async Task FetchChild(int orderId)
    {
      Console.WriteLine($"  (fetching shipment for order {orderId} in the background)");
      await Task.Delay(500);
      Carrier = "Fabrikam Freight";
    }
  }
}
