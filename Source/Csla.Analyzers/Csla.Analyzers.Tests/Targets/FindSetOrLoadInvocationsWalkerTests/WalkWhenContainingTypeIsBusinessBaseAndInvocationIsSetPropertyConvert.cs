namespace Csla.Analyzers.Tests.Targets.FindSetOrLoadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsSetPropertyConvert
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsSetPropertyConvert>
  {
    public void Go()
    {
      this.SetPropertyConvert<int, int>(null, 0);
    }
  }
}
