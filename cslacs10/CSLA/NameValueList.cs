using System;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Configuration;

namespace CSLA
{
  /// <summary>
  /// This is a base class from which readonly name/value 
  /// business classes can be quickly and easily created.
  /// </summary>
  /// <remarks>
  /// As discussed in Chapter 5, inherit from this class to
  /// quickly and easily create name/value list objects for
  /// population of ListBox or ComboBox controls and for
  /// validation of list-based data items in your business
  /// objects.
  /// </remarks>
  [Serializable()]
  abstract public class NameValueList : NameObjectCollectionBase, ICloneable
  {
#region ICloneable

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>A new object containing the exact data of the original object.</returns>
    public Object Clone()
    {

      MemoryStream buffer = new MemoryStream();
      BinaryFormatter formatter = new BinaryFormatter();

      formatter.Serialize(buffer, this);
      buffer.Position = 0;
      return formatter.Deserialize(buffer);
    }

#endregion

#region Collection methods

    /// <summary>
    /// Returns a value from the list.
    /// </summary>
    /// <param name="index">The positional index of the value in the collection.</param>
    /// <returns>The specified value.</returns>
    public string this [int index]
    {
      get
      {
        return (string)base.BaseGet(index);
      }
    }

    /// <summary>
    /// Returns a value from the list.
    /// </summary>
    /// <param name="Name">The name of the value.</param>
    /// <returns>The specified value.</returns>
    public string this [string name]
    {
      get
      {
        return (string)base.BaseGet(name);
      }
    }

    /// <summary>
    /// Adds a name/value pair to the list.
    /// </summary>
    /// <param name="Name">The name of the item.</param>
    /// <param name="Value">The value to be added.</param>
    protected void Add(string name, string newvalue)
    {
      base.BaseAdd(name, newvalue);
    }

    /// <summary>
    /// Returns the first name found in the list with the specified
    /// value.
    /// </summary>
    /// <remarks>
    /// This method throws an exception if no matching value is found
    /// in the list.
    /// </remarks>
    /// <param name="item">The value to search for in the list.</param>
    /// <returns>The name of the item found.</returns>
    public string Key(string item) 
    {
      foreach(string keyName in this)
        if(this[keyName] == item)
          return keyName;

      // we didn't find a match - throw an exception
      throw new ApplicationException("No matching item in collection");
    }

#endregion

#region Create and Load

    protected NameValueList()
    {
      // prevent public creation
    }

    protected NameValueList(SerializationInfo info, StreamingContext context) :
      base(info, context)
    {
      // we have nothing to serialize
    }

#endregion

#region Data Access

    private void DataPortal_Create(Object criteria)
    {
      throw new NotSupportedException("Invalid operation - create not allowed");
    }

    /// <summary>
    /// Override this method to allow retrieval of an existing business
    /// object based on data in the database.
    /// </summary>
    /// <remarks>
    /// In many cases you can call the SimpleFetch method to
    /// retrieve simple name/value data from a single table in
    /// a database. In more complex cases you may need to implement
    /// your own data retrieval code using ADO.NET.
    /// </remarks>
    /// <param name="Criteria">An object containing criteria values to identify the object.
    /// </param>
    virtual protected void DataPortal_Fetch(Object criteria)
    {
      throw new NotSupportedException("Invalid operation - fetch not allowed");
    }

    private void DataPortal_Update()
    {
      throw new NotSupportedException("Invalid operation - update not allowed");
    }

    private void DataPortal_Delete(Object criteria)
    {
      throw new NotSupportedException("Invalid operation - delete not allowed");
    }

    protected string DB(string databaseName)
    {
      string val = ConfigurationSettings.AppSettings["DB:" + databaseName];
      if(val == null)
        return string.Empty;
      else
        return val;
    }

    /// <summary>
    /// Provides default/simple loading for most lists. 
    /// It is called to load data from the database
    /// </summary>
    /// <param name="DataBaseName">
    /// The name of the database to read. This database name
    /// must correspond to a database entry in the application
    /// configuration file.
    /// </param>
    /// <param name="TableName">The name of the table to read.</param>
    /// <param name="NameColumn">The name of the column containing name or key values.</param>
    /// <param name="ValueColumn">The name of the column containing data values.</param>
    protected void SimpleFetch(string dataBaseName, 
      string tableName, string nameColumn, string valueColumn)
    {
      SqlConnection cn = new SqlConnection(DB(dataBaseName));
      SqlCommand cm = new SqlCommand();

      cn.Open();
      try
      {
        cm.Connection = cn;
        cm.CommandText = 
          "SELECT " + nameColumn + "," + valueColumn + " FROM " + tableName;
        CSLA.Data.SafeDataReader dr = new CSLA.Data.SafeDataReader(cm.ExecuteReader());
        try
        {
          while(dr.Read())
            Add((string)dr.GetValue(0), (string)dr.GetValue(1));
        }
        finally
        {
          dr.Close();
        }
      }
      finally
      {
        cn.Close();
      }
    }

#endregion

  }
}
