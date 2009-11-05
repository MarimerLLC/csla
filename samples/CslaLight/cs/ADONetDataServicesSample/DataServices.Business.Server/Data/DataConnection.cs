//INSTANT C# NOTE: Formerly VB.NET project-level imports:
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace DataServices.Business
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