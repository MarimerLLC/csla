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

using System.ServiceModel;
using System.Runtime.Serialization;

namespace Web
{
	[ServiceContract()]
	public interface ICompanyService
	{

	  [OperationContract()]
	  UserInfo GetUser(string userName, string password);

	  [OperationContract()]
	  CompanyInfo GetCompany(int companyId);

	  [OperationContract()]
	  void UpdateCompany(CompanyInfo company);

	  [OperationContract()]
	  int InsertCompany(CompanyInfo company);

	  [OperationContract()]
	  void DeleteCompany(int companyId);

	}


	[DataContract()]
	public class UserInfo
	{
	  private string _userName = string.Empty;
	  private string _role = string.Empty;
	  private bool _isAuthenticated = false;

	  private UserInfo()
	  {
	  }

	  public UserInfo(string userName, string role, bool isAuthenticated)
	  {
		_userName = userName;
		_role = role;
		_isAuthenticated = isAuthenticated;
	  }

	  [DataMember()]
	  public bool IsAuthenticated
	  {
		get
		{
		  return _isAuthenticated;
		}
		set
		{
		  _isAuthenticated = value;
		}
	  }


	  [DataMember()]
	  public string UserName
	  {
		get
		{
		  return _userName;
		}
		set
		{
		  _userName = value;
		}
	  }

	  [DataMember()]
	  public string Role
	  {
		get
		{
		  return _role;
		}
		set
		{
		  _role = value;
		}
	  }
	}

	[DataContract()]
	public class CompanyInfo
	{
	  private int _companyId;
	  private string _companyName = string.Empty;
	  private string _dateAdded = string.Empty;


	  private CompanyInfo()
	  {
	  }

	  public CompanyInfo(int companyId, string companyName, string dateAdded)
	  {
		_companyId = companyId;
		_companyName = companyName;
		_dateAdded = dateAdded;
	  }

	  [DataMember()]
	  public int CompanyId
	  {
		get
		{
		  return _companyId;
		}
		set
		{
		  _companyId = value;
		}
	  }

	  [DataMember()]
	  public string CompanyName
	  {
		get
		{
		  return _companyName;
		}
		set
		{
		  _companyName = value;
		}
	  }

	  [DataMember()]
	  public string DateAdded
	  {
		get
		{
		  return _dateAdded;
		}
		set
		{
		  _dateAdded = value;
		}
	  }

	  public object DateAddedValue
	  {
		get
		{
		  if (string.IsNullOrEmpty(_dateAdded))
		  {
			return DBNull.Value;
		  }
		  else
		  {
			return _dateAdded;
		  }
		}
	  }

	}


} //end of root namespace