using System;
using Csla;

namespace Templates
{
  [Serializable]
  public class NameValueList : NameValueListBase<int, string>
  {
    #region Factory Methods

    private static NameValueList _list;

    public static NameValueList GetNameValueList()
    {
      if (_list == null)
        _list = DataPortal.Fetch<NameValueList>();
      return _list;
    }

    public static void InvalidateCache()
    {
      _list = null;
    }

    private NameValueList()
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
