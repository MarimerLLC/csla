using System;
using System.Data.SqlClient;
using CSLA;

namespace ProjectTracker.Library
{
  [Serializable()]
  public abstract class Assignment : BusinessBase
  {
    protected SmartDate _Assigned = new SmartDate(DateTime.Now);
    protected int _Role = 0;

#region Business Properties and Methods

    public string Assigned
    {
      get
      {
        return _Assigned.Text;
      }
    }

    public string Role
    {
      get
      {
        return Roles[_Role.ToString()];
      }
      set
      {
        if(Role != value)
        {
          _Role = Convert.ToInt32(Roles.Key(value));
          MarkDirty();
        }
      }
    }

#endregion
  
#region Roles List

    private static RoleList _Roles;

    static Assignment()
    {
      _Roles = RoleList.GetList();
    }

    public static RoleList Roles
    {
      get
      {
        return _Roles;
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
