using System;
using System.Data;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace CSLA.Data
{
  /// <summary>
  /// An ObjectAdapter is used to convert data in an object 
  /// or collection into a DataTable.
  /// </summary>
  public class ObjectAdapter
  {
    ArrayList _columns = new ArrayList();
                              
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
    /// <param name="TableName"></param>
    /// <param name="source">A reference to the object or collection acting as a data source.</param>
    public void Fill(DataSet ds, string tableName, object source)
    {
      DataTable dt = ds.Tables[tableName];
      bool exists = (dt != null);

      if(!exists)
        dt = new DataTable(tableName);

      Fill(dt, source);

      if(!exists)
        ds.Tables.Add(dt);
    }

    /// <summary>
    /// Fills a DataTable with data values from an object or collection.
    /// </summary>
    /// <param name="dt">A reference to the DataTable to be filled.</param>
    /// <param name="source">A reference to the object or collection acting as a data source.</param>
    public void Fill(DataTable dt, object source)
    {
      AutoDiscover(source);
      DataCopy(dt, source);
    }

    #region Data Copy

    void DataCopy(DataTable dt, object source)
    {
      if(source == null) return;
      if(_columns.Count < 1) return;

      if(source is IListSource)
      {
        DataCopyIList(dt, ((IListSource)source).GetList());
      }
      else
      {
        if(source is IList)
        {
          DataCopyIList(dt, (IList)source);
        }
        else
        {
          // they gave us a regular object - create a list
          ArrayList col = new ArrayList();
          col.Add(source);
          DataCopyIList(dt, (IList)col);
        }
      }
    }

    void DataCopyIList(DataTable dt, IList ds)
    {
      // create columns if needed
      foreach(string column in _columns)
      {
        if(!dt.Columns.Contains(column))
          dt.Columns.Add(column);
      }

      // load the data into the control
      dt.BeginLoadData();
      for(int index = 0; index < ds.Count; index++)
      {
        DataRow dr = dt.NewRow();
        foreach(string column in _columns)
        {
          try
          {
            dr[column] = GetField(ds[index], column);
          }
          catch(Exception ex)
          {
            dr[column] = ex.Message;
          }
        }
        dt.Rows.Add(dr);
      }
      dt.EndLoadData();
    }

    #endregion

    #region AutoDiscover

    void AutoDiscover(object source)
    {
      object innerSource;

      if(source is IListSource)
      {
        innerSource = ((IListSource)source).GetList();
      }
      else
      {
        innerSource = source;
      }

      _columns.Clear();

      if(innerSource is DataView)
      {
        ScanDataView((DataView)innerSource);
      }
      else
      {
        if(innerSource is IList)
        {
          ScanIList((IList)innerSource);
        }
        else
        {
          // they gave us a regular object
          ScanObject(innerSource);
        }
      }
    }

    void ScanDataView(DataView ds)
    {
      for(int field = 0; field < ds.Table.Columns.Count; field++)
        _columns.Add(ds.Table.Columns[field].ColumnName);
    }

    void ScanIList(IList ds)
    {
      if(ds.Count > 0)
      {
        // retrieve the first item from the list
        object obj = ds[0];

        if(obj is ValueType && obj.GetType().IsPrimitive)
        {
          // the value is a primitive value type
          _columns.Add("Value");
        }
        else
        {
          if(obj is string)
          {
            // the value is a simple string
            _columns.Add("Text");
          }
          else
          {
            // we have a complex Structure or object
            ScanObject(obj);
          }
        }
      }
    }

    void ScanObject(object source)
    {
      Type sourceType = source.GetType();

      // retrieve a list of all public properties
      PropertyInfo [] props = sourceType.GetProperties();
      for(int column = 0; column < props.Length; column++)
        if(props[column].CanRead)
          _columns.Add(props[column].Name);

      // retrieve a list of all public fields
      FieldInfo [] fields = sourceType.GetFields();
      for(int column = 0; column < fields.Length; column++)
        _columns.Add(fields[column].Name);
    }

    #endregion

    #region GetField

    string GetField(object obj, string fieldName)
    {

      if(obj is DataRowView)
      {
        // this is a DataRowView from a DataView
        return ((DataRowView)obj)[fieldName].ToString();
      }
      else
      {
        if(obj is ValueType && obj.GetType().IsPrimitive)
        {
          // this is a primitive value type
          if(obj == null)
            return string.Empty;
          else
            return obj.ToString();
        }
        else
        {
          if(obj is string)
          {
            // this is a simple string
            if(obj == null)
              return string.Empty;
            else
              return obj.ToString();
          }
          else
          {
            // this is an object or Structure
            try
            {
              Type sourcetype = obj.GetType();

              // see if the field is a property
              PropertyInfo prop = sourcetype.GetProperty(fieldName);

              if(prop == null || !prop.CanRead)
              {
                // no readable property of that name exists - check for a field
                FieldInfo field = sourcetype.GetField(fieldName);

                if(field == null)
                {
                  // no field exists either, throw an exception
                  throw new System.Data.DataException(
                    "No such value exists: " + fieldName);
                }
                else
                {
                  // got a field, return its value
                  return field.GetValue(obj).ToString();
                }
              }
              else
              {
                // found a property, return its value
                return prop.GetValue(obj, null).ToString();
              }
            }
            catch(Exception ex)
            {
              throw new System.Data.DataException(
                "Error reading value: " + fieldName, ex);
            }
          }
        }
      }
    }

    #endregion

  }
}
