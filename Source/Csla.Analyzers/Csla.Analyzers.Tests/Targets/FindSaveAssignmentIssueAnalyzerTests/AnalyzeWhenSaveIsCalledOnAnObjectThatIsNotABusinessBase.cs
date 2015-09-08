namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveIsCalledOnAnObjectThatIsNotABusinessBase
  {
    public AnalyzeWhenSaveIsCalledOnAnObjectThatIsNotABusinessBase Save() { return null; }
  }

  public class AnalyzeWhenSaveIsCalledOnAnObjectThatIsNotABusinessBaseCaller
  {
    public void Call()
    {
      var x = new AnalyzeWhenSaveIsCalledOnAnObjectThatIsNotABusinessBase();
      x.Save();
    }
  }
}