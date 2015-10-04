namespace Csla.Analyzers.Tests.Targets.FindSetOrLoadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsNotCslaMethod
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsNotCslaMethod>
  {
    public void Go()
    {
      this.GetHashCode();
    }
  }
}
