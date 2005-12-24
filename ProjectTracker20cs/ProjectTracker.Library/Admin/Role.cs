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
    private byte[] _timestamp = new byte[8];

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

    #region Validation Rules

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(
        Csla.Validation.CommonRules.StringRequired, "Name");
      ValidationRules.AddRule(NoDuplicates, "Id");
    }

    bool NoDuplicates(object target, Csla.Validation.RuleArgs e)
    {
      Roles parent = (Roles)this.Parent;
      foreach (Role item in parent)
        if (item.Id == _id && ReferenceEquals(item, this))
        {
          e.Description = "Role Id must be unique";
          return false;
        }
      return true;
    }

    #endregion

    #region Authorization Rules

    protected override void AddAuthorizationRules()
    {
      AuthorizationRules.AllowWrite(
        "Id", "Administrator");
      AuthorizationRules.AllowWrite(
        "Name", "Administrator");
    }

    #endregion

    #region Factory Methods

    internal static Role NewRole()
    {
      return new Role();
    }

    internal static Role 
      GetRole(Csla.Data.SafeDataReader dr)
    {
      return new Role(dr);
    }

    private Role()
    {
      MarkAsChild();
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
        cm.CommandText = "addRole";
        DoInsertUpdate(cm);
      }
    }

    internal void Update(SqlConnection cn)
    {
      // if we're not dirty then don't update the database.
      if (!this.IsDirty) return;

      using (SqlCommand cm = cn.CreateCommand())
      {
        cm.CommandText = "updateRole";
        cm.Parameters.AddWithValue("@lastChanged", _timestamp);
        DoInsertUpdate(cm);
      }
    }

    void DoInsertUpdate(SqlCommand cm)
    {
      cm.CommandType = CommandType.StoredProcedure;
      cm.Parameters.AddWithValue("@id", _id);
      cm.Parameters.AddWithValue("@name", _name);
      using (SqlDataReader dr = cm.ExecuteReader())
      {
        dr.Read();
        dr.GetBytes(0, 0, _timestamp, 0, 8);
      }
      MarkOld();
    }

    internal void DeleteSelf(SqlConnection cn)
    {
      // if we're not dirty then don't update the database
      if (!this.IsDirty) return;

      // if we're new then don't update the database
      if (this.IsNew) return;

      DeleteRole(cn, _id);
      MarkNew();
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
