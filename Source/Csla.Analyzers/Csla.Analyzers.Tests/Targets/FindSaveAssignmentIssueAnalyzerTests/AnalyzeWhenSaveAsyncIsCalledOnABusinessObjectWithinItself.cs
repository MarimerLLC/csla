using System.Threading.Tasks;

namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveAsyncIsCalledOnABusinessObjectWithinItself
    : BusinessBase<AnalyzeWhenSaveAsyncIsCalledOnABusinessObjectWithinItself>
  {
    public async Task Foo()
    {
      await SaveAsync();
    }
  }
}