using System;
using Csla;
using Csla.Rules;
using Csla.Rules.CommonRules;
using Csla.Serialization;
using System.Linq;
using CslaExtensions;
#if !SILVERLIGHT
using Rolodex.Data;
using Rolodex.DataAccess;

#endif

namespace Rolodex
{
    [Serializable()]
    public class EditableEmployeeStatusList : ExtendedBusinessListBase<EditableEmployeeStatusList, EditableEmployeeStatus>
    {
        #region Constructors

#if SILVERLIGHT
        [Obsolete("Internal use only")]
        public EditableEmployeeStatusList() { }
#else
        private EditableEmployeeStatusList() { }
#endif

        #endregion

        #region Factory Methods

        public static void GetEditableEmployeeStatusList(EventHandler<DataPortalResult<EditableEmployeeStatusList>> callback)
        {
            DataPortal.BeginFetch<EditableEmployeeStatusList>(callback);
        }

        #endregion

        #region Rules
        public static void AddObjectAuthorizationRules()
        {
            AuthorizationHelper.AddObjectAuthorizationRules(typeof(EditableEmployeeStatusList));
        }
        #endregion


#if !SILVERLIGHT

        public static EditableEmployeeStatusList GetEditableEmployeeStatusList()
        {
            return DataPortal.Fetch<EditableEmployeeStatusList>();
        }

        private void DataPortal_Fetch()
        {
            ExceptionManager.Process(
                () =>
                {
                    using (var repository = new Repository())
                    {
                        repository.GetEmployeeStatuses().ForEach(one => Add(EditableEmployeeStatus.GetEditableEmployeeStatus(one)));
                    }
                });
        }
        [Transactional(TransactionalTypes.TransactionScope)]
        protected override void DataPortal_Update()
        {
            ExceptionManager.Process(
                () =>
                {
                    using (var repository = new Repository())
                    {
                        base.Child_Update(repository);
                        repository.Save();
                    }
                });
        }

#endif
    }
}