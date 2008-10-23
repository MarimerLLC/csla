using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using Csla.Serialization;

namespace Csla.Test.ValidationRules
{
  [Serializable]
  public class ShortCircuit : BusinessBase<ShortCircuit>
  {
    string _test = string.Empty;
    public string Test
    {
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get
      {
        CanReadProperty(true);
        return _test;
      }
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      set
      {
        CanWriteProperty(true);
        if (!_test.Equals(value))
        {
          _test = value;
          PropertyHasChanged();
        }
      }
    }

    protected override object GetIdValue()
    {
      return 0;
    }

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, "Test");
      ValidationRules.AddRule<ShortCircuit>(AlwaysWarns, "Test");
      ValidationRules.AddRule<ShortCircuit>(AlwaysFails, "Test", 10);
    }

    public int Threshold
    {
      get { return ValidationRules.ProcessThroughPriority; }
      set { ValidationRules.ProcessThroughPriority = value; }
    }

    public void CheckRules()
    {
      ValidationRules.CheckRules();
    }

    private static bool AlwaysWarns(ShortCircuit target, Csla.Validation.RuleArgs e)
    {
      e.Description = "Always warns";
      e.Severity = Csla.Validation.RuleSeverity.Warning;
      return false;
    }

    private static bool AlwaysFails(ShortCircuit target, Csla.Validation.RuleArgs e)
    {
      e.Description = "Always fails";
      return false;
    }

    public ShortCircuit()
    {
      ValidationRules.CheckRules();
    }
  }
}
