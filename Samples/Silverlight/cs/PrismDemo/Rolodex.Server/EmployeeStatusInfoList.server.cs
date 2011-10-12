using Csla;
using System.Linq;
using Rolodex.DataAccess;

namespace Rolodex
{
    public partial class EmployeeStatusInfoList
    {
        private EmployeeStatusInfoList() { }

        public static EmployeeStatusInfoList GetEmployeeStatusInfoList()
        {
            return DataPortal.Fetch<EmployeeStatusInfoList>();
        }

        private void DataPortal_Fetch()
        {
            ExceptionManager.Process(() =>
            {
                PreFetch();
                using (var repository = new Repository())
                {
                    repository.GetEmployeeStatuses().ForEach(one => Add(new NameValuePair(one.EmployeeStatusID, one.StatusName)));
                }
                PostFetch();
            });
        }
    }
}
