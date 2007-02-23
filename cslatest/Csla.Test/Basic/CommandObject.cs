using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.Basic
{
    public class CommandObject : Csla.CommandBase
    {
        public void ExecuteServerCode()
        {
            Csla.DataPortal.Execute(this);
        }

        protected override void DataPortal_Execute()
        {
            Csla.ApplicationContext.GlobalContext.Add("CommandObject", "DataPortal_Execute called");
        }

        public CommandObject()
        {
        }
    }
}
