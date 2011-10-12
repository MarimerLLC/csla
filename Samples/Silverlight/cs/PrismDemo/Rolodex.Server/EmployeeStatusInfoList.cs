using System;
using Csla.Serialization;
using CslaExtensions;
using Csla;

namespace Rolodex
{
    [Serializable]
    public partial class EmployeeStatusInfoList : ExtendedNameValueListBase<int, string>
    {
        public static void GetEmployeeStatusInfoList(EventHandler<DataPortalResult<EmployeeStatusInfoList>> callback)
        {
            DataPortal.BeginFetch<EmployeeStatusInfoList>(callback);
        }
    }
}
