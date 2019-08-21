using System;

namespace Csla.Analyzers.IntegrationTests
{
  [Serializable]
  public class SerializableArgumentTests
    : BusinessBase<SerializableArgumentTests>
  {
    [Fetch]
    private void DataPortal_Fetch(int x) { }

    [Fetch]
    private void DataPortal_Fetch(SerializedObject x) { }

    // This should fail because it's not serializable.
    [Fetch]
    private void DataPortal_Fetch(NonSerializedObject x) { }

    [FetchChild]
    private void Child_Fetch(int x) { }

    [FetchChild]
    private void Child_Fetch(SerializedObject x) { }

    [FetchChild]
    private void Child_Fetch(NonSerializedObject x) { }
  }

  public class NonSerializedObject { }

  [Serializable]
  public class SerializedObject { }
}
