using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace TestApp
{
  [Serializable]
  [Csla.Server.ObjectFactory("TestApp.CustomerEditFactory, Permissions")]
  public class CustomerEdit : BusinessBase<CustomerEdit>
  {
    #region Properties

    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public static readonly PropertyInfo<string> CityProperty = RegisterProperty<string>(c => c.City);
    public string City
    {
      get { return GetProperty(CityProperty); }
      set { SetProperty(CityProperty, value); }
    }

    #endregion

    #region Business rules

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.ReadProperty, NameProperty, "CustomerEdit.Name.Read"));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, NameProperty, "CustomerEdit.Name.Write"));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.ReadProperty, CityProperty, "CustomerEdit.City.Read"));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.WriteProperty, CityProperty, "CustomerEdit.City.Write"));

      BusinessRules.AddRule(new CanRead { PrimaryProperty = NameProperty, Priority = -2 });
      BusinessRules.AddRule(new CanWrite { PrimaryProperty = NameProperty, Priority = -2 });
      BusinessRules.AddRule(new StopProcessing { PrimaryProperty = NameProperty, Priority = -1 });

      BusinessRules.AddRule(new CanRead { PrimaryProperty = CityProperty, Priority = -2 });
      BusinessRules.AddRule(new CanWrite { PrimaryProperty = CityProperty, Priority = -2 });
      BusinessRules.AddRule(new StopProcessing { PrimaryProperty = CityProperty, Priority = -1 });

      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(NameProperty));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(CityProperty));
    }

    private class StopProcessing : Csla.Rules.BusinessRule
    {
      protected override void Execute(Csla.Rules.RuleContext context)
      {
        var obj = (CustomerEdit)context.Target;
        if (obj.BrokenRulesCollection.Where(c => c.Property == PrimaryProperty.Name).Count() > 0)
          context.AddInformationResult(null, true);
      }
    }

    private class CanWrite : Csla.Rules.BusinessRule
    {
      protected override void Execute(Csla.Rules.RuleContext context)
      {
        var target = (Csla.Core.BusinessBase)context.Target;
        if (!target.CanWriteProperty(PrimaryProperty))
          context.AddInformationResult("Not allowed to write property");
      }
    }

    private class CanRead : Csla.Rules.BusinessRule
    {
      protected override void Execute(Csla.Rules.RuleContext context)
      {
        var target = (Csla.Core.BusinessBase)context.Target;
        if (!target.CanReadProperty(PrimaryProperty))
          context.AddInformationResult("Not allowed to read property");
      }
    }

    #endregion

    #region Authorization rules

    private static void AddObjectAuthorizationRules()
    {
      Csla.Rules.BusinessRules.AddRule(typeof(CustomerEdit), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.CreateObject, "CustomerEdit.Create()"));
      Csla.Rules.BusinessRules.AddRule(typeof(CustomerEdit), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.GetObject, "CustomerEdit.Get()"));
      Csla.Rules.BusinessRules.AddRule(typeof(CustomerEdit), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.EditObject, "CustomerEdit.Edit()"));
      Csla.Rules.BusinessRules.AddRule(typeof(CustomerEdit), new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.DeleteObject, "CustomerEdit.Delete()"));
    }

    #endregion

    #region Constructors

    public CustomerEdit()
    { }

    public static CustomerEdit NewCustomer()
    {
      return DataPortal.Create<CustomerEdit>();
    }

    public static CustomerEdit GetCustomer(int id)
    {
      return DataPortal.Fetch<CustomerEdit>(
        new SingleCriteria<CustomerEdit, int>(id));
    }

    #endregion
  }
}
