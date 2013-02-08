using System;
using Csla;
using CslaExtensions;
using Csla.Serialization;

namespace Rolodex
{
    [Serializable]
    public partial class CompanyInfoList : ExtendedReadOnlyListBase<CompanyInfoList, CompanyInfo>
    {
        #region Factory methods

        public static void GetCompanyInfoList(string partialName, EventHandler<DataPortalResult<CompanyInfoList>> callback)
        {
            DataPortal.BeginFetch<CompanyInfoList>(new SingleCriteria<CompanyInfoList, string>(partialName), callback);
        }

        #endregion
    }
}
