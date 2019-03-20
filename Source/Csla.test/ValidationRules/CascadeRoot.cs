using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Core;
using Csla.Rules;
using Csla.Rules.CommonRules;

namespace Csla.Test.ValidationRules
{
  public class CascadeRoot : BusinessBase<CascadeRoot>
  {
    public void CheckRules()
    {
      BusinessRules.CheckRules();
    }

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

    internal static PropertyInfo<int> ValueAaProperty = RegisterProperty<int>(c => c.ValueAa);
    public int ValueAa
    {
      get { return GetProperty(ValueAaProperty); }
      set { SetProperty(ValueAaProperty, value); }
    }

    internal static PropertyInfo<int> ValueAbProperty = RegisterProperty<int>(c => c.ValueAb);
    public int ValueAb
    {
      get { return GetProperty(ValueAbProperty); }
      set { SetProperty(ValueAbProperty, value); }
    }

    internal static PropertyInfo<int> ValueAcProperty = RegisterProperty<int>(c => c.ValueAc);
    public int ValueAc
    {
      get { return GetProperty(ValueAcProperty); }
      set { SetProperty(ValueAcProperty, value); }
    }

    internal static PropertyInfo<int> ValueAdProperty = RegisterProperty<int>(c => c.ValueAd);
    public int ValueAd
    {
      get { return GetProperty(ValueAdProperty); }
      set { SetProperty(ValueAdProperty, value); }
    }

    internal static PropertyInfo<int> ValueAeProperty = RegisterProperty<int>(c => c.ValueAe);
    public int ValueAe
    {
      get { return GetProperty(ValueAeProperty); }
      set { SetProperty(ValueAeProperty, value); }
    }

    internal static PropertyInfo<int> ValueAfProperty = RegisterProperty<int>(c => c.ValueAf);
    public int ValueAf
    {
      get { return GetProperty(ValueAfProperty); }
      set { SetProperty(ValueAfProperty, value); }
    }

    internal static PropertyInfo<int> ValueAgProperty = RegisterProperty<int>(c => c.ValueAg);
    public int ValueAg
    {
      get { return GetProperty(ValueAgProperty); }
      set { SetProperty(ValueAgProperty, value); }
    }

    internal static PropertyInfo<int> ValueBaProperty = RegisterProperty<int>(c => c.ValueBa);
    public int ValueBa
    {
      get { return GetProperty(ValueBaProperty); }
      set { SetProperty(ValueBaProperty, value); }
    }

    internal static PropertyInfo<int> ValueBbProperty = RegisterProperty<int>(c => c.ValueBb);
    public int ValueBb
    {
      get { return GetProperty(ValueBbProperty); }
      set { SetProperty(ValueBbProperty, value); }
    }

    internal static PropertyInfo<decimal> ValueCaProperty = RegisterProperty<decimal>(c => c.ValueCa);
    public decimal ValueCa
    {
      get { return GetProperty(ValueCaProperty); }
      set { SetProperty(ValueCaProperty, value); }
    }

    internal static PropertyInfo<decimal> ValueCbProperty = RegisterProperty<decimal>(c => c.ValueCb);
    public decimal ValueCb
    {
      get { return GetProperty(ValueCbProperty); }
      set { SetProperty(ValueCbProperty, value); }
    }

    internal static PropertyInfo<decimal> ValueCcProperty = RegisterProperty<decimal>(c => c.ValueCc);
    public decimal ValueCc
    {
      get { return GetProperty(ValueCcProperty); }
      set { SetProperty(ValueCcProperty, value); }
    }

    internal static PropertyInfo<decimal> ValueCdProperty = RegisterProperty<decimal>(c => c.ValueCd);
    public decimal ValueCd
    {
      get { return GetProperty(ValueCdProperty); }
      set { SetProperty(ValueCdProperty, value); }
    }

    internal static PropertyInfo<decimal> ValueCeProperty = RegisterProperty<decimal>(c => c.ValueCe);
    public decimal ValueCe
    {
      get { return GetProperty(ValueCeProperty); }
      set { SetProperty(ValueCeProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();

      BusinessRules.AddRule(new Increment(Num1Property, Num2Property));
      BusinessRules.AddRule(new Increment(Num2Property, Num3Property));
      BusinessRules.AddRule(new Increment(Num3Property, Num4Property));
      BusinessRules.AddRule(new IncrementNotAffected(Num4Property, Num5Property));

      // complex ruleset 
      // calculate the sum of Aa and Ab to Ac
      // copy value of Ac to Ae
      // calculate sum of Ad and Ae to Af
      // copy value of Af to Ag
      BusinessRules.AddRule(new CalcSumRule(ValueAcProperty, ValueAaProperty, ValueAbProperty) { Priority = -1 });
      BusinessRules.AddRule(new CopyValueRule(ValueAeProperty, ValueAcProperty));
      BusinessRules.AddRule(new CalcSumRule(ValueAfProperty, ValueAdProperty, ValueAeProperty) { Priority = -1 });
      BusinessRules.AddRule(new CopyValueRule(ValueAgProperty, ValueAfProperty));

      // check that the sum of Ba and Bb is always 100 (and error message on both properties)
      BusinessRules.AddRule(new CheckSumRule(ValueBaProperty, ValueBbProperty, 100));
      BusinessRules.AddRule(new CheckSumRule(ValueBbProperty, ValueBaProperty, 100));

      // calculate sum of Ca, Cb, Cc and Cd to Ce
      // calculate fraction of Ce to Cd
      // must then recalculate sum again as Cd was changed.
      BusinessRules.AddRule(new CalcSumRule(ValueCeProperty, ValueCaProperty, ValueCbProperty, ValueCcProperty, ValueCdProperty));
      BusinessRules.AddRule(new CalculateFractionRule(ValueCdProperty, ValueCeProperty, 0.25m));
    }
    public bool CascadeOnDirtyProperties
    {
      get { return BusinessRules.CascadeOnDirtyProperties; }
      set { BusinessRules.CascadeOnDirtyProperties = value; }
    }
  }

  internal class CheckSumRule : CommonBusinessRule
  {
    private readonly int _sumValue;
    private readonly PropertyInfo<int> _value1Property;
    private readonly PropertyInfo<int> _value2Property;
    public CheckSumRule(PropertyInfo<int> value1Property, PropertyInfo<int> value2Property, int sumValue)
      : base(value1Property)
    {
      _sumValue = sumValue;
      InputProperties.Add(value1Property);
      InputProperties.Add(value2Property);

      _value1Property = value1Property;
      _value2Property = value2Property;

      RuleUri.AddQueryParameter("sumvalue", sumValue.ToString());
    }

    protected override string GetMessage()
    {
      return HasMessageDelegate ? base.MessageText : "The sum of {0} and {1} must be equal to {2}.";
    }

    protected override void Execute(IRuleContext context)
    {
      var value1 = context.GetInputValue(_value1Property);
      var value2 = context.GetInputValue(_value2Property);
      if (value1 + value2 != _sumValue)
      {
        var message = string.Format(GetMessage(), PrimaryProperty.FriendlyName, _value2Property.FriendlyName, _sumValue);
        context.Results.Add(new RuleResult(RuleName, PrimaryProperty, message) { Severity = Severity });
      }
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

    protected override void Execute(IRuleContext context)
    {
      var val = context.GetInputValue(_primaryProperty);
      context.AddOutValue(_affectedProperty, val + 1);
    }
  }

  internal class CopyValueRule : CommonBusinessRule
  {
    private readonly PropertyInfo<int> _sourceProperty;

    public CopyValueRule(PropertyInfo<int> targetProperty, PropertyInfo<int> sourceProperty)
      : base(targetProperty)
    {
      InputProperties.Add(sourceProperty);
      _sourceProperty = sourceProperty;
    }

    protected override void Execute(IRuleContext context)
    {
      var source = context.GetInputValue(_sourceProperty);
      context.AddOutValue(source);
    }
  }

  public class CalculateFractionRule : CommonBusinessRule
  {
    private readonly decimal _fraction;
    private readonly PropertyInfo<decimal> _sourceProperty;

    public CalculateFractionRule(PropertyInfo<decimal> targetProperty, PropertyInfo<decimal> sourceProperty, decimal fraction)
      : base(targetProperty)
    {
      InputProperties.Add(sourceProperty);

      _sourceProperty = sourceProperty;
      _fraction = fraction;

      RuleUri.AddQueryParameter("fraction", fraction.ToString());
    }

    protected override void Execute(IRuleContext context)
    {
      var source = context.GetInputValue(_sourceProperty);
      context.AddOutValue(Math.Round(source * _fraction, 2, MidpointRounding.AwayFromZero));
    }
  }

  public class CalcSumRule : PropertyRule
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CalcSumRule"/> class.
    /// </summary>
    /// <param name="primaryProperty">The primary property.</param>
    /// <param name="inputProperties">The input properties.</param>
    public CalcSumRule(IPropertyInfo primaryProperty, params IPropertyInfo[] inputProperties)
      : base(primaryProperty)
    {
      InputProperties = new List<IPropertyInfo>();
      InputProperties.AddRange(inputProperties);
    }

    /// <summary>
    /// Business or validation rule implementation.
    /// </summary>
    /// <param name="context">Rule context object.</param>
    protected override void Execute(IRuleContext context)
    {
      // Use linq Sum to calculate the sum value
      dynamic sum = context.InputPropertyValues.Aggregate<KeyValuePair<IPropertyInfo, object>, dynamic>(0, (current, item) => current + (dynamic)item.Value);

      // add calculated value to OutValues
      // When rule is completed the RuleEngine will update businessobject
      context.AddOutValue(PrimaryProperty, sum);
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

    protected override void Execute(IRuleContext context)
    {
      var val = context.GetInputValue(_primaryProperty);
      context.AddOutValue(_affectedProperty, val + 1);
    }
  }
}
