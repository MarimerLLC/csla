//-----------------------------------------------------------------------
// <copyright file="ContextBOVerifier.server.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Testing.Business.ApplicationContext
{
  [Serializable]
  public class ClientContextBOVerifier : ContextBOVerifier<ClientContextBOVerifier>
  {
    private static int _dummy = 0;
    //protected static PropertyInfo<string> InhReceivedContextValueProperty = 
    //  RegisterProperty<string>(ReceivedContextValueProperty);

    public ClientContextBOVerifier(bool isNew): base(isNew)
    {
      _dummy = _dummy + 0;
    }

    protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
    {
      _dummy = 0;
      base.OnDeserialized(context);
    }

    protected override void DataPortal_Insert()
    {
      SetReceivedContextValuePropertyFrom(Contexts.Client);
    }

    //DataPortal_Update is used to verify that the ClientContext["MSG"] 
    //changed on the server does not change the value on the client
    protected override void DataPortal_Update()
    {
      SetContextValueModified(Contexts.Client);
    }
  }

  [Serializable]
  public class GlobalContextBOVerifier : ContextBOVerifier<GlobalContextBOVerifier>
  {
    public GlobalContextBOVerifier(bool isNew) : base(isNew) { }

    protected override void DataPortal_Insert()
    {
      SetReceivedContextValuePropertyFrom(Contexts.Global);
    }

    //DataPortal_Update is used to verify that the ClientContext["MSG"] 
    //changed on the server does not change the value on the client
    protected override void DataPortal_Update()
    {
      SetContextValueModified(Contexts.Global);
    }
  }
}