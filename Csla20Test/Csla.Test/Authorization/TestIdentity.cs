using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;

namespace Csla.Test.Security
{
    public class TestIdentity : IIdentity 
    {
        private bool _isAuthenticated;
        private string _authType = "Test authentication";
        private string _name = "";

        public bool IsInRole(string role)
        {
            if (role == "Admin")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string AuthenticationType
        {
            get { return _authType; }
        }

        public bool IsAuthenticated
        {
            get { return _isAuthenticated; }
        }

        public string Name
        {
            get { return _name; }
        }

        public TestIdentity(string username, string password)
        {
            _isAuthenticated = true;
            _name = username;
        }
    }
}
