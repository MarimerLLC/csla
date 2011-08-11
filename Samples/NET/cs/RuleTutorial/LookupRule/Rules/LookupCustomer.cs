using System.Collections.Generic;
using Csla.Core;
using Csla.Rules;
using LookupRule.Commands;

namespace LookupRule.Rules
{
  /// <summary>
  /// A typical LookupCustomer command for use in WEB applications
  /// 
  /// In a WEB app you will typically want to update all the fields as returned in the HTTP POST command
  /// and call CheckRules to run all rules afterwards. 
  /// </summary>
  public class LookupCustomer : PropertyRule
  {
    private IPropertyInfo NameProperty { get; set; }

    public LookupCustomer(IPropertyInfo primaryProperty, IPropertyInfo nameProperty)
      : base(primaryProperty)
    {
      NameProperty = nameProperty;

      if (InputProperties == null)
        InputProperties = new List<IPropertyInfo>() { PrimaryProperty };
      AffectedProperties.Add(nameProperty);

      IsAsync = false;
      CanRunAsAffectedProperty = false;
      CanRunInCheckRules = true;
      CanRunOnServer = false;
    }

    protected override void Execute(RuleContext context)
    {
      var id = (int) context.InputPropertyValues[PrimaryProperty];

      // use a command or read-only object for lookup
      var lookup = LookupCustomerCommand.Execute(id);

      context.AddOutValue(NameProperty, lookup.Name);
    }
  }
}
