using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Csla;

namespace Rolodex
{
    public partial class RolodexIdentity
    {
        [Obsolete("Internal use only")]
        public RolodexIdentity() { }

        public static void GetIdentity(string username, string password, EventHandler<DataPortalResult<RolodexIdentity>> completed)
        {
            GetCslaIdentity<RolodexIdentity>(completed, new CredentialsCriteria(username, password));
        }
    }
}
