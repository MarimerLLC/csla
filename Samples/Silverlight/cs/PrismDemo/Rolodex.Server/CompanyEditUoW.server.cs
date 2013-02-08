using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace Rolodex
{
    public partial class CompanyEditUoW
    {
        private CompanyEditUoW() { }

        #region Factory Methods

        public static CompanyEditUoW GetCompanyEdit(int companyID)
        {
            var command = new CompanyEditUoW();
            command.LoadProperty(CompanyIDProperty, companyID);
            return DataPortal.Execute<CompanyEditUoW>(command);
        }

        protected override void DataPortal_Execute()
        {
            if (CompanyID > 0)
            {
                LoadProperty(CompanyProperty, CompanyEdit.GetCompanyEdit(CompanyID));
            }
            else
            {
                LoadProperty(CompanyProperty, CompanyEdit.NewCompanyEdit());
            }
            LoadProperty(StatusesProperty, EmployeeStatusInfoList.GetEmployeeStatusInfoList());
        }

        #endregion
    }
}
