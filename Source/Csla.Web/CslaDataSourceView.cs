//-----------------------------------------------------------------------
// <copyright file="CslaDataSourceView.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>The object responsible for managing data binding</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Web.UI;

namespace Csla.Web
{

  /// <summary>
  /// The object responsible for managing data binding
  /// to a specific CSLA .NET object.
  /// </summary>
  public class CslaDataSourceView : DataSourceView
  {

    private CslaDataSource _owner;
    private string _typeAssemblyName;
    private string _typeName;
    private bool _typeSupportsPaging;
    private bool _typeSupportsSorting;

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="owner">The CslaDataSource object
    /// that owns this view.</param>
    /// <param name="viewName">The name of the view.</param>
    public CslaDataSourceView(CslaDataSource owner, string viewName)
      : base(owner, viewName)
    {
      _owner = owner;
    }

    /// <summary>
    /// Get or set the name of the assembly (no longer used).
    /// </summary>
    /// <value>Obsolete - do not use.</value>
    public string TypeAssemblyName
    {
      get { return _typeAssemblyName; }
      set { _typeAssemblyName = value; }
    }

    /// <summary>
    /// Get or set the full type name of the business object
    /// class to be used as a data source.
    /// </summary>
    /// <value>Full type name of the business class.</value>
    public string TypeName
    {
      get { return _typeName; }
      set { _typeName = value; }
    }

    /// <summary>
    /// Get or set a value indicating whether the
    /// business object data source supports paging.
    /// </summary>
    /// <remarks>
    /// To support paging, the business object
    /// (collection) must implement 
    /// <see cref="Csla.Core.IReportTotalRowCount"/>.
    /// </remarks>
    public bool TypeSupportsPaging
    {
      get { return _typeSupportsPaging; }
      set { _typeSupportsPaging = value; }
    }

    /// <summary>
    /// Get or set a value indicating whether the
    /// business object data source supports sorting.
    /// </summary>
    public bool TypeSupportsSorting
    {
      get { return _typeSupportsSorting; }
      set { _typeSupportsSorting = value; }
    }

    #region Select

    /// <summary>
    /// Implements the select behavior for
    /// the control by raising the 
    /// <see cref="CslaDataSource.SelectObject"/> event.
    /// </summary>
    /// <param name="arguments">Arguments object.</param>
    /// <returns>The data returned from the select.</returns>
    protected override System.Collections.IEnumerable 
      ExecuteSelect(DataSourceSelectArguments arguments)
    {
      // get the object from the page
      SelectObjectArgs args = new SelectObjectArgs(arguments);
      _owner.OnSelectObject(args);
      object result = args.BusinessObject;

      if (arguments.RetrieveTotalRowCount)
      {
        int rowCount;
        if (result == null)
          rowCount = 0;
        else if (result is Csla.Core.IReportTotalRowCount)
          rowCount = ((Csla.Core.IReportTotalRowCount)result).TotalRowCount;
        else if (result is IList)
          rowCount = ((IList)result).Count;
        else if (result is IEnumerable)
        {
          IEnumerable temp = (IEnumerable)result;
          int count = 0;
          foreach (object item in temp)
            count++;
          rowCount = count;
        }
        else
          rowCount = 1;
        arguments.TotalRowCount = rowCount;
      }

      // if the result isn't IEnumerable then
      // wrap it in a collection
      if (!(result is IEnumerable))
      {
        ArrayList list = new ArrayList();
        if (result != null)
          list.Add(result);
        result = list;
      }

      // now return the object as a result
      return (IEnumerable)result;
    }

    #endregion

    #region Insert

    /// <summary>
    /// Gets a value indicating whether the data source can
    /// insert data.
    /// </summary>
    public override bool CanInsert
    {
      get
      {
        if (typeof(Csla.Core.IUndoableObject).IsAssignableFrom(
          CslaDataSource.GetType(_typeAssemblyName, _typeName)))
          return true;
        else
          return false;
      }
    }

    /// <summary>
    /// Implements the insert behavior for
    /// the control by raising the 
    /// <see cref="CslaDataSource.InsertObject"/> event.
    /// </summary>
    /// <param name="values">The values from
    /// the UI that are to be inserted.</param>
    /// <returns>The number of rows affected.</returns>
    protected override int ExecuteInsert(
      IDictionary values)
    {
      // tell the page to insert the object
      InsertObjectArgs args = 
        new InsertObjectArgs(values);
      _owner.OnInsertObject(args);
      return args.RowsAffected;
    }

    #endregion

    #region Delete

    /// <summary>
    /// Gets a value indicating whether the data source can
    /// delete data.
    /// </summary>
    public override bool CanDelete
    {
      get
      {
        if (typeof(Csla.Core.IUndoableObject).IsAssignableFrom(
          CslaDataSource.GetType(_typeAssemblyName, _typeName)))
          return true;
        else
          return false;
      }
    }

    /// <summary>
    /// Implements the delete behavior for
    /// the control by raising the 
    /// <see cref="CslaDataSource.DeleteObject"/> event.
    /// </summary>
    /// <param name="keys">The key values from
    /// the UI that are to be deleted.</param>
    /// <param name="oldValues">The old values
    /// from the UI.</param>
    /// <returns>The number of rows affected.</returns>
    protected override int ExecuteDelete(IDictionary keys, IDictionary oldValues)
    {
      
      // tell the page to delete the object
      DeleteObjectArgs args = new DeleteObjectArgs(keys, oldValues);
      _owner.OnDeleteObject(args);
      return args.RowsAffected;
    }

    #endregion

    #region Update

    /// <summary>
    /// Gets a value indicating whether the data source can
    /// update data.
    /// </summary>
    public override bool CanUpdate
    {
      get
      {
        if (typeof(Csla.Core.IUndoableObject).IsAssignableFrom(
          CslaDataSource.GetType(_typeAssemblyName, _typeName)))
          return true;
        else
          return false;
      }
    }

    /// <summary>
    /// Implements the update behavior for
    /// the control by raising the 
    /// <see cref="CslaDataSource.UpdateObject"/> event.
    /// </summary>
    /// <param name="keys">The key values from the UI
    /// that identify the object to be updated.</param>
    /// <param name="values">The values from
    /// the UI that are to be inserted.</param>
    /// <param name="oldValues">The old values
    /// from the UI.</param>
    /// <returns>The number of rows affected.</returns>
    protected override int ExecuteUpdate(IDictionary keys, IDictionary values, IDictionary oldValues)
    {
      // tell the page to update the object
      UpdateObjectArgs args = new UpdateObjectArgs(keys, values, oldValues);
      _owner.OnUpdateObject(args);
      return args.RowsAffected;
    }

    #endregion

    #region Other Operations

    /// <summary>
    /// Gets a value indicating whether the data source supports
    /// paging of the data.
    /// </summary>
    public override bool CanPage
    {
      get 
      {
        return _typeSupportsPaging;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the data source can
    /// retrieve the total number of rows of data. Always
    /// returns true.
    /// </summary>
    public override bool CanRetrieveTotalRowCount
    {
      get { return true; }
    }

    /// <summary>
    /// Gets a alue indicating whether the data source supports
    /// sorting of the data. Always returns false.
    /// </summary>
    public override bool CanSort
    {
      get 
      {
        return _typeSupportsSorting;
      }
    }

    #endregion

  }
}
