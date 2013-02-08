using System;
using Csla;
using Csla.Rules.CommonRules;
using Csla.Serialization;
using CslaExtensions;

namespace Rolodex
{
    [Serializable]
    public partial class CompanyEdit : ExtendedBusinessBase<CompanyEdit>
    {

        #region Properties

        public static readonly PropertyInfo<int> CompanyIDProperty = RegisterProperty<int>(c => c.CompanyID);
        /// <Summary>
        /// Gets or sets the CompanyID value.
        /// </Summary>
        public int CompanyID
        {
            get { return GetProperty(CompanyIDProperty); }
            set { SetProperty(CompanyIDProperty, value); }
        }

        public static readonly PropertyInfo<string> CompanyNameProperty = RegisterProperty<string>(c => c.CompanyName, "Company name");
        /// <Summary>
        /// Gets or sets the CompanyName value.
        /// </Summary>
        public string CompanyName
        {
            get { return GetProperty(CompanyNameProperty); }
            set { SetProperty(CompanyNameProperty, value); }
        }

        public static readonly PropertyInfo<string> NotesProperty = RegisterProperty<string>(c => c.Notes);
        /// <Summary>
        /// Gets or sets the Notes value.
        /// </Summary>
        public string Notes
        {
            get { return GetProperty(NotesProperty); }
            set { SetProperty(NotesProperty, value); }
        }

        public static readonly PropertyInfo<EmlpoyeeEditList> EmployeesProperty = RegisterProperty<EmlpoyeeEditList>(c => c.Employees);
        /// <Summary>
        /// Gets or sets the Employees value.
        /// </Summary>
        public EmlpoyeeEditList Employees
        {
            get { return GetProperty(EmployeesProperty); }
            set { SetProperty(EmployeesProperty, value); }
        }

        #endregion

        #region Rules
        public static void AddObjectAuthorizationRules()
        {
            AuthorizationHelper.AddObjectAuthorizationRules(typeof(CompanyEdit));
        }

        protected override void AddBusinessRules()
        {
            base.AddBusinessRules();
            FieldManager.GetRegisteredProperties().ForEach(oneProperty =>
            {
                BusinessRules.AddRule(
                    new IsInRole(
                        Csla.Rules.AuthorizationActions.ReadProperty,
                        oneProperty,
                        AuthorizationHelper.ReadRoles));
                BusinessRules.AddRule(
                    new IsInRole(
                        Csla.Rules.AuthorizationActions.WriteProperty,
                        oneProperty,
                        AuthorizationHelper.WriteRoles));
            });

            BusinessRules.AddRule(new Required(CompanyNameProperty));
            BusinessRules.AddRule(new MaxLength(CompanyNameProperty, 30));
        }
        #endregion


        #region Factory Methods

        public static void GetCompanyEdit(int companyID, EventHandler<DataPortalResult<CompanyEdit>> callback)
        {
            DataPortal.BeginFetch<CompanyEdit>(new SingleCriteria<CompanyEdit, int>(companyID), callback);
        }


        #endregion
    }
}
