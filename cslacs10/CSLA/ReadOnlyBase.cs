using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Configuration;

namespace CSLA
{
  /// <summary>
  /// This is a base class from which readonly business classes
  /// can be derived.
  /// </summary>
  /// <remarks>
  /// This base class only supports data retrieve, not updating or
  /// deleting. Any business classes derived from this base class
  /// should only implement readonly properties.
  /// </remarks>
  abstract public class ReadOnlyBase : ICloneable
  {
    #region ICloneable

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>A new object containing the exact data of the original object.</returns>
    public object Clone()
    {

      MemoryStream buffer = new MemoryStream();
      BinaryFormatter formatter = new BinaryFormatter();

      formatter.Serialize(buffer, this);
      buffer.Position = 0;
      return formatter.Deserialize(buffer);
    }

    #endregion

    #region Data Access

    private void DataPortal_Create(object criteria)
    {
      throw new NotSupportedException("Invalid operation - create not allowed");
    }

    /// <summary>
    /// Override this method to allow retrieval of an existing business
    /// object based on data in the database.
    /// </summary>
    /// <param name="criteria">
    /// An object containing criteria values to identify the object.</param>
    virtual protected void DataPortal_Fetch(object criteria)
    {
      throw new NotSupportedException("Invalid operation - fetch not allowed");
    }

    private void DataPortal_Update()
    {
      throw new NotSupportedException("Invalid operation - update not allowed");
    }

    private void DataPortal_Delete(object criteria)
    {
      throw new NotSupportedException("Invalid operation - delete not allowed");
    }

    /// <summary>
    /// Returns the specified database connection string from the application
    /// configuration file.
    /// </summary>
    /// <remarks>
    /// The database connection string must be in the <c>appSettings</c> section
    /// of the application configuration file. The database name should be
    /// prefixed with 'DB:'. For instance, <c>DB:mydatabase</c>.
    /// </remarks>
    /// <param name="databaseName">Name of the database.</param>
    /// <returns>A database connection string.</returns>
    protected string DB(string databaseName)
    {
      string val = ConfigurationSettings.AppSettings["DB:" + databaseName];
      if(val == null)
        return string.Empty;
      else
        return val;
    }

    #endregion

	}
}
