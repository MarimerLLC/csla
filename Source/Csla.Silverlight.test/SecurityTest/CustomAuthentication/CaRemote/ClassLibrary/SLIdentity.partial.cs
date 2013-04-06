using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Core;

namespace ClassLibrary
{
  public partial class SLIdentity
  {
    public void DataPortal_Fetch(ClassLibrary.SLIdentity.CredentialsCriteria criteria)
    {
      if (criteria.Username == "TestUser" && criteria.Password == "1234")
      {
        this.Roles = new MobileList<string>(criteria.Roles.Split(';'));
        this.IsAuthenticated = true;
        this.Name = criteria.Username;
        this.AuthenticationType = "Custom";
      }
      else
      {
        this.Roles = new MobileList<string>();
        this.IsAuthenticated = false;
        this.Name = string.Empty;
        this.AuthenticationType = "";
      }
    }
  }
}
