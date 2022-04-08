using System.Net.Http;
using Csla;

namespace ClientApp
{
  public class CustomProxy : Csla.Channels.Http.HttpProxy
  {
    public static string ServerUrl { get; set; }

    public CustomProxy(ApplicationContext applicationContext, HttpClient httpClient, Csla.Channels.Http.HttpProxyOptions options)
      : base(applicationContext, httpClient, options)
    {
      base.DataPortalUrl = ServerUrl;
    }
  }
}
