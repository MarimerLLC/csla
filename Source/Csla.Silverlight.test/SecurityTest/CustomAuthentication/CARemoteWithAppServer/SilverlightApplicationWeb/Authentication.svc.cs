using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SilverlightApplicationWeb
{
  // NOTE: If you change the class name "Authentication" here, you must also update the reference to "Authentication" in Web.config.
  public class Authentication : IAuthentication
  {
    public bool Authenticate(string username, string password)
    {
      if (username == "TestUser" && password == "1234")
        return true;
      else
        return false;
    }
  }
}
