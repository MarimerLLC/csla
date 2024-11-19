using System;
using Csla;

namespace Company.CslaLibrary1
{
  [Serializable]
  public class NameValueList : NameValueListBase<int, string>
  {
    private static NameValueList _list;

    public static NameValueList GetNameValueList(ApplicationContext applicationContext)
    {
      if (_list == null)
      {
        var dataPortal = applicationContext.GetRequiredService<IDataPortal<NameValueList>>();
        _list = dataPortal.Fetch();
      }
        
      return _list;
    }

    public static void InvalidateCache()
    {
      _list = null;
    }

    [Fetch]
    private void Fetch()
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
  }
}
