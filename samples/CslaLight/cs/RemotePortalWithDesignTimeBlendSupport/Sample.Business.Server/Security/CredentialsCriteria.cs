//INSTANT C# NOTE: Formerly VB.NET project-level imports:


//INSTANT C# NOTE: Formerly VB.NET project-level imports:
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using System.Text;
using Csla;
using Csla.Core;
using Csla.Serialization;

namespace Sample.Business
{
	[Serializable()]
	public class CredentialsCriteria : CriteriaBase
	{

	  public CredentialsCriteria()
	  {
	  }

	  private string _userName = string.Empty;
	  private string _password = string.Empty;

	  private static PropertyInfo<string> _passwordProperty = RegisterProperty<string>(typeof(CredentialsCriteria), new PropertyInfo<string>("Password", "Password"));

	  public string Password
	  {
		get
		{
		  return ReadProperty(_passwordProperty);
		}
	  }


	  private static PropertyInfo<string> _userNameProperty = RegisterProperty<string>(typeof(CredentialsCriteria), new PropertyInfo<string>("UserName", "User Name"));
	  public string UserName
	  {
		get
		{
		  return ReadProperty(_userNameProperty);
		}
	  }

	  public CredentialsCriteria(string userName, string password)
	  {
		LoadProperty(_userNameProperty, userName);
		LoadProperty(_passwordProperty, password);
	  }

	}

} //end of root namespace