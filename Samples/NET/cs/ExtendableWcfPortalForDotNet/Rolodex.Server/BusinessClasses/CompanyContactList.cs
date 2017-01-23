using System;
using Csla;
using RolodexEF;

namespace Rolodex.Business.BusinessClasses
{
  [Serializable]
  public class CompanyContactList : BusinessListBase<CompanyContactList, CompanyContact>
  {
    internal Company ParentCompany
    {
      get { return Parent as Company; }
    }

    protected override CompanyContact AddNewCore()
    {
      var newContact = CompanyContact.NewCompanyContact();
      Add(newContact);
      return newContact;
    }

    internal static CompanyContactList NewCompanyContactList()
    {
      return DataPortal.CreateChild<CompanyContactList>();
    }

    internal static CompanyContactList GetCompanyContactList(Companies company)
    {
      return DataPortal.FetchChild<CompanyContactList>(company);
    }

    private void Child_Fetch(Companies company)
    {
      RaiseListChangedEvents = false;
      foreach (var item in company.CompanyContacts)
      {
        Add(CompanyContact.GetCompanyContact(item));
      }
      RaiseListChangedEvents = true;
    }
  }
}