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
using Csla.Validation;
using Csla.Server;

#if !SILVERLIGHT
using System.Data.SqlClient;
#endif

namespace Sample.Business
{
	public class CompanyObjectFactory : ObjectFactory
	{

	  private CompanyObjectFactoryTarget GetCompany(SingleCriteria<CompanyObjectFactoryTarget, int> criteria)
	  {
		CompanyObjectFactoryTarget returnValue = null;
		using (SqlConnection connection = new SqlConnection(DataConnection.ConnectionString))
		{
		  connection.Open();
		  using (SqlCommand command = new SqlCommand("GetCompany", connection))
		  {
			command.CommandType = System.Data.CommandType.StoredProcedure;
			command.Parameters.Add(new SqlParameter("@companyID", criteria.Value));
			using (Csla.Data.SafeDataReader reader = new Csla.Data.SafeDataReader(command.ExecuteReader()))
			{
			  if (reader.Read())
			  {
				returnValue = CompanyObjectFactoryTarget.LoadCompany(reader.GetInt32("CompanyID"), reader.GetString("CompanyName"), reader.GetSmartDate("DateAdded"));
				MarkOld(returnValue);
			  }
			}
		  }
		  connection.Close();
		}
		return returnValue;

	  }

	  private CompanyObjectFactoryTarget CreateCompany()
	  {
		return CompanyObjectFactoryTarget.NewCompany();
	  }

	  private CompanyObjectFactoryTarget SaveCompany(CompanyObjectFactoryTarget target)
	  {
		//insert DB code here
		if (target.IsDeleted)
		{
		  //DB code to delete company
		  using (SqlConnection connection = new SqlConnection(DataConnection.ConnectionString))
		  {
			connection.Open();
			using (SqlCommand command = new SqlCommand("CompaniesDelete", connection))
			{
			  command.CommandType = System.Data.CommandType.StoredProcedure;
			  command.Parameters.Add(new SqlParameter("@companyID", target.CompanyId));
			  command.ExecuteNonQuery();
			}
			connection.Close();
		  }
		}
		else if (target.IsNew)
		{
		  //DB code to insert company
		  using (SqlConnection connection = new SqlConnection(DataConnection.ConnectionString))
		  {
			connection.Open();
			using (SqlCommand command = new SqlCommand("CompaniesInsert", connection))
			{
			  command.CommandType = System.Data.CommandType.StoredProcedure;
			  command.Parameters.Add(new SqlParameter("@companyID", target.CompanyId));
			  command.Parameters["@companyID"].Direction = System.Data.ParameterDirection.Output;
			  command.Parameters.Add(new SqlParameter("@companyName", target.CompanyName));
			  command.Parameters.Add(new SqlParameter("@dateAdded", target.DateAddedValue));
			  command.ExecuteNonQuery();
			  target.SetID(System.Convert.ToInt32(command.Parameters["@companyID"].Value));
			}
			connection.Close();
		  }
		}
		else
		{
		  using (SqlConnection connection = new SqlConnection(DataConnection.ConnectionString))
		  {
			connection.Open();
			using (SqlCommand command = new SqlCommand("CompaniesUpdate", connection))
			{
			  command.CommandType = System.Data.CommandType.StoredProcedure;
			  command.Parameters.Add(new SqlParameter("@companyID", target.CompanyId));
			  command.Parameters.Add(new SqlParameter("@companyName", target.CompanyName));
			  command.Parameters.Add(new SqlParameter("@dateAdded", target.DateAddedValue));
			  command.ExecuteNonQuery();
			}
			connection.Close();
		  }
		}
		MarkOld(target);
		return target;
	  }

	}

} //end of root namespace