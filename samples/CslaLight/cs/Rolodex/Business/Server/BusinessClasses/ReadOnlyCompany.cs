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
using System.ComponentModel;

#if!SILVERLIGHT
using Csla.Data;
using System.Data.SqlClient;
using Rolodex.Business.Data;
#endif

namespace Rolodex.Business.BusinessClasses
{
  [Serializable]
  public class ReadOnlyCompany : ReadOnlyBase<ReadOnlyCompany>
  {
    
#if SILVERLIGHT
    public ReadOnlyCompany() { }
#else
    private ReadOnlyCompany() { }
#endif


    private static PropertyInfo<int> CompanyIdProperty = RegisterProperty<int>(new PropertyInfo<int>("CompanyId", "Company Id", 0));
    public int CompanyId
    {
      get
      {
        return GetProperty(CompanyIdProperty);
      }
    }

    private static PropertyInfo<string> CompanyNameProperty = RegisterProperty<string>(new PropertyInfo<string>("CompanyName", "Company Name", string.Empty));
    public string CompanyName
    {
      get
      {
        return GetProperty(CompanyNameProperty);
      }
    }

    protected override void AddAuthorizationRules()
    {
      string[] canRead = new string[] { "AdminUser", "RegularUser", "ReadOnlyUser" };
      AuthorizationRules.AllowGet(typeof(Company), canRead);
      AuthorizationRules.AllowRead(CompanyNameProperty, canRead);
      AuthorizationRules.AllowRead(CompanyIdProperty, canRead);
    }

#if !SILVERLIGHT

    public static ReadOnlyCompany GetReadOnlyCompany(SafeDataReader reader)
    {
      return DataPortal.FetchChild<ReadOnlyCompany>(reader);
    }

    private void Child_Fetch(SafeDataReader reader)
    {
      LoadProperty<int>(CompanyIdProperty, reader.GetInt32("CompanyId"));
      LoadProperty<string>(CompanyNameProperty, reader.GetString("CompanyName"));
    }


#endif
  }
}
