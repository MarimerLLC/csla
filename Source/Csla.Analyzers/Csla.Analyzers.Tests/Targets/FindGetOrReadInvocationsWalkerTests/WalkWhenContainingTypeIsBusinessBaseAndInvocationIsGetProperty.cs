namespace Csla.Analyzers.Tests.Targets.FindGetOrReadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsGetProperty
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsGetProperty>
  {
    public void Go()
    {
      this.GetProperty(null);
    }
  }
}
