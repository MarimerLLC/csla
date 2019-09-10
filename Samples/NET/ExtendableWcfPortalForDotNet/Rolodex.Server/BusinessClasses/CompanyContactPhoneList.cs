using System;
using Csla;
using RolodexEF;

namespace Rolodex.Business.BusinessClasses
{
  [Serializable]
  public class CompanyContactPhoneList : BusinessListBase<CompanyContactPhoneList, CompanyContactPhone>
  {
    protected override CompanyContactPhone AddNewCore()
    {
      var newContactPhone = CompanyContactPhone.NewCompanyContactPhone();
      Add(newContactPhone);
      return newContactPhone;
    }

    internal static CompanyContactPhoneList GetCompanyContactPhoneList(CompanyContacts contact)
    {
      return DataPortal.FetchChild<CompanyContactPhoneList>(contact);
    }

    private void Child_Fetch(CompanyContacts contact)
    {
      RaiseListChangedEvents = false;
      foreach (var item in contact.CompanyContactPhones)
      {
        Add(CompanyContactPhone.GetCompanyContactPhone(item));
      }
      RaiseListChangedEvents = true;
    }

    internal static CompanyContactPhoneList NewCompanyContactPhoneList()
    {
      return DataPortal.CreateChild<CompanyContactPhoneList>();
    }
  }
}