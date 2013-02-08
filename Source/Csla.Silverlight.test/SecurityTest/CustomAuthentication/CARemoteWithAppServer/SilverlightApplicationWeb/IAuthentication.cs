using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SilverlightApplicationWeb
{
  // NOTE: If you change the interface name "IAuthentication" here, you must also update the reference to "IAuthentication" in Web.config.
  [ServiceContract]
  public interface IAuthentication
  {
    [OperationContract]
    bool Authenticate(string username, string password);
  }
}
