using Csla;
using Rolodex.Data;
using Rolodex.DataAccess;

namespace Rolodex
{
    public partial class CompanyEdit
    {
        private CompanyEdit() { }

        #region Factory Methods

        public static CompanyEdit GetCompanyEdit(int companyID)
        {
            return DataPortal.Fetch<CompanyEdit>(new SingleCriteria<CompanyEdit, int>(companyID));
        }

        public static CompanyEdit NewCompanyEdit()
        {
            return DataPortal.Create<CompanyEdit>();
        }


        protected override void DataPortal_Create()
        {
            base.DataPortal_Create();
            LoadProperty(EmployeesProperty, EmlpoyeeEditList.NewEmlpoyeeEditList());
        }
        #endregion

        #region Data Access

        private void DataPortal_Fetch(SingleCriteria<CompanyEdit, int> criteria)
        {
            ExceptionManager.Process(
                () =>
                    {
                        using (var repository = new Repository())
                        {
                            var dto = repository.GetCompany(criteria.Value);

                            LoadProperty(CompanyIDProperty, dto.CompanyID);
                            LoadProperty(CompanyNameProperty, dto.CompanyName);
                            LoadProperty(NotesProperty, dto.Notes);

                            LoadProperty(EmployeesProperty, EmlpoyeeEditList.GetEmlpoyeeEditList(dto));
                        }
                    });
        }

        protected override void DataPortal_Insert()
        {
            ExceptionManager.Process(
                () =>
                    {
                        using (var repository = new Repository())
                        {
                            var dto = ToDTO();
                            repository.Insert(dto);
                            LoadProperty(CompanyIDProperty, dto.CompanyID);
                            FieldManager.UpdateChildren(repository, dto.CompanyID);
                            repository.Save();
                        }
                    });
        }

        protected override void DataPortal_Update()
        {
            ExceptionManager.Process(
                () =>
                    {
                        using (var repository = new Repository())
                        {
                            repository.Update(ToDTO(), false);
                            FieldManager.UpdateChildren(repository, CompanyID);
                            repository.Save();
                        }
                    });
        }

        protected override void DataPortal_DeleteSelf()
        {
            ExceptionManager.Process(
                () =>
                    {
                        using (var repository = new Repository())
                        {
                            repository.Delete(ToDTO(), false);

                            FieldManager.UpdateChildren(repository, CompanyID);
                            repository.Save();
                        }
                    });
        }

        private Company ToDTO()
        {
            using (BypassPropertyChecks)
            {
                return new Company()
                {
                    CompanyID = CompanyID,
                    CompanyName = CompanyName,
                    Notes = Notes
                };
            }
        }
        #endregion
    }
}
