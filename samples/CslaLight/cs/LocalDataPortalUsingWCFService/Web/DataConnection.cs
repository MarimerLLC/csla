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

namespace Web
{
	public class DataConnection
	{
	  private DataConnection()
	  {
	  }

	  public static string ConnectionString
	  {
		get
		{
		  return System.Configuration.ConfigurationManager.ConnectionStrings["SampleConnectionString"].ConnectionString;
		}
	  }
	}
} //end of root namespace