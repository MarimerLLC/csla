namespace Csla.Analyzers.Tests.Targets.FindSetOrLoadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadPropertyConvert
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadPropertyConvert>
  {
    public void Go()
    {
      this.LoadPropertyConvert<int, int>(null, 0);
    }
  }
}
