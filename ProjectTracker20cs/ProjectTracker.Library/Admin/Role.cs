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
    private bool _idSet;
    private string _name = String.Empty;
    private byte[] _timestamp = new byte[7];

    public int Id
    {
      get
      {
        CanReadProperty(true);
        if (!_idSet)
        {
          // generate a default id value
          _idSet = true;
          Roles parent = (Roles)this.Parent;
          int max = 0;
          foreach (Role item in parent)
            if (item.Id > max)
              max = item.Id;
          _id = max + 1;
        }
        return _id;
      }
      set
      {
        CanWriteProperty(true);
        if (!_id.Equals(value))
        {
          _idSet = true;
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

    private Role()
    {
      MarkAsChild();
    }

    #endregion

    #region Factory Methods

    internal static Role NewRole()
    {
      return new Role();
    }

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
      _idSet = true;
      _name = dr.GetString("name");
      dr.GetBytes("lastChanged", 0, _timestamp, 0, 8);
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
        using (SqlDataReader dr = cm.ExecuteReader())
        {
          dr.Read();
          dr.GetBytes(0, 0, _timestamp, 0, 8);
        }
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
        cm.Parameters.AddWithValue("@lastChanged", _timestamp);
        using (SqlDataReader dr = cm.ExecuteReader())
        {
          dr.Read();
          dr.GetBytes(0, 0, _timestamp, 0, 8);
        }
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

  }
}
