using System.Threading.Tasks;
using Csla.Rules;

namespace BusinessLibrary.Shared
{
  public class MyExpensiveRule : BusinessRuleAsync
  {
    protected override async Task ExecuteAsync(IRuleContext context)
    {
      var result = await MyExpensiveCommand.DoCommandAsync();
      if (result == null)
        context.AddErrorResult("Command failed to run");
      else if (result.Result)
        context.AddInformationResult(result.ResultText);
      else
        context.AddErrorResult(result.ResultText);
      context.Complete();
    }
  }
}
