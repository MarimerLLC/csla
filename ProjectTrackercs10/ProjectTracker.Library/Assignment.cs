using System;
using System.Data.SqlClient;
using CSLA;

namespace ProjectTracker.Library
{
  [Serializable()]
  public abstract class Assignment : BusinessBase
  {
    protected SmartDate _assigned = new SmartDate(DateTime.Now);
    protected int _role = 0;

    #region Business Properties and Methods

    public string Assigned
    {
      get
      {
        return _assigned.Text;
      }
    }

    public string Role
    {
      get
      {
        return Roles[_role.ToString()];
      }
      set
      {
        if(value == null) value = string.Empty;
        if(Role != value)
        {
          _role = Convert.ToInt32(Roles.Key(value));
          MarkDirty();
        }
      }
    }

    #endregion
  
    #region Roles List

    private static RoleList _roles;

//    static Assignment()
//    {
//      _roles = RoleList.GetList();
//    }

    public static RoleList Roles
    {
      get
      {
        if(_roles == null)
          _roles = RoleList.GetList();
        return _roles;
      }
    }

    protected static string DefaultRole
    {
      get
      {
        // return the first role in the list
        return Roles[0];
      }
    }

    #endregion

    #region Constructors

    protected Assignment()
    {
      MarkAsChild();
    }

    #endregion

  }
}
