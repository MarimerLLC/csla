using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;
using Csla.Validation;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ProjectResource : BusinessBase<ProjectResource>
  {

    #region Business Methods

    private int _resourceId;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private SmartDate _assigned = new SmartDate(DateTime.Today);
    private int _role;
    private byte[] _timestamp = new byte[8];

    public int ResourceId
    {
      get
      {
        CanReadProperty(true);
        return _resourceId;
      }
    }

    public string FirstName
    {
      get
      {
        CanReadProperty(true);
        return _firstName;
      }
    }

    public string LastName
    {
      get
      {
        CanReadProperty(true);
        return _lastName;
      }
    }

    public string FullName
    {
      get
      {
        if (CanReadProperty("FirstName") && CanReadProperty("LastName"))
          return string.Format("{0}, {1}", LastName, FirstName);
        else
          throw new System.Security.SecurityException("Property read not allowed");
      }
    }

    public string Assigned
    {
      get
      {
        CanReadProperty(true);
        return _assigned.Text;
      }
    }

    public int Role
    {
      get
      {
        CanReadProperty(true);
        return _role;
      }
      set
      {
        CanWriteProperty(true);
        if (!_role.Equals(value))
        {
          _role = value;
          PropertyHasChanged();
        }
      }
    }

    public Resource GetResource()
    {
      return Resource.GetResource(_resourceId);
    }

    protected override object GetIdValue()
    {
      return _resourceId;
    }

    #endregion

    #region Business Rules

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(new Csla.Validation.RuleHandler(ValidRole), "Role");
    }

    private bool ValidRole(object target, RuleArgs e)
    {
      if (RoleList.GetList().ContainsKey(_role))
        return true;
      else
      {
        e.Description = "Role must be in RoleList";
        return false;
      }
    }

    #endregion

    #region Constructors

    private ProjectResource()
    {
      MarkAsChild();
    }

    #endregion

    #region Factory Methods

    internal static ProjectResource NewProjectResource(Resource resource, int role)
    {
      return new ProjectResource(resource, role);
    }

    internal static ProjectResource NewProjectResource(int resourceId, int role)
    {
      return new ProjectResource(Resource.GetResource(resourceId), role);
    }

    internal static ProjectResource NewProjectResource(int resourceId)
    {
      return new ProjectResource(Resource.GetResource(resourceId), RoleList.DefaultRole());
    }

    internal static ProjectResource GetResource(SafeDataReader dr)
    {
      return new ProjectResource(dr);
    }

    #endregion

    #region Data Access

    private ProjectResource(Resource resource, int role)
    {
      MarkAsChild();
      _resourceId = resource.Id;
      _lastName = resource.LastName;
      _firstName = resource.FirstName;
      _assigned.Date = DateTime.Now;
      _role = role;
    }

    private ProjectResource(SafeDataReader dr)
    {
      MarkAsChild();
      _resourceId = dr.GetInt32("ResourceId");
      _lastName = dr.GetString("LastName");
      _firstName = dr.GetString("FirstName");
      _assigned = dr.GetSmartDate("Assigned");
      _role = dr.GetInt32("Role");
      dr.GetBytes("LastChanged", 0, _timestamp, 0, 8);
      MarkOld();
    }

    internal void Insert(Project project)
    {
      // if we're not dirty then don't update the database
      if (!this.IsDirty) return;

      using (SqlConnection cn = new SqlConnection(DataBase.DbConn))
      {
        cn.Open();
        using (SqlCommand cm = cn.CreateCommand())
        {
          cm.CommandType = CommandType.StoredProcedure;
          cm.CommandText = "addAssignment";
          LoadParameters(cm, project);
          using (SqlDataReader dr = cm.ExecuteReader())
          {
            dr.Read();
            dr.GetBytes(0, 0, _timestamp, 0, 8);
          }
          cm.ExecuteNonQuery();
          MarkOld();
        }
      }
    }

    internal void Update(Project project)
    {
      // if we're not dirty then don't update the database
      if (!this.IsDirty) return;

      using (SqlConnection cn = new SqlConnection(DataBase.DbConn))
      {
        cn.Open();
        using (SqlCommand cm = cn.CreateCommand())
        {
          cm.CommandType = CommandType.StoredProcedure;
          cm.CommandText = "updateAssignment";
          LoadParameters(cm, project);
          cm.Parameters.AddWithValue("@lastChanged", _timestamp);
          using (SqlDataReader dr = cm.ExecuteReader())
          {
            dr.Read();
            dr.GetBytes(0, 0, _timestamp, 0, 8);
          }
          cm.ExecuteNonQuery();
          MarkOld();
        }
      }
    }

    private void LoadParameters(SqlCommand cm, Project project)
    {
      cm.Parameters.AddWithValue("@projectId", project.Id);
      cm.Parameters.AddWithValue("@resourceId", _resourceId);
      cm.Parameters.AddWithValue("@assigned", _assigned.DBValue);
      cm.Parameters.AddWithValue("@role", _role);
    }

    internal void DeleteSelf(Project project)
    {
      // if we're not dirty then don't update the database
      if (!this.IsDirty) return;

      // if we're new then don't update the database
      if (!this.IsNew) return;

      using (SqlConnection cn = new SqlConnection(DataBase.DbConn))
      {
        cn.Open();
        using (SqlCommand cm = cn.CreateCommand())
        {
          cm.CommandType = CommandType.StoredProcedure;
          cm.CommandText = "deleteAssignment";
          cm.Parameters.AddWithValue("@projectId", project.Id);
          cm.Parameters.AddWithValue("@resourceId", _resourceId);
          cm.ExecuteNonQuery();
          MarkNew();
        }
      }
    }

    #endregion

  }
}
