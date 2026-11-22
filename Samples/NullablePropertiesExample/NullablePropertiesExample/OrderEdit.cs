using Csla;
using Csla.Rules;
using Csla.Rules.CommonRules;

namespace NullablePropertiesExample
{
  /// <summary>
  /// Shows how to declare CSLA properties with nullable reference types enabled.
  /// </summary>
  /// <remarks>
  /// Decide per property whether it can be null. A property that always has a
  /// value once the object is complete is a not-null property
  /// (NotNullPropertyInfo, RegisterNotNullProperty): its getter returns T, and
  /// CSLA rejects null writes and checks the property when the data portal
  /// completes the object. A property that can be null (a read authorization
  /// rule, an async lazy load, an optional value) uses PropertyInfo and is
  /// declared T?. Code that doesn't migrate yet keeps PropertyInfo and uses !.
  /// </remarks>
  public partial class OrderEdit : BusinessBase<OrderEdit>
  {
    // Value type: a not-null property is never null, so there is nothing to load.
    public static readonly NotNullPropertyInfo<int> IdProperty = RegisterNotNullProperty<int>(nameof(Id));
    public int Id
    {
      get => GetProperty(IdProperty);
      private set => LoadProperty(IdProperty, value);
    }

    // Not-null string with a string.Empty default: no ! in the getter.
    // It reads string.Empty until something is loaded, and a null write
    // (for example from a data-bound control) is stored as string.Empty.
    public static readonly NotNullPropertyInfo<string> CustomerNameProperty =
      RegisterNotNullProperty<string>(nameof(CustomerName), null, string.Empty);
    public string CustomerName
    {
      get => GetProperty(CustomerNameProperty);
      set => SetProperty(CustomerNameProperty, value);
    }

    // Not-null child without a default value: every operation that completes
    // the object (Create and Fetch here) must load it, or the data portal
    // throws when the operation ends. Analyzer CSLA0033 points out an
    // operation that doesn't load it.
    public static readonly NotNullPropertyInfo<LineItemList> LineItemsProperty =
      RegisterNotNullProperty<LineItemList>(nameof(LineItems));
    public LineItemList LineItems
    {
      get => GetProperty(LineItemsProperty);
      private set => LoadProperty(LineItemsProperty, value);
    }

    // Lazy-loaded child, synchronous: not-null properties can't be lazy
    // loaded (CSLA0036), so keep PropertyInfo and use !.
    // LazyGetProperty runs the value generator on first access and returns its
    // result, so the getter never returns null as long as the generator
    // doesn't. CSLA doesn't check that for you.
    public static readonly PropertyInfo<OrderHistory> HistoryProperty =
      RegisterProperty<OrderHistory>(nameof(History), RelationshipTypes.LazyLoad);
    public OrderHistory History
    {
      get => LazyGetProperty(HistoryProperty,
        () => ApplicationContext.GetRequiredService<IChildDataPortal<OrderHistory>>().FetchChild(Id))!;
      private set => LoadProperty(HistoryProperty, value);
    }

    // Lazy-loaded child, asynchronous: declare it nullable.
    // LazyGetPropertyAsync starts the load and returns the default value (null)
    // right away. The real value arrives later, with a PropertyChanged event,
    // so callers have to handle null.
    // The PropertyInfo type argument stays non-nullable (PropertyInfo<ShipmentInfo>);
    // only the property is declared ShipmentInfo?.
    // The factory only runs when the value isn't loaded or loading yet, so
    // reading the property again doesn't start another fetch.
    public static readonly PropertyInfo<ShipmentInfo> ShipmentProperty =
      RegisterProperty<ShipmentInfo>(nameof(Shipment), RelationshipTypes.LazyLoad);
    public ShipmentInfo? Shipment =>
      LazyGetPropertyAsync(ShipmentProperty,
        () => ApplicationContext.GetRequiredService<IChildDataPortal<ShipmentInfo>>().FetchChildAsync(Id));

    // Child with a read authorization rule: declare it nullable.
    // Fetch always loads a value, but GetProperty returns the default (null)
    // to a user who isn't in the Manager role. As a not-null property without
    // a default value it would break its promise, so CSLA0034 warns about that.
    public static readonly PropertyInfo<CreditSummary> CreditProperty = RegisterProperty<CreditSummary>(nameof(Credit));
    public CreditSummary? Credit
    {
      get => GetProperty(CreditProperty);
      private set => LoadProperty(CreditProperty, value);
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new IsInRole(AuthorizationActions.ReadProperty, CreditProperty, "Manager"));
    }

    [Create]
    private void Create([Inject] IChildDataPortal<LineItemList> lineItemsPortal)
    {
      LoadProperty(LineItemsProperty, lineItemsPortal.CreateChild());
      BusinessRules.CheckRules();
    }

    [Fetch]
    private void Fetch(int id, [Inject] IChildDataPortal<LineItemList> lineItemsPortal, [Inject] IChildDataPortal<CreditSummary> creditPortal)
    {
      using (BypassPropertyChecks)
      {
        Id = id;
        CustomerName = "Contoso";
        LineItems = lineItemsPortal.FetchChild(id);
        Credit = creditPortal.FetchChild(id);
      }
    }
  }
}
