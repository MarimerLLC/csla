using System;
using System.ComponentModel;
using System.Reflection;
using Csla.Serialization;
using Csla.Serialization.Mobile;

namespace Csla.Silverlight
{
  /// <summary>
  /// Inherit from this base class to easily
  /// create a serializable class.
  /// </summary>
  [Serializable]
  public abstract class MobileObject : IMobileObject
  {
    #region Serialize

    void IMobileObject.Serialize(SerializationInfo info, MobileFormatter formatter)
    {
      var thisType = this.GetType();
      info.TypeName = thisType.FullName + "," + thisType.Assembly.FullName;
      Serialize(info, formatter);
    }

    /// <summary>
    /// Method called by MobileFormatter when an object
    /// should serialize its data. The data should be
    /// serialized into the SerializationInfo parameter.
    /// </summary>
    /// <param name="info">
    /// Object to contain the serialized data.
    /// </param>
    /// <param name="formatter">
    /// Reference to the formatter performing the serialization.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void Serialize(SerializationInfo info, MobileFormatter formatter)
    {
      FieldInfo[] fields;

      var currentType = this.GetType();

      while (currentType != null)
      {
        // get the list of fields in this type
        fields = currentType.GetFields(
          BindingFlags.NonPublic |
          BindingFlags.Instance |
          BindingFlags.Public);

        foreach (FieldInfo field in fields)
        {
          // see if this field is marked as not undoable
          if (!field.IsNotSerialized && !IsNonSerialized(field))
          {
            var value = GetValue(field);
            var mobile = value as IMobileObject;
            if (mobile == null)
              info.AddValue(
                string.Format("{0}!{1}", field.DeclaringType.Name, field.Name),
                value);
            else
              info.AddValue(
                string.Format("{0}!{1}", field.DeclaringType.Name, field.Name),
                formatter.SerializeObject(mobile));
          }
        }
        currentType = currentType.BaseType;
      }
    }

    /// <summary>
    /// Override in a subclass to return the value
    /// of the requested field. Remember you can
    /// only return values of fields declared directly
    /// in your class.
    /// </summary>
    /// <param name="field">
    /// FieldInfo describing the field to be retrieved.
    /// </param>
    /// <returns></returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual object GetValue(FieldInfo field)
    {
      throw new NotImplementedException("GetValue must be overridden in all subclasses");
    }

    private static bool IsNonSerialized(FieldInfo field)
    {
      var a = field.GetCustomAttributes(typeof(Csla.Serialization.NonSerializedAttribute), false);
      return a.Length > 0;
    }

    #endregion

    #region Deserialize

    void IMobileObject.Deserialize(SerializationInfo info, MobileFormatter formatter)
    {
      Deserialize(info, formatter);
    }

    /// <summary>
    /// Method called by MobileFormatter when an object
    /// should be deserialized. The data should be
    /// deserialized from the SerializationInfo parameter.
    /// </summary>
    /// <param name="info">
    /// Object containing the serialized data.
    /// </param>
    /// <param name="formatter">
    /// Reference to the formatter performing the deserialization.
    /// </param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void Deserialize(SerializationInfo info, MobileFormatter formatter)
    {
      FieldInfo[] fields;

      var currentType = this.GetType();

      while (currentType != null)
      {
        // get the list of fields in this type
        fields = currentType.GetFields(
          BindingFlags.NonPublic |
          BindingFlags.Instance |
          BindingFlags.Public);

        foreach (FieldInfo field in fields)
        {
          // see if this field is marked as not undoable
          if (!field.IsNotSerialized && !IsNonSerialized(field))
          {
            var value = info.GetValue(
              string.Format("{0}!{1}", field.DeclaringType.Name, field.Name));
            var valueInfo = value as SerializationInfo;
            if (valueInfo == null)
              SetValue(field, Convert.ChangeType(value, field.FieldType, null));
            else
              SetValue(field, formatter.GetObject(valueInfo.ReferenceId));
          }
        }
        currentType = currentType.BaseType;
      }
    }

    /// <summary>
    /// Override in a subclass to set the value
    /// of the requested field. Remember you can
    /// only set values of fields declared directly
    /// in your class.
    /// </summary>
    /// <param name="field">
    /// FieldInfo describing the field to be set.
    /// </param>
    /// <param name="value">
    /// Value to be set into the field.
    /// </param>
    /// <returns></returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void SetValue(FieldInfo field, object value)
    {
      throw new NotImplementedException("SetValue must be overridden in all subclasses");
    }

    #endregion
  }
}
