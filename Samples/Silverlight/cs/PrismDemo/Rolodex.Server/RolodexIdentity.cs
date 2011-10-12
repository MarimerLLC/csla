using System;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;

namespace Rolodex
{
    [Serializable()]
    public partial class RolodexIdentity : CslaIdentity
    {
        public static RolodexIdentity CurrentIdentity
        {
            get { return Csla.ApplicationContext.User.Identity as RolodexIdentity; }
        }

        public static PropertyInfo<int> UserIdProperty =
          RegisterProperty<int>(typeof(RolodexIdentity), new PropertyInfo<int>("UserId"));
        public int UserId
        {
            get { return GetProperty(UserIdProperty); }
        }

        new public static RolodexIdentity UnauthenticatedIdentity()
        {
#pragma warning disable 0618
            var returnValue = new RolodexIdentity();
#pragma warning restore 0618
            returnValue.IsAuthenticated=false;
            return returnValue;
        }
    }
}
