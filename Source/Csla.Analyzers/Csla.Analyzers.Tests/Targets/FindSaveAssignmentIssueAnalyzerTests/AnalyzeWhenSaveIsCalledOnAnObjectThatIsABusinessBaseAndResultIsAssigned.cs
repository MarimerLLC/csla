namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssigned
    : BusinessBase<AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssigned>
  { }

  public class AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssignedCaller
  {
    public void Call()
    {
      var x = new AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssigned();
      x = x.Save();
    }
  }
}