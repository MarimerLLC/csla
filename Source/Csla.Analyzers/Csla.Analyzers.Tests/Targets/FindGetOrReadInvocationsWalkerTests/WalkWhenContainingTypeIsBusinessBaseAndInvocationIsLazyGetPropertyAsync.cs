namespace Csla.Analyzers.Tests.Targets.FindGetOrReadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyGetPropertyAsync
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyGetPropertyAsync>
  {
    public void Go()
    {
      this.LazyGetPropertyAsync<int>(null, null);
    }
  }
}
