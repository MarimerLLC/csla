using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla;
using Csla.Serialization;

namespace BasicBinding
{
  [Serializable]
  public class DataItem : BusinessBase<DataItem>
  {
    public readonly static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public static void NewDataItem(EventHandler<DataPortalResult<DataItem>> callback)
    {
      DataPortal.BeginCreate<DataItem>(callback);
    }

    public static void GetDataItem(string name, EventHandler<DataPortalResult<DataItem>> callback)
    {
      DataPortal.BeginFetch<DataItem>(name, callback);
    }

    public void DataPortal_Fetch(string name, Csla.DataPortalClient.LocalProxy<DataItem>.CompletedHandler callback)
    {
      try
      {
        using (BypassPropertyChecks)
        {
          Name = name;
        }
        callback(this, null);
      }
      catch (Exception ex)
      {
        callback(null, ex);
      }
    }
  }
}
