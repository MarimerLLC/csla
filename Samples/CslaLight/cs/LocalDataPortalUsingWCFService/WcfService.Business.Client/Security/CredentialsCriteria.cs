//INSTANT C# NOTE: Formerly VB.NET project-level imports:
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Text;
using Csla;
using Csla.Core;
using Csla.Serialization;

namespace WcfService.Business.Client
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