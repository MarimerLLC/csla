using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Server
{
    public class DataPortal : IDataPortalServer
    {
        #region IDataPortalServer Members

        public DataPortalResult Create(Type objectType, object criteria, DataPortalContext context)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public DataPortalResult Fetch(object criteria, DataPortalContext context)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public DataPortalResult Update(object obj, DataPortalContext context)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public DataPortalResult Delete(object criteria, DataPortalContext context)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
