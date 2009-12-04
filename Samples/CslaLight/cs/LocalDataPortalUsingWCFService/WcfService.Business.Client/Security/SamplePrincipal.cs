//INSTANT C# NOTE: Formerly VB.NET project-level imports:
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;
using Csla.DataPortalClient;
using System.ComponentModel;
using System.Security.Principal;

#if !SILVERLIGHT
using System.Data.SqlClient;
#endif

namespace WcfService.Business.Client
{
  [Serializable()]
  public class SamplePrincipal : BusinessPrincipalBase
  {

    private SamplePrincipal(IIdentity identity)
      : base(identity)
    {
    }


#if SILVERLIGHT
	  public SamplePrincipal()
	  {
	  }
#else
    private SamplePrincipal()
    {
    }
#endif


#if SILVERLIGHT



	  public static event EventHandler<EventArgs> SignalLogin;

	  public static void Login(string userName, string password, EventHandler<EventArgs> completed)
	  {
		SamplePrincipal.SignalLogin += completed;
		SampleIdentity.GetIdentity(userName, password, HandleLogin);
	  }

	  private static void HandleLogin(object sender, DataPortalResult<SampleIdentity> e)
	  {
		if (e.Object != null && e.Error == null)
		{
		  SetPrincipal(e.Object);
		}
		else
		{
		  SetPrincipal(SampleIdentity.UnauthenticatedIdentity());
		}
		if (SignalLogin != null)
			SignalLogin(e.Object, EventArgs.Empty);
	  }

#endif

    private static void SetPrincipal(Csla.Security.CslaIdentity identity)
    {
      SamplePrincipal principal = new SamplePrincipal(identity);
      Csla.ApplicationContext.User = principal;
    }

    public static void Logout()
    {
      Csla.Security.CslaIdentity identity = SampleIdentity.UnauthenticatedIdentity();
      SamplePrincipal principal = new SamplePrincipal(identity);
      Csla.ApplicationContext.User = principal;
    }

    public override bool IsInRole(string role)
    {
      return ((ICheckRoles)base.Identity).IsInRole(role);
    }

  }

} //end of root namespace