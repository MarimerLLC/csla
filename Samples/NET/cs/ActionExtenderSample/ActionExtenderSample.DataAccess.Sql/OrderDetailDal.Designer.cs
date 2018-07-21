//-----------------------------------------------------------------------
// <copyright file="OrderDetailDal.Designer.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary></summary>
// <remarks>Generated file.</remarks>
//-----------------------------------------------------------------------

using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;
using ActionExtenderSample.DataAccess;

namespace ActionExtenderSample.DataAccess.Sql
{
  /// <summary>
  /// DAL SQL Server implementation of <see cref="IOrderDetailDal"/>
  /// </summary>
  public partial class OrderDetailDal : IOrderDetailDal
  {

    #region DAL methods

    /// <summary>
    /// Inserts a new OrderDetail object in the database.
    /// </summary>
    /// <param name="orderID">The parent Order ID.</param>
    /// <param name="orderDetailID">The Order Detail ID.</param>
    /// <param name="productID">The Product ID.</param>
    /// <param name="purchaseUnitPrice">The Purchase Unit Price.</param>
    /// <param name="quantity">The Quantity.</param>
    public void Insert(Guid orderID, Guid orderDetailID, Guid productID, decimal purchaseUnitPrice, int quantity)
    {
      using (var ctx = ConnectionManager<SqlConnection>.GetManager("ActionExtenderSample"))
      {
        using (var cmd = new SqlCommand("AddOrderDetail", ctx.Connection))
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("@OrderID", orderID).DbType = DbType.Guid;
          cmd.Parameters.AddWithValue("@OrderDetailID", orderDetailID).DbType = DbType.Guid;
          cmd.Parameters.AddWithValue("@ProductID", productID).DbType = DbType.Guid;
          cmd.Parameters.AddWithValue("@PurchaseUnitPrice", purchaseUnitPrice).DbType = DbType.Decimal;
          cmd.Parameters.AddWithValue("@Quantity", quantity).DbType = DbType.Int32;
          cmd.ExecuteNonQuery();
        }
      }
    }

    /// <summary>
    /// Updates in the database all changes made to the OrderDetail object.
    /// </summary>
    /// <param name="orderDetailID">The Order Detail ID.</param>
    /// <param name="productID">The Product ID.</param>
    /// <param name="purchaseUnitPrice">The Purchase Unit Price.</param>
    /// <param name="quantity">The Quantity.</param>
    public void Update(Guid orderDetailID, Guid productID, decimal purchaseUnitPrice, int quantity)
    {
      using (var ctx = ConnectionManager<SqlConnection>.GetManager("ActionExtenderSample"))
      {
        using (var cmd = new SqlCommand("UpdateOrderDetail", ctx.Connection))
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("@OrderDetailID", orderDetailID).DbType = DbType.Guid;
          cmd.Parameters.AddWithValue("@ProductID", productID).DbType = DbType.Guid;
          cmd.Parameters.AddWithValue("@PurchaseUnitPrice", purchaseUnitPrice).DbType = DbType.Decimal;
          cmd.Parameters.AddWithValue("@Quantity", quantity).DbType = DbType.Int32;
          var rowsAffected = cmd.ExecuteNonQuery();
          if (rowsAffected == 0)
            throw new DataNotFoundException("OrderDetail");
        }
      }
    }

    /// <summary>
    /// Deletes the OrderDetail object from database.
    /// </summary>
    /// <param name="orderDetailID">The Order Detail ID.</param>
    public void Delete(Guid orderDetailID)
    {
      using (var ctx = ConnectionManager<SqlConnection>.GetManager("ActionExtenderSample"))
      {
        using (var cmd = new SqlCommand("DeleteOrderDetail", ctx.Connection))
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("@OrderDetailID", orderDetailID).DbType = DbType.Guid;
          cmd.ExecuteNonQuery();
        }
      }
    }

    #endregion

  }
}
