using System;
using System.Security.Principal;

namespace Csla.Security
{
    [Serializable()]
    public class BusinessPrincipalBase : IPrincipal
    {
        private IIdentity _identity;

        public virtual IIdentity Identity
        {
            get { return _identity; }
        }

        public virtual bool IsInRole(string role)
        {
            return false;
        }

        protected BusinessPrincipalBase(IIdentity identity)
        {
            _identity = identity;
        }
    }
}
