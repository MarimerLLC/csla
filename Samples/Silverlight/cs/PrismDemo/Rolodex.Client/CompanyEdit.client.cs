using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.DataPortalClient;

namespace Rolodex
{
    public partial class CompanyEdit
    {
        [Obsolete("Internal use only")]
        public CompanyEdit() { }


        public static void NewCompanyEdit(EventHandler<DataPortalResult<CompanyEdit>> callback)
        {
            DataPortal.BeginCreate<CompanyEdit>(callback, DataPortal.ProxyModes.LocalOnly);
        }

        [Obsolete("Internal use only")]
        new public void DataPortal_Create(LocalProxy<CompanyEdit>.CompletedHandler callback)
        {
            LoadProperty(EmployeesProperty, EmlpoyeeEditList.NewEmlpoyeeEditList());
        }

    }
}
