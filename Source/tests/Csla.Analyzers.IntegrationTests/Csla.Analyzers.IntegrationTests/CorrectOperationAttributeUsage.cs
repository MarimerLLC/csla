using System;

namespace Csla.Analyzers.IntegrationTests
{
  public sealed class NotAStereotype
  {
    [Fetch]
    private void Fetch() { }
  }

  [Serializable]
  public sealed class StaticOperation
    : BusinessBase<StaticOperation>
  {
    [Fetch]
    private static void Fetch() { }
  }
}
