namespace Csla.Analyzers.Tests.Targets.FindSetOrLoadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadPropertyAsync
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadPropertyAsync>
  {
    public void Go()
    {
      this.LoadPropertyAsync<int>(null, null);
    }
  }
}
