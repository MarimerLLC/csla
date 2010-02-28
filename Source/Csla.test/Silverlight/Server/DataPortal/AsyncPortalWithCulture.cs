using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Csla.DataPortalClient;
using Csla.Serialization;

namespace Csla.Testing.Business.DataPortal
{
  [Serializable]
  public class AsyncPortalWithCulture : Csla.CommandBase<AsyncPortalWithCulture>
  {
    public string CurrentUICulture
    {
      get
      {
        return ReadProperty(CurrentUICultureProperty);
      }
      set
      {
        LoadProperty(CurrentUICultureProperty, value);
      }
    }
    public static PropertyInfo<string> CurrentUICultureProperty = 
      RegisterProperty<string>(new PropertyInfo<string>("CurrentUICulture"));

    public string CurrentCulture
    {
      get
      {
        return ReadProperty(CurrentCultureProperty);
      }
      set
      {
        LoadProperty(CurrentCultureProperty, value);
      }
    }
    public static PropertyInfo<string> CurrentCultureProperty =
      RegisterProperty<string>(new PropertyInfo<string>("CurrentCulture"));


#if SILVERLIGHT
    public AsyncPortalWithCulture() { }
#else
    protected AsyncPortalWithCulture() { }
#endif

    public static void BeginExecuteCommand(EventHandler<DataPortalResult<AsyncPortalWithCulture>> handler)
    {
      var command = new AsyncPortalWithCulture();
      var dp = new DataPortal<AsyncPortalWithCulture>();
      dp.ExecuteCompleted += handler;
      dp.BeginExecute(command);
    }

#if !SILVERLIGHT
    protected override void DataPortal_Execute()
    {    
      CurrentCulture = Thread.CurrentThread.CurrentCulture.Name;
      CurrentUICulture = Thread.CurrentThread.CurrentUICulture.Name;
    }

#else
    public void DataPortal_Execute(LocalProxy<AsyncPortalWithCulture>.CompletedHandler handler)
    {
      handler(this, null);
    }
#endif
  }
}
