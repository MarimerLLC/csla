using System;
using Csla;
using CslaExtensions;
using Csla.Serialization;

namespace Rolodex
{
    [Serializable]
    public partial class CompanyInfo : ExtendedReadOnlyBase<CompanyInfo>
    {

        #region Properies

        public static readonly PropertyInfo<int> CompanyIDProperty = RegisterProperty<int>(c => c.CompanyID);
        public int CompanyID
        {
            get { return GetProperty(CompanyIDProperty); }
        }

        public static readonly PropertyInfo<string> CompanyNameProperty = RegisterProperty<string>(c => c.CompanyName);
        public string CompanyName
        {
            get { return GetProperty(CompanyNameProperty); }
        }

        #endregion

        #region Factory methods

        #endregion
    }
}
