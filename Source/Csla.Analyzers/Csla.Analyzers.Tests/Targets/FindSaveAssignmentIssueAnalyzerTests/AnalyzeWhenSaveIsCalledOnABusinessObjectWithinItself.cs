namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveIsCalledOnABusinessObjectWithinItself
    : BusinessBase<AnalyzeWhenSaveIsCalledOnABusinessObjectWithinItself>
  {
    public void Foo()
    {
      Save();
    }
  }
}