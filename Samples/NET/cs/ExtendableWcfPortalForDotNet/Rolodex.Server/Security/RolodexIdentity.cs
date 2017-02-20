using System;
using System.Linq;
using Csla;
using Csla.Core;
using Csla.Security;
using Rolodex.Business.Data;

namespace Rolodex.Business.Security
{
  [Serializable()]
  public class RolodexIdentity : CslaIdentity
  {
    public static readonly PropertyInfo<int> UserIdProperty =
      RegisterProperty<int>(typeof(RolodexIdentity), new PropertyInfo<int>("UserId", "User Id", 0));

    public int UserId
    {
      get { return GetProperty<int>(UserIdProperty); }
    }

    public static RolodexIdentity GetIdentity(string username, string password)
    {
      return DataPortal.Fetch<RolodexIdentity>(new CredentialsCriteria(username, password));
    }

    private void DataPortal_Fetch(CredentialsCriteria criteria)
    {
      using (var manager =
        Csla.Data.ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
      {
        var user = (from oneUser in manager.ObjectContext.Users
          where oneUser.UserName == criteria.Username
          select oneUser).FirstOrDefault();
        if (user != null && user.Password == criteria.Password)
        {
          LoadProperty<int>(UserIdProperty, user.UserId);
          Name = user.UserName;
          Roles = new MobileList<string>(new[] {user.Role});
          IsAuthenticated = true;
        }
      }
    }
  }
}