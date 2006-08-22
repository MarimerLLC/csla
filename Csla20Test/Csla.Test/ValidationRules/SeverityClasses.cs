using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.ValidationRules
{
  public class SeverityRoot : Csla.BusinessBase<SeverityRoot>
  {
    string _test;
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
      ValidationRules.AddRule<SeverityRoot>(AlwaysError, "Test");
      ValidationRules.AddRule<SeverityRoot>(AlwaysWarns, "Test");
      ValidationRules.AddRule<SeverityRoot>(AlwaysInfo, "Test");
    }

    private static bool AlwaysInfo(SeverityRoot target, Csla.Validation.RuleArgs e)
    {
      e.Description = "Always info";
      e.Severity = Csla.Validation.RuleSeverity.Information;
      return false;
    }

    private static bool AlwaysWarns(SeverityRoot target, Csla.Validation.RuleArgs e)
    {
      e.Description = "Always warns";
      e.Severity = Csla.Validation.RuleSeverity.Warning;
      return false;
    }

    private static bool AlwaysError(SeverityRoot target, Csla.Validation.RuleArgs e)
    {
      e.Description = "Always error";
      e.Severity = Csla.Validation.RuleSeverity.Error;
      return false;
    }

    public SeverityRoot()
    {
      ValidationRules.CheckRules();
    }
  }

  public class NoErrorRoot : Csla.BusinessBase<NoErrorRoot>
  {
    string _test;
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
      ValidationRules.AddRule<NoErrorRoot>(AlwaysWarns, "Test");
      ValidationRules.AddRule<NoErrorRoot>(AlwaysInfo, "Test");
    }

    private static bool AlwaysInfo(NoErrorRoot target, Csla.Validation.RuleArgs e)
    {
      e.Description = "Always info";
      e.Severity = Csla.Validation.RuleSeverity.Information;
      return false;
    }

    private static bool AlwaysWarns(NoErrorRoot target, Csla.Validation.RuleArgs e)
    {
      e.Description = "Always warns";
      e.Severity = Csla.Validation.RuleSeverity.Warning;
      return false;
    }

    public NoErrorRoot()
    {
      ValidationRules.CheckRules();
    }
  }
}
