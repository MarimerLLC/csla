using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Security
{
    public class AuthorizationRules
    {
        public bool IsReadAllowed(string propertyName) { return true; }
        public bool IsReadDenied(string propertyName) { return true; }
        public bool IsWriteAllowed(string propertyName) { return true; }
        public bool IsWriteDenied(string propertyName) { return true; }
    }
}
