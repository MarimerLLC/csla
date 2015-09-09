using System.Threading.Tasks;

namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsNotABusinessBase
  {
    public async Task<AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsNotABusinessBase> SaveAsync()
    {
      return await Task.FromResult<AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsNotABusinessBase>(null);
    }
  }

  public class AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsNotABusinessBaseCaller
  {
    public async Task Call()
    {
      var x = new AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsNotABusinessBase();
      await x.SaveAsync();
    }
  }
}