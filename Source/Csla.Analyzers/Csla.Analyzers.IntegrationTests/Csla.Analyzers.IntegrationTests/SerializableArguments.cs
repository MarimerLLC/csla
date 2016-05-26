using System;
using System.Runtime.Serialization;

namespace Csla.Analyzers.IntegrationTests
{
  [Serializable]
  public class SerializableArguments
    : BusinessBase<SerializableArguments>
  {
    private void DataPortal_Fetch(int x) { }

    private void DataPortal_Fetch(SerializedObjectWithInterface x) { }

    private void DataPortal_Fetch(SerializedObjectWithAttribute x) { }

    private void DataPortal_Fetch(NonSerializedObject x) { }
  }

  public class NonSerializedObject { }

  public class SerializedObjectWithInterface
    : ISerializable
  {
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      throw new NotImplementedException();
    }
  }

  [Serializable]
  public class SerializedObjectWithAttribute { }
}
