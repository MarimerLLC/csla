namespace Csla.Analyzers.Tests.Targets.FindSetOrLoadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsSetProperty
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsSetProperty>
  {
    public void Go()
    {
      this.SetProperty(null, null);
    }
  }
}
