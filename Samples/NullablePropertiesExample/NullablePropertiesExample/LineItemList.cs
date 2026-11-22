using Csla;

namespace NullablePropertiesExample
{
  public partial class LineItemList : BusinessListBase<LineItemList, LineItemEdit>
  {
    [CreateChild]
    private void CreateChild()
    {
    }

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
