using System;

namespace Csla.Serialization
{
  /// <summary>
  /// Indicates that an object may be
  /// serialized by the MobileFormatter.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public class SerializableAttribute : Attribute
  {
  }
}
