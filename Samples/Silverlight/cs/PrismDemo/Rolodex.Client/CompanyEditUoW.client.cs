using System;
using Csla;


namespace Rolodex
{
    public partial class CompanyEditUoW
    {
        [Obsolete("Internal use only")]
        public CompanyEditUoW() { }

        #region Factory Methods

        public static void GetCompanyEdit(int companyID, EventHandler<DataPortalResult<CompanyEditUoW>> callback)
        {
#pragma warning disable 0618 
            var command = new CompanyEditUoW();
            command.LoadProperty(CompanyIDProperty, companyID);
#pragma warning restore 0618 

            DataPortal.BeginExecute<CompanyEditUoW>(command, callback);
        }


        #endregion
    }
}
