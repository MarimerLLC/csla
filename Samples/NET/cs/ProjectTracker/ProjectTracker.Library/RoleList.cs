using Csla;
using Csla.Data;
using System;
using System.Linq;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class RoleList : NameValueListBase<int, string>
  {
    public static int DefaultRole()
    {
      RoleList list = null;
#if SILVERLIGHT
      list = _list; // get list from cache
      if (list == null)
      {
        // cache is empty
        GetList((o, e) => { }); // call factory to initialize cache for next time
        return 0; 
      }
#else
      list = GetList(); // call factory to get list
#endif
      if (list.Count > 0)
        return list.Items[0].Key;
      else
        throw new NullReferenceException("No roles available; default role can not be returned");
    }

    private static RoleList _list;

    /// <summary>
    /// Clears the in-memory RoleList cache
    /// so the list of roles is reloaded on
    /// next request.
    /// </summary>
    public static void InvalidateCache()
    {
      _list = null;
    }

    public static void GetList(EventHandler<DataPortalResult<RoleList>> callback)
    {
      if (_list == null)
        DataPortal.BeginFetch<RoleList>((o, e) =>
          {
            _list = e.Object;
            callback(o, e);
          });
      else
        callback(null, new DataPortalResult<RoleList>(_list, null, null));
    }

#if !SILVERLIGHT
    public static RoleList GetList()
    {
      if (_list == null)
        _list = DataPortal.Fetch<RoleList>();
      return _list;
    }
#endif
  }
}