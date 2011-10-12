using System;
using Csla;
using Rolodex.Data;
using Rolodex.DataAccess;


namespace Rolodex
{
    public partial class EmlpoyeeEdit
    {
        private EmlpoyeeEdit() { }

        internal static EmlpoyeeEdit GetEmlpoyeeEdit(Emlpoyee employee)
        {
            return DataPortal.FetchChild<EmlpoyeeEdit>(employee);
        }

        private void Child_Fetch(Emlpoyee employee)
        {
            LoadProperty(EmlpoyeeIDProperty, employee.EmlpoyeeID);
            LoadProperty(EmployeeStatusIDProperty, employee.EmployeeStatusID);
            LoadProperty(FirstNameProperty, employee.FirstName);
            LoadProperty(LastNameProperty, employee.LastName);
            LoadProperty(CompanyIDProperty, employee.CompanyID);
        }

        private void Child_Insert(Repository repository, int companyID)
        {
            LoadProperty(CompanyIDProperty, companyID);
            var dto = ToDTO();
            repository.Insert(dto);
            LoadProperty(EmployeeStatusIDProperty, dto.EmployeeStatusID);
        }

        private void Child_Update(Repository repository, int companyID)
        {
            repository.Update(ToDTO(), false);
        }

        private void Child_DeleteSelf(Repository repository, int companyID)
        {

            repository.Delete(ToDTO(), false);

        }

        private Emlpoyee ToDTO()
        {
            using (BypassPropertyChecks)
            {
                return new Emlpoyee()
                {
                    CompanyID = CompanyID,
                    EmlpoyeeID = EmlpoyeeID,
                    EmployeeStatusID = EmployeeStatusID,
                    FirstName = FirstName,
                    LastName = LastName
                };
            }
        }
    }
}
