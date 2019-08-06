using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Library
{
  [Serializable]
  public class CustomerEdit : BusinessBase<CustomerEdit>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c =>c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c=>c.Name);
    [Display(Description = "Customer name")]
    [Required(ErrorMessage = "Name required")]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public static readonly PropertyInfo<string> StatusProperty = RegisterProperty<string>(c=>c.Status);
    public string Status
    {
      get { return GetProperty(StatusProperty); }
      set { SetProperty(StatusProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new StringOnlyLetters { PrimaryProperty = NameProperty });
      //BusinessRules.AddRule(new StringOnlyLettersAsync(NameProperty));

      BusinessRules.AddRule(
        new Csla.Rules.CommonRules.IsInRole(
          Csla.Rules.AuthorizationActions.WriteProperty, IdProperty, "None"));
    }

    /// <summary>
    /// Example sync and local business rule
    /// </summary>
    private class StringOnlyLetters : Csla.Rules.BusinessRule
    {
      protected override void Execute(Csla.Rules.IRuleContext context)
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

    /// <summary>
    /// Example async business rule
    /// </summary>
    private class StringOnlyLettersAsync : Csla.Rules.BusinessRule
    {
      public StringOnlyLettersAsync(Csla.Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      {
        IsAsync = true;
        InputProperties.Add(primaryProperty);
      }

      protected async override void Execute(Csla.Rules.IRuleContext context)
      {
        var tcs = new TaskCompletionSource<bool>();
        new Task(() =>
          {
            var name = (string)context.InputPropertyValues[PrimaryProperty];
            tcs.SetResult(string.IsNullOrEmpty(name) ||
              !(from c in name.ToCharArray()
                where char.IsDigit(c)
                select c)
                .Any());
          }).Start();
        var result = await tcs.Task;
        if (!result)
          context.AddErrorResult("Name must consist of only letters.");
        context.Complete();
      }
    }

    public static async Task<CustomerEdit> NewCustomerEditAsync()
    {
      return await DataPortal.CreateAsync<CustomerEdit>();
    }

    public static async Task<CustomerEdit> GetCustomerEditAsync(int id)
    {
      return await DataPortal.FetchAsync<CustomerEdit>(id);
    }

    public static void NewCustomerEdit(EventHandler<DataPortalResult<CustomerEdit>> callback)
    {
      DataPortal.BeginCreate<CustomerEdit>(callback);
    }

    public static void GetCustomerEdit(int id, EventHandler<DataPortalResult<CustomerEdit>> callback)
    {
      DataPortal.BeginFetch<CustomerEdit>(id, callback);
    }

#if !SILVERLIGHT && !NETFX_CORE && !WINDOWS_PHONE

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

    [RunLocal]
    protected override void DataPortal_Create()
    {
      using (BypassPropertyChecks)
      {
        Id = 7000;
        Status = "Created " + ApplicationContext.ExecutionLocation.ToString();
      }
      base.DataPortal_Create();
    }

    private void DataPortal_Fetch(int id)
    {
      System.Threading.Thread.Sleep(1500);
      using (BypassPropertyChecks)
      {
        Id = id;
        Name = "Test " + id;
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