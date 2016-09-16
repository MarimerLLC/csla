namespace Csla.Analyzers.Tests.Targets.FindGetOrReadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyReadProperty
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyReadProperty>
  {
    public void Go()
    {
      this.LazyReadProperty<int>(null, null);
    }
  }
}
