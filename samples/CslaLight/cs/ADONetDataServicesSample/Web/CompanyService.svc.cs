//INSTANT C# NOTE: Formerly VB.NET project-level imports:
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;
using System.Web.Security;
using System.Web.Profile;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Data.Services;
using System.ServiceModel.Web;

namespace Web
{
	public class CompanyService : DataService<CompanyEntities>
	{
	  // TODO: replace [[class name]] with your data class name

	  // This method is called only once to initialize service-wide policies.
	  public static void InitializeService(IDataServiceConfiguration config)
	  {
		config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
		config.SetEntitySetAccessRule("*", EntitySetRights.All);
	  }

	}

} //end of root namespace