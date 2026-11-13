using System.Diagnostics.CodeAnalysis;
using Csla;
using Csla.Rules;
using Csla.Rules.CommonRules;

namespace NullablePropertiesExample
{
  /// <summary>
  /// Shows how to declare CSLA properties with nullable reference types enabled.
  /// </summary>
  /// <remarks>
  /// GetProperty, ReadProperty, LazyGetProperty and LazyGetPropertyAsync all
  /// return P?, because each of them can return the property's default value:
  /// when the user isn't allowed to read the property, while an async lazy load
  /// is still running, or when nothing has been loaded. Decide per property:
  /// if it can't be null in practice, keep it non-nullable and use ! in the
  /// getter; otherwise declare the property type nullable.
  /// </remarks>
  public partial class OrderEdit : BusinessBase<OrderEdit>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get => GetProperty(IdProperty);
      private set => LoadProperty(IdProperty, value);
    }

    // Non-nullable string: use !.
    // A string property defaults to string.Empty, and GetProperty returns that
    // default when the user can't read the property, so it is only null if
    // code sets it to null.
    public static readonly PropertyInfo<string> CustomerNameProperty = RegisterProperty<string>(nameof(CustomerName));
    public string CustomerName
    {
      get => GetProperty(CustomerNameProperty)!;
      set => SetProperty(CustomerNameProperty, value);
    }

    // Lazy-loaded child, synchronous: use !.
    // LazyGetProperty runs the value generator on first access and returns its
    // result, so the getter never returns null as long as the generator
    // doesn't. (Add a read authorization rule and that changes: see Credit.)
    public static readonly PropertyInfo<LineItemList> LineItemsProperty =
      RegisterProperty<LineItemList>(nameof(LineItems), RelationshipTypes.LazyLoad);
    public LineItemList LineItems
    {
      get => LazyGetProperty(LineItemsProperty,
        () => ApplicationContext.GetRequiredService<IChildDataPortal<LineItemList>>().FetchChild(Id))!;
      private set => LoadProperty(LineItemsProperty, value);
    }

    // Lazy-loaded child, asynchronous: declare it nullable.
    // LazyGetPropertyAsync starts the load and returns the default value (null)
    // right away. The real value arrives later, with a PropertyChanged event,
    // so callers have to handle null.
    // The PropertyInfo type argument stays non-nullable (PropertyInfo<ShipmentInfo>);
    // only the property is declared ShipmentInfo?.
    public static readonly PropertyInfo<ShipmentInfo> ShipmentProperty =
      RegisterProperty<ShipmentInfo>(nameof(Shipment), RelationshipTypes.LazyLoad);
    [SuppressMessage("Usage", "CSLA0007", Justification = "The guard keeps each read from starting another fetch.")]
    public ShipmentInfo? Shipment
    {
      get
      {
        // LazyGetPropertyAsync takes a Task, which is already running by the
        // time it's passed in. Only start the fetch when the value isn't
        // loaded or loading yet, so reading the property doesn't fetch again.
        if (FieldManager.FieldExists(ShipmentProperty) || PropertyIsLoading(ShipmentProperty))
          return GetProperty(ShipmentProperty);
        return LazyGetPropertyAsync(ShipmentProperty,
          ApplicationContext.GetRequiredService<IChildDataPortal<ShipmentInfo>>().FetchChildAsync(Id));
      }
    }

    // Child with a read authorization rule: declare it nullable.
    // Fetch always loads a value, but GetProperty returns the default (null)
    // to a user who isn't in the Manager role.
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

    [Fetch]
    private void Fetch(int id, [Inject] IChildDataPortal<CreditSummary> creditPortal)
    {
      using (BypassPropertyChecks)
      {
        Id = id;
        CustomerName = "Contoso";
        Credit = creditPortal.FetchChild(id);
      }
    }
  }
}
