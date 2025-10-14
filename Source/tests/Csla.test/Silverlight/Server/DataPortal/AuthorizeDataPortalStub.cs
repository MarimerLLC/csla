//-----------------------------------------------------------------------
// <copyright file="AuthorizeDataPortalStub.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
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

      if(!typeof(Csla.Core.ICommandObject).IsAssignableFrom(clientRequest.ObjectType))
        throw new SecurityException("Authorization Failed");
    }

    #endregion
  }
}