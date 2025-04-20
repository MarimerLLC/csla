//-----------------------------------------------------------------------
// <copyright file="ObjectAdapter.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>An ObjectAdapter is used to convert data in an object </summary>
//-----------------------------------------------------------------------

using System.Collections;
using System.Data;
using System.ComponentModel;
using System.Reflection;
using Csla.Properties;
using System.Diagnostics.CodeAnalysis;

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
    /// <exception cref="ArgumentNullException"><paramref name="ds"/> or <paramref name="source"/> is <see langword="null"/>.</exception>
    public void Fill(DataSet ds, object source)
    {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

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
    /// <exception cref="ArgumentNullException"><paramref name="ds"/> or <paramref name="source"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="tableName"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public void Fill(DataSet ds, string tableName, object source)
    {
      if (ds is null)
        throw new ArgumentNullException(nameof(ds));
      if (source is null)
        throw new ArgumentNullException(nameof(source));
      if (string.IsNullOrWhiteSpace(tableName))
        throw new ArgumentException(string.Format(Resources.StringNotNullOrWhiteSpaceException, nameof(tableName)), nameof(tableName));


      var dt = ds.Tables[tableName];

      bool addNewTable = false;
      if (dt is null)
      {
        dt = new DataTable(tableName);
        addNewTable = true;
      }

      Fill(dt, source);

      if (addNewTable)
        ds.Tables.Add(dt);
    }

    /// <summary>
    /// Fills a DataTable with data values from an object or collection.
    /// </summary>
    /// <param name="dt">A reference to the DataTable to be filled.</param>
    /// <param name="source">A reference to the object or collection acting as a data source.</param>
    /// <exception cref="ArgumentNullException"><paramref name="dt"/> or <paramref name="source"/> is <see langword="null"/>.</exception>
    public void Fill(DataTable dt, object source)
    {
      if (dt is null)
        throw new ArgumentNullException(nameof(dt));
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      // get the list of columns from the source
      List<string> columns = GetColumns(source);
      if (columns.Count < 1)
        return;

      // create columns in DataTable if needed
      foreach (string column in columns)
        if (!dt.Columns.Contains(column))
          dt.Columns.Add(column);

      // get an IList and copy the data
      CopyData(dt, GetIList(source), columns);
    }

    #region DataCopyIList

    private static IList GetIList(object source)
    {
      if (source is IListSource listSource)
        return listSource.GetList();
      else if (source is IList list)
        return list;
      else
      {
        // this is a regular object - create a list
        ArrayList col =
        [
          source
        ];
        return col;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    private static void CopyData(DataTable dt, IList ds, List<string> columns)
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
            var obj = ds[index];
            if (obj is null)
              throw new DataException(Resources.ErrorReadingValueException + " " + column);

            dr[column] = GetField(obj, column);
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

    private static List<string> GetColumns(object source)
    {
      List<string> result;
      // first handle DataSet/DataTable
      object innerSource;
      if (source is IListSource iListSource)
        innerSource = iListSource.GetList();
      else
        innerSource = source;

      if (innerSource is DataView dataView)
        result = ScanDataView(dataView);
      else
      {
        // now handle lists/arrays/collections
        if (innerSource is IEnumerable)
        {
          Type? childType = Utilities.GetChildItemType(innerSource.GetType());
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

    private static List<string> ScanDataView(DataView ds)
    {
      List<string> result = [];
      for (int field = 0; field < ds.Table!.Columns.Count; field++)
        result.Add(ds.Table.Columns[field].ColumnName);
      return result;
    }

   private static List<string> ScanObject(Type? sourceType)
   {
      List<string> result = [];

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
      if (obj is DataRowView dataRowView)
      {
        // this is a DataRowView from a DataView
        result = dataRowView[fieldName].ToString()!;
      }
      else if (obj is ValueType && obj.GetType().IsPrimitive)
      {
        // this is a primitive value type
        result = obj.ToString()!;
      }
      else
      {
        if (obj is string tmp)
        {
          result = tmp;
        }
        else
        {
          // this is an object or Structure
          try
          {
            Type sourceType = obj.GetType();

            // see if the field is a property
            PropertyInfo? prop = sourceType.GetProperty(fieldName);

            if ((prop == null) || (!prop.CanRead))
            {
              // no readable property of that name exists - 
              // check for a field
              FieldInfo? field = sourceType.GetField(fieldName);
              if (field == null)
              {
                // no field exists either, throw an exception
                throw new DataException(Resources.NoSuchValueExistsException + " " + fieldName);
              }
              else
              {
                // got a field, return its value
                result = field.GetValue(obj)!.ToString()!;
              }
            }
            else
            {
              // found a property, return its value
              result = prop.GetValue(obj, null)!.ToString()!;
            }
          }
          catch (Exception ex)
          {
            throw new DataException(Resources.ErrorReadingValueException + " " + fieldName, ex);
          }
        }
      }
      return result;
    }

    #endregion

  }
}