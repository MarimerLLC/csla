using System;

namespace Csla.DataPortalClient
{
    public interface IDataPortalProxy : Server.IDataPortalServer
    {
        bool IsServerRemote { get;}
    }
}
