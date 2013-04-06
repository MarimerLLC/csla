using System;
using Csla;
using Csla.Rules.CommonRules;
using Csla.Serialization;
using CslaExtensions;

namespace Rolodex
{
    [Serializable]
    public partial class EmlpoyeeEditList : ExtendedBusinessListBase<EmlpoyeeEditList, EmlpoyeeEdit>
    {
        #region Methods

        public EmlpoyeeEdit NewEmlpoyeeEdit(int companyID)
        {
            var employee = EmlpoyeeEdit.NewEmlpoyeeEdit(companyID);
            Add(employee);
            OnAddedNew(employee);
            return employee;
        }

        internal  static EmlpoyeeEditList NewEmlpoyeeEditList()
        {
            return DataPortal.CreateChild<EmlpoyeeEditList>();
        }

        #endregion
    }
}
