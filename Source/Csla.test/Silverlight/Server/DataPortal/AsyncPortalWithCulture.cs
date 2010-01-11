using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Csla.Testing.Business.DataPortal
{
  public class AsyncPortalWithCulture : Csla.CommandBase 
  {
    public string CurrentCulture { get; set; }
    public string CurrentUICulture { get; set; }

#if SILVERLIGHT
    public AsyncPortalWithCulture(){}
#else
    protected AsyncPortalWithCulture() { }
#endif

    public static void BeginExecuteCommand(EventHandler<DataPortalResult<AsyncPortalWithCulture>> handler)
    {
      var command = new AsyncPortalWithCulture();
      Csla.DataPortal.BeginExecute<AsyncPortalWithCulture>(command, handler);
    }

#if !SILVERLIGHT
    protected override void DataPortal_Execute()
    {
      CurrentCulture = Thread.CurrentThread.CurrentCulture.Name;
      CurrentUICulture = Thread.CurrentThread.CurrentUICulture.Name;
    }
#else
    public void DataPortal_Execute(LocalProxy<SetAppSettingValueCmd>.CompletedHandler handler)
    {
      handler(this, null);
    }
#endif
  }
}
