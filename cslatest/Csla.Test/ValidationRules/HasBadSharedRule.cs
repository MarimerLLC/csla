using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace Csla.Test.ValidationRules
{
  [Serializable]
  public class HasBadSharedRule : BusinessBase<HasBadSharedRule>
  {
    int _data;
    public int Data
    {
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get
      {
        CanReadProperty(true);
        return _data;
      }
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      set
      {
        CanWriteProperty(true);
        if (!_data.Equals(value))
        {
          _data = value;
          PropertyHasChanged();
        }
      }
    }

    protected override object GetIdValue()
    {
      return _data;
    }

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(BadRule, "Data");
    }

    private bool BadRule(object target, Csla.Validation.RuleArgs e)
    {
      e.Description = "Bad rule";
      return false;
    }
  }
}
