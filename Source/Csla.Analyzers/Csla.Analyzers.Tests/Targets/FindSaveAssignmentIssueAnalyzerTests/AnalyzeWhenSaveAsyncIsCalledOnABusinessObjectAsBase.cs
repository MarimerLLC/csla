using System.Threading.Tasks;

namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveAsyncIsCalledOnABusinessObjectAsBase
    : BusinessBase<AnalyzeWhenSaveAsyncIsCalledOnABusinessObjectAsBase>
  {
    public async Task Foo()
    {
      await base.SaveAsync();
    }
  }
}