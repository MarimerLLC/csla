using System;
using System.ComponentModel;

namespace Csla.Test.Serialization
{
  [Serializable()]
  public class OverrideSerializationRoot : SerializationRoot
  {
    protected override bool ShouldHandlerSerialize(PropertyChangedEventHandler value)
    {
      if (value.Method.DeclaringType != null && value.Method.DeclaringType.Name == @"Action`2")
      {
        return false;
      }
      return base.ShouldHandlerSerialize(value);
    }
    protected override bool ShouldHandlerSerialize(PropertyChangingEventHandler value)
    {
      if (value.Method.DeclaringType != null && value.Method.DeclaringType.Name == @"Action`2")
      {
          return false;
      }
      return base.ShouldHandlerSerialize(value);
    }
  }
}