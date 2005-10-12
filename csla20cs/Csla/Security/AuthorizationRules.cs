using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Csla.Security
{

    /// <summary>
    /// Maintains a list of allowed and denied user roles
    /// for each property.
    /// </summary>
    /// <remarks></remarks>
    [Serializable()]
    public class AuthorizationRules
    {
        private Dictionary<string, RolesForProperty> _rules;

        private Dictionary<string, RolesForProperty> Rules
        {
            get
            {
                if (_rules == null)
                    _rules = new Dictionary<string, RolesForProperty>();
                return _rules;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public RolesForProperty GetRolesForProperty(string propertyName)
        {
            RolesForProperty currentRoles = null;
            if (!Rules.ContainsKey(propertyName))
            {
                currentRoles = new RolesForProperty();
                Rules.Add(propertyName, currentRoles);
            }
            else
                currentRoles = Rules[propertyName];
            return currentRoles;
        }

        public void AllowRead(string propertyName, params string[] roles)
        {
            RolesForProperty currentRoles = GetRolesForProperty(propertyName);
            foreach (string item in roles)
            {
                currentRoles.ReadAllowed.Add(item);
            }
        }

        public void DenyRead(string propertyName, params string[] roles)
        {
            RolesForProperty currentRoles = GetRolesForProperty(propertyName);
            foreach (string item in roles)
            {
                currentRoles.ReadDenied.Add(item);
            }
        }

        public void AllowWrite(string propertyName, params string[] roles)
        {
            RolesForProperty currentRoles = GetRolesForProperty(propertyName);
            foreach (string item in roles)
            {
                currentRoles.WriteAllowed.Add(item);
            }
        }

        public void DenyWrite(string propertyName, params string[] roles)
        {
            RolesForProperty currentRoles = GetRolesForProperty(propertyName);
            foreach (string item in roles)
            {
                currentRoles.WriteDenied.Add(item);
            }
        }

        public bool IsReadAllowed(string propertyName)
        {
            return GetRolesForProperty(propertyName).IsReadAllowed(Thread.CurrentPrincipal);
        }

        public bool IsReadDenied(string propertyName)
        {
            return GetRolesForProperty(propertyName).IsReadDenied(Thread.CurrentPrincipal);
        }

        public bool IsWriteAllowed(string propertyName)
        {
            return GetRolesForProperty(propertyName).IsWriteAllowed(Thread.CurrentPrincipal);
        }

        public bool IsWriteDenied(string propertyName)
        {
            return GetRolesForProperty(propertyName).IsWriteDenied(Thread.CurrentPrincipal);
        }
    }
}