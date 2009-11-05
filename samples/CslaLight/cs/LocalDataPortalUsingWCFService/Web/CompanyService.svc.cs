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

using System.Data.SqlClient;

// NOTE: If you change the class name "CompanyService" here, you must also update the reference to "CompanyService" in Web.config and in the associated .svc file.
namespace Web
{
	public class CompanyService : ICompanyService
	{



	  public void DeleteCompany(int companyId)
	  {
		using (SqlConnection connection = new SqlConnection(DataConnection.ConnectionString))
		{
		  connection.Open();
		  using (SqlCommand command = new SqlCommand("CompaniesDelete", connection))
		  {
			command.CommandType = System.Data.CommandType.StoredProcedure;
			command.Parameters.Add(new SqlParameter("@companyID", companyId));
			command.ExecuteNonQuery();
		  }
		  connection.Close();
		}
	  }

	  public CompanyInfo GetCompany(int companyId)
	  {
		CompanyInfo returnValue = null;
		using (SqlConnection connection = new SqlConnection(DataConnection.ConnectionString))
		{
		  connection.Open();
		  using (SqlCommand command = new SqlCommand("GetCompany", connection))
		  {
			command.CommandType = System.Data.CommandType.StoredProcedure;
			command.Parameters.Add(new SqlParameter("@companyID", companyId));
			using (Csla.Data.SafeDataReader reader = new Csla.Data.SafeDataReader(command.ExecuteReader()))
			{
			  if (reader.Read())
			  {
				returnValue = new CompanyInfo(reader.GetInt32("CompanyID"), reader.GetString("CompanyName"), reader.GetSmartDate("DateAdded").Text);
			  }
			}
		  }
		  connection.Close();
		}
		return returnValue;
	  }

	  public int InsertCompany(CompanyInfo company)
	  {
		int returnValue = 0;
		using (SqlConnection connection = new SqlConnection(DataConnection.ConnectionString))
		{
		  connection.Open();
		  using (SqlCommand command = new SqlCommand("CompaniesInsert", connection))
		  {
			command.CommandType = System.Data.CommandType.StoredProcedure;
			command.Parameters.Add(new SqlParameter("@companyID", company.CompanyId));
			command.Parameters["@companyID"].Direction = System.Data.ParameterDirection.Output;
			command.Parameters.Add(new SqlParameter("@companyName", company.CompanyName));
			command.Parameters.Add(new SqlParameter("@dateAdded", company.DateAddedValue));
			command.ExecuteNonQuery();
			returnValue = System.Convert.ToInt32(command.Parameters["@companyID"].Value);
		  }
		  connection.Close();
		}
		return returnValue;
	  }

	  public void UpdateCompany(CompanyInfo company)
	  {
		using (SqlConnection connection = new SqlConnection(DataConnection.ConnectionString))
		{
		  connection.Open();
		  using (SqlCommand command = new SqlCommand("CompaniesUpdate", connection))
		  {
			command.CommandType = System.Data.CommandType.StoredProcedure;
			command.Parameters.Add(new SqlParameter("@companyID", company.CompanyId));
			command.Parameters.Add(new SqlParameter("@companyName", company.CompanyName));
			command.Parameters.Add(new SqlParameter("@dateAdded", company.DateAddedValue));
			command.ExecuteNonQuery();
		  }
		  connection.Close();
		}
	  }

	  public UserInfo GetUser(string userName, string password)
	  {
		UserInfo returnValue = new UserInfo("", "", false);
		using (SqlConnection connection = new SqlConnection(DataConnection.ConnectionString))
		{
		  connection.Open();
		  using (SqlCommand command = new SqlCommand("GetUser", connection))
		  {
			command.CommandType = System.Data.CommandType.StoredProcedure;
			command.Parameters.Add(new SqlParameter("@userName", userName));
			using (Csla.Data.SafeDataReader reader = new Csla.Data.SafeDataReader(command.ExecuteReader()))
			{
			  if (reader.Read())
			  {
				if (password == reader.GetString("Password"))
				{
				  returnValue = new UserInfo(reader.GetString(1), reader.GetString("Role"), true);
				}
			  }
			}
		  }
		}
		return returnValue;
	  }
	}

} //end of root namespace