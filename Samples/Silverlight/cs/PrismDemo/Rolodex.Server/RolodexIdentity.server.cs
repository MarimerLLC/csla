using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Core;
using Rolodex.DataAccess;

namespace Rolodex
{
    public partial class RolodexIdentity
    {
        private RolodexIdentity() { }

        public static RolodexIdentity GetIdentity(string username, string password)
        {
            return GetCslaIdentity<RolodexIdentity>(new CredentialsCriteria(username, password));
        }

        private void DataPortal_Fetch(CredentialsCriteria criteria)
        {
            ExceptionManager.Process(() =>
            {
                using (var repository = new Repository())
                {
                    var user = repository.GetUser(criteria.UserName);
                    if (user != null && user.UserPassword == criteria.Password)
                    {
                        LoadProperty(UserIdProperty, user.UserID);
                        Name = user.UserLogin;
                        Roles = new MobileList<string>(new string[] { user.UserRole });
                        IsAuthenticated = true;
                    }

                }
            });

        }
    }
}
