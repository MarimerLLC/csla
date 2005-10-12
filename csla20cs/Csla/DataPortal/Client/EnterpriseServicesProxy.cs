using System;

namespace Csla.DataPortalClient
{
    public abstract class EnterpriseServicesProxy : DataPortalClient.IDataPortalProxy
    {

        protected abstract Server.Hosts.EnterpriseServicesPortal GetServerObject();

        public virtual Server.DataPortalResult Create(Type objectType, object criteria, Server.DataPortalContext context)
        {
            Server.Hosts.EnterpriseServicesPortal svc = GetServerObject();
            try
            {
                return svc.Create(objectType, criteria, context);
            }
            finally
            {
                svc.Dispose();
            }
        }

        public virtual Server.DataPortalResult Fetch(object criteria, Server.DataPortalContext context)
        {
            Server.Hosts.EnterpriseServicesPortal svc = GetServerObject();
            try
            {
                return svc.Fetch(criteria, context);
            }
            finally
            {
                svc.Dispose();
            }
        }

        public virtual Server.DataPortalResult Update(object obj, Server.DataPortalContext context)
        {
            Server.Hosts.EnterpriseServicesPortal svc = GetServerObject();
            try
            {
                return svc.Update(obj, context);
            }
            finally
            {
                svc.Dispose();
            }
        }

        public virtual Server.DataPortalResult Delete(object criteria, Server.DataPortalContext context)
        {
            Server.Hosts.EnterpriseServicesPortal svc = GetServerObject();
            try
            {
                return svc.Delete(criteria, context);
            }
            finally
            {
                svc.Dispose();
            }
        }
        public virtual bool IsServerRemote
        {
            get { return true; }
        }
    }
}
