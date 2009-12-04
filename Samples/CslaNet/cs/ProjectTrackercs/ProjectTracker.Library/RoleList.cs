using Csla;
using Csla.Data;
using System;
using System.Linq;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class RoleList : NameValueListBase<int, string>
  {
    #region  Business Methods

    public static int DefaultRole()
    {
      RoleList list = GetList();
      if (list.Count > 0)
        return list.Items[0].Key;
      else
        throw new NullReferenceException("No roles available; default role can not be returned");
    }

    #endregion

    #region  Factory Methods

    private static RoleList _list;

    public static RoleList GetList()
    {
      if (_list == null)
        _list = DataPortal.Fetch<RoleList>();
      return _list;
    }

    /// <summary>
    /// Clears the in-memory RoleList cache
    /// so the list of roles is reloaded on
    /// next request.
    /// </summary>
    public static void InvalidateCache()
    {
      _list = null;
    }

    private RoleList()
    { /* require use of factory methods */ }

    #endregion

    #region  Data Access

    private void DataPortal_Fetch()
    {
      this.RaiseListChangedEvents = false;
      using (var ctx = 
        ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.
        GetManager(ProjectTracker.DalLinq.Database.PTracker))
      {
        var data = from role in ctx.DataContext.Roles
                   select new NameValuePair(role.Id, role.Name);
        IsReadOnly = false;
        this.AddRange(data);
        IsReadOnly = true;
      }
      this.RaiseListChangedEvents = true;
    }

    #endregion

  }
}