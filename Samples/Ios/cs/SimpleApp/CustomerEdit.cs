using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;
using MonoTouch.Dialog;

namespace Library
{
  [Serializable]
  public class CustomerEdit : BusinessBase<CustomerEdit>
  {
    #region Business Methods

    private static PropertyInfo<int> IdProperty = RegisterProperty<int> (c => c.Id);
    public int Id {
      get { return GetProperty (IdProperty); }
      set { SetProperty (IdProperty, value); }
    }

    private static PropertyInfo<string> NameProperty = RegisterProperty<string> (c => c.Name);
    [Entry("Customer name")]
    public string Name {
      get { return GetProperty (NameProperty); }
      set { SetProperty (NameProperty, value); }
    }

    private static PropertyInfo<DateTime> BirthDateProperty = RegisterProperty<DateTime> (c => c.BirthDate);
    [Caption("Date of Birth")]
    [Date]
    public DateTime BirthDate {
      get { return GetProperty (BirthDateProperty); }
      set { SetProperty (BirthDateProperty, value); }
    }

    private static PropertyInfo<string> StatusProperty = RegisterProperty<string> (c => c.Status);
    public string Status {
      get { return GetProperty (StatusProperty); }
      set { SetProperty (StatusProperty, value); }
    }

    #endregion

    #region Business Rules

    protected override void AddBusinessRules ()
    {
      base.AddBusinessRules ();
      BusinessRules.AddRule (new Csla.Rules.CommonRules.Required (NameProperty));
      BusinessRules.AddRule (new StringOnlyLetters { PrimaryProperty = NameProperty });
      BusinessRules.AddRule (new ToUpper (NameProperty));
      BusinessRules.AddRule (new CheckBirthDate (StatusProperty, BirthDateProperty));
      //BusinessRules.AddRule(new StringOnlyLettersAsync(NameProperty));
      
      BusinessRules.AddRule (new Csla.Rules.CommonRules.IsInRole (Csla.Rules.AuthorizationActions.WriteProperty, IdProperty, "None"));
    }

    public override void BeginSave (bool forceUpdate, EventHandler<Csla.Core.SavedEventArgs> handler, object userState)
    {
      base.BeginSave (forceUpdate, handler, userState);
    }

    private class StringOnlyLetters : Csla.Rules.BusinessRule
    {
      protected override void Execute (Csla.Rules.RuleContext context)
      {
        var ce = (CustomerEdit)context.Target;
        bool result = string.IsNullOrEmpty (ce.Name) || !(from c in ce.Name.ToCharArray ()
          where char.IsDigit (c)
          select c).Any ();
        if (!result)
          context.AddErrorResult ("Name must consist of only letters.");
      }
    }

    public class ToUpper : Csla.Rules.BusinessRule
    {
      public ToUpper (Csla.Core.IPropertyInfo primaryProperty) : base(primaryProperty)
      {
        InputProperties = new List<Csla.Core.IPropertyInfo> { PrimaryProperty };
      }
      protected override void Execute (Csla.Rules.RuleContext context)
      {
        var input = context.InputPropertyValues[PrimaryProperty].ToString ();
        if (!string.IsNullOrEmpty (input)) {
          context.AddOutValue (PrimaryProperty, input.ToUpper ());
        }
      }
    }

    public class CheckBirthDate : Csla.Rules.BusinessRule
    {
      public CheckBirthDate (Csla.Core.IPropertyInfo primaryProperty, Csla.Core.IPropertyInfo birthDateProperty) : base(primaryProperty)
      {
        InputProperties = new List<Csla.Core.IPropertyInfo> ();
        InputProperties.Add (birthDateProperty);
      }

      protected override void Execute (Csla.Rules.RuleContext context)
      {
        var birthDateProperty = context.InputPropertyValues.Single (p => p.Key.Name == "BirthDateProperty");
        
        var status = "Unknown Date of Birth";
        if (birthDateProperty.Value != null) {
          var birthDate = (DateTime)birthDateProperty.Value;
          if (birthDate.CompareTo (DateTime.Today) == 0) {
            status = "Happy Birthday";
          }
          else {
            status = "It's not your birthday";
          }
        }
        
        context.AddOutValue (PrimaryProperty, status);
        
      }
    }
    #endregion
    #region Factory Methods

    #if SILVERLIGHT
    public static void BeginNewCustomer (DataPortal.ProxyModes proxyMode, EventHandler<DataPortalResult<CustomerEdit>> callback)
    {
      var dp = new DataPortal<CustomerEdit> (DataPortal.ProxyModes.LocalOnly);
      dp.CreateCompleted += callback;
      dp.BeginCreate ();
    }

    public static void BeginGetCustomer (DataPortal.ProxyModes proxyMode, EventHandler<DataPortalResult<CustomerEdit>> callback)
    {
      var dp = new DataPortal<CustomerEdit> ();
      dp.FetchCompleted += callback;
      dp.BeginFetch ();
    }

    [Obsolete("For use by MobileFormatter")]
    public CustomerEdit ()
    {
      // required by MobileFormatter
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public override void DataPortal_Create (Csla.DataPortalClient.LocalProxy<CustomerEdit>.CompletedHandler handler)
    {
      var bw = new System.ComponentModel.BackgroundWorker ();
      bw.DoWork += (s, e) => { e.Result = e.Argument; };
      bw.RunWorkerCompleted += (s, e) =>
      {
        try {
          using (BypassPropertyChecks) {
            BirthDate = DateTime.Today;
            Status = "Created " + ApplicationContext.ExecutionLocation.ToString ();
          }
          CreateComplete (handler);
        }
        catch (Exception ex) {
          handler (this, ex);
        }
      };
      bw.RunWorkerAsync ();
    }

    private void CreateComplete (Csla.DataPortalClient.LocalProxy<CustomerEdit>.CompletedHandler handler)
    {
      base.DataPortal_Create (handler);
    }
    #else
    public static CustomerEdit GetCustomer (int id)
    {
      return DataPortal.Fetch<CustomerEdit> (new SingleCriteria<CustomerEdit, int> (id));
    }

    private CustomerEdit ()
    {
    }
    /* require use of factory methods */
    #endif

    #endregion

    #region Data Access

    #if !SILVERLIGHT
    protected override void DataPortal_Create ()
    {
      using (BypassPropertyChecks) {
        Status = "Created " + ApplicationContext.ExecutionLocation.ToString ();
      }
      base.DataPortal_Create ();
    }

    private void DataPortal_Fetch (SingleCriteria<CustomerEdit, int> criteria)
    {
      System.Threading.Thread.Sleep (1500);
      using (BypassPropertyChecks) {
        Id = criteria.Value;
        Name = "Test " + criteria.Value;
        Status = "Retrieved " + ApplicationContext.ExecutionLocation.ToString ();
      }
    }

    protected override void DataPortal_Insert ()
    {
      System.Threading.Thread.Sleep (1500);
      
      using (BypassPropertyChecks) {
        Id = 987;
        Status = "Inserted " + ApplicationContext.ExecutionLocation.ToString ();
      }
    }

    protected override void DataPortal_Update ()
    {
      System.Threading.Thread.Sleep (1500);
      using (BypassPropertyChecks) {
        Status = "Updated " + ApplicationContext.ExecutionLocation.ToString ();
      }
    }

    protected override void DataPortal_DeleteSelf ()
    {
      System.Threading.Thread.Sleep (1500);
      using (BypassPropertyChecks) {
        Status = "Deleted " + ApplicationContext.ExecutionLocation.ToString ();
      }
    }
    #endif
    
    #endregion
  }
}
