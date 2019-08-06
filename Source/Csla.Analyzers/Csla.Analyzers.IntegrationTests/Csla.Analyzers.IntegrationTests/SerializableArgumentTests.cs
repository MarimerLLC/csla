using System;

namespace Csla.Analyzers.IntegrationTests
{
  [Serializable]
  public class SerializableArgumentTests
    : BusinessBase<SerializableArgumentTests>
  {
    private void DataPortal_Fetch(int x) { }

    private void DataPortal_Fetch(SerializedObject x) { }

    // This should fail because it's not serializable.
    private void DataPortal_Fetch(NonSerializedObject x) { }

    private void Child_Fetch(int x) { }

    private void Child_Fetch(SerializedObject x) { }

    private void Child_Fetch(NonSerializedObject x) { }
  }

  public class NonSerializedObject { }

  [Serializable]
  public class SerializedObject { }
}
