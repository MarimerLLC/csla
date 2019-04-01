using Csla.Core;
using Csla.Rules;
using Csla.Rules.CommonRules;
using Rolodex.Business.BusinessClasses;

namespace Rolodex.Business.Rules
{
  public class IsDuplicateNameAsync : CommonBusinessRule
  {
    private IPropertyInfo SecondaryProperty { get; set; }

    public IsDuplicateNameAsync(IPropertyInfo primaryProperty, IPropertyInfo secondaryProperty)
      : base(primaryProperty)
    {
      SecondaryProperty = secondaryProperty;

      InputProperties.Add(primaryProperty);
      InputProperties.Add(secondaryProperty);

      IsAsync = true;

      // setting all to false will only allow the rule to run when the property is set - typically by the user from the UI.
      CanRunAsAffectedProperty = false;
      CanRunInCheckRules = false;
      CanRunOnServer = false;
    }

    protected override void Execute(IRuleContext context)
    {
      var value1 = (int) context.InputPropertyValues[PrimaryProperty];
      var value2 = (string) context.InputPropertyValues[SecondaryProperty];

      // uses the async methods in DataPortal to perform data access on a background thread. 
      DuplicateCompanyCommand.BeginExecute(value1, value2, (o, e) =>
      {
        if (e.Error != null)
        {
          context.AddErrorResult(string.Format("Error checking for duplicate company name.  {0}", e.Error));
        }
        else if (e.Object.IsDuplicate)
        {
          context.AddErrorResult("Duplicate company name.");
        }

        context.Complete();
      });
    }
  }
}