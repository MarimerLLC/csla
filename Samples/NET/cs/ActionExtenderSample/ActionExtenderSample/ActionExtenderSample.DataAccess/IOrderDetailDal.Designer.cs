//-----------------------------------------------------------------------
// <copyright file="IOrderDetailDal.Designer.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary></summary>
// <remarks>Generated file.</remarks>
//-----------------------------------------------------------------------
using System;
using System.Data;
using Csla;

namespace ActionExtenderSample.DataAccess
{
  /// <summary>
  /// DAL Interface for OrderDetail type
  /// </summary>
  public partial interface IOrderDetailDal
  {
    /// <summary>
    /// Inserts a new OrderDetail object in the database.
    /// </summary>
    /// <param name="orderID">The parent Order ID.</param>
    /// <param name="orderDetailID">The Order Detail ID.</param>
    /// <param name="productID">The Product ID.</param>
    /// <param name="purchaseUnitPrice">The Purchase Unit Price.</param>
    /// <param name="quantity">The Quantity.</param>
    void Insert(Guid orderID, Guid orderDetailID, Guid productID, Decimal purchaseUnitPrice, int quantity);

    /// <summary>
    /// Updates in the database all changes made to the OrderDetail object.
    /// </summary>
    /// <param name="orderDetailID">The Order Detail ID.</param>
    /// <param name="productID">The Product ID.</param>
    /// <param name="purchaseUnitPrice">The Purchase Unit Price.</param>
    /// <param name="quantity">The Quantity.</param>
    void Update(Guid orderDetailID, Guid productID, Decimal purchaseUnitPrice, int quantity);

    /// <summary>
    /// Deletes the OrderDetail object from database.
    /// </summary>
    /// <param name="orderDetailID">The Order Detail ID.</param>
    void Delete(Guid orderDetailID);
  }
}
