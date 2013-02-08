using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Core;
using Csla.Serialization;

namespace Rolodex.Business.Security
{
    [Serializable()]
    public class CredentialsCriteria : CriteriaBase<CredentialsCriteria>
    {

        public CredentialsCriteria() { }

        private string _username;
        private string _password;

        public string Username
        {
            get
            {
                return _username;
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
        }

        public CredentialsCriteria(string username, string password)
            : base()
        {
            _username = username;
            _password = password;
        }

        protected override void OnGetState(Csla.Serialization.Mobile.SerializationInfo info, StateMode mode)
        {
            info.AddValue("_username", _username);
            info.AddValue("_password", _password);
            base.OnGetState(info, mode);
        }

        protected override void OnSetState(Csla.Serialization.Mobile.SerializationInfo info, StateMode mode)
        {
            _username = (string)info.Values["_username"].Value;
            _password = (string)info.Values["_password"].Value;
            base.OnSetState(info, mode);
        }
    }
}
