using System;

namespace Csla.Serialization
{
  /// <summary>
  /// Indicates that an object may be
  /// serialized by the MobileFormatter.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
  public class SerializableAttribute : Attribute
  {
  }
}
