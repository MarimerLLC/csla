namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveIsCalledOnABusinessObjectAsThis
    : BusinessBase<AnalyzeWhenSaveIsCalledOnABusinessObjectAsThis>
  {
    public void Foo()
    {
      this.Save();
    }
  }
}