using System;
using Csla;
using Csla.Rules.CommonRules;
using Csla.Serialization;
using CslaExtensions;

namespace Rolodex
{
    [Serializable]
    public partial class CompanyEditUoW : ExtendedCommandBase<CompanyEditUoW>
    {
        public static readonly PropertyInfo<CompanyEdit> CompanyProperty = RegisterProperty<CompanyEdit>(c => c.Company);
        public CompanyEdit Company
        {
            get { return ReadProperty(CompanyProperty); }
        }

        public static readonly PropertyInfo<EmployeeStatusInfoList> StatusesProperty = RegisterProperty<EmployeeStatusInfoList>(c => c.Statuses);
        public EmployeeStatusInfoList Statuses
        {
            get { return ReadProperty(StatusesProperty); }
        }

        public static readonly PropertyInfo<int> CompanyIDProperty = RegisterProperty<int>(c => c.CompanyID);
        public int CompanyID
        {
            get { return ReadProperty(CompanyIDProperty); }
        }
    }
}
