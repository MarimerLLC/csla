namespace Csla.Analyzers.Tests.Targets.FindGetOrReadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsReadProperty
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsReadProperty>
  {
    public void Go()
    {
      this.ReadProperty(null);
    }
  }
}
