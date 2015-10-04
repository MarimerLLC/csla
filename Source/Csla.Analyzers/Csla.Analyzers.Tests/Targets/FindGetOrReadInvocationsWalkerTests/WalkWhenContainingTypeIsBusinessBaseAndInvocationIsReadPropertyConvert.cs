namespace Csla.Analyzers.Tests.Targets.FindGetOrReadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsReadPropertyConvert
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsReadPropertyConvert>
  {
    public void Go()
    {
      this.ReadPropertyConvert<int, int>(null);
    }
  }
}
