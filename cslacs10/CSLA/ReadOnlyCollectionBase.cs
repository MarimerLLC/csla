using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace CSLA
{
	/// <summary>
	/// Summary description for ReadOnlyCollectionBase.
	/// </summary>
  [Serializable()]
  abstract public class ReadOnlyCollectionBase : 
      CSLA.Core.SortableCollectionBase, ICloneable,
      Serialization.ISerializationNotification
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
    protected override void OnInsert(int index, object val)
    {
      if(!ActivelySorting && locked)
        throw new NotSupportedException(
              "Insert is invalid for a read-only collection");
    }

    /// <summary>
    /// Prevents removal of items from the collection when the
    /// collection is locked.
    /// </summary>
    protected override void OnRemove(int index, object val)
    {
      if(!ActivelySorting && locked)
        throw new NotSupportedException(
              "Remove is invalid for a read-only collection");
    }

    /// <summary>
    /// Prevents clearing the collection when the
    /// collection is locked.
    /// </summary>
    protected override void OnClear()
    {
      if(!ActivelySorting && locked)
        throw new NotSupportedException(
              "Clear is invalid for a read-only collection");
    }

    /// <summary>
    /// Prevents changing an item reference when the 
    /// collection is locked.
    /// </summary>
    protected override void OnSet(int index, object oldValue, object newValue)
    {
      if(!ActivelySorting && locked)
        throw new NotSupportedException(
              "Items can not be changed in a read-only collection");
    }

    #endregion

    #region ICloneable

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>A new object containing the exact data of the original object.</returns>
    public object Clone()
    {
      MemoryStream buffer = new MemoryStream();
      BinaryFormatter formatter = new BinaryFormatter();

      Serialization.SerializationNotification.OnSerializing(this);
      formatter.Serialize(buffer, this);
      Serialization.SerializationNotification.OnSerialized(this);
      buffer.Position = 0;
      object temp = formatter.Deserialize(buffer);
      Serialization.SerializationNotification.OnDeserialized(temp);
      return temp;
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
      return ConfigurationSettings.AppSettings["DB:" + databaseName];
    }

    #endregion

    #region ISerializationNotification

    void Serialization.ISerializationNotification.Deserialized()
    {
      Deserialized();
    }

    /// <summary>
    /// This method is called on a newly deserialized object
    /// after deserialization is complete.
    /// </summary>
    protected virtual void Deserialized()
    {
      foreach(object child in List)
        if (child is Serialization.ISerializationNotification)
          ((Serialization.ISerializationNotification)child).Deserialized();
    }

    void Serialization.ISerializationNotification.Serialized()
    {
      Serialized();
    }

    /// <summary>
    /// This method is called on the original instance of the
    /// object after it has been serialized.
    /// </summary>
    protected virtual void Serialized()
    {
      foreach(object child in List)
        if (child is Serialization.ISerializationNotification)
          ((Serialization.ISerializationNotification)child).Serialized();
    }

    void Serialization.ISerializationNotification.Serializing()
    {
      Serializing();
    }

    /// <summary>
    /// This method is called before an object is serialized.
    /// </summary>
    protected virtual void Serializing()
    {
      foreach(object child in List)
        if (child is Serialization.ISerializationNotification)
          ((Serialization.ISerializationNotification)child).Serializing();
    }

    #endregion

	}
}
