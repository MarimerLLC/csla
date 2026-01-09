using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLibrary
{
  using Csla.Rules;

  public class CheckCase : BusinessRule
  {
    public CheckCase(Csla.Core.IPropertyInfo primaryProperty)
      :base(primaryProperty)
    { }

    protected override void Execute(IRuleContext context)
    {
      if (context.Target is null || PrimaryProperty is null)
      {
        return;
      }

      var text = ReadProperty(context.Target, PrimaryProperty) as string;
      if (string.IsNullOrWhiteSpace(text)) return;
      var ideal = text.Substring(0, 1).ToUpper();
      ideal += text.Substring(1).ToLower();
      if (text != ideal)
        context.AddWarningResult("Check capitalization");
    }
  }
}
