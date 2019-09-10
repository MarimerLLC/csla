//-----------------------------------------------------------------------
// <copyright file="OrderDetailCollection.Designer.cs" company="Marimer LLC">
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
using ActionExtenderSample.DataAccess;

namespace ActionExtenderSample.Business
{

  /// <summary>
  /// OrderDetailCollection (editable child list).<br/>
  /// This is a generated <see cref="OrderDetailCollection"/> business object.
  /// </summary>
  /// <remarks>
  /// This class is child of <see cref="Order"/> editable root object.<br/>
  /// The items of the collection are <see cref="OrderDetail"/> objects.
  /// </remarks>
  [Serializable]
  public partial class OrderDetailCollection : BusinessBindingListBase<OrderDetailCollection, OrderDetail>
  {

    #region Collection Business Methods

    /// <summary>
    /// Removes a <see cref="OrderDetail"/> item from the collection.
    /// </summary>
    /// <param name="orderDetailID">The OrderDetailID of the item to be removed.</param>
    public void Remove(Guid orderDetailID)
    {
      foreach (var orderDetail in this)
      {
        if (orderDetail.OrderDetailID == orderDetailID)
        {
          Remove(orderDetail);
          break;
        }
      }
    }

    /// <summary>
    /// Determines whether a <see cref="OrderDetail"/> item is in the collection.
    /// </summary>
    /// <param name="orderDetailID">The OrderDetailID of the item to search for.</param>
    /// <returns><c>true</c> if the OrderDetail is a collection item; otherwise, <c>false</c>.</returns>
    public bool Contains(Guid orderDetailID)
    {
      foreach (var orderDetail in this)
      {
        if (orderDetail.OrderDetailID == orderDetailID)
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Determines whether a <see cref="OrderDetail"/> item is in the collection's DeletedList.
    /// </summary>
    /// <param name="orderDetailID">The OrderDetailID of the item to search for.</param>
    /// <returns><c>true</c> if the OrderDetail is a deleted collection item; otherwise, <c>false</c>.</returns>
    public bool ContainsDeleted(Guid orderDetailID)
    {
      foreach (var orderDetail in DeletedList)
      {
        if (orderDetail.OrderDetailID == orderDetailID)
        {
          return true;
        }
      }

      return false;
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderDetailCollection"/> class.
    /// </summary>
    /// <remarks> Do not use to create a Csla object. Use factory methods instead.</remarks>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public OrderDetailCollection()
    {
      // Use factory methods and do not use direct creation.

      // show the framework that this is a child object
      MarkAsChild();

      var rlce = RaiseListChangedEvents;
      RaiseListChangedEvents = false;
      AllowNew = true;
      AllowEdit = true;
      AllowRemove = true;
      RaiseListChangedEvents = rlce;
    }

    #endregion

    #region Data Access

    /// <summary>
    /// Loads all <see cref="OrderDetailCollection"/> collection items from the given SafeDataReader.
    /// </summary>
    /// <param name="dr">The SafeDataReader to use.</param>
    private void Child_Fetch(SafeDataReader dr)
    {
      var rlce = RaiseListChangedEvents;
      RaiseListChangedEvents = false;
      var args = new DataPortalHookArgs(dr);
      OnFetchPre(args);
      while (dr.Read())
      {
        Add(DataPortal.FetchChild<OrderDetail>(dr));
      }

      OnFetchPost(args);
      RaiseListChangedEvents = rlce;
    }

    #endregion

    #region DataPortal Hooks

    /// <summary>
    /// Occurs after setting query parameters and before the fetch operation.
    /// </summary>
    partial void OnFetchPre(DataPortalHookArgs args);

    /// <summary>
    /// Occurs after the fetch operation (object or collection is fully loaded and set up).
    /// </summary>
    partial void OnFetchPost(DataPortalHookArgs args);

    #endregion

  }
}
