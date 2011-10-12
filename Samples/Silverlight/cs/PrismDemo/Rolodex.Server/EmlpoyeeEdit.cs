using System;
using Csla;
using Csla.Rules.CommonRules;
using Csla.Serialization;
using CslaExtensions;

namespace Rolodex
{
    [Serializable]
    public partial class EmlpoyeeEdit : ExtendedBusinessBase<EmlpoyeeEdit>
    {
        #region Properties

        
        public static readonly PropertyInfo<int> EmlpoyeeIDProperty = RegisterProperty<int>(c => c.EmlpoyeeID);
        /// <Summary>
        /// Gets or sets the EmlpoyeeID value.
        /// </Summary>
        public int EmlpoyeeID
        {
            get { return GetProperty(EmlpoyeeIDProperty); }
            set { SetProperty(EmlpoyeeIDProperty, value); }
        }

        public static readonly PropertyInfo<int> CompanyIDProperty = RegisterProperty<int>(c => c.CompanyID);
        /// <Summary>
        /// Gets or sets the CompanyID value.
        /// </Summary>
        public int CompanyID
        {
            get { return GetProperty(CompanyIDProperty); }
            set { SetProperty(CompanyIDProperty, value); }
        }

        public static readonly PropertyInfo<int> EmployeeStatusIDProperty = RegisterProperty<int>(c => c.EmployeeStatusID, "Status");
        /// <Summary>
        /// Gets or sets the EmployeeStatusID value.
        /// </Summary>
        public int EmployeeStatusID
        {
            get { return GetProperty(EmployeeStatusIDProperty); }
            set { SetProperty(EmployeeStatusIDProperty, value); }
        }

        public static readonly PropertyInfo<string> FirstNameProperty = RegisterProperty<string>(c => c.FirstName, "First name");
        /// <Summary>
        /// Gets or sets the FirstName value.
        /// </Summary>
        public string FirstName
        {
            get { return GetProperty(FirstNameProperty); }
            set { SetProperty(FirstNameProperty, value); }
        }

        public static readonly PropertyInfo<string> LastNameProperty = RegisterProperty<string>(c => c.LastName, "Last name");
        /// <Summary>
        /// Gets or sets the LastName value.
        /// </Summary>
        public string LastName
        {
            get { return GetProperty(LastNameProperty); }
            set { SetProperty(LastNameProperty, value); }
        }

        #endregion

        #region Rules
        public static void AddObjectAuthorizationRules()
        {
            AuthorizationHelper.AddObjectAuthorizationRules(typeof(EmlpoyeeEdit));
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

            BusinessRules.AddRule(new Required(FirstNameProperty));
            BusinessRules.AddRule(new Required(LastNameProperty));
            BusinessRules.AddRule(new MaxLength(FirstNameProperty, 30));
            BusinessRules.AddRule(new MaxLength(LastNameProperty, 50));
            BusinessRules.AddRule(new IntRequired(EmployeeStatusIDProperty));
        }
        #endregion

        #region Factory Methods

        internal static EmlpoyeeEdit NewEmlpoyeeEdit(int companyID)
        {
            var returnValue = DataPortal.CreateChild<EmlpoyeeEdit>();
            returnValue.CompanyID = companyID;
            return returnValue;
        }
        #endregion
    }
}
