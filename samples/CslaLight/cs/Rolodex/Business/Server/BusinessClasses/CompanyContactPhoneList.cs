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
  public class CompanyContactPhoneList : BusinessListBase<CompanyContactPhoneList,CompanyContactPhone>
  {

#if SILVERLIGHT
    public CompanyContactPhoneList() { MarkAsChild(); }
    protected override void AddNewCore()
    {
      CompanyContactPhone newContactPhone = CompanyContactPhone.NewCompanyContactPhone();
      Add(newContactPhone);
    }

#else
    private CompanyContactPhoneList() { MarkAsChild(); }
    protected override object AddNewCore()
    {
      CompanyContactPhone newContactPhone = CompanyContactPhone.NewCompanyContactPhone();
      Add(newContactPhone);
      return newContactPhone;
    }
#endif

    internal static CompanyContactPhoneList NewCompanyContactPhoneList()
    {
      return new CompanyContactPhoneList();
    }
  }
}
