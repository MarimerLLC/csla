using System;
using Csla;
using Csla.Rules;
using Csla.Rules.CommonRules;

namespace Rolodex.Business.BusinessClasses
{
  [Serializable]
  public class ReadOnlyCompany : BusinessBase<ReadOnlyCompany>
  {
    public static readonly PropertyInfo<int> CompanyIdProperty =
      RegisterProperty<int>(new PropertyInfo<int>("CompanyId", "Company Id", 0));

    public int CompanyId
    {
      get { return GetProperty(CompanyIdProperty); }
    }

    public static readonly PropertyInfo<string> CompanyNameProperty =
      RegisterProperty<string>(new PropertyInfo<string>("CompanyName", "Company Name", string.Empty));

    public string CompanyName
    {
      get { return GetProperty(CompanyNameProperty); }
    }

    public static void AddObjectAuthorizationRules()
    {
      var canRead = new[] {"AdminUser", "RegularUser", "ReadOnlyUser"};
      BusinessRules.AddRule(typeof(Company), new IsInRole(AuthorizationActions.GetObject, canRead));
    }

    protected override void AddBusinessRules()
    {
      var canRead = new[] {"AdminUser", "RegularUser", "ReadOnlyUser"};

      FieldManager.GetRegisteredProperties()
        .ForEach(item => BusinessRules.AddRule(new IsInRole(AuthorizationActions.ReadProperty, item, canRead)));
    }

    public static ReadOnlyCompany GetReadOnlyCompany(int companyId, string companyName)
    {
      return DataPortal.FetchChild<ReadOnlyCompany>(companyId, companyName);
    }

    private void Child_Fetch(int companyId, string companyName)
    {
      LoadProperty(CompanyIdProperty, companyId);
      LoadProperty(CompanyNameProperty, companyName);
    }
  }
}