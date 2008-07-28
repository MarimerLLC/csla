using System;
using System.Net;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Ink;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Animation;
//using System.Windows.Shapes;
using Csla;
using Csla.Serialization;
using Csla.Core;

namespace ClassLibrary
{
  [Serializable()]
  public class SLWindowsIdentity : Csla.Silverlight.Security.WindowsIdentity
  {
#if SILVERLIGHT
    public SLWindowsIdentity() { }
#else
    internal SLWindowsIdentity() { }
#endif

    internal bool IsInRole(string role)
    {
      return base.IsInRole(role);
    }

    internal static void GetSLWindowsIdentity(EventHandler<DataPortalResult<SLWindowsIdentity>> completed)
    {
      DataPortal<SLWindowsIdentity> dp = new DataPortal<SLWindowsIdentity>();
      dp.FetchCompleted += completed;
      dp.BeginFetch();
    }

#if !SILVERLIGHT
    private void DataPortal_Fetch()
    {
      base.PopulateWindowsIdentity();
    }
#endif
  }
}
