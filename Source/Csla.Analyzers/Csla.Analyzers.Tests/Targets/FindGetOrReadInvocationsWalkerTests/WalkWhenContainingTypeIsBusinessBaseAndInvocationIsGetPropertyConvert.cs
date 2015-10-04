namespace Csla.Analyzers.Tests.Targets.FindGetOrReadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsGetPropertyConvert
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsGetPropertyConvert>
  {
    public void Go()
    {
      this.GetPropertyConvert<int, int>(null);
    }
  }
}
