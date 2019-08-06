using System;
using System.Net;
using Csla;
using Csla.Serialization;
using Csla.Core;

namespace ClassLibrary
{
  [Serializable()]
  public class SLWindowsIdentity : Csla.Silverlight.Security.WindowsIdentity
  {
    public SLWindowsIdentity() { }

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
