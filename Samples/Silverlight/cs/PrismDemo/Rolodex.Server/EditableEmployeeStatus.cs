using System;
using Csla;
using Csla.Rules.CommonRules;
using CslaExtensions;
using Csla.Serialization;


#if !SILVERLIGHT
using Rolodex.Data;
using Rolodex.DataAccess;

#else

#endif

namespace Rolodex
{
    [Serializable()]
    public class EditableEmployeeStatus : ExtendedBusinessBase<EditableEmployeeStatus>
    {
        #region Constructors

#if SILVERLIGHT
		[Obsolete("Internal use only")]
		public EditableEmployeeStatus() { }
#else
        private EditableEmployeeStatus() { }
#endif

        #endregion

        #region Properties

        public static readonly PropertyInfo<int> EmployeeStatusIDProperty = RegisterProperty<int>(c => c.EmployeeStatusID);
        /// <Summary>
        /// Gets or sets the EmployeeStatusID value.
        /// </Summary>
        public int EmployeeStatusID
        {
            get { return GetProperty(EmployeeStatusIDProperty); }
            set { SetProperty(EmployeeStatusIDProperty, value); }
        }

        public static readonly PropertyInfo<string> StatusNameProperty = RegisterProperty<string>(c => c.StatusName, "Status name");
        /// <Summary>
        /// Gets or sets the StatusName value.
        /// </Summary>
        public string StatusName
        {
            get { return GetProperty(StatusNameProperty); }
            set { SetProperty(StatusNameProperty, value); }
        }

        #endregion

        #region Factory Methods


        #endregion

        #region Rules
        public static void AddObjectAuthorizationRules()
        {
            AuthorizationHelper.AddObjectAuthorizationRules(typeof(EditableEmployeeStatus));
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

            BusinessRules.AddRule(new Required(StatusNameProperty));
            BusinessRules.AddRule(new MaxLength(StatusNameProperty, 30));
        }
        #endregion

#if !SILVERLIGHT

        internal static EditableEmployeeStatus GetEditableEmployeeStatus(EmployeeStatus employeeStatus)
        {
            return DataPortal.FetchChild<EditableEmployeeStatus>(employeeStatus);
        }

        private void Child_Fetch(EmployeeStatus employeeStatus)
        {
            LoadProperty(EmployeeStatusIDProperty, employeeStatus.EmployeeStatusID);
            LoadProperty(StatusNameProperty, employeeStatus.StatusName);
        }

        private void Child_Insert(Repository repository)
        {
            var dto = ToDTO();
            repository.Insert(dto);
            LoadProperty(EmployeeStatusIDProperty, dto.EmployeeStatusID);
        }

        private void Child_Update(Repository repository)
        {
            repository.Update(ToDTO(), false);
        }

        private void Child_DeleteSelf(Repository repository)
        {
            repository.Delete(ToDTO(), false);
        }

        private EmployeeStatus ToDTO()
        {
            using (BypassPropertyChecks)
            {
                return new EmployeeStatus()
                           {
                               EmployeeStatusID = EmployeeStatusID,
                               StatusName = StatusName
                           };
            }
        }

#endif
    }
}