//-----------------------------------------------------------------------
// <copyright file="Order.Designer.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary></summary>
// <remarks>Generated file.</remarks>
//-----------------------------------------------------------------------

using System;
using System.Data;
using Csla;
using Csla.Data;
using Csla.Rules.CommonRules;
using System.ComponentModel.DataAnnotations;
using ActionExtenderSample.DataAccess;

namespace ActionExtenderSample.Business
{

  /// <summary>
  /// Order (editable root object).<br/>
  /// This is a generated <see cref="Order"/> business object.
  /// </summary>
  /// <remarks>
  /// This class contains one child collection:<br/>
  /// - <see cref="OrderDetailList"/> of type <see cref="OrderDetailCollection"/> (1:M relation to <see cref="OrderDetail"/>)
  /// </remarks>
  [Serializable]
  public partial class Order : BusinessBase<Order>
  {

    #region Business Properties

    /// <summary>
    /// Maintains metadata about <see cref="OrderID"/> property.
    /// </summary>
    [NotUndoable]
    public static readonly PropertyInfo<Guid> OrderIDProperty = RegisterProperty<Guid>(p => p.OrderID, "Order ID");
    /// <summary>
    /// Gets or sets the Order ID.
    /// </summary>
    /// <value>The Order ID.</value>
    public Guid OrderID
    {
      get { return GetProperty(OrderIDProperty); }
      private set { SetProperty(OrderIDProperty, value); }
    }

    /// <summary>
    /// Maintains metadata about <see cref="UserName"/> property.
    /// </summary>
    public static readonly PropertyInfo<string> UserNameProperty = RegisterProperty<string>(p => p.UserName, "User Name");
    /// <summary>
    /// Gets or sets the User Name.
    /// </summary>
    /// <value>The User Name.</value>
    [Required, StringLength(50)]
    public string UserName
    {
      get { return GetProperty(UserNameProperty); }
      set { SetProperty(UserNameProperty, value); }
    }

    /// <summary>
    /// Maintains metadata about <see cref="OrderNumber"/> property.
    /// </summary>
    public static readonly PropertyInfo<string> OrderNumberProperty = RegisterProperty<string>(p => p.OrderNumber, "Order Number");
    /// <summary>
    /// Gets or sets the Order Number.
    /// </summary>
    /// <value>The Order Number.</value>
    [Required, StringLength(20)]
    public string OrderNumber
    {
      get { return GetProperty(OrderNumberProperty); }
      set { SetProperty(OrderNumberProperty, value); }
    }

    /// <summary>
    /// Maintains metadata about <see cref="OrderDate"/> property.
    /// </summary>
    public static readonly PropertyInfo<SmartDate> OrderDateProperty = RegisterProperty<SmartDate>(p => p.OrderDate, "Order Date");
    /// <summary>
    /// Gets or sets the Order Date.
    /// </summary>
    /// <value>The Order Date.</value>
    [Required]
    public string OrderDate
    {
      get { return GetPropertyConvert<SmartDate, string>(OrderDateProperty); }
      set { SetPropertyConvert<SmartDate, string>(OrderDateProperty, value); }
    }

    /// <summary>
    /// Maintains metadata about <see cref="CardType"/> property.
    /// </summary>
    public static readonly PropertyInfo<string> CardTypeProperty = RegisterProperty<string>(p => p.CardType, "Card Type");
    /// <summary>
    /// Gets or sets the Card Type.
    /// </summary>
    /// <value>The Card Type.</value>
    [Required, StringLength(5)]
    public string CardType
    {
      get { return GetProperty(CardTypeProperty); }
      set { SetProperty(CardTypeProperty, value); }
    }

    /// <summary>
    /// Maintains metadata about <see cref="CardHolder"/> property.
    /// </summary>
    public static readonly PropertyInfo<string> CardHolderProperty = RegisterProperty<string>(p => p.CardHolder, "Card Holder");
    /// <summary>
    /// Gets or sets the Card Holder.
    /// </summary>
    /// <value>The Card Holder.</value>
    [Required, StringLength(100)]
    public string CardHolder
    {
      get { return GetProperty(CardHolderProperty); }
      set { SetProperty(CardHolderProperty, value); }
    }

    /// <summary>
    /// Maintains metadata about <see cref="CreditCard"/> property.
    /// </summary>
    public static readonly PropertyInfo<string> CreditCardProperty = RegisterProperty<string>(p => p.CreditCard, "Credit Card");
    /// <summary>
    /// Gets or sets the Credit Card.
    /// </summary>
    /// <value>The Credit Card.</value>
    [Required]
    public string CreditCard
    {
      get { return GetProperty(CreditCardProperty); }
      set { SetProperty(CreditCardProperty, value); }
    }

    /// <summary>
    /// Maintains metadata about <see cref="ExpDate"/> property.
    /// </summary>
    public static readonly PropertyInfo<string> ExpDateProperty = RegisterProperty<string>(p => p.ExpDate, "Exp Date");
    /// <summary>
    /// Gets or sets the Exp Date.
    /// </summary>
    /// <value>The Exp Date.</value>
    [Required, StringLength(6)]
    public string ExpDate
    {
      get { return GetProperty(ExpDateProperty); }
      set { SetProperty(ExpDateProperty, value); }
    }

    /// <summary>
    /// Maintains metadata about child <see cref="OrderDetailList"/> property.
    /// </summary>
    public static readonly PropertyInfo<OrderDetailCollection> OrderDetailListProperty = RegisterProperty<OrderDetailCollection>(p => p.OrderDetailList, "Order Detail List", RelationshipTypes.Child);
    /// <summary>
    /// Gets the Order Detail List ("parent load" child property).
    /// </summary>
    /// <value>The Order Detail List.</value>
    public OrderDetailCollection OrderDetailList
    {
      get { return GetProperty(OrderDetailListProperty); }
      private set { LoadProperty(OrderDetailListProperty, value); }
    }

    #endregion

    #region Factory Methods

    /// <summary>
    /// Factory method. Creates a new <see cref="Order"/> object.
    /// </summary>
    /// <returns>A reference to the created <see cref="Order"/> object.</returns>
    public static Order NewOrder()
    {
      return DataPortal.Create<Order>();
    }

    /// <summary>
    /// Factory method. Loads a <see cref="Order"/> object, based on given parameters.
    /// </summary>
    /// <param name="orderID">The OrderID parameter of the Order to fetch.</param>
    /// <returns>A reference to the fetched <see cref="Order"/> object.</returns>
    public static Order GetOrder(Guid orderID)
    {
      return DataPortal.Fetch<Order>(orderID);
    }

    /// <summary>
    /// Factory method. Deletes a <see cref="Order"/> object, based on given parameters.
    /// </summary>
    /// <param name="orderID">The OrderID of the Order to delete.</param>
    public static void DeleteOrder(Guid orderID)
    {
      DataPortal.Delete<Order>(orderID);
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="Order"/> class.
    /// </summary>
    /// <remarks> Do not use to create a Csla object. Use factory methods instead.</remarks>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public Order()
    {
      // Use factory methods and do not use direct creation.
    }

    #endregion

    #region Business Rules and Property Authorization

    /// <summary>
    /// Override this method in your business class to be notified when you need to set up shared business rules.
    /// </summary>
    /// <remarks>
    /// This method is automatically called by CSLA.NET when your object should associate
    /// per-type validation rules with its properties.
    /// </remarks>
    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();

      // Property Business Rules

      // OrderDate
      BusinessRules.AddRule(new MinLength(OrderDateProperty, 1));
      // CreditCard
      BusinessRules.AddRule(new MaxLength(CreditCardProperty, 20));
      BusinessRules.AddRule(new MinLength(CreditCardProperty, 20));

      AddBusinessRulesExtend();
    }

    /// <summary>
    /// Allows the set up of custom shared business rules.
    /// </summary>
    partial void AddBusinessRulesExtend();

    #endregion

    #region Data Access

    /// <summary>
    /// Loads default values for the <see cref="Order"/> object properties.
    /// </summary>
    [RunLocal]
    protected override void DataPortal_Create()
    {
      LoadProperty(OrderIDProperty, Guid.NewGuid());
      LoadProperty(OrderDateProperty, new SmartDate(DateTime.Today));
      LoadProperty(OrderDetailListProperty, DataPortal.CreateChild<OrderDetailCollection>());
      var args = new DataPortalHookArgs();
      OnCreate(args);
      base.DataPortal_Create();
    }

    /// <summary>
    /// Loads a <see cref="Order"/> object from the database, based on given criteria.
    /// </summary>
    /// <param name="orderID">The Order ID.</param>
    protected void DataPortal_Fetch(Guid orderID)
    {
      var args = new DataPortalHookArgs(orderID);
      OnFetchPre(args);
      using (var dalManager = DalFactoryActionExtenderSample.GetManager())
      {
        var dal = dalManager.GetProvider<IOrderDal>();
        var data = dal.Fetch(orderID);
        Fetch(data);
      }

      OnFetchPost(args);
      // check all object rules and property rules
      BusinessRules.CheckRules();
    }

    private void Fetch(IDataReader data)
    {
      using (var dr = new SafeDataReader(data))
      {
        if (dr.Read())
        {
          Fetch(dr);
          FetchChildren(dr);
        }
      }
    }

    /// <summary>
    /// Loads a <see cref="Order"/> object from the given SafeDataReader.
    /// </summary>
    /// <param name="dr">The SafeDataReader to use.</param>
    private void Fetch(SafeDataReader dr)
    {
      // Value properties
      LoadProperty(OrderIDProperty, dr.GetGuid("OrderID"));
      LoadProperty(UserNameProperty, dr.GetString("UserName"));
      LoadProperty(OrderNumberProperty, dr.GetString("OrderNumber"));
      LoadProperty(OrderDateProperty, dr.GetSmartDate("OrderDate", true));
      LoadProperty(CardTypeProperty, dr.GetString("CardType"));
      LoadProperty(CardHolderProperty, dr.GetString("CardHolder"));
      LoadProperty(CreditCardProperty, dr.GetString("CreditCard"));
      LoadProperty(ExpDateProperty, dr.GetString("ExpDate"));
      var args = new DataPortalHookArgs(dr);
      OnFetchRead(args);
    }

    /// <summary>
    /// Loads child objects from the given SafeDataReader.
    /// </summary>
    /// <param name="dr">The SafeDataReader to use.</param>
    private void FetchChildren(SafeDataReader dr)
    {
      dr.NextResult();
      LoadProperty(OrderDetailListProperty, DataPortal.FetchChild<OrderDetailCollection>(dr));
    }

    /// <summary>
    /// Inserts a new <see cref="Order"/> object in the database.
    /// </summary>
    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Insert()
    {
      using (var dalManager = DalFactoryActionExtenderSample.GetManager())
      {
        var args = new DataPortalHookArgs();
        OnInsertPre(args);
        var dal = dalManager.GetProvider<IOrderDal>();
        using (BypassPropertyChecks)
        {
          dal.Insert(
            OrderID,
            UserName,
            OrderNumber,
            ReadProperty(OrderDateProperty),
            CardType,
            CardHolder,
            CreditCard,
            ExpDate
          );
        }

        OnInsertPost(args);
        // flushes all pending data operations
        FieldManager.UpdateChildren(this);
      }
    }

    /// <summary>
    /// Updates in the database all changes made to the <see cref="Order"/> object.
    /// </summary>
    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Update()
    {
      using (var dalManager = DalFactoryActionExtenderSample.GetManager())
      {
        var args = new DataPortalHookArgs();
        OnUpdatePre(args);
        var dal = dalManager.GetProvider<IOrderDal>();
        using (BypassPropertyChecks)
        {
          dal.Update(
            OrderID,
            UserName,
            OrderNumber,
            ReadProperty(OrderDateProperty),
            CardType,
            CardHolder,
            CreditCard,
            ExpDate
          );
        }

        OnUpdatePost(args);
        // flushes all pending data operations
        FieldManager.UpdateChildren(this);
      }
    }

    /// <summary>
    /// Self deletes the <see cref="Order"/> object.
    /// </summary>
    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_DeleteSelf()
    {
      DataPortal_Delete(OrderID);
    }

    /// <summary>
    /// Deletes the <see cref="Order"/> object from database.
    /// </summary>
    /// <param name="orderID">The Order ID.</param>
    [Transactional(TransactionalTypes.TransactionScope)]
    private void DataPortal_Delete(Guid orderID)
    {
      using (var dalManager = DalFactoryActionExtenderSample.GetManager())
      {
        var args = new DataPortalHookArgs();
        // flushes all pending data operations
        FieldManager.UpdateChildren(this);
        OnDeletePre(args);
        var dal = dalManager.GetProvider<IOrderDal>();
        using (BypassPropertyChecks)
        {
          dal.Delete(orderID);
        }

        OnDeletePost(args);
      }
    }

    #endregion

    #region DataPortal Hooks

    /// <summary>
    /// Occurs after setting all defaults for object creation.
    /// </summary>
    partial void OnCreate(DataPortalHookArgs args);

    /// <summary>
    /// Occurs in DataPortal_Delete, after setting query parameters and before the delete operation.
    /// </summary>
    partial void OnDeletePre(DataPortalHookArgs args);

    /// <summary>
    /// Occurs in DataPortal_Delete, after the delete operation, before Commit().
    /// </summary>
    partial void OnDeletePost(DataPortalHookArgs args);

    /// <summary>
    /// Occurs after setting query parameters and before the fetch operation.
    /// </summary>
    partial void OnFetchPre(DataPortalHookArgs args);

    /// <summary>
    /// Occurs after the fetch operation (object or collection is fully loaded and set up).
    /// </summary>
    partial void OnFetchPost(DataPortalHookArgs args);

    /// <summary>
    /// Occurs after the low level fetch operation, before the data reader is destroyed.
    /// </summary>
    partial void OnFetchRead(DataPortalHookArgs args);

    /// <summary>
    /// Occurs after setting query parameters and before the update operation.
    /// </summary>
    partial void OnUpdatePre(DataPortalHookArgs args);

    /// <summary>
    /// Occurs in DataPortal_Insert, after the update operation, before setting back row identifiers (RowVersion) and Commit().
    /// </summary>
    partial void OnUpdatePost(DataPortalHookArgs args);

    /// <summary>
    /// Occurs in DataPortal_Insert, after setting query parameters and before the insert operation.
    /// </summary>
    partial void OnInsertPre(DataPortalHookArgs args);

    /// <summary>
    /// Occurs in DataPortal_Insert, after the insert operation, before setting back row identifiers (ID and RowVersion) and Commit().
    /// </summary>
    partial void OnInsertPost(DataPortalHookArgs args);

    #endregion

  }
}
