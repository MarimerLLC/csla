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

#if !SILVERLIGHT
using System.Data.SqlClient;
#endif


namespace WcfService.Business.Client
{
	[Serializable()]
	public class SampleIdentity : CslaIdentity
  {

#if SILVERLIGHT

    public static void GetIdentity(string username, string password, EventHandler<DataPortalResult<SampleIdentity>> completed)
	  {
		GetCslaIdentity<SampleIdentity>(completed, new CredentialsCriteria(username, password));
	  }

	  private void EndQuery(object sender, CompanyServiceReference.GetUserCompletedEventArgs e)
	  {
		try
		{
		  if (e.Error == null)
		  {
			CompanyServiceReference.UserInfo user = e.Result;

			if (user.IsAuthenticated)
			{
			  this.Roles = new MobileList<string>(new string[] {user.Role});
			  this.AuthenticationType = "WCF";
			  this.Name = user.UserName;
			  this.IsAuthenticated = true;
			}

			if (IsAuthenticated)
			{
			  _handler(this, null);
			}
			else
			{
			  _handler(null, new ArgumentException("Invalid user/passaword"));
			}
		  }
		  else
		  {
			_handler(null, e.Error);
		  }

		}
		catch (Exception ex)
		{
		  _handler(null, ex);
		}
		finally
		{
		  var client = Csla.Data.ServiceClientManager<CompanyServiceReference.CompanyServiceClient, CompanyServiceReference.ICompanyService>.GetManager(Constants.ClientName).Client;
		  client.GetUserCompleted -= EndQuery;
		}

	  }

	  private LocalProxy<SampleIdentity>.CompletedHandler _handler;

	  [EditorBrowsable(EditorBrowsableState.Never)]
	  public void DataPortal_Fetch(CredentialsCriteria criteria, LocalProxy<SampleIdentity>.CompletedHandler handler)
	  {
		try
		{
		  _handler = handler;
		  var client = Csla.Data.ServiceClientManager<CompanyServiceReference.CompanyServiceClient, CompanyServiceReference.ICompanyService>.GetManager(Constants.ClientName).Client;
		  client.GetUserCompleted += EndQuery;
		  client.GetUserAsync(criteria.UserName, criteria.Password);


		}
		catch (Exception ex)
		{
		  _handler = null;
		  handler(null, ex);
		}
	  }

	#endif

	}

} //end of root namespace