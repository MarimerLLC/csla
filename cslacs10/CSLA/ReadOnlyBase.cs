using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using CSLA.Resources;

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
  [Serializable()]
  abstract public class ReadOnlyBase : ICloneable,
                                       Serialization.ISerializationNotification
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
      throw new NotSupportedException(Strings.GetResourceString("CreateNotSupportedException"));
    }

    /// <summary>
    /// Override this method to allow retrieval of an existing business
    /// object based on data in the database.
    /// </summary>
    /// <param name="criteria">
    /// An object containing criteria values to identify the object.</param>
    virtual protected void DataPortal_Fetch(object criteria)
    {
      throw new NotSupportedException(Strings.GetResourceString("FetchNotSupportedException"));
    }

    private void DataPortal_Update()
    {
      throw new NotSupportedException(Strings.GetResourceString("UpdateNotSupportedException"));
    }

    private void DataPortal_Delete(object criteria)
    {
      throw new NotSupportedException(Strings.GetResourceString("DeleteNotSupportedException"));
    }

    /// <summary>
    /// Called by the server-side DataPortal prior to calling the 
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    protected virtual void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {
    }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    protected virtual void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
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
      // now cascade the call to all child objects/collections
      FieldInfo [] fields;

      // get the list of fields in this type
      fields = this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

      foreach(FieldInfo field in fields)
        if(!field.FieldType.IsValueType && !Attribute.IsDefined(field, typeof(NotUndoableAttribute)))
        {
          // it's a ref type, so check for ISerializationNotification
          object value = field.GetValue(this);
          if(value is Serialization.ISerializationNotification)
            ((Serialization.ISerializationNotification)value).Deserialized();
        }
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
      // cascade the call to all child objects/collections
      FieldInfo [] fields;

      // get the list of fields in this type
      fields = this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

      foreach(FieldInfo field in fields)
        if(!field.FieldType.IsValueType && !Attribute.IsDefined(field, typeof(NotUndoableAttribute)))
        {
          // it's a ref type, so check for ISerializationNotification
          object value = field.GetValue(this);
          if(value is Serialization.ISerializationNotification)
            ((Serialization.ISerializationNotification)value).Serialized();
        }
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
      // cascade the call to all child objects/collections
      FieldInfo [] fields;

      // get the list of fields in this type
      fields = this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

      foreach(FieldInfo field in fields)
        if(!field.FieldType.IsValueType && !Attribute.IsDefined(field, typeof(NotUndoableAttribute)))
        {
          // it's a ref type, so check for ISerializationNotification
          object value = field.GetValue(this);
          if(value is Serialization.ISerializationNotification)
            ((Serialization.ISerializationNotification)value).Serializing();
        }
    }

    #endregion

	}
}
