using System;
using System.Net;
using System.Windows;
#if !__ANDROID__
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
#endif

namespace cslalighttest.Properties
{
  public static class Resources
  {
    public static string RemotePortalUrl
    {
      get { return "http://localhost:4832/WcfPortal.svc"; }
    }
  }
}
