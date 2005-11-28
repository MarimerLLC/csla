using System;
using System.Data;
using System.Data.SqlClient;
using Csla;

namespace ProjectTracker.Library.Admin
{

  [Serializable()]
  public class Role : BusinessBase<Role>
  {

    #region Business Methods

    private int _id;
    private string _name;

    public int Id
    {
      get
      {
        CanReadProperty(true);
        return _id;
      }
      set
      {
        CanWriteProperty(true);
        if (!_id.Equals(value))
        {
          _id = value;
          PropertyHasChanged();
        }
      }
    }

    public string Name
    {
      get
      {
        CanReadProperty(true);
        return _name;
      }
      set
      {
        CanWriteProperty(true);
        if (!_name.Equals(value))
        {
          _name = value;
          PropertyHasChanged();
        }
      }
    }

    protected override object GetIdValue()
    {
      return _id;
    }

    #endregion

    #region Constructors

    /// <summary>
    /// DO NOT USE THIS CONSTRUCTOR. It
    /// exists solely for use by Web
    /// Forms data binding.
    /// </summary>
    /// <remarks></remarks>
    public Role()
    {
      MarkAsChild();
    }

    #endregion

    #region Factory Methods

    internal static Role GetRole(Csla.Data.SafeDataReader dr)
    {
      return new Role(dr);
    }

    #endregion

    #region Data Access

    private Role(Csla.Data.SafeDataReader dr)
    {
      MarkAsChild();
      _id = dr.GetInt32("id");
      _name = dr.GetString("name");
      MarkOld();
    }

    internal void Insert(SqlConnection cn)
    {
      // if we're not dirty then don't update the database
      if (!this.IsDirty) return;

      using (SqlCommand cm = cn.CreateCommand())
      {
        cm.CommandType = CommandType.StoredProcedure;
        cm.CommandText = "addRole";
        cm.Parameters.AddWithValue("@id", _id);
        cm.Parameters.AddWithValue("@name", _name);
        cm.ExecuteNonQuery();
      }
    }

    internal void Update(SqlConnection cn)
    {
      // if we're not dirty then don't update the database.
      if (!this.IsDirty) return;

      using (SqlCommand cm = cn.CreateCommand())
      {
        cm.CommandType = CommandType.StoredProcedure;
        cm.CommandText = "updateRole";
        cm.Parameters.AddWithValue("@id", _id);
        cm.Parameters.AddWithValue("@name", _name);
        cm.ExecuteNonQuery();
      }
    }

    internal void DeleteSelf(SqlConnection cn)
    {
      // if we're not dirty then don't update the database
      if (!this.IsDirty) return;

      // if we're new then don't update the database
      if (this.IsNew) return;

      DeleteRole(cn, _id);
    }

    internal static void DeleteRole(SqlConnection cn, int id)
    {
      using (SqlCommand cm = cn.CreateCommand())
      {
        cm.CommandType = CommandType.StoredProcedure;
        cm.CommandText = "deleteRole";
        cm.Parameters.AddWithValue("@id", id);
        cm.ExecuteNonQuery();
      }
    }

    #endregion

    #region Web Support

    /// <summary>
    /// For use from Web Forms to atomically
    /// insert/update a Role.
    /// </summary>
    /// <param name="forceUpdate">True to update data, False to insert data.</param>
    internal void WebSave(bool forceUpdate)
    {
      if (forceUpdate)
        MarkOld();
      MarkDirty();
      DataPortal.Update(this);
    }

    protected override void DataPortal_Insert()
    {
      using (SqlConnection cn = new SqlConnection(DataBase.DbConn))
      {
        cn.Open();
        Insert(cn);
        cn.Close();
      }
    }

    protected override void DataPortal_Update()
    {
      using (SqlConnection cn = new SqlConnection(DataBase.DbConn))
      {
        cn.Open();
        Update(cn);
        cn.Close();
      }
    }

    #endregion

  }
}
