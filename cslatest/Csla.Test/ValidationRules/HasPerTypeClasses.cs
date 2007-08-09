using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.ValidationRules
{
  public class HasPerTypeRules : Csla.BusinessBase<HasPerTypeRules>
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

    //static HasPerTypeRules()
    //{
    //  Csla.Validation.SharedValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, typeof(HasPerTypeRules), "Test");
    //}

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, "Test");
      int value = (int)ApplicationContext.GlobalContext["Shared"];
      ApplicationContext.GlobalContext["Shared"] = ++value;
    }

    protected override void AddInstanceBusinessRules()
    {
      ValidationRules.AddInstanceRule(Csla.Validation.CommonRules.StringMaxLength, new Csla.Validation.CommonRules.MaxLengthRuleArgs("Test", 5));
    }

    public HasPerTypeRules()
    {
      ValidationRules.CheckRules();
    }

    protected override object GetIdValue()
    {
      return 0;
    }
  }

  public class HasPerTypeRules2 : Csla.BusinessBase<HasPerTypeRules2>
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

    //static HasPerTypeRules2()
    //{
    //  Csla.Validation.SharedValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, typeof(HasPerTypeRules2), "Test");
    //}

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, "Test");
      int value = (int)ApplicationContext.GlobalContext["Shared"];
      ApplicationContext.GlobalContext["Shared"] = ++value;
    }

    protected override void AddInstanceBusinessRules()
    {
      ValidationRules.AddInstanceRule(Csla.Validation.CommonRules.StringMaxLength, new Csla.Validation.CommonRules.MaxLengthRuleArgs("Test", 5));
    }

    public HasPerTypeRules2()
    {
      ValidationRules.CheckRules();
    }

    protected override object GetIdValue()
    {
      return 0;
    }
  }


  public class HasOnlyPerTypeRules : Csla.BusinessBase<HasOnlyPerTypeRules>
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

    //static HasPerTypeRules()
    //{
    //  Csla.Validation.SharedValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, typeof(HasPerTypeRules), "Test");
    //}

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, "Test");
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringMaxLength, new Csla.Validation.CommonRules.MaxLengthRuleArgs("Test", 5));
      int value = (int)ApplicationContext.GlobalContext["Shared"];
      ApplicationContext.GlobalContext["Shared"] = ++value;
    }

    public HasOnlyPerTypeRules()
    {
      ValidationRules.CheckRules();
    }

    protected override object GetIdValue()
    {
      return 0;
    }
  }
}
