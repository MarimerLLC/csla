using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;
using Csla.Serialization;

namespace Csla.Test.Security
{
  [Serializable]
  public class TestIdentity : Csla.Security.CslaIdentity
  {
    public TestIdentity() { }

    public bool IsInRole(string role)
    {
      if (role == "Admin")
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    
    public TestIdentity(string username, string password)
    {
      this.IsAuthenticated = true;
      this.Name = username;
    }
  }
}
