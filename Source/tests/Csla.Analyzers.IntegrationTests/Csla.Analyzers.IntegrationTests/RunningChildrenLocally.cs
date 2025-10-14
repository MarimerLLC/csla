using System;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[Serializable]
public sealed class FooAttribute : Attribute { }

namespace Csla.Analyzers.IntegrationTests
{
  [Serializable]
  public sealed class RunningChildrenLocally
    : BusinessBase<RunningChildrenLocally>
  {
    [Fetch]
    [RunLocal]
    private void DataPortal_Fetch() { }

    [FetchChild]
    [RunLocal]
    [Foo]
    private void Child_Fetch() { }

    [UpdateChild]
    [RunLocal, Foo]
    private void Child_Update() { }
  }
}
