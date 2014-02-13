using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;
using System.Threading.Tasks;

namespace SimpleApp
{
  [Serializable]
  public class CustomerEdit : BusinessBase<CustomerEdit>
  {
    private static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    private static PropertyInfo<DateTime> BirthDateProperty = RegisterProperty<DateTime>(c => c.BirthDate);
    public DateTime BirthDate
    {
      get { return GetProperty(BirthDateProperty); }
      set { SetProperty(BirthDateProperty, value); }
    }

    private static PropertyInfo<string> StatusProperty = RegisterProperty<string>(c => c.Status);
    public string Status
    {
      get { return GetProperty(StatusProperty); }
      set { SetProperty(StatusProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(NameProperty));
      BusinessRules.AddRule(new StringOnlyLetters { PrimaryProperty = NameProperty });
      BusinessRules.AddRule(new ToUpper(NameProperty));
      BusinessRules.AddRule(new CheckBirthDate(StatusProperty, BirthDateProperty));
      //BusinessRules.AddRule(new StringOnlyLettersAsync(NameProperty));

      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, IdProperty, "None"));
    }

    private class StringOnlyLetters : Csla.Rules.BusinessRule
    {
      protected override void Execute(Csla.Rules.RuleContext context)
      {
        var ce = (CustomerEdit)context.Target;
        bool result = string.IsNullOrEmpty(ce.Name) || !(from c in ce.Name.ToCharArray()
                                                         where char.IsDigit(c)
                                                         select c).Any();
        if (!result)
          context.AddErrorResult("Name must consist of only letters.");
      }
    }

    public class ToUpper : Csla.Rules.BusinessRule
    {
      public ToUpper(Csla.Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      {
        InputProperties = new List<Csla.Core.IPropertyInfo> { PrimaryProperty };
      }
      protected override void Execute(Csla.Rules.RuleContext context)
      {
        var input = context.InputPropertyValues[PrimaryProperty].ToString();
        if (!string.IsNullOrEmpty(input))
        {
          context.AddOutValue(PrimaryProperty, input.ToUpper());
        }
      }
    }

    public class CheckBirthDate : Csla.Rules.BusinessRule
    {
      public CheckBirthDate(Csla.Core.IPropertyInfo primaryProperty, Csla.Core.IPropertyInfo birthDateProperty)
        : base(primaryProperty)
      {
        InputProperties = new List<Csla.Core.IPropertyInfo>();
        InputProperties.Add(birthDateProperty);
      }

      protected override void Execute(Csla.Rules.RuleContext context)
      {
        var birthDateProperty = context.InputPropertyValues.Single(p => p.Key.Name == "BirthDateProperty");

        var status = "Unknown Date of Birth";
        if (birthDateProperty.Value != null)
        {
          var birthDate = (DateTime)birthDateProperty.Value;
          if (birthDate.CompareTo(DateTime.Today) == 0)
          {
            status = "Happy Birthday";
          }
          else
          {
            status = "It's not your birthday";
          }
        }

        context.AddOutValue(PrimaryProperty, status);

      }
    }

    public static void NewCustomerEdit(EventHandler<DataPortalResult<CustomerEdit>> callback)
    {
      DataPortal.BeginCreate<CustomerEdit>(callback);
    }

    public static void GetCustomerEdit(int id, EventHandler<DataPortalResult<CustomerEdit>> callback)
    {
      DataPortal.BeginFetch<CustomerEdit>(id, callback);
    }

    public static async Task<CustomerEdit> NewCustomerEditAsync()
    {
      return await DataPortal.CreateAsync<CustomerEdit>();
    }

    public static async Task<CustomerEdit> GetCustomerEditAsync(int id)
    {
      return await DataPortal.FetchAsync<CustomerEdit>(id);
    }

#if !SILVERLIGHT && !WINDOWS_PHONE && !NETFX_CORE

    public static CustomerEdit NewCustomerEdit()
    {
      return DataPortal.Create<CustomerEdit>();
    }

    public static CustomerEdit GetCustomerEdit(int id)
    {
      return DataPortal.Fetch<CustomerEdit>(id);
    }

    public static void DeleteCustomerEdit(int id)
    {
      DataPortal.Delete<CustomerEdit>(id);
    }

#endif

    protected override void DataPortal_Create()
    {
      using (BypassPropertyChecks)
      {
        Status = "Created " + ApplicationContext.ExecutionLocation.ToString();
      }
      base.DataPortal_Create();
    }

    private void DataPortal_Fetch(SingleCriteria<CustomerEdit, int> criteria)
    {
      System.Threading.Thread.Sleep(1500);
      using (BypassPropertyChecks)
      {
        Id = criteria.Value;
        Name = "Test " + criteria.Value;
        Status = "Retrieved " + ApplicationContext.ExecutionLocation.ToString();
      }
    }

    protected override void DataPortal_Insert()
    {
      System.Threading.Thread.Sleep(1500);

      using (BypassPropertyChecks)
      {
        Id = 987;
        Status = "Inserted " + ApplicationContext.ExecutionLocation.ToString();
      }
    }

    protected override void DataPortal_Update()
    {
      System.Threading.Thread.Sleep(1500);
      using (BypassPropertyChecks)
      {
        Status = "Updated " + ApplicationContext.ExecutionLocation.ToString();
      }
    }

    protected override void DataPortal_DeleteSelf()
    {
      System.Threading.Thread.Sleep(1500);
      using (BypassPropertyChecks)
      {
        Status = "Deleted " + ApplicationContext.ExecutionLocation.ToString();
      }
    }
  }
}
