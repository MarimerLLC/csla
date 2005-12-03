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

    #region Business Methods

    /// <summary>
    /// Remove a role based on the role's
    /// id value.
    /// </summary>
    /// <param name="id">Id value of the role to remove.</param>
    public void Remove(int id)
    {
      foreach (Role item in this)
      {
        if (item.Id == id)
        {
          Remove(item);
          break;
        }
      }
    }

    /// <summary>
    /// Get a role based on its id value.
    /// </summary>
    /// <param name="id">Id valud of the role to return</param>
    public Role GetRoleById(int id)
    {
      foreach (Role item in this)
        if (item.Id == id)
          return item;
      return null;
    }

    protected override object AddNewCore()
    {
      Role item = Role.NewRole();
      Add(item);
      return item;
    }

    #endregion

    #region Constructors

    private Roles()
    {
      this.AllowNew = true;
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

    private void DataPortal_Fetch(Criteria criteria)
    {
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

    [Transactional(TransactionalTypes.TransactionScope)]
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

  }
}
