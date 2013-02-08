using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Core;
using Csla.Rules;

namespace Csla.Test.ValidationRules
{
  public class CascadeRoot : BusinessBase<CascadeRoot>
  {
    public static readonly PropertyInfo<int> Num1Property = RegisterProperty<int>(c => c.Num1);
    public int Num1
    {
      get { return GetProperty(Num1Property); }
      set { SetProperty(Num1Property, value); }
    }

    public static readonly PropertyInfo<int> Num2Property = RegisterProperty<int>(c => c.Num2);
    public int Num2
    {
      get { return GetProperty(Num2Property); }
      set { SetProperty(Num2Property, value); }
    }

    public static readonly PropertyInfo<int> Num3Property = RegisterProperty<int>(c => c.Num3);
    public int Num3
    {
      get { return GetProperty(Num3Property); }
      set { SetProperty(Num3Property, value); }
    }

    public static readonly PropertyInfo<int> Num4Property = RegisterProperty<int>(c => c.Num4);
    public int Num4
    {
      get { return GetProperty(Num4Property); }
      set { SetProperty(Num4Property, value); }
    }

    public static readonly PropertyInfo<int> Num5Property = RegisterProperty<int>(c => c.Num5);
    public int Num5
    {
      get { return GetProperty(Num5Property); }
      set { SetProperty(Num5Property, value); }
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();

      BusinessRules.AddRule(new Increment(Num1Property, Num2Property));
      BusinessRules.AddRule(new Increment(Num2Property, Num3Property));
      BusinessRules.AddRule(new Increment(Num3Property, Num4Property));
      BusinessRules.AddRule(new IncrementNotAffected(Num4Property, Num5Property));
    }
    public bool CascadeOnDirtyProperties
    {
      get { return BusinessRules.CascadeOnDirtyProperties; }
      set { BusinessRules.CascadeOnDirtyProperties = value; }
    }
  }


  internal class Increment : Csla.Rules.BusinessRule
  {
    private readonly PropertyInfo<int> _primaryProperty;
    private readonly PropertyInfo<int> _affectedProperty;

    public Increment(PropertyInfo<int> primaryProperty, PropertyInfo<int> affectedProperty) : base(primaryProperty)
    {
      _primaryProperty = primaryProperty;
      _affectedProperty = affectedProperty;
      if (InputProperties == null) InputProperties = new List<IPropertyInfo>();
      InputProperties.Add(primaryProperty);
      AffectedProperties.Add(affectedProperty);
    }

    protected override void Execute(RuleContext context)
    {
      var val = context.GetInputValue(_primaryProperty);
      context.AddOutValue(_affectedProperty, val + 1);
    }
  }

  internal class IncrementNotAffected : Csla.Rules.CommonRules.CommonBusinessRule
  {
    private readonly PropertyInfo<int> _primaryProperty;
    private readonly PropertyInfo<int> _affectedProperty;

    public IncrementNotAffected(PropertyInfo<int> primaryProperty, PropertyInfo<int> affectedProperty)
      : base(primaryProperty)
    {
      _primaryProperty = primaryProperty;
      _affectedProperty = affectedProperty;
      if (InputProperties == null) InputProperties = new List<IPropertyInfo>();
      InputProperties.Add(primaryProperty);
      AffectedProperties.Add(affectedProperty);

      this.CanRunAsAffectedProperty = false; 
    }

    protected override void Execute(RuleContext context)
    {
      var val = context.GetInputValue(_primaryProperty);
      context.AddOutValue(_affectedProperty, val + 1);
    }
  }
}
