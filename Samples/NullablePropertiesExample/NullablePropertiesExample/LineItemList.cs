using Csla;

namespace NullablePropertiesExample
{
  public partial class LineItemList : BusinessListBase<LineItemList, LineItemEdit>
  {
    [FetchChild]
    private void FetchChild(int orderId, [Inject] IChildDataPortal<LineItemEdit> itemPortal)
    {
      Console.WriteLine($"  (fetching line items for order {orderId})");
      using (LoadListMode)
      {
        Add(itemPortal.FetchChild("Widget", 2));
        Add(itemPortal.FetchChild("Gadget", 1));
      }
    }
  }
}
