using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using Csla;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class Project : BusinessBase<Project>
  {
    #region Business Methods

    Guid _id;
    string _name = string.Empty;
    SmartDate _started = new SmartDate(false);
    SmartDate _ended;
    string _description = string.Empty;

    //ProjectResources _resources =
    //  ProjectResources.NewProjectResources();

    protected override object GetIdValue()
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public Guid Id
    {
      get
      {
        if (CanReadProperty())
          return _id;
        else
          throw new System.Security.SecurityException("Can not get property");
      }
    }

    public string Name
    {
      get
      {
        if (CanReadProperty())
          return _name;
        else
          throw new System.Security.SecurityException("Can not get property");
      }
      set
      {
        if (CanWriteProperty())
        {
          if (value == null) value = string.Empty;
          if (_name != value)
          {
            _name = value;
            PropertyHasChanged();
          }
        }
      }
    }

    public string Started
    {
      get
      {
        if (CanReadProperty())
          return _started.Text;
        else
          throw new System.Security.SecurityException("Can not get property");
      }
      set
      {
        if (CanWriteProperty())
        {
          if (value == null) value = string.Empty;
          if (_name != value)
          {
            _started.Text = value;
            PropertyHasChanged();
          }
        }
      }
    }

    public string Ended
    {
      get
      {
        if (CanReadProperty())
          return _ended.Text;
        else
          throw new System.Security.SecurityException("Can not get property");
      }
      set
      {
        if (CanWriteProperty())
        {
          if (value == null) value = string.Empty;
          if (_name != value)
          {
            _ended.Text = value;
            PropertyHasChanged();
          }
        }
      }
    }

    public string Description
    {
      get
      {
        if (CanReadProperty())
          return _description;
        else
          throw new System.Security.SecurityException("Can not get property");
      }
      set
      {
        if (CanWriteProperty())
        {
          if (value == null) value = string.Empty;
          if (_name != value)
          {
            _description = value;
            PropertyHasChanged();
          }
        }
      }
    }

    //public ProjectResources Resources
    //{
    //  get
    //  {
    //    return _resources;
    //  }
    //}

    //public override bool IsValid
    //{
    //  get
    //  {
    //    return base.IsValid && _resources.IsValid;
    //  }
    //}

    //public override bool IsDirty
    //{
    //  get
    //  {
    //    return base.IsDirty || _resources.IsDirty;
    //  }
    //}

    #endregion

    #region Business Rules

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringRequired, "Name");
      ValidationRules.AddRule(Csla.Validation.CommonRules.StringMaxLength, 
        new Csla.Validation.CommonRules.MaxLengthRuleArgs("Name", 50));

      ValidationRules.AddRule(StartDateGTEndDate, "Started");
      ValidationRules.AddRule(StartDateGTEndDate, "Ended");
    }

    bool StartDateGTEndDate(object target, Csla.Validation.RuleArgs e)
    {
      if (_started > _ended)
      {
        e.Description = "Start date can't be after end date";
        return true;
      }
      else
        return false;
    }

    #endregion

    #region Constructors

    /// <summary>
    /// DO NOT USE. Call the static NewProject() method instead.
    /// </summary>
    /// <remarks>
    /// This constructor is public only to support Web Forms
    /// data binding. Do not call it directly. To create a new
    /// object, call the appropriate factory method.
    /// </remarks>
    public Project()
    {
      // add AuthorizationRules here
      AuthorizationRules.AllowWrite("Name", "ProjectManager");
      AuthorizationRules.AllowWrite("Started", "ProjectManager");
      AuthorizationRules.AllowWrite("Ended", "ProjectManager");
      AuthorizationRules.AllowWrite("Description", "ProjectManager");
    }

    #endregion

    #region Criteria

    [Serializable()]
    private class Criteria
    {
      private Guid _id;

      public Guid Id
      {
        get { return _id; }
      }

      public Criteria(Guid id)
      {
        _id = id;
      }
    }

    #endregion

    #region Factory Methods

    /// <summary>
    /// Create a new project object.
    /// </summary>
    public static Project NewProject()
    {
      if (!Thread.CurrentPrincipal.IsInRole("ProjectManager"))
        throw new System.Security.SecurityException(
          "User not authorized to add a project");
      return (Project)DataPortal.Create(new Criteria(Guid.Empty));
    }

    // load existing object by id
    public static Project GetProject(Guid id)
    {
      return (Project)DataPortal.Fetch(new Criteria(id));
    }

    // delete object
    public static void DeleteProject(Guid id)
    {
      if (!Thread.CurrentPrincipal.IsInRole("ProjectManager") &&
        !Thread.CurrentPrincipal.IsInRole("Administrator"))
        throw new System.Security.SecurityException(
          "User not authorized to remove a project");
      DataPortal.Delete(new Criteria(id));
    }

    public override Project Save()
    {
      if (IsDeleted)
      {
        System.Security.Principal.IIdentity user =
          Thread.CurrentPrincipal.Identity;
        bool b = user.IsAuthenticated;
        if (!Thread.CurrentPrincipal.IsInRole("ProjectManager") &&
          !Thread.CurrentPrincipal.IsInRole("Administrator"))
          throw new System.Security.SecurityException(
            "User not authorized to remove a project");
      }
      else
      {
        // no deletion - we're adding or updating
        if (!Thread.CurrentPrincipal.IsInRole("ProjectManager"))
          throw new System.Security.SecurityException(
            "User not authorized to update a project");
      }
      return base.Save();
    }

    #endregion

    #region Data Access

    private string DbConn
    {
      get
      {
        return System.Configuration.ConfigurationManager.ConnectionStrings["PTracker"].ConnectionString;
      }
    }

    protected override void DataPortal_Create(object criteria)
    {
      Started = DateTime.Today.ToShortDateString();
      ValidationRules.CheckRules();
    }

    protected override void DataPortal_Fetch(object criteria)
    {
      Criteria crit = (Criteria)criteria;
      // retrieve data from db
      using (SqlConnection cn = new SqlConnection(DbConn))
      {
        cn.Open();
        using (SqlCommand cm = cn.CreateCommand())
        {
          cm.CommandType = CommandType.StoredProcedure;
          cm.CommandText = "getProject";
          cm.Parameters.AddWithValue("@ID", crit.Id);

          using (Csla.Data.SafeDataReader dr = new Csla.Data.SafeDataReader(cm.ExecuteReader()))
          {
            dr.Read();
            _id = dr.GetGuid("ID");
            _name = dr.GetString("Name");
            SmartDate result;
            result = dr.GetSmartDate("Started", _started.EmptyIsMin);
            _started = result;
            _ended = dr.GetSmartDate("Ended", _ended.EmptyIsMin);
            _description = dr.GetString("Description");

            //// load child objects
            //dr.NextResult();
            //_resources = ProjectResources.GetProjectResources(dr);
          }
        }
      }
    }

    protected override void DataPortal_Insert()
    {
      using (SqlConnection cn = new SqlConnection(DbConn))
      {
        cn.Open();
        using (SqlCommand cm = cn.CreateCommand())
        {
          cm.CommandType = CommandType.StoredProcedure;
          cm.CommandText = "addProject";
          LoadParameters(cm);

          cm.ExecuteNonQuery();
        }
      }
      //// update child objects
      //_resources.Update(this);
    }

    protected override void DataPortal_Update()
    {
      using (SqlConnection cn = new SqlConnection(DbConn))
      {
        cn.Open();
        using (SqlCommand cm = cn.CreateCommand())
        {
          cm.CommandType = CommandType.StoredProcedure;
          cm.CommandText = "updateProject";
          LoadParameters(cm);

          cm.ExecuteNonQuery();
        }
      }
      //// update child objects
      //_resources.Update(this);
    }

    void LoadParameters(SqlCommand cm)
    {
      cm.Parameters.AddWithValue("@ID", _id.ToString());
      cm.Parameters.AddWithValue("@Name", _name);
      cm.Parameters.AddWithValue("@Started", _started.DBValue);
      cm.Parameters.AddWithValue("@Ended", _ended.DBValue);
      cm.Parameters.AddWithValue("@Description", _description);
    }

    protected override void DataPortal_DeleteSelf()
    {
      DataPortal_Delete(new Criteria(_id));
    }

    protected override void DataPortal_Delete(object criteria)
    {
      Criteria crit = (Criteria)criteria;
      using (SqlConnection cn = new SqlConnection(DbConn))
      {
        cn.Open();
        using (SqlCommand cm = cn.CreateCommand())
        {
          cm.CommandType = CommandType.StoredProcedure;
          cm.CommandText = "deleteProject";
          cm.Parameters.AddWithValue("@ID", crit.Id);
          cm.ExecuteNonQuery();
        }
      }
    }

    #endregion

  }
}
