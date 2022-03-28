using System.Collections.Generic;
using System.Linq;
using Csla.Core;
using Csla.Rules;

namespace BusinessRuleDemo
{

  /// <summary>
  /// This class demonstrates how to utilize inner rules inside a business rule. 
  /// 
  /// This rule calls inner rule StringRequired on PrimaryProperty if countryCode is US
  /// </summary>
  public class StringRequiredIfUS : Csla.Rules.BusinessRule
  {
    private IPropertyInfo _countryProperty;
    private IBusinessRule _innerRule;
    // TODO: Add additional parameters to your rule to the constructor
    public StringRequiredIfUS(IPropertyInfo primaryProperty, IPropertyInfo countryProperty)
      : base(primaryProperty)
    {
      _countryProperty = countryProperty;
      _innerRule = (IBusinessRule)new Csla.Rules.CommonRules.Required(primaryProperty);
      InputProperties = new List<IPropertyInfo>();

      // this rule needs the Country property
      InputProperties.Add(countryProperty);

      // add input properties required by inner rules
      var inputProps = _innerRule.InputProperties.Where(inputProp => !InputProperties.Contains(inputProp));
      if (inputProps.Count() > 0)
        InputProperties.AddRange(inputProps);
    }

#pragma warning disable CSLA0017 // Find Business Rules That Do Not Use Add() Methods on the Context
    protected override void Execute(IRuleContext context)
#pragma warning restore CSLA0017 // Find Business Rules That Do Not Use Add() Methods on the Context
    {
      // TODO: Add actual rule code here. 
      var country = (string)context.InputPropertyValues[_countryProperty];
      if (country == CountryNVL.UnitedStates)
      {
        _innerRule.Execute(context.GetChainedContext(_innerRule));
      }
    }
  }
}
