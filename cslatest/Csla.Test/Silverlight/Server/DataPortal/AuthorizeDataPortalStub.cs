using System.Security;
using Csla.Server;

namespace  Csla.Testing.Business.DataPortal
{
  public class AuthorizeDataPortalStub : IAuthorizeDataPortal
  {
    public AuthorizeRequest ClientRequest;

    #region IAuthorizeDataPortal Members

    public void Authorize(AuthorizeRequest clientRequest)
    {
      ClientRequest = clientRequest;
    }

    #endregion
  }

  public class DontAuthorizeDataPortalStub : IAuthorizeDataPortal
  {
    public AuthorizeRequest ClientRequest;

    #region IAuthorizeDataPortal Members

    public void Authorize(AuthorizeRequest clientRequest)
    {
      ClientRequest = clientRequest;

      if(!typeof(Csla.CommandBase).IsAssignableFrom(clientRequest.ObjectType))
        throw new SecurityException("Authorization Failed");
    }

    #endregion
  }
}