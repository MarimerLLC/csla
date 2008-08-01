using UnitDriven;

namespace Csla.Test.Silverlight.ApplicationContext
{
  //[TestClass]
  public partial class ClientContextTests
  {
  }

  //[Serializable]
  public partial class ClientContextBOVerifier
  {
    protected void DataPortal_Insert(Csla.DataPortalClient.LocalProxy<ClientContextBOVerifier>.CompletedHandler handler)
    {
      LoadProperty(ReceivedContextValueProperty, (string)Csla.ApplicationContext.ClientContext["MSG"]);

      handler(this, null);
    }
  }
}