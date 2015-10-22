namespace Csla.Analyzers.Tests.Targets.FindGetOrReadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyReadPropertyAsync
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLazyReadPropertyAsync>
  {
    public void Go()
    {
      this.LazyReadPropertyAsync<int>(null, null);
    }
  }
}
