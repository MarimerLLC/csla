using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;
using Csla.Silverlight;
using Csla.Validation;

#if!SILVERLIGHT
using System.Data.SqlClient;
using Rolodex.Business.Data;
using Csla.Data;
#endif

namespace Rolodex.Business.BusinessClasses
{
  [Serializable]
  public class CompanyContactList : BusinessListBase<CompanyContactList, CompanyContact>
  {

#if SILVERLIGHT
    public CompanyContactList() { MarkAsChild(); }
    protected override void AddNewCore()
    {
      CompanyContact newContact = CompanyContact.NewCompanyContact();
      Add(newContact);
    }
#else
    private CompanyContactList() { MarkAsChild();}
    protected override object AddNewCore()
    {
      CompanyContact newContact = CompanyContact.NewCompanyContact();
      Add(newContact);
      return newContact;
    }
#endif



    internal static CompanyContactList NewCompanyContactList()
    {
      return new CompanyContactList();
    }

#if !SILVERLIGHT
    internal static CompanyContactList GetCompanyContactList(SafeDataReader reader)
    {
      return DataPortal.FetchChild<CompanyContactList>(reader);
    }

    private void Child_Fetch(SafeDataReader reader)
    {
      this.RaiseListChangedEvents = false;
      while (reader.Read())
      {
        Add(CompanyContact.GetCompanyContact(reader));
      }
      this.RaiseListChangedEvents = true;
    }

#endif


  }
}
