using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Configuration;

namespace CSLA
{
	/// <summary>
	/// Summary description for ReadOnlyCollectionBase.
	/// </summary>
  [Serializable()]
  abstract public class ReadOnlyCollectionBase : 
      CSLA.Core.BindableCollectionBase, ICloneable
	{
    public ReadOnlyCollectionBase()
    {
      AllowEdit = false;
      AllowNew = false;
      AllowRemove = false;
    }

    #region Remove, Clear, Set

    /// <summary>
    /// Indicates that the collection is locked, so insert, remove
    /// and change operations are disallowed.
    /// </summary>
    protected bool locked = true;

    /// <summary>
    /// Prevents insertion of new items into the collection when the
    /// collection is locked.
    /// </summary>
    protected override void OnInsert(int index, Object val)
    {
      if(locked)
        throw new NotSupportedException(
              "Insert is invalid for a read-only collection");
    }

    /// <summary>
    /// Prevents removal of items from the collection when the
    /// collection is locked.
    /// </summary>
    protected override void OnRemove(int index, Object val)
    {
      if(locked)
        throw new NotSupportedException(
              "Remove is invalid for a read-only collection");
    }

    /// <summary>
    /// Prevents clearing the collection when the
    /// collection is locked.
    /// </summary>
    protected override void OnClear()
    {
      if(locked)
        throw new NotSupportedException(
              "Clear is invalid for a read-only collection");
    }

    /// <summary>
    /// Prevents changing an item reference when the 
    /// collection is locked.
    /// </summary>
    protected override void OnSet(int index, Object oldValue, Object newValue)
    {
      if(locked)
        throw new NotSupportedException(
              "Items can not be changed in a read-only collection");
    }

    #endregion

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

#region Data Access

    private void DataPortal_Create(Object criteria)
    {
      throw new NotSupportedException("Invalid operation - create not allowed");
    }

    /// <summary>
    /// Override this method to allow retrieval of an existing business
    /// object based on data in the database.
    /// </summary>
    /// <param name="criteria">
    /// An object containing criteria values to identify the object.</param>
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
