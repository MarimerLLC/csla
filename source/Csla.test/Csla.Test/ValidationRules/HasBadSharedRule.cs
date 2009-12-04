using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using Csla.Serialization;

namespace Csla.Test.ValidationRules
{
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  [Serializable]
  public class HasBadSharedRule : BusinessBase<HasBadSharedRule>
  {
    int _data;
    public int Data
    {
      get
      {
        CanReadProperty("Data", true);
        return _data;
      }
      set
      {
        CanWriteProperty("Data", true);
        if (!_data.Equals(value))
        {
          _data = value;
          PropertyHasChanged("Data");
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
