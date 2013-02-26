using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rolodex.Data;
using Csla;

namespace Rolodex
{
    public partial class CompanyInfo
    {
        private CompanyInfo() { }

        #region Factory

        internal static CompanyInfo GetCompanyInfo(Rolodex.Data.CompanyInfo company)
        {
            return DataPortal.FetchChild<CompanyInfo>(company);
        }

        private void Child_Fetch(Rolodex.Data.CompanyInfo company)
        {
            LoadProperty(CompanyIDProperty, company.CompanyID);
            LoadProperty(CompanyNameProperty, company.CompanyName);
        }

        #endregion
    }
}
