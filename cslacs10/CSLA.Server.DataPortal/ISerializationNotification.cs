using System;

/// <summary>
/// Contains interfaces and classes to help support serialization
/// of objects.
/// </summary>
namespace Serialization
{
  /// <summary>
  /// Objects can implement this interface if they wish to be
  /// notified of serialization events.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Note that .NET serialization does NOT call these methods. Only
  /// code that checks for the ISerializationNotification interface
  /// when serializating and deserializing objects will invoke these
  /// methods.
  /// </para><para>
  /// The CSLA .NET framework's DataPortal processing and the Clone
  /// method in BusinessBase automatically make these calls.
  /// </para>
  /// </remarks>
  public interface ISerializationNotification
  {
    /// <summary>
    /// This method is called before an object is serialized.
    /// </summary>
    void Serializing();
    /// <summary>
    /// This method is called on the original instance of the
    /// object after it has been serialized.
    /// </summary>
    void Serialized();
    /// <summary>
    /// This method is called on a newly deserialized object
    /// after deserialization is complete.
    /// </summary>
    void Deserialized();
  }

  /// <summary>
  /// Helper methods for invoking the ISerializatoinNotification
  /// methods.
  /// </summary>
  public class SerializationNotification
  {
    /// <summary>
    /// Invokes the Serializing method on the target object
    /// if it has implemented ISerializationNotification.
    /// </summary>
    /// <param name="target">Object on which the method should be invoked.</param>
    public static void OnSerializing(object target)
    {
      if(target is ISerializationNotification)
        ((ISerializationNotification)target).Serializing();
    }

    /// <summary>
    /// Invokes the Serialized method on the target object
    /// if it has implemented ISerializationNotification.
    /// </summary>
    /// <param name="target">Object on which the method should be invoked.</param>
    public static void OnSerialized(object target)
    {
      if(target is ISerializationNotification)
        ((ISerializationNotification)target).Serialized();
    }

    /// <summary>
    /// Invokes the Deserialized method on the target object
    /// if it has implemented ISerializationNotification.
    /// </summary>
    /// <param name="target">Object on which the method should be invoked.</param>
    public static void OnDeserialized(object target)
    {
      if(target is ISerializationNotification)
        ((ISerializationNotification)target).Deserialized();
    }
  }
}
