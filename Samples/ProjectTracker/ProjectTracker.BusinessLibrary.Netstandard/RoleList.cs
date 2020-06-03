using Csla;
using ProjectTracker.Dal;
using System;
using System.Threading.Tasks;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class RoleList : NameValueListBase<int, string>
  {
    private static RoleList CachedList;

    /// <summary>
    /// Clears the in-memory RoleList cache
    /// so the list of roles is reloaded on
    /// next request.
    /// </summary>
    public static void InvalidateCache()
    {
      CachedList = null;
    }

    /// <summary>
    /// Used by async loaders to load the cache.
    /// </summary>
    internal static void SetCache(RoleList list)
    {
      CachedList = list;
    }

    internal static bool IsCached
    {
      get { return CachedList != null; }
    }

    public static RoleList GetCachedList()
    {
      if (IsCached)
        return CachedList;
      else
        throw new InvalidOperationException("RoleList must be cached before use");
    }

    public static int DefaultRole()
    {
      var list = GetCachedList();
      if (list.Count > 0)
        return list.Items[0].Key;
      else
        throw new InvalidOperationException("No roles available; default role can not be returned");
    }

    public static async Task<RoleList> CacheListAsync()
    {
      if (!IsCached)
        SetCache(await DataPortal.FetchAsync<RoleList>());
      return CachedList;
    }

    public static RoleList CacheList()
    {
      if (!IsCached)
        SetCache(DataPortal.Fetch<RoleList>());
      return CachedList;
    }

    [Fetch]
    private void Fetch([Inject] IRoleDal dal)
    {
      using (LoadListMode)
      {
        foreach (var item in dal.Fetch())
          Add(new NameValuePair(item.Id, item.Name));
      }
    }
  }
}