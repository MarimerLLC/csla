//-----------------------------------------------------------------------
// <copyright file="OrderDetail.Designer.cs" company="Marimer LLC">
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
  /// OrderDetail (editable child object).<br/>
  /// This is a generated <see cref="OrderDetail"/> business object.
  /// </summary>
  /// <remarks>
  /// This class is an item of <see cref="OrderDetailCollection"/> collection.
  /// </remarks>
  [Serializable]
  public partial class OrderDetail : BusinessBase<OrderDetail>
  {

    #region Business Properties

    /// <summary>
    /// Maintains metadata about <see cref="OrderDetailID"/> property.
    /// </summary>
    [NotUndoable]
    public static readonly PropertyInfo<Guid> OrderDetailIDProperty = RegisterProperty<Guid>(p => p.OrderDetailID, "Order Detail ID");
    /// <summary>
    /// Gets or sets the Order Detail ID.
    /// </summary>
    /// <value>The Order Detail ID.</value>
    public Guid OrderDetailID
    {
      get { return GetProperty(OrderDetailIDProperty); }
      private set { SetProperty(OrderDetailIDProperty, value); }
    }

    /// <summary>
    /// Maintains metadata about <see cref="ProductID"/> property.
    /// </summary>
    public static readonly PropertyInfo<Guid> ProductIDProperty = RegisterProperty<Guid>(p => p.ProductID, "Product ID");
    /// <summary>
    /// Gets or sets the Product ID.
    /// </summary>
    /// <value>The Product ID.</value>
    [Required]
    public Guid ProductID
    {
      get { return GetProperty(ProductIDProperty); }
      set { SetProperty(ProductIDProperty, value); }
    }

    /// <summary>
    /// Maintains metadata about <see cref="PurchaseUnitPrice"/> property.
    /// </summary>
    public static readonly PropertyInfo<decimal> PurchaseUnitPriceProperty = RegisterProperty<decimal>(p => p.PurchaseUnitPrice, "Purchase Unit Price");
    /// <summary>
    /// Gets or sets the Purchase Unit Price.
    /// </summary>
    /// <value>The Purchase Unit Price.</value>
    [Required]
    public decimal PurchaseUnitPrice
    {
      get { return GetProperty(PurchaseUnitPriceProperty); }
      set { SetProperty(PurchaseUnitPriceProperty, value); }
    }

    /// <summary>
    /// Maintains metadata about <see cref="Quantity"/> property.
    /// </summary>
    public static readonly PropertyInfo<int> QuantityProperty = RegisterProperty<int>(p => p.Quantity, "Quantity");
    /// <summary>
    /// Gets or sets the Quantity.
    /// </summary>
    /// <value>The Quantity.</value>
    [Required]
    public int Quantity
    {
      get { return GetProperty(QuantityProperty); }
      set { SetProperty(QuantityProperty, value); }
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderDetail"/> class.
    /// </summary>
    /// <remarks> Do not use to create a Csla object. Use factory methods instead.</remarks>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public OrderDetail()
    {
      // Use factory methods and do not use direct creation.

      // show the framework that this is a child object
      MarkAsChild();
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

      // Quantity
      BusinessRules.AddRule(new MinValue<int>(QuantityProperty, 1));

      AddBusinessRulesExtend();
    }

    /// <summary>
    /// Allows the set up of custom shared business rules.
    /// </summary>
    partial void AddBusinessRulesExtend();

    #endregion

    #region Data Access

    /// <summary>
    /// Loads default values for the <see cref="OrderDetail"/> object properties.
    /// </summary>
    [RunLocal]
    protected override void Child_Create()
    {
      LoadProperty(OrderDetailIDProperty, Guid.NewGuid());
      var args = new DataPortalHookArgs();
      OnCreate(args);
      base.Child_Create();
    }

    /// <summary>
    /// Loads a <see cref="OrderDetail"/> object from the given SafeDataReader.
    /// </summary>
    /// <param name="dr">The SafeDataReader to use.</param>
    private void Child_Fetch(SafeDataReader dr)
    {
      // Value properties
      LoadProperty(OrderDetailIDProperty, dr.GetGuid("OrderDetailID"));
      LoadProperty(ProductIDProperty, dr.GetGuid("ProductID"));
      LoadProperty(PurchaseUnitPriceProperty, dr.GetDecimal("PurchaseUnitPrice"));
      LoadProperty(QuantityProperty, dr.GetInt32("Quantity"));
      var args = new DataPortalHookArgs(dr);
      OnFetchRead(args);
      // check all object rules and property rules
      BusinessRules.CheckRules();
    }

    /// <summary>
    /// Inserts a new <see cref="OrderDetail"/> object in the database.
    /// </summary>
    /// <param name="parent">The parent object.</param>
    [Transactional(TransactionalTypes.TransactionScope)]
    private void Child_Insert(Order parent)
    {
      using (var dalManager = DalFactoryActionExtenderSample.GetManager())
      {
        var args = new DataPortalHookArgs();
        OnInsertPre(args);
        var dal = dalManager.GetProvider<IOrderDetailDal>();
        using (BypassPropertyChecks)
        {
          dal.Insert(
            parent.OrderID,
            OrderDetailID,
            ProductID,
            PurchaseUnitPrice,
            Quantity
          );
        }

        OnInsertPost(args);
      }
    }

    /// <summary>
    /// Updates in the database all changes made to the <see cref="OrderDetail"/> object.
    /// </summary>
    [Transactional(TransactionalTypes.TransactionScope)]
    private void Child_Update()
    {
      if (!IsDirty)
        return;

      using (var dalManager = DalFactoryActionExtenderSample.GetManager())
      {
        var args = new DataPortalHookArgs();
        OnUpdatePre(args);
        var dal = dalManager.GetProvider<IOrderDetailDal>();
        using (BypassPropertyChecks)
        {
          dal.Update(
            OrderDetailID,
            ProductID,
            PurchaseUnitPrice,
            Quantity
          );
        }

        OnUpdatePost(args);
      }
    }

    /// <summary>
    /// Self deletes the <see cref="OrderDetail"/> object from database.
    /// </summary>
    [Transactional(TransactionalTypes.TransactionScope)]
    private void Child_DeleteSelf()
    {
      using (var dalManager = DalFactoryActionExtenderSample.GetManager())
      {
        var args = new DataPortalHookArgs();
        OnDeletePre(args);
        var dal = dalManager.GetProvider<IOrderDetailDal>();
        using (BypassPropertyChecks)
        {
          dal.Delete(ReadProperty(OrderDetailIDProperty));
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
