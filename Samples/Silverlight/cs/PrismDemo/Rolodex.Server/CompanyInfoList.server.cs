using System;
using System.Linq;
using Csla;
using Rolodex.DataAccess;


namespace Rolodex
{
    public partial class CompanyInfoList
    {
        private CompanyInfoList() { }

        #region Factory methods

        public static CompanyInfoList GetCompanyInfoList(string partialName)
        {
            return DataPortal.Fetch<CompanyInfoList>(new SingleCriteria<CompanyInfoList, string>(partialName));
        }

        private void DataPortal_Fetch(SingleCriteria<CompanyInfoList, string> criteria)
        {
            ExceptionManager.Process(() =>
            {
                PreFetch();
                using (var repository = new Repository())
                {
                    repository.GetCompanyInfos(criteria.Value).ForEach(one => Add(CompanyInfo.GetCompanyInfo(one)));
                }
                PostFetch();
            });
        }

        #endregion
    }
}
