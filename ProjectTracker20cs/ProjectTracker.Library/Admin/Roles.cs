using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;

namespace ProjectTracker.Library.Admin
{

  /// <summary>
  /// Used to maintain the list of roles
  /// in the system.
  /// </summary>
  [Serializable()]
  public class Roles : BusinessListBase<Roles, Role>
  {

    #region Constructors

    private Roles()
    {

    }

    #endregion

    #region Criteria

    [Serializable()]
    private class Criteria
    {
      // no criteria
    }

    #endregion

    #region Factory Methods

    public static Roles GetRoles()
    {
      return DataPortal.Fetch<Roles>(new Criteria());
    }

    #endregion

    #region Data Access

    public override Roles Save()
    {
      Roles result;
      result = base.Save();
      // this runs on the client and invalidates
      // the RoleList cache
      RoleList.InvalidateCache();
      return result;
    }

    protected override void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      if (ApplicationContext.ExecutionLocation == ApplicationContext.ExecutionLocations.Server)
      {
        // this runs on the server and invalidates
        // the RoleList cache
        RoleList.InvalidateCache();
      }
    }

    protected override void DataPortal_Fetch(object criteria)
    {
      Criteria crit = (Criteria)criteria;
      using (SqlConnection cn = new SqlConnection(DataBase.DbConn))
      {
        cn.Open();
        using (SqlCommand cm = cn.CreateCommand())
        {
          cm.CommandType = CommandType.StoredProcedure;
          cm.CommandText = "getRoles";

          using (SafeDataReader dr = new SafeDataReader(cm.ExecuteReader()))
          {
            while (dr.Read())
            {
              this.Add(Role.GetRole(dr));
            }
            dr.Close();
          }
        }
        cn.Close();
      }
    }

    [Transactional(TransactionalTypes.Manual)]
    protected override void DataPortal_Update()
    {
      using (SqlConnection cn = new SqlConnection(DataBase.DbConn))
      {
        cn.Open();
        foreach (Role item in DeletedList)
        {
          item.DeleteSelf(cn);
        }
        DeletedList.Clear();

        foreach (Role item in this)
        {
          if (item.IsNew)
            item.Insert(cn);
          else
            item.Update(cn);
        }
        cn.Close();
      }
    }

    #endregion

    #region Atomic Operations

    public static void InsertRole(Role role)
    {
      role.WebSave(false);
    }

    public static void UpdateRole(Role role)
    {
      role.WebSave(true);
    }

    public static void DeleteRole(int id)
    {
      DataPortal.Execute(new RoleDeleter(id));
    }

    public static void DeleteRole(Role role)
    {
      DataPortal.Execute(new RoleDeleter(role.Id));
    }

    [Serializable()]
    private class RoleDeleter : CommandBase
    {

      private int _id;

      public int Id
      {
        get { return _id; }
      }

      public RoleDeleter(int id)
      {
        _id = id;
      }

      protected override void DataPortal_Execute()
      {
        using (SqlConnection cn = new SqlConnection(DataBase.DbConn))
        {
          cn.Open();
          Role.DeleteRole(cn, _id);
          cn.Close();
        }
      }
    }

    #endregion

  }
}
