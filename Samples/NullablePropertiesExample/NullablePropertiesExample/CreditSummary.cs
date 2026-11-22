using Csla;

namespace NullablePropertiesExample
{
  public partial class CreditSummary : ReadOnlyBase<CreditSummary>
  {
    public static readonly NotNullPropertyInfo<decimal> LimitProperty = RegisterNotNullProperty<decimal>(nameof(Limit));
    public decimal Limit
    {
      get => GetProperty(LimitProperty);
      private set => LoadProperty(LimitProperty, value);
    }

    [FetchChild]
    private void FetchChild(int orderId)
    {
      Limit = 10_000m;
    }
  }
}
