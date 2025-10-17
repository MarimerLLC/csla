//-----------------------------------------------------------------------
// <copyright file="CslaDesignerDataSourceView.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Object responsible for providing details about</summary>
//-----------------------------------------------------------------------

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Web.UI.Design;

namespace Csla.Web.Design
{
  /// <summary>
  /// Object responsible for providing details about
  /// data binding to a specific CSLA .NET object.
  /// </summary>
  public class CslaDesignerDataSourceView : DesignerDataSourceView
  {

    private readonly CslaDataSourceDesigner _owner;

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="owner"/> or <paramref name="viewName"/> is <see langword="null"/>.</exception>
    public CslaDesignerDataSourceView(CslaDataSourceDesigner owner, string viewName)
      : base(owner, viewName)
    {
      _owner = owner;
    }

    /// <summary>
    /// Returns a set of sample data used to populate
    /// controls at design time.
    /// </summary>
    /// <param name="minimumRows">Minimum number of sample rows
    /// to create.</param>
    /// <param name="isSampleData">Returns True if the data
    /// is sample data.</param>
    public override IEnumerable GetDesignTimeData(int minimumRows, out bool isSampleData)
    {
      IDataSourceViewSchema schema = Schema;
      DataTable result = new DataTable();

      // create the columns
      foreach (IDataSourceFieldSchema item in schema.GetFields())
      {
        result.Columns.Add(item.Name, item.DataType);
      }

      // create sample data
      for (int index = 1; index <= minimumRows; index++)
      {
        object?[] values = new object[result.Columns.Count];
        int colIndex = 0;
        foreach (DataColumn col in result.Columns)
        {
          if (col.DataType.Equals(typeof(string)))
            values[colIndex] = "abc";
          else if (col.DataType.Equals(typeof(DateTime)))
            values[colIndex] = DateTime.Today.ToShortDateString();
          else if (col.DataType.Equals(typeof(DateTimeOffset)))
            values[colIndex] = DateTime.Today.ToShortDateString();
          else if (col.DataType.Equals(typeof(bool)))
            values[colIndex] = false;
          else if (col.DataType.IsPrimitive)
            values[colIndex] = index;
          else if (col.DataType.Equals(typeof(Guid)))
            values[colIndex] = Guid.Empty;
          else if (col.DataType.IsValueType)
            values[colIndex] = Activator.CreateInstance(col.DataType);
          else
            values[colIndex] = null;
          colIndex += 1;
        }
        result.LoadDataRow(values, LoadOption.OverwriteChanges);
      }

      isSampleData = true;
      return result.DefaultView;
    }

    /// <summary>
    /// Returns schema information corresponding to the properties
    /// of the CSLA .NET business object.
    /// </summary>
    /// <remarks>
    /// All public properties are returned except for those marked
    /// with the <see cref="System.ComponentModel.BrowsableAttribute">Browsable attribute</see>
    /// as False.
    /// </remarks>
    public override IDataSourceViewSchema Schema =>
      new ObjectSchema(
        _owner,
        _owner.DataSourceControl.TypeName).GetViews()[0];

    /// <summary>
    /// Get a value indicating whether data binding can retrieve
    /// the total number of rows of data.
    /// </summary>
    public override bool CanRetrieveTotalRowCount => true;

    private Type GetObjectType()
    {
      Type result;
      try
      {
        var typeService = (ITypeResolutionService)(_owner.Site.GetService(typeof(ITypeResolutionService)));
        result = typeService.GetType(_owner.DataSourceControl.TypeName, true, false);
      }
      catch
      {
        result = typeof(object);
      }
      return result;
    }

    /// <summary>
    /// Get a value indicating whether data binding can directly
    /// delete the object.
    /// </summary>
    /// <remarks>
    /// If this returns true, the web page must handle the
    /// <see cref="CslaDataSource.DeleteObject">DeleteObject</see>
    /// event.
    /// </remarks>
    public override bool CanDelete
    {
      get
      {
        Type objectType = GetObjectType();
        if (typeof(Core.IUndoableObject).IsAssignableFrom(objectType))
        {
          return true;
        }
        else if (objectType.GetMethod("Remove") != null)
        {
          return true;
        }
        else
        {
          return false;
        }
      }
    }

    /// <summary>
    /// Get a value indicating whether data binding can directly
    /// insert an instance of the object.
    /// </summary>
    /// <remarks>
    /// If this returns true, the web page must handle the
    /// <see cref="CslaDataSource.InsertObject">InsertObject</see>
    /// event.
    /// </remarks>
    public override bool CanInsert
    {
      get
      {
        Type objectType = GetObjectType();
        if (typeof(Core.IUndoableObject).IsAssignableFrom(objectType))
        {
          return true;
        }
        else
        {
          return false;
        }
      }
    }

    /// <summary>
    /// Get a value indicating whether data binding can directly
    /// update or edit the object.
    /// </summary>
    /// <remarks>
    /// If this returns true, the web page must handle the
    /// <see cref="CslaDataSource.UpdateObject">UpdateObject</see>
    /// event.
    /// </remarks>
    public override bool CanUpdate
    {
      get
      {
        Type objectType = GetObjectType();
        if (typeof(Core.IUndoableObject).IsAssignableFrom(objectType))
        {
          return true;
        }
        else
        {
          return false;
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether the data source supports
    /// paging.
    /// </summary>
    public override bool CanPage => _owner.DataSourceControl.TypeSupportsPaging;

    /// <summary>
    /// Gets a value indicating whether the data source supports
    /// sorting.
    /// </summary>
    public override bool CanSort => _owner.DataSourceControl.TypeSupportsSorting;
  }
}
