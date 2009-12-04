using System;
using Csla;

namespace $rootnamespace$
{
  [Serializable]
  public class $safeitemname$ : NameValueListBase<int, string>
  {
    #region Factory Methods

    private static $safeitemname$ _list;

    public static $safeitemname$ GetNameValueList()
    {
      if (_list == null)
        _list = DataPortal.Fetch<$safeitemname$>();
      return _list;
    }

    public static void InvalidateCache()
    {
      _list = null;
    }

    private $safeitemname$()
    { /* require use of factory methods */ }

    #endregion

    #region Data Access

    private void DataPortal_Fetch()
    {
      RaiseListChangedEvents = false;
      IsReadOnly = false;
      // TODO: load values
      //object listData = null;
      //foreach (var item in listData)
      //  Add(new NameValueListBase<int, string>.
      //    NameValuePair(item.Key, item.Value));
      IsReadOnly = true;
      RaiseListChangedEvents = true;
    }

    #endregion
  }
}
