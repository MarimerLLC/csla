using System;
using System.Linq;
using Csla;
using Csla.Serialization;

#if!SILVERLIGHT
using Rolodex.Business.Data;
using RolodexEF;
using Csla.Data;
#endif

namespace Rolodex.Business.BusinessClasses
{
    [Serializable]
    public class DuplicateCompanyCommand : CommandBase<DuplicateCompanyCommand>
    {
#if SILVERLIGHT
        public DuplicateCompanyCommand(){}
#else
        protected DuplicateCompanyCommand() { }
#endif
        private static PropertyInfo<int> CompanyIDProperty = RegisterProperty(typeof(DuplicateCompanyCommand), new PropertyInfo<int>("CompanyID"));
        public int CompanyID
        {
            get { return ReadProperty(CompanyIDProperty); }
        }

        private static PropertyInfo<string> CompanyNameProperty = RegisterProperty(typeof(DuplicateCompanyCommand), new PropertyInfo<string>("CompanyName"));
        public string CompanyName
        {
            get { return ReadProperty(CompanyNameProperty); }
        }

        private static PropertyInfo<bool> IsDuplicateProperty = RegisterProperty(typeof(DuplicateCompanyCommand), new PropertyInfo<bool>("IsDuplicate"));
        public bool IsDuplicate
        {
            get { return ReadProperty(IsDuplicateProperty); }
        }

        public DuplicateCompanyCommand(string companyName, int companyId)
        {
            LoadProperty(CompanyIDProperty, companyId);
            LoadProperty(CompanyNameProperty, companyName);
        }


#if !SILVERLIGHT

        protected override void DataPortal_Execute()
        {

            using (ObjectContextManager<RolodexEntities> manager = ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
            {
                int companyId = ReadProperty(CompanyIDProperty);
                string companyName = ReadProperty(CompanyNameProperty);
                Companies company = (from oneCompany in manager.ObjectContext.Companies
                                     where
                                      oneCompany.CompanyId != companyId &&
                                      oneCompany.CompanyName == companyName
                                     select oneCompany).FirstOrDefault();
                if (company != null)
                    LoadProperty(IsDuplicateProperty, true);
                else
                    LoadProperty(IsDuplicateProperty, false);
            }

        }
#endif
    }
}
