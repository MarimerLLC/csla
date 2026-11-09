//-----------------------------------------------------------------------
// <copyright file="CalculatedRoot.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Business object with calculated properties</summary>
//-----------------------------------------------------------------------

using Csla.Core;
using Csla.Rules;

namespace Csla.Test.CalculatedProperties
{
  [Serializable]
  public partial class CalculatedRoot : BusinessBase<CalculatedRoot>
  {
    public static readonly PropertyInfo<int> Num1Property = RegisterProperty<int>(nameof(Num1));
    public int Num1
    {
      get => GetProperty(Num1Property);
      set => SetProperty(Num1Property, value);
    }

    public static readonly PropertyInfo<int> Num2Property = RegisterProperty<int>(nameof(Num2));
    public int Num2
    {
      get => GetProperty(Num2Property);
      set => SetProperty(Num2Property, value);
    }

    // calculated by a rule, getter only
    public static readonly PropertyInfo<int> TotalProperty = RegisterProperty<int>(nameof(Total));
    [CalculatedProperty]
    public int Total => GetProperty(TotalProperty);

    // calculated, set directly through SetProperty
    public static readonly PropertyInfo<string> LabelProperty = RegisterProperty<string>(nameof(Label));
    [CalculatedProperty]
    public string Label
    {
      get => GetProperty(LabelProperty);
      private set => SetProperty(LabelProperty, value);
    }

    // calculated, private backing field
    public static readonly PropertyInfo<string> SummaryProperty = RegisterProperty<string>(nameof(Summary), RelationshipTypes.PrivateField);
    private string _summary = SummaryProperty.DefaultValue;
    [CalculatedProperty]
    public string Summary
    {
      get => GetProperty(SummaryProperty, _summary);
      private set => SetProperty(SummaryProperty, ref _summary, value);
    }

    // NOT calculated, but set by a rule
    public static readonly PropertyInfo<int> DoubledProperty = RegisterProperty<int>(nameof(Doubled));
    public int Doubled => GetProperty(DoubledProperty);

    // not serialized, holds a non-native (IMobileObject) value
    public static readonly PropertyInfo<MobileList<string>> TagsProperty = RegisterProperty<MobileList<string>>(nameof(Tags));
    [DoNotSerialize]
    public MobileList<string> Tags
    {
      get => GetProperty(TagsProperty);
      set => SetProperty(TagsProperty, value);
    }

    public void SetLabel(string value) => Label = value;

    public void SetSummary(string value) => Summary = value;

    public void RecalculateTotal() => BusinessRules.CheckRules(TotalProperty);

    public void RecalculateDoubled() => BusinessRules.CheckRules(DoubledProperty);

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new SumRule(TotalProperty, Num1Property, Num2Property));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Dependency(Num1Property, TotalProperty));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Dependency(Num2Property, TotalProperty));
      BusinessRules.AddRule(new SumRule(DoubledProperty, Num1Property, Num1Property));
    }

    [Create]
    private void Create()
    {
      BusinessRules.CheckRules();
    }

    [Fetch]
    private void Fetch(int num1, int num2, bool runRules)
    {
      LoadProperty(Num1Property, num1);
      LoadProperty(Num2Property, num2);
      if (runRules)
      {
        BusinessRules.CheckRules();
        // Doubled is not a calculated property, so its rule output marks the object dirty
        MarkOld();
      }
    }

    private class SumRule : BusinessRule
    {
      private readonly IPropertyInfo[] _inputs;

      public SumRule(IPropertyInfo primaryProperty, params IPropertyInfo[] inputs)
        : base(primaryProperty)
      {
        _inputs = inputs;
        InputProperties.AddRange(inputs.Distinct());
      }

      protected override void Execute(IRuleContext context)
      {
        var sum = _inputs.Sum(p => (int)context.InputPropertyValues[p]);
        context.AddOutValue(PrimaryProperty, sum);
      }
    }
  }
}
