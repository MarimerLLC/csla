namespace Csla.Analyzers.Tests.Targets.FindGetOrReadInvocationsWalkerTests
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
