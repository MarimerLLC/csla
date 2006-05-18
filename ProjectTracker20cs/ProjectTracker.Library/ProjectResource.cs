using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;
using Csla.Validation;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ProjectResource : 
    BusinessBase<ProjectResource>, IHoldRoles
  {
    #region Business Methods

    private int _resourceId;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private SmartDate _assigned;
    private int _role;
    private byte[] _timestamp = new byte[8];

    [System.ComponentModel.DataObjectField(false, true)]
    public int ResourceId
    {
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get
      {
        CanReadProperty(true);
        return _resourceId;
      }
    }

    public string FirstName
    {
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get
      {
        CanReadProperty(true);
        return _firstName;
      }
    }

    public string LastName
    {
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get
      {
        CanReadProperty(true);
        return _lastName;
      }
    }

    public string FullName
    {
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get
      {
        if (CanReadProperty("FirstName") && 
          CanReadProperty("LastName"))
          return string.Format("{0}, {1}", LastName, FirstName);
        else
          throw new System.Security.SecurityException(
            "Property read not allowed");
      }
    }

    public string Assigned
    {
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get
      {
        CanReadProperty(true);
        return _assigned.Text;
      }
    }

    public int Role
    {
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get
      {
        CanReadProperty(true);
        return _role;
      }
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
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

    #region Validation Rules

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(
        new Csla.Validation.RuleHandler(
          Assignment.ValidRole), "Role");
    }

    #endregion

    #region Authorization Rules
    
    protected override void AddAuthorizationRules()
    {
      AuthorizationRules.AllowWrite(
        "Role", "ProjectManager");
    }

    #endregion

    #region Factory Methods

    internal static ProjectResource NewProjectResource(int resourceId)
    {
      return new ProjectResource(
        Resource.GetResource(resourceId), 
        RoleList.DefaultRole());
    }

    internal static ProjectResource GetResource(SafeDataReader dr)
    {
      return new ProjectResource(dr);
    }

    private ProjectResource()
    {
      MarkAsChild();
    }

    private ProjectResource(Resource resource, int role)
    {
      MarkAsChild();
      _resourceId = resource.Id;
      _lastName = resource.LastName;
      _firstName = resource.FirstName;
      _assigned.Date = Assignment.GetDefaultAssignedDate();
      _role = role;
    }

    private ProjectResource(SafeDataReader dr)
    {
      MarkAsChild();
      Fetch(dr);
    }

    #endregion

    #region Data Access

    private void Fetch(SafeDataReader dr)
    {
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

      using (SqlConnection cn = new SqlConnection(Database.PTrackerConnection))
      {
        cn.Open();
        _timestamp = Assignment.AddAssignment(
          cn, project.Id, _resourceId, _assigned, _role);
        MarkOld();
      }
    }

    internal void Update(Project project)
    {
      // if we're not dirty then don't update the database
      if (!this.IsDirty) return;

      using (SqlConnection cn = new SqlConnection(Database.PTrackerConnection))
      {
        cn.Open();
        _timestamp = Assignment.UpdateAssignment(
          cn, project.Id, _resourceId, _assigned, _role, _timestamp);
        MarkOld();
      }
    }

    internal void DeleteSelf(Project project)
    {
      // if we're not dirty then don't update the database
      if (!this.IsDirty) return;

      // if we're new then don't update the database
      if (this.IsNew) return;

      using (SqlConnection cn = new SqlConnection(Database.PTrackerConnection))
      {
        cn.Open();
        Assignment.RemoveAssignment(cn, project.Id, _resourceId);
        MarkNew();
      }
    }

    #endregion

  }
}
