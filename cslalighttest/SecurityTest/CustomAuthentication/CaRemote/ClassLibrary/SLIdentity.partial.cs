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
        SetCslaIdentity(new MobileList<string>(new string[] { "User", "Admin" }), true, criteria.Username);
      }
      else
      {
        SetCslaIdentity(null, false, "");
      }
    }
  }
}
