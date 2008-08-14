using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;
using Csla.Validation;
using System.Text.RegularExpressions;
using Csla.Security;
using System.ComponentModel;
using System.Threading;

namespace DataBinding.Business
{
  [Serializable]
  public partial class Customer : BusinessBase<Customer>
  {
    #region Properties

    private static PropertyInfo<int> IdProperty = RegisterProperty(new PropertyInfo<int>("Id"));
    private static PropertyInfo<string> NameProperty = RegisterProperty(new PropertyInfo<string>("Name"));
    private static PropertyInfo<DateTime> BirthDateProperty = RegisterProperty(new PropertyInfo<DateTime>("BirthDate"));
    private static PropertyInfo<bool> IsPreferredProperty = RegisterProperty(new PropertyInfo<bool>("IsPreferred"));

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

    public bool IsPreferred
    {
      get { return GetProperty(IsPreferredProperty); }
      set { SetProperty(IsPreferredProperty, value); }
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
      ValidationRules.AddRule(Csla.Validation.CommonRules.CanRead, NameProperty, -1);
      ValidationRules.AddRule(Csla.Validation.CommonRules.CanRead, BirthDateProperty, -1);
      ValidationRules.AddRule(Csla.Validation.CommonRules.CanWrite, NameProperty, -1);
      ValidationRules.AddRule(Csla.Validation.CommonRules.CanWrite, BirthDateProperty, -1);

      ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, NameProperty);
      ValidationRules.AddRule(NiceName, NameProperty);
      ValidationRules.AddRule(HasNumbers, NameProperty);

      ValidationRules.AddRule(IsReserved, new AsyncRuleArgs(NameProperty));
      ValidationRules.AddRule(HasNotes, new AsyncRuleArgs(IsPreferredProperty, NameProperty));
      base.AddBusinessRules();
    }

    protected override void AddInstanceAuthorizationRules()
    {
      AuthorizationRules.AllowRead(BirthDateProperty, "Administrators");
      AuthorizationRules.AllowWrite(BirthDateProperty, "Administrators");

      AuthorizationRules.AllowRead(BirthDateProperty, "Users");
      AuthorizationRules.DenyWrite(BirthDateProperty, "Users");

      AuthorizationRules.DenyRead(BirthDateProperty, "Guests");
    }
    public static void AddObjectAuthorizationRules()
    {
      AuthorizationRules.AllowGet(typeof(Customer), "Administrators");
      AuthorizationRules.AllowEdit(typeof(Customer), "Administrators");
      AuthorizationRules.AllowDelete(typeof(Customer), "Administrators");
      AuthorizationRules.AllowCreate(typeof(Customer), "Administrators");

      AuthorizationRules.AllowGet(typeof(Customer), "Users");
      AuthorizationRules.AllowEdit(typeof(Customer), "Users");

      AuthorizationRules.AllowGet(typeof(Customer), "Guests");
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

    public static void IsReserved(AsyncValidationRuleContext context)
    {
      BackgroundWorker worker = new BackgroundWorker();
      worker.DoWork += (o, e) =>
      {
        Thread.Sleep(3000); // simulate network call or long running process

        string name = (string)context.PropertyValues["Name"];
        if (name == "<reserved>")
        {
          context.OutArgs.Description = "This value is reserved, you must select a new value";
          context.OutArgs.Severity = RuleSeverity.Error;
          context.OutArgs.Result = false;
        }
      };
      worker.RunWorkerCompleted += (o, e) =>
      {
        context.Complete();
      };

      worker.RunWorkerAsync();
    }

    public static void HasNotes(AsyncValidationRuleContext context)
    {

      BackgroundWorker worker = new BackgroundWorker();
      worker.DoWork += (o, e) =>
      {
        Thread.Sleep(1000); // simulate network call or long running process

        bool isPreferred = (bool)context.PropertyValues["IsPreferred"];
        string name = (string)context.PropertyValues["Name"];
        if (isPreferred && name == "justin")
        {
          context.OutArgs.Description = "This customer is extra special, please take good care of him!";
          context.OutArgs.Severity = RuleSeverity.Information;
          context.OutArgs.Result = false;
        }
      };
      worker.RunWorkerCompleted += (o, e) =>
      {
        context.Complete();
      };

      worker.RunWorkerAsync();
    }

    #endregion
  }
}
