using Csla;
using Csla.Data;
using System;
using System.Linq;
using Csla.Serialization;
using Csla.Rules;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class RoleList : NameValueListBase<int, string>
  {
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

    /// <summary>
    /// Used by async loaders to load the cache.
    /// </summary>
    internal static void SetCache(RoleList list)
    {
      _list = list;
    }

    internal static bool IsCached
    {
      get { return _list != null; }
    }

    public static void GetList(EventHandler<DataPortalResult<RoleList>> callback)
    {
      if (_list == null)
        DataPortal.BeginFetch<RoleList>((o, e) =>
        {
          SetCache(e.Object);
          callback(o, e);
        });
      else
        callback(null, new DataPortalResult<RoleList>(_list, null, null));
    }

    public static async System.Threading.Tasks.Task<RoleList> GetListAsync()
    {
        var roleList = await DataPortal.FetchAsync<RoleList>();
        SetCache(roleList);
        return roleList;
    }

#if NETFX_CORE
    public static int DefaultRole()
    {
      var list = _list; // get list from cache
      if (list == null)
      {
        // cache is empty
#if __ANDROID__
        GetListAsync();
#else
        GetList((o, e) => { }); // call factory to initialize cache for next time
#endif
        return 0; 
      }
      else
      {
        if (list.Count > 0)
          return list.Items[0].Key;
        else
          throw new NullReferenceException("No roles available; default role can not be returned");
      }
    }

    public static RoleList GetList()
    {
      if (_list != null)
        return _list;
      else
        return new RoleList();
    }
#else
    public static RoleList GetList()
    {
      if (!IsCached)
        SetCache(DataPortal.Fetch<RoleList>());
      return _list;
    }

    public static int DefaultRole()
    {
      var list = GetList(); // call factory to get list
      if (list.Count > 0)
        return list.Items[0].Key;
      else
        throw new NullReferenceException("No roles available; default role can not be returned");
    }

    private void DataPortal_Fetch()
    {
      var rlce = RaiseListChangedEvents;
      RaiseListChangedEvents = false;
      IsReadOnly = false;
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IRoleDal>();
        foreach (var item in dal.Fetch())
          Add(new NameValuePair(item.Id, item.Name));
      }
      IsReadOnly = true;
      RaiseListChangedEvents = rlce;
    }
#endif
  }
}