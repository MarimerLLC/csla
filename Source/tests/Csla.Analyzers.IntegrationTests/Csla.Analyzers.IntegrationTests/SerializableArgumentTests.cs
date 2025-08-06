
using System;

namespace Csla.Analyzers.IntegrationTests
{
  [Serializable]
  public class SerializableArgumentTests
    : BusinessBase<SerializableArgumentTests>
  {
    [Fetch]
    private void Fetch(int x) { }

    [Fetch]
    private void Fetch(SerializedObject x) { }

    [Fetch]
    private void Fetch(Guid id) { }

    // This should fail because it's not serializable.
    [Fetch]
    private void Fetch(NonSerializedObject x) { }

    [Fetch]
    private void FetchWithInject([Inject] NonSerializedObject x) { }

    [FetchChild]
    private void FetchChild(int x) { }

    [FetchChild]
    private void FetchChild(SerializedObject x) { }

    [FetchChild]
    private void FetchChild(NonSerializedObject x) { }
  }

  public class NonSerializedObject { }

  [Serializable]
  public class SerializedObject { }
}
