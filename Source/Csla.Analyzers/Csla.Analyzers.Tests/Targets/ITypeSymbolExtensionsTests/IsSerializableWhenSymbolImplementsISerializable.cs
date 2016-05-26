using System;
using System.Runtime.Serialization;

namespace Csla.Analyzers.Tests.Targets.ITypeSymbolExtensionsTests
{
  public class IsSerializableWhenSymbolImplementsISerializable
    : ISerializable
  {
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      throw new NotImplementedException();
    }
  }
}