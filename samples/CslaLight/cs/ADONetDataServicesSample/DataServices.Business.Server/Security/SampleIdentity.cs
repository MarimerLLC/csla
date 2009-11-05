//INSTANT C# NOTE: Formerly VB.NET project-level imports:
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;
using Csla.DataPortalClient;
using System.ComponentModel;

#if ! SILVERLIGHT
using System.Data.SqlClient;
#endif


namespace DataServices.Business
{
  [Serializable()]
  public class SampleIdentity : CslaIdentity
  {

#if SILVERLIGHT

	  public static void GetIdentity(string username, string password, EventHandler<DataPortalResult<SampleIdentity>> completed)
	  {
		GetCslaIdentity<SampleIdentity>(completed, new CredentialsCriteria(username, password));
	  }

	  private void EndQuery(IAsyncResult e)
	  {
		try
		{
		  var queryUsers = (System.Data.Services.Client.DataServiceQuery<CompanyServiceReference.Users>)e.AsyncState;
		  var users = queryUsers.EndExecute(e).ToList();

		  var query = 
		  		(from oneUser in users
		  		where oneUser.UserName.ToUpper() == _criteria.UserName.ToUpper() && oneUser.Password == _criteria.Password
		  		select oneUser).FirstOrDefault();
		  if (query != null)
		  {
			this.Roles = new MobileList<string>(new string[] {query.Role});
			this.AuthenticationType = "ADO";
			this.Name = query.UserName;
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
		catch (Exception ex)
		{
		  _handler(null, ex);
		}

	  }

	  private LocalProxy<SampleIdentity>.CompletedHandler _handler;
	  private CredentialsCriteria _criteria;

	  [EditorBrowsable(EditorBrowsableState.Never)]
	  public void DataPortal_Fetch(CredentialsCriteria criteria, LocalProxy<SampleIdentity>.CompletedHandler handler)
	  {
		try
		{
		  _handler = handler;
		  _criteria = criteria;
		  var context = Csla.Data.DataServiceContextManager<CompanyServiceReference.CompanyEntities>.GetManager(Company.GetServiceUri()).DataServiceContext;
		  context.Users.BeginExecute(EndQuery, context.Users);

		}
		catch (Exception ex)
		{
		  _handler = null;
		  handler(null, ex);
		}
	  }


#else



#endif

  }

} //end of root namespace