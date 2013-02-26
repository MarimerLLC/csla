using System;
using Csla;
using Csla.Serialization;

namespace Rolodex
{
    [Serializable()]
    public partial class CredentialsCriteria : Csla.CriteriaBase<CredentialsCriteria>
    {


        public static PropertyInfo<string> UserNameProperty = RegisterProperty<string>(c => c.UserName);
        public string UserName
        {
            get { return ReadProperty(UserNameProperty); }
        }

        public static PropertyInfo<string> PasswordProperty = RegisterProperty<string>(c => c.Password);
        public string Password
        {
            get { return ReadProperty(PasswordProperty); }
        }

        public CredentialsCriteria(string username, string password)
        {
            LoadProperty(UserNameProperty, username);
            LoadProperty(PasswordProperty, password);
        }
    }
}
