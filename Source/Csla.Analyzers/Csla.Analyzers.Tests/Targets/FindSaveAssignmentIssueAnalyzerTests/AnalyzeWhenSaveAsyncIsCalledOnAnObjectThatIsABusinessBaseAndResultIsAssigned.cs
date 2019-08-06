using System.Threading.Tasks;

namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssigned
    : BusinessBase<AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssigned>
  { }

  public class AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssignedCaller
  {
    public async Task Call()
    {
      var x = new AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsAssigned();
      x = await x.SaveAsync();
    }
  }
}