namespace Csla.Analyzers.Tests.Targets.FindSetOrLoadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadProperty
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadProperty>
  {
    public void Go()
    {
      this.LoadProperty(null, null);
    }
  }
}
