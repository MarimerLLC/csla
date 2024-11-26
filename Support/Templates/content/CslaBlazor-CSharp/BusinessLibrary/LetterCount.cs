﻿using Csla.Rules;

namespace CslaBlazor.BusinessLibrary
{
  public class LetterCount : BusinessRule
  {
    public LetterCount(Csla.Core.IPropertyInfo primaryProperty, Csla.Core.IPropertyInfo outputProperty)
      : base(primaryProperty)
    {
      AffectedProperties.Add(outputProperty);
    }

    protected override void Execute(IRuleContext context)
    {
      var text = (string)ReadProperty(context.Target, PrimaryProperty);
      var count = text.Length;
      context.AddOutValue(AffectedProperties[1], count);
    }
  }
}
