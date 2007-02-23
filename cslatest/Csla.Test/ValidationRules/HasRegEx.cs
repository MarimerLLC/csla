using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.ValidationRules
{
  class HasRegEx : BusinessBase<HasRegEx>
  {
    private string _ssn;

    public string Ssn
    {
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get { CanReadProperty(true); return _ssn; }
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      set
      {
        CanWriteProperty(true);
        if (_ssn != value)
        {
          _ssn = value;
          PropertyHasChanged();
        }
      }
    }

    private string _ssn2;

    public string Ssn2
    {
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get { CanReadProperty(true); return _ssn2; }
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      set
      {
        CanWriteProperty(true);
        if (_ssn2 != value)
        {
          _ssn2 = value;
          PropertyHasChanged();
        }
      }
    }

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(Csla.Validation.CommonRules.RegExMatch,
        new Csla.Validation.CommonRules.RegExRuleArgs("Ssn", Csla.Validation.CommonRules.RegExPatterns.SSN));
      ValidationRules.AddRule(Csla.Validation.CommonRules.RegExMatch,
        new Csla.Validation.CommonRules.RegExRuleArgs("Ssn2", @"^\d{3}-\d{2}-\d{4}$"));
    }


    protected override object GetIdValue()
    {
      return string.Empty;
    }
  }
}
