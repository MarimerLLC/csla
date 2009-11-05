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

    public static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
    }

    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public static PropertyInfo<string> CityProperty = RegisterProperty<string>(c => c.City);
    public string City
    {
      get { return GetProperty(CityProperty); }
      set { SetProperty(CityProperty, value); }
    }

    #endregion

    #region Business rules

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(Csla.Validation.CommonRules.CanRead, NameProperty, -2);
      ValidationRules.AddRule(Csla.Validation.CommonRules.CanWrite, NameProperty, -2);
      ValidationRules.AddRule(StopProcessing, NameProperty, -1);
      ValidationRules.AddRule(Csla.Validation.CommonRules.CanRead, CityProperty, -2);
      ValidationRules.AddRule(Csla.Validation.CommonRules.CanWrite, CityProperty, -2);
      ValidationRules.AddRule(StopProcessing, CityProperty, -1);

      ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, NameProperty);
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, CityProperty);
    }

    private static bool StopProcessing(object target, Csla.Validation.RuleArgs e)
    {
      var obj = target as CustomerEdit;
      if (obj.BrokenRulesCollection.Where(c => c.Property == e.PropertyName).Count() > 0)
        e.StopProcessing = true;
      return true;
    }

    #endregion

    #region Authorization rules

    protected override void AddAuthorizationRules()
    {
      AuthorizationRules.AllowRead(NameProperty, "CustomerEdit.Name.Read");
      AuthorizationRules.AllowWrite(NameProperty, "CustomerEdit.Name.Write");
      AuthorizationRules.AllowRead(CityProperty, "CustomerEdit.City.Read");
      AuthorizationRules.AllowWrite(CityProperty, "CustomerEdit.City.Write");
    }

    private static void AddObjectAuthorizationRules()
    {
      Csla.Security.AuthorizationRules.AllowCreate(typeof(CustomerEdit), "CustomerEdit.Create()");
      Csla.Security.AuthorizationRules.AllowGet(typeof(CustomerEdit), "CustomerEdit.Get()");
      Csla.Security.AuthorizationRules.AllowEdit(typeof(CustomerEdit), "CustomerEdit.Edit()");
      Csla.Security.AuthorizationRules.AllowDelete(typeof(CustomerEdit), "CustomerEdit.Delete()");
    }

    #endregion

    #region Constructors

    private CustomerEdit()
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
