//INSTANT C# NOTE: Formerly VB.NET project-level imports:
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

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

namespace Sample.Business
{
	public class SampleIdentityFactory
	{

	  public SampleIdentity GetSampleIdentity(CredentialsCriteria criteria)
	  {
		SampleIdentity returnValue = new SampleIdentity();
		using (SqlConnection connection = new SqlConnection(DataConnection.ConnectionString))
		{
		  connection.Open();
		  using (SqlCommand command = new SqlCommand("GetUser", connection))
		  {
			command.CommandType = System.Data.CommandType.StoredProcedure;
			command.Parameters.Add(new SqlParameter("@userName", criteria.UserName));
			using (Csla.Data.SafeDataReader reader = new Csla.Data.SafeDataReader(command.ExecuteReader()))
			{
			  if (reader.Read())
			  {
				if (criteria.Password == reader.GetString("Password"))
				{
				  returnValue.LoadData(reader.GetString(1), new MobileList<string>(new string[] {reader.GetString("Role")}));

				}
			  }
			}
		  }
		}
		return returnValue;
	  }


	}

} //end of root namespace