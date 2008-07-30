using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;
using Csla.Validation;
using System.Text.RegularExpressions;

namespace DataBinding.Business
{
  [Serializable]
  public partial class Customer : BusinessBase<Customer>
  {
    #region Properties

    private static PropertyInfo<int> IdProperty = RegisterProperty(new PropertyInfo<int>("Id"));
    private static PropertyInfo<string> NameProperty = RegisterProperty(new PropertyInfo<string>("Name"));
    private static PropertyInfo<DateTime> BirthDateProperty = RegisterProperty(new PropertyInfo<DateTime>("BirthDate"));

    public int Id
    {
      get { return GetProperty(IdProperty); }
    }

    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public DateTime BirthDate
    {
      get { return GetProperty(BirthDateProperty); }
      set { SetProperty(BirthDateProperty, value); }
    } 

    #endregion

    #region Object overrides

    public override string ToString()
    {
      return Name;
    } 

    #endregion

    #region Business rules

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, NameProperty);
      ValidationRules.AddRule(NiceName, NameProperty);
      ValidationRules.AddRule(HasNumbers, NameProperty);
      base.AddBusinessRules();
    }

    public static bool NiceName(object target, RuleArgs e)
    {
      string value = (string)Utilities.CallByName(target, e.PropertyName, CallType.Get);
      if (!string.IsNullOrEmpty(value) && value.ToLower().StartsWith("justin"))
      {
        e.Severity = RuleSeverity.Information;
        e.Description = "Nice name!";
        return false;
      }
      return true;
    }

    private static Regex HasNumbersRegex = new Regex("[0-9]");
    public static bool HasNumbers(object target, RuleArgs e)
    {
      string value = (string)Utilities.CallByName(target, e.PropertyName, CallType.Get);
      if (!string.IsNullOrEmpty(value) && HasNumbersRegex.IsMatch(value))
      {
        e.Severity = RuleSeverity.Warning;
        e.Description = "Names shouldn't have numbers, you should probably correct this!";
        return false;
      }
      return true;
    }
    #endregion
  }
}
