//-----------------------------------------------------------------------
// <copyright file="IOrderDal.Designer.cs" company="Marimer LLC">
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
  /// DAL Interface for Order type
  /// </summary>
  public partial interface IOrderDal
  {
    /// <summary>
    /// Loads a Order object from the database.
    /// </summary>
    /// <param name="orderID">The Order ID.</param>
    /// <returns>A data reader to the Order.</returns>
    IDataReader Fetch(Guid orderID);

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
    void Insert(Guid orderID, string userName, string orderNumber, SmartDate orderDate, string cardType, string cardHolder, string creditCard, string expDate);

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
    void Update(Guid orderID, string userName, string orderNumber, SmartDate orderDate, string cardType, string cardHolder, string creditCard, string expDate);

    /// <summary>
    /// Deletes the Order object from database.
    /// </summary>
    /// <param name="orderID">The Order ID.</param>
    void Delete(Guid orderID);
  }
}
