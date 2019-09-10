using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Core;
using Csla.Rules;

namespace PropertyStatus
{
  public class AsyncRule : BusinessRuleAsync
  {
    public AsyncRule(IPropertyInfo primaryProperty)
      : base(primaryProperty)
    {
      InputProperties = new List<IPropertyInfo> { PrimaryProperty };
    }

    protected override async Task ExecuteAsync(IRuleContext context)
    {
      await Task.Delay(3000);
      string val = (string)context.InputPropertyValues[PrimaryProperty];
      if (val == "Error")
        context.AddErrorResult("Invalid data!");
      else if (val == "Warning")
        context.AddWarningResult("This might not be a great idea!");
      else if (val == "Information")
        context.AddInformationResult("Just an FYI!");
      context.Complete();
    }
  }
}
