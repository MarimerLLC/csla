namespace Csla.Analyzers.Tests.Targets.FindSetOrLoadInvocationsWalkerTests
{
  public class WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadPropertyMarkDirty
    : BusinessBase<WalkWhenContainingTypeIsBusinessBaseAndInvocationIsLoadPropertyMarkDirty>
  {
    public void Go()
    {
      this.LoadPropertyMarkDirty(null, null);
    }
  }
}
