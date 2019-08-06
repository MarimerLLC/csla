using System.Threading.Tasks;

namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssigned
    : BusinessBase<AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssigned>
  { }

  public class AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssignedCaller
  {
    public async Task Call()
    {
      var x = new AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsNotAssigned();
      await x.SaveAsync();
    }
  }
}