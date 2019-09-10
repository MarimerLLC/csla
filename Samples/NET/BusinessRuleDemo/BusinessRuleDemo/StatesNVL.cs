using System;
using System.Linq;
using Csla;

namespace BusinessRuleDemo
{
  [Serializable]
  public class StatesNVL : NameValueListBase<string, string>
  {
    #region Factory Methods

    private static StatesNVL _list;

    public static StatesNVL GetNameValueList()
    {
      if (_list == null)
        _list = DataPortal.Fetch<StatesNVL>();
      return _list;
    }

    public static void BeginGetNameValueList(EventHandler<DataPortalResult<StatesNVL>> callback)
    {
      if (_list != null)
      {
        callback(null, new DataPortalResult<StatesNVL>(_list, null, null));
        return;
      }

      var portal = new DataPortal<StatesNVL>();
      portal.FetchCompleted += (o, e) =>
                                 {
                                   if (e.Error == null)
                                   {
                                     _list = e.Object;
                                   }
                                   callback(o, e);
                                 };
      portal.BeginFetch();
      return;
    }

    public static void InvalidateCache()
    {
      _list = null;
    }

    public StatesNVL()
    { /* require use of factory methods */ }

    #endregion

    #region Data Access

    private void DataPortal_Fetch()
    {
      RaiseListChangedEvents = false;
      IsReadOnly = false;

      var data = System.IO.File.ReadAllLines("AmericanStates.txt");
      foreach (var x in data.Select(s => s.Split(',')))
      {
        this.Add(new NameValuePair(x[0].Trim(), x[1].Trim()));
      }

      IsReadOnly = true;
      RaiseListChangedEvents = true;
    }

    #endregion
  }
}
