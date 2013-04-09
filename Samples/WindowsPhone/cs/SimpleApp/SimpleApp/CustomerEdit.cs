using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Library
{
  [Serializable]
  public class CustomerEdit : BusinessBase<CustomerEdit>
  {
    private static PropertyInfo<int> IdProperty = RegisterProperty<int>(c =>c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(c=>c.Name);
    [Display(Description = "Customer name")]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    private static PropertyInfo<string> StatusProperty = RegisterProperty<string>(c=>c.Status);
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
      //BusinessRules.AddRule(new StringOnlyLettersAsync(NameProperty));

      BusinessRules.AddRule(
        new Csla.Rules.CommonRules.IsInRole(
          Csla.Rules.AuthorizationActions.WriteProperty, IdProperty, "None"));
    }

    private class StringOnlyLetters : Csla.Rules.BusinessRule
    {
      protected override void Execute(Csla.Rules.RuleContext context)
      {
        var ce = (CustomerEdit)context.Target;
        bool result = string.IsNullOrEmpty(ce.Name) ||
          !(from c in ce.Name.ToCharArray()
            where char.IsDigit(c)
            select c)
            .Any();
        if (!result)
          context.AddErrorResult("Name must consist of only letters.");
      }
    }

    private class StringOnlyLettersAsync : Csla.Rules.BusinessRule
    {
      public StringOnlyLettersAsync(Csla.Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      {
        IsAsync = true;
        InputProperties = new List<Csla.Core.IPropertyInfo> { primaryProperty };
      }

      protected override void Execute(Csla.Rules.RuleContext context)
      {
        var bw = new System.ComponentModel.BackgroundWorker();
        bw.DoWork += (o, e) =>
          {
            System.Threading.Thread.Sleep(2500);
            var name = (string)context.InputPropertyValues[PrimaryProperty];
            bool result = string.IsNullOrEmpty(name) ||
              !(from c in name.ToCharArray()
                where char.IsDigit(c)
                select c)
                .Any();
            if (!result)
              context.AddErrorResult("Name must consist of only letters.");
          };
        bw.RunWorkerCompleted += (o, e) =>
          {
            if (e.Error != null)
              context.AddErrorResult(e.Error.Message);
            context.Complete();
          };
        bw.RunWorkerAsync();
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

    protected override void DataPortal_Create()
    {
      using (BypassPropertyChecks)
        Status = "Created " + ApplicationContext.ExecutionLocation.ToString();
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
        Status = "Updated " + ApplicationContext.ExecutionLocation.ToString();
    }

    protected override void DataPortal_DeleteSelf()
    {
      System.Threading.Thread.Sleep(1500);
      using (BypassPropertyChecks)
        Status = "Deleted " + ApplicationContext.ExecutionLocation.ToString();
    }
  }
}