using System;

namespace Csla.Analyzers.IntegrationTests
{
  [Serializable]
  public class SerializableArguments
    : BusinessBase<SerializableArguments>
  {
    private void DataPortal_Fetch(int x) { }

    private void DataPortal_Fetch(SerializedObject x) { }

    private void DataPortal_Fetch(NonSerializedObject x) { }
  }

  public class NonSerializedObject { }

  [Serializable]
  public class SerializedObject { }
}
