
using Csla.Serialization;

namespace Csla.Testing.Business.ApplicationContext
{
  [Serializable]
  public class ClientContextBOVerifier : ContextBOVerifier<ClientContextBOVerifier>
  {
    public ClientContextBOVerifier(bool isNew): base(isNew){}

    //DataPortal_Insert is used to verify that the ClientContext["MSG"] is received on the server
    protected void DataPortal_Insert(DataPortalClient.LocalProxy<ClientContextBOVerifier>.CompletedHandler handler)
    {
      SetReceivedContextValuePropertyFrom(Contexts.Client);

      handler(this, null);
    }



    //DataPortal_Update is used to verify that the ClientContext["MSG"] 
    //changed on the server does not change the value on the client
    protected void DataPortal_Update(DataPortalClient.LocalProxy<ClientContextBOVerifier>.CompletedHandler handler)
    {
      SetContextValueModified(Contexts.Client);
      
      handler(this, null);
    }
  }

  [Serializable]
  public class GlobalContextBOVerifier : ContextBOVerifier<GlobalContextBOVerifier>
  {
    public GlobalContextBOVerifier(bool isNew) : base(isNew) { }

    //DataPortal_Insert is used to verify that the GlobalContext["MSG"] is received on the server
    protected void DataPortal_Insert(DataPortalClient.LocalProxy<GlobalContextBOVerifier>.CompletedHandler handler)
    {
      SetReceivedContextValuePropertyFrom(Contexts.Global);

      handler(this, null);
    }

    //DataPortal_Update is used to verify that the GlobalContext["MSG"] 
    //changed on the server does not change the value on the client
    protected void DataPortal_Update(DataPortalClient.LocalProxy<GlobalContextBOVerifier>.CompletedHandler handler)
    {
      SetContextValueModified(Contexts.Global);

      handler(this, null);
    }
  }

}