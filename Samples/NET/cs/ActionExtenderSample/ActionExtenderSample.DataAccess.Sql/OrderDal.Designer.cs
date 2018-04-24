//-----------------------------------------------------------------------
// <copyright file="OrderDal.Designer.cs" company="Marimer LLC">
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
  /// DAL SQL Server implementation of <see cref="IOrderDal"/>
  /// </summary>
  public partial class OrderDal : IOrderDal
  {

    #region DAL methods

    /// <summary>
    /// Loads a Order object from the database.
    /// </summary>
    /// <param name="orderID">The Order ID.</param>
    /// <returns>A data reader to the Order.</returns>
    public IDataReader Fetch(Guid orderID)
    {
      using (var ctx = ConnectionManager<SqlConnection>.GetManager("ActionExtenderSample"))
      {
        using (var cmd = new SqlCommand("GetOrder", ctx.Connection))
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("@OrderID", orderID).DbType = DbType.Guid;
          return cmd.ExecuteReader();
        }
      }
    }

    /// <summary>
    /// Inserts a new Order object in the database.
    /// </summary>
    /// <param name="orderID">The Order ID.</param>
    /// <param name="userName">The User Name.</param>
    /// <param name="orderNumber">The Order Number.</param>
    /// <param name="orderDate">The Order Date.</param>
    /// <param name="cardType">The Card Type.</param>
    /// <param name="cardHolder">The Card Holder.</param>
    /// <param name="creditCard">The Credit Card.</param>
    /// <param name="expDate">The Exp Date.</param>
    public void Insert(Guid orderID, string userName, string orderNumber, SmartDate orderDate, string cardType, string cardHolder, string creditCard, string expDate)
    {
      using (var ctx = ConnectionManager<SqlConnection>.GetManager("ActionExtenderSample"))
      {
        using (var cmd = new SqlCommand("AddOrder", ctx.Connection))
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("@OrderID", orderID).DbType = DbType.Guid;
          cmd.Parameters.AddWithValue("@UserName", userName).DbType = DbType.String;
          cmd.Parameters.AddWithValue("@OrderNumber", orderNumber).DbType = DbType.String;
          cmd.Parameters.AddWithValue("@OrderDate", orderDate.DBValue).DbType = DbType.DateTime;
          cmd.Parameters.AddWithValue("@CardType", cardType).DbType = DbType.String;
          cmd.Parameters.AddWithValue("@CardHolder", cardHolder).DbType = DbType.String;
          cmd.Parameters.AddWithValue("@CreditCard", creditCard).DbType = DbType.String;
          cmd.Parameters.AddWithValue("@ExpDate", expDate).DbType = DbType.String;
          cmd.ExecuteNonQuery();
        }
      }
    }

    /// <summary>
    /// Updates in the database all changes made to the Order object.
    /// </summary>
    /// <param name="orderID">The Order ID.</param>
    /// <param name="userName">The User Name.</param>
    /// <param name="orderNumber">The Order Number.</param>
    /// <param name="orderDate">The Order Date.</param>
    /// <param name="cardType">The Card Type.</param>
    /// <param name="cardHolder">The Card Holder.</param>
    /// <param name="creditCard">The Credit Card.</param>
    /// <param name="expDate">The Exp Date.</param>
    public void Update(Guid orderID, string userName, string orderNumber, SmartDate orderDate, string cardType, string cardHolder, string creditCard, string expDate)
    {
      using (var ctx = ConnectionManager<SqlConnection>.GetManager("ActionExtenderSample"))
      {
        using (var cmd = new SqlCommand("UpdateOrder", ctx.Connection))
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("@OrderID", orderID).DbType = DbType.Guid;
          cmd.Parameters.AddWithValue("@UserName", userName).DbType = DbType.String;
          cmd.Parameters.AddWithValue("@OrderNumber", orderNumber).DbType = DbType.String;
          cmd.Parameters.AddWithValue("@OrderDate", orderDate.DBValue).DbType = DbType.DateTime;
          cmd.Parameters.AddWithValue("@CardType", cardType).DbType = DbType.String;
          cmd.Parameters.AddWithValue("@CardHolder", cardHolder).DbType = DbType.String;
          cmd.Parameters.AddWithValue("@CreditCard", creditCard).DbType = DbType.String;
          cmd.Parameters.AddWithValue("@ExpDate", expDate).DbType = DbType.String;
          var rowsAffected = cmd.ExecuteNonQuery();
          if (rowsAffected == 0)
            throw new DataNotFoundException("Order");
        }
      }
    }

    /// <summary>
    /// Deletes the Order object from database.
    /// </summary>
    /// <param name="orderID">The Order ID.</param>
    public void Delete(Guid orderID)
    {
      using (var ctx = ConnectionManager<SqlConnection>.GetManager("ActionExtenderSample"))
      {
        using (var cmd = new SqlCommand("DeleteOrder", ctx.Connection))
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("@OrderID", orderID).DbType = DbType.Guid;
          var rowsAffected = cmd.ExecuteNonQuery();
          if (rowsAffected == 0)
            throw new DataNotFoundException("Order");
        }
      }
    }

    #endregion

  }
}
