//-----------------------------------------------------------------------
// <copyright file="SelectObjectArgs.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Argument object used in the SelectObject event.</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;

namespace Csla.Web
{
/// <summary>
/// Argument object used in the SelectObject event.
/// </summary>
  [Serializable]
  public class SelectObjectArgs : EventArgs
  {

    private object _businessObject;
    private string _sortExpression;
    private string _sortProperty;
    private ListSortDirection _sortDirection;
    private int _startRowIndex;
    private int _maximumRows;
    private bool _retrieveTotalRowCount;

    /// <summary>
    /// Get or set a reference to the business object
    /// that is created and populated by the SelectObject
    /// event handler in the web page.
    /// </summary>
    /// <value>A reference to a CSLA .NET business object.</value>
    public object BusinessObject
    {
      get { return _businessObject; }
      set { _businessObject = value; }
    }

    /// <summary>
    /// Gets the sort expression that should be used to
    /// sort the data being returned to the data source
    /// control.
    /// </summary>
    public string SortExpression
    {
      get
      {
        return _sortExpression;
      }
    }

    /// <summary>
    /// Gets the property name for the sort if only one
    /// property/column name is specified.
    /// </summary>
    /// <remarks>
    /// If multiple properties/columns are specified
    /// for the sort, you must parse the value from
    /// <see cref="SortExpression"/> to find all the
    /// property names and sort directions for the sort.
    /// </remarks>
    public string SortProperty
    {
      get
      {
        return _sortProperty;
      }
    }

    /// <summary>
    /// Gets the sort direction for the sort if only
    /// one property/column name is specified.
    /// </summary>
    /// <remarks>
    /// If multiple properties/columns are specified
    /// for the sort, you must parse the value from
    /// <see cref="SortExpression"/> to find all the
    /// property names and sort directions for the sort.
    /// </remarks>
    public ListSortDirection SortDirection
    {
      get
      {
        return _sortDirection;
      }
    }

    /// <summary>
    /// Gets the index for the first row that will be
    /// displayed. This should be the first row in
    /// the resulting collection set into the
    /// <see cref="BusinessObject"/> property.
    /// </summary>
    public int StartRowIndex
    {
      get
      {
        return _startRowIndex;
      }
    }

    /// <summary>
    /// Gets the maximum number of rows that
    /// should be returned as a result of this
    /// query. For paged collections, this is the
    /// page size.
    /// </summary>
    public int MaximumRows
    {
      get
      {
        return _maximumRows;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the
    /// query should return the total row count
    /// through the
    /// <see cref="Csla.Core.IReportTotalRowCount"/>
    /// interface.
    /// </summary>
    public bool RetrieveTotalRowCount
    {
      get
      {
        return _retrieveTotalRowCount;
      }
    }

    /// <summary>
    /// Creates an instance of the object, initializing
    /// it with values from data binding.
    /// </summary>
    /// <param name="args">Values provided from data binding.</param>
    public SelectObjectArgs(System.Web.UI.DataSourceSelectArguments args)
    {

      _startRowIndex = args.StartRowIndex;
      _maximumRows = args.MaximumRows;
      _retrieveTotalRowCount = args.RetrieveTotalRowCount;

      _sortExpression = args.SortExpression;
      if (!(string.IsNullOrEmpty(_sortExpression)))
      {
        if (_sortExpression.Length >= 5 &&
          _sortExpression.Substring(_sortExpression.Length - 5) == " DESC")
        {
          _sortProperty = _sortExpression.Substring(0, _sortExpression.Length - 5);
          _sortDirection = ListSortDirection.Descending;

        }
        else
        {
          _sortProperty = args.SortExpression;
          _sortDirection = ListSortDirection.Ascending;
        }
      }
    }
  }
}
