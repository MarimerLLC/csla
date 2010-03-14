using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;
using Csla.DataPortalClient;
using System.ComponentModel;

#if!SILVERLIGHT
using System.Data.SqlClient;
using Rolodex.Business.Data;
using RolodexEF;
#endif

namespace Rolodex.Business.Security
{
    [Serializable()]
    public class RolodexIdentity : CslaIdentity
    {

        private static PropertyInfo<int> UserIdProperty =
          RegisterProperty<int>(typeof(RolodexIdentity), new PropertyInfo<int>("UserId", "User Id", 0));
        public int UserId
        {
            get
            {
                return GetProperty<int>(UserIdProperty);
            }
        }

#if SILVERLIGHT

    public RolodexIdentity() {}

    public static void GetIdentity(string username, string password, EventHandler<DataPortalResult<RolodexIdentity>> completed)
    {
      GetCslaIdentity<RolodexIdentity>(completed, new CredentialsCriteria(username, password));
    }

#else
        public static RolodexIdentity GetIdentity(string username, string password)
        {
            return DataPortal.Fetch<RolodexIdentity>(new CredentialsCriteria(username, password));
        }

        private void DataPortal_Fetch(CredentialsCriteria criteria)
        {
            using (Csla.Data.ObjectContextManager<RolodexEntities> manager = Csla.Data.ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
            {
                Users user = (from oneUser in manager.ObjectContext.Users
                              where oneUser.UserName == criteria.Username
                              select oneUser).FirstOrDefault();
                if (user != null && user.Password == criteria.Password)
                {
                    LoadProperty<int>(UserIdProperty, user.UserId);
                    Name = user.UserName;
                    Roles = new MobileList<string>(new string[] { user.Role });
                    IsAuthenticated = true;
                }

            }

        }


#endif
    }
}
