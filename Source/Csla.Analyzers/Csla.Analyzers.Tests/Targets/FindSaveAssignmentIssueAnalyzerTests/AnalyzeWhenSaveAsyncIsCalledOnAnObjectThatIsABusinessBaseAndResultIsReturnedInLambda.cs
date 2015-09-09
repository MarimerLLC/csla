using System;
using System.Threading.Tasks;

namespace Csla.Analyzers.Tests.Targets.FindSaveAssignmentIssueAnalyzerTests
{
  public class AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda
    : BusinessBase<AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda>
  { }

  public class AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambdaCaller
  {
    public async Task Call()
    {
      var x = new AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda();

      await this.Run(async () => await x.SaveAsync());
    }

    private async Task<AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda> Run(
      Func<Task<AnalyzeWhenSaveAsyncIsCalledOnAnObjectThatIsABusinessBaseAndResultIsReturnedInLambda>> code)
    {
      return await code();
    }
  }
}