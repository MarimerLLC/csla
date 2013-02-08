//-----------------------------------------------------------------------
// <copyright file="ContextBOVerifier.client.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using Csla.Serialization;
using System;

namespace Csla.Testing.Business.ApplicationContext
{
  [Serializable]
  public class ClientContextBOVerifier : ContextBOVerifier<ClientContextBOVerifier>
  {
    public ClientContextBOVerifier() : this(true) { }
    public ClientContextBOVerifier(bool isNew) : base(isNew) { }

#if SILVERLIGHT
    //DataPortal_Insert is used to verify that the ClientContext["MSG"] is received on the server
    public override void DataPortal_Insert(DataPortalClient.LocalProxy<ClientContextBOVerifier>.CompletedHandler handler)
    {
      SetReceivedContextValuePropertyFrom(Contexts.Client);

      handler(this, null);
    }



    //DataPortal_Update is used to verify that the ClientContext["MSG"] 
    //changed on the server does not change the value on the client
    public override void DataPortal_Update(DataPortalClient.LocalProxy<ClientContextBOVerifier>.CompletedHandler handler)
    {
      SetContextValueModified(Contexts.Client);

      handler(this, null);
    }
#endif
  }

  [Serializable]
  public class GlobalContextBOVerifier : ContextBOVerifier<GlobalContextBOVerifier>
  {
    public GlobalContextBOVerifier() : this(true) { }
    public GlobalContextBOVerifier(bool isNew) : base(isNew) { }

#if SILVERLIGHT
    //DataPortal_Insert is used to verify that the GlobalContext["MSG"] is received on the server
    public override void DataPortal_Insert(DataPortalClient.LocalProxy<GlobalContextBOVerifier>.CompletedHandler handler)
    {
      SetReceivedContextValuePropertyFrom(Contexts.Global);

      handler(this, null);
    }

    //DataPortal_Update is used to verify that the GlobalContext["MSG"] 
    //changed on the server does not change the value on the client
    public override void DataPortal_Update(DataPortalClient.LocalProxy<GlobalContextBOVerifier>.CompletedHandler handler)
    {
      SetContextValueModified(Contexts.Global);

      handler(this, null);
    }
#endif
  }

}