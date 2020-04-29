//-----------------------------------------------------------------------
// <copyright file="ObjectAdapter.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>An ObjectAdapter is used to convert data in an object </summary>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;
using System.Reflection;
using Csla.Properties;

namespace Csla.Data
{

  /// <summary>
  /// An ObjectAdapter is used to convert data in an object 
  /// or collection into a DataTable.
  /// </summary>
  public class ObjectAdapter
  {
    /// <summary>
    /// Fills the DataSet with data from an object or collection.
    /// </summary>
    /// <remarks>
    /// The name of the DataTable being filled is will be the class name of
    /// the object acting as the data source. The
    /// DataTable will be inserted if it doesn't already exist in the DataSet.
    /// </remarks>
    /// <param name="ds">A reference to the DataSet to be filled.</param>
    /// <param name="source">A reference to the object or collection acting as a data source.</param>
    public void Fill(DataSet ds, object source)
    {
      string className = source.GetType().Name;
      Fill(ds, className, source);
    }

    /// <summary>
    /// Fills the DataSet with data from an object or collection.
    /// </summary>
    /// <remarks>
    /// The name of the DataTable being filled is specified as a parameter. The
    /// DataTable will be inserted if it doesn't already exist in the DataSet.
    /// </remarks>
    /// <param name="ds">A reference to the DataSet to be filled.</param>
    /// <param name="tableName"></param>
    /// <param name="source">A reference to the object or collection acting as a data source.</param>
    public void Fill(DataSet ds, string tableName, object source)
    {
      DataTable dt;
      bool exists;

      dt = ds.Tables[tableName];
      exists = (dt != null);

      if (!exists)
        dt = new DataTable(tableName);

      Fill(dt, source);

      if (!exists)
        ds.Tables.Add(dt);
    }

    /// <summary>
    /// Fills a DataTable with data values from an object or collection.
    /// </summary>
    /// <param name="dt">A reference to the DataTable to be filled.</param>
    /// <param name="source">A reference to the object or collection acting as a data source.</param>
    public void Fill(DataTable dt, object source)
    {
      if (source == null)
        throw new ArgumentException(Resources.NothingNotValid);

      // get the list of columns from the source
      List<string> columns = GetColumns(source);
      if (columns.Count < 1) return;

      // create columns in DataTable if needed
      foreach (string column in columns)
        if (!dt.Columns.Contains(column))
          dt.Columns.Add(column);

      // get an IList and copy the data
      CopyData(dt, GetIList(source), columns);
    }

    #region DataCopyIList

    private IList GetIList(object source)
    {
      if (source is IListSource)
        return ((IListSource)source).GetList();
      else if (source is IList)
        return source as IList;
      else
      {
        // this is a regular object - create a list
        ArrayList col = new ArrayList();
        col.Add(source);
        return col;
      }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    private void CopyData(
      DataTable dt, IList ds, List<string> columns)
    {
      // load the data into the DataTable
      dt.BeginLoadData();
      for (int index = 0; index < ds.Count; index++)
      {
        DataRow dr = dt.NewRow();
        foreach (string column in columns)
        {
          try
          {
            dr[column] = GetField(ds[index], column);
          }
          catch (Exception ex)
          {
            dr[column] = ex.Message;
          }
        }
        dt.Rows.Add(dr);
      }
      dt.EndLoadData();
    }

    #endregion

    #region GetColumns

    private List<string> GetColumns(object source)
    {
      List<string> result;
      // first handle DataSet/DataTable
      object innerSource;
      IListSource iListSource = source as IListSource;
      if (iListSource != null)
        innerSource = iListSource.GetList();
      else
        innerSource = source;

      DataView dataView = innerSource as DataView;
      if (dataView != null)
        result = ScanDataView(dataView);
      else
      {
        // now handle lists/arrays/collections
        IEnumerable iEnumerable = innerSource as IEnumerable;
        if (iEnumerable != null)
        {
          Type childType = Utilities.GetChildItemType(
            innerSource.GetType());
          result = ScanObject(childType);
        }
        else
        {
          // the source is a regular object
          result = ScanObject(innerSource.GetType());
        }
      }
      return result;
    }

    private List<string> ScanDataView(DataView ds)
    {
      List<string> result = new List<string>();
      for (int field = 0; field < ds.Table.Columns.Count; field++)
        result.Add(ds.Table.Columns[field].ColumnName);
      return result;
    }

    private List<string> ScanObject(Type sourceType)
    {
      List<string> result = new List<string>();

      if (sourceType != null)
      {
        // retrieve a list of all public properties
        PropertyInfo[] props = sourceType.GetProperties();
        if (props.Length >= 0)
          for (int column = 0; column < props.Length; column++)
            if (props[column].CanRead)
              result.Add(props[column].Name);

        // retrieve a list of all public fields
        FieldInfo[] fields = sourceType.GetFields();
        if (fields.Length >= 0)
          for (int column = 0; column < fields.Length; column++)
            result.Add(fields[column].Name);
      }
      return result;
    }

    #endregion

    #region GetField

    private static string GetField(object obj, string fieldName)
    {
      string result;
      DataRowView dataRowView = obj as DataRowView;
      if (dataRowView != null)
      {
        // this is a DataRowView from a DataView
        result = dataRowView[fieldName].ToString();
      }
      else if (obj is ValueType && obj.GetType().IsPrimitive)
      {
        // this is a primitive value type
        result = obj.ToString();
      }
      else
      {
        string tmp = obj as string;
        if (tmp != null)
        {
          // this is a simple string
          result = (string)obj;
        }
        else
        {
          // this is an object or Structure
          try
          {
            Type sourceType = obj.GetType();

            // see if the field is a property
            PropertyInfo prop = sourceType.GetProperty(fieldName);

            if ((prop == null) || (!prop.CanRead))
            {
              // no readable property of that name exists - 
              // check for a field
              FieldInfo field = sourceType.GetField(fieldName);
              if (field == null)
              {
                // no field exists either, throw an exception
                throw new DataException(
                  Resources.NoSuchValueExistsException +
                  " " + fieldName);
              }
              else
              {
                // got a field, return its value
                result = field.GetValue(obj).ToString();
              }
            }
            else
            {
              // found a property, return its value
              result = prop.GetValue(obj, null).ToString();
            }
          }
          catch (Exception ex)
          {
            throw new DataException(
              Resources.ErrorReadingValueException +
              " " + fieldName, ex);
          }
        }
      }
      return result;
    }

    #endregion

  }
}