using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
  public class CustomProxy : Csla.DataPortalClient.HttpProxy
  {
    public static string ServerUrl { get; set; }

    public CustomProxy()
    {
      base.DataPortalUrl = ServerUrl;
    }
  }
}
