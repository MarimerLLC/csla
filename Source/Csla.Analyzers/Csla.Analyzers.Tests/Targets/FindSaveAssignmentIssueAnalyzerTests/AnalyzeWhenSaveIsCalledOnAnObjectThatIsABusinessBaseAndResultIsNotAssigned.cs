namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssigned
    : BusinessBase<AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssigned>
  { }

  public class AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssignedCaller
  {
    public void Call()
    {
      var x = new AnalyzeWhenSaveIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssigned();
      x.Save();
    }
  }
}