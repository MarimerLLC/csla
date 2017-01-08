using System;

namespace Rolodex.Silverlight.Events
{
    public class EditCompanyEventArgs : EventArgs
    {
        public int CompanyID { get; private set; }
        private EditCompanyEventArgs() { }
        public EditCompanyEventArgs(int companyID)
        {
            CompanyID = companyID;
        }
    }
}
