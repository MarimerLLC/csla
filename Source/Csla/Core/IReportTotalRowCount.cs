//-----------------------------------------------------------------------
// <copyright file="IReportTotalRowCount.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement this interface in a collection</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Core
{
  /// <summary>
  /// Implement this interface in a collection
  /// to report a total row count to
  /// Csla.Web.CslaDataSource, where that
  /// row count is different from the collection's
  /// normal Count property value.
  /// </summary>
  /// <remarks>
  /// This interface is used to provide paging
  /// support for web data binding through
  /// Csla.Web.CslaDataSource. You should
  /// implement this interface in your business
  /// collection class, along with windowed
  /// data loading, to provide efficient paging
  /// support.
  /// </remarks>
  public interface IReportTotalRowCount
  {
    /// <summary>
    /// The total number of rows of available
    /// data.
    /// </summary>
    int TotalRowCount { get;}
  }
}