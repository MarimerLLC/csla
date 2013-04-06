using System;

namespace Csla.Serialization
{
  /// <summary>
  /// Indicates that a field should not be
  /// serialized by the MobileFormatter.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field)]
  public class NonSerializedAttribute : Attribute
  {
  }
}
