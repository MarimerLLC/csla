//INSTANT C# NOTE: Formerly VB.NET project-level imports:
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;
using Csla.Silverlight;
using Csla.Validation;
using Sample.Business;

#if !SILVERLIGHT
using Csla.Data;
using System.Data.SqlClient;
#endif

namespace Sample.Business
{
  [Serializable()]
  public class ReadOnlyCompanyList : ReadOnlyListBase<ReadOnlyCompanyList, ReadOnlyCompany>
  {
#if SILVERLIGHT
	public ReadOnlyCompanyList()
	{
	}
#else
	private ReadOnlyCompanyList()
	{
	}
#endif

	public static void GetCompanyList(EventHandler<DataPortalResult<ReadOnlyCompanyList>> handler)
	{
	  DataPortal<ReadOnlyCompanyList> dp = new DataPortal<ReadOnlyCompanyList>();
	  dp.FetchCompleted += handler;
	  dp.BeginFetch();
	}

	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public void DesignTime_Create()
	{
    IsReadOnly = false;
    ReadOnlyCompany company = null;
    company = ReadOnlyCompany.DesignTime_Create("Sample Company 1");
    Add(company);
    company = ReadOnlyCompany.DesignTime_Create("Sample Company 2");
    Add(company);
    IsReadOnly = true;
	}

#if !SILVERLIGHT

	private new void DataPortal_Fetch()
	{
	  RaiseListChangedEvents = false;
	  IsReadOnly = false;
	  using (SqlConnection connection = new SqlConnection(DataConnection.ConnectionString))
	  {
		connection.Open();
		using (SqlCommand command = new SqlCommand("GetCompanies", connection))
		{
		  command.CommandType = System.Data.CommandType.StoredProcedure;
		  using (Csla.Data.SafeDataReader reader = new Csla.Data.SafeDataReader(command.ExecuteReader()))
		  {
			while (reader.Read())
			{
			  Add(ReadOnlyCompany.GetReadOnlyCompany(reader));
			}
		  }
		}
		connection.Close();
	  }
	  IsReadOnly = true;
	  RaiseListChangedEvents = true;
	}

#endif
  }
}
