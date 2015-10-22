namespace Csla.Analyzers.Tests.Targets.FindGetOrReadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyGetProperty
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyGetProperty>
  {
    public void Go()
    {
      this.LazyGetProperty<int>(null, null);
    }
  }
}
