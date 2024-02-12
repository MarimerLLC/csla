using System.Net.Http;
using Csla;
using Csla.Configuration;

namespace ClientApp
{
  public class CustomProxy : Csla.Channels.Http.HttpProxy
  {
    public static string ServerUrl { get; set; }

    public CustomProxy(ApplicationContext applicationContext, HttpClient httpClient, Csla.Channels.Http.HttpProxyOptions options, DataPortalOptions dataPortalOptions)
      : base(applicationContext, httpClient, options, dataPortalOptions)
    {
      base.DataPortalUrl = ServerUrl;
    }
  }
}
