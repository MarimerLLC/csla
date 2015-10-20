namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveIsCalledOnABusinessObjectAsBase
    : BusinessBase<AnalyzeWhenSaveIsCalledOnABusinessObjectAsBase>
  {
    public void Foo()
    {
      base.Save();
    }
  }
}