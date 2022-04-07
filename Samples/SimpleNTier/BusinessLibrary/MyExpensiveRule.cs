using System.Threading.Tasks;
using Csla;
using Csla.Rules;

namespace BusinessLibrary.Shared
{
  public class MyExpensiveRule : BusinessRuleAsync
  {
    protected override async Task ExecuteAsync(IRuleContext context)
    {
      var portal = context.ApplicationContext.GetRequiredService<IDataPortal<MyExpensiveCommand>>();
      var cmd = portal.Create();
      var result = await portal.ExecuteAsync(cmd);
      if (result == null)
        context.AddErrorResult("Command failed to run");
      else if (result.Result)
        context.AddInformationResult(result.ResultText);
      else
        context.AddErrorResult(result.ResultText);
    }
  }
}
