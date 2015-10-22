using System.Threading.Tasks;

namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveAsyncIsCalledOnABusinessObjectAsThis
    : BusinessBase<AnalyzeWhenSaveAsyncIsCalledOnABusinessObjectAsThis>
  {
    public async Task Foo()
    {
      await this.SaveAsync();
    }
  }
}