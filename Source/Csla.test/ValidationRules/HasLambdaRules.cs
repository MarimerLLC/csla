using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Rules;

namespace Csla.Test.ValidationRules
{
  [Serializable]
  public class HasLambdaRules : BusinessBase<HasLambdaRules>
  {
    public static PropertyInfo<string> SsnProperty = RegisterProperty<string>(c => c.Ssn);

    public string Ssn
    {
      get { return GetProperty(SsnProperty); }
      set { SetProperty(SsnProperty, value); }
    }

    public static PropertyInfo<string> Ssn2Property = RegisterProperty<string>(c => c.Ssn2);

    public string Ssn2
    {
      get { return GetProperty(Ssn2Property); }
      set { SetProperty(Ssn2Property, value); }
    }

    public static HasLambdaRules New()
    {
      return Csla.DataPortal.Create<HasLambdaRules>();
    }

    [RunLocal]
    protected override void DataPortal_Create()
    {
      LoadProperty(SsnProperty, string.Empty);
      LoadProperty(Ssn2Property, string.Empty);
      BusinessRules.CheckRules();
    }

    public string[] GetRuleDescriptions()
    {return BusinessRules.GetRuleDescriptions();
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule<HasLambdaRules>(SsnProperty, (o) => !string.IsNullOrEmpty(o.Ssn), "must have value 1");
      BusinessRules.AddRule<HasLambdaRules>(SsnProperty, (o) => o.Ssn != null  && o.Ssn.Length > 15, "cannot be longer than 15 chars");

      BusinessRules.AddRule<HasLambdaRules>(SsnProperty, (o) => !string.IsNullOrEmpty(o.Ssn2), () =>  "must have value 1");
      BusinessRules.AddRule<HasLambdaRules>(SsnProperty, (o) => !string.IsNullOrEmpty(o.Ssn2), () =>  "must have value 2");
    }
  }
}
