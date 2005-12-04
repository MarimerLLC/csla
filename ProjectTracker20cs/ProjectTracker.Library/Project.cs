using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using Csla;
using Csla.Data;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class Project : BusinessBase<Project>
  {
    #region Business Methods

    private Guid _id;
    private string _name = string.Empty;
    private SmartDate _started;
    private SmartDate _ended = new SmartDate(false);
    private string _description = string.Empty;

    private ProjectResources _resources =
      ProjectResources.NewProjectResources();

    public Guid Id
    {
      get
      {
        CanReadProperty(true);
        return _id;
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
        if (value == null) value = string.Empty;
        if (_name != value)
        {
          _name = value;
          PropertyHasChanged();
        }
      }
    }

    public string Started
    {
      get
      {
        CanReadProperty(true);
        return _started.Text;
      }
      set
      {
        CanWriteProperty(true);
        if (value == null) value = string.Empty;
        if (_started != value)
        {
          _started.Text = value;
          ValidationRules.CheckRules("Ended");
          PropertyHasChanged();
        }
      }
    }

    public string Ended
    {
      get
      {
        CanReadProperty(true);
        return _ended.Text;
      }
      set
      {
        CanWriteProperty(true);
        if (value == null) value = string.Empty;
        if (_ended != value)
        {
          _ended.Text = value;
          ValidationRules.CheckRules("Started");
          PropertyHasChanged();
        }
      }
    }

    public string Description
    {
      get
      {
        CanReadProperty(true);
          return _description;
      }
      set
      {
        CanWriteProperty(true);
        if (value == null) value = string.Empty;
        if (_description != value)
        {
          _description = value;
          PropertyHasChanged();
        }

      }
    }

    public ProjectResources Resources
    {
      get { return _resources; }
    }

    public override bool IsValid
    {
      get { return base.IsValid && _resources.IsValid; }
    }

    public override bool IsDirty
    {
      get { return base.IsDirty || _resources.IsDirty; }
    }

    protected override object GetIdValue()
    {
      return _id;
    }

    #endregion

    #region Validation Rules

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
        return false;
      }
      else
        return true;
    }

    #endregion

    #region Authorization Rules

    protected override void AddAuthorizationRules()
    {
      // add AuthorizationRules here
      AuthorizationRules.AllowWrite("Name", "ProjectManager");
      AuthorizationRules.AllowWrite("Started", "ProjectManager");
      AuthorizationRules.AllowWrite("Ended", "ProjectManager");
      AuthorizationRules.AllowWrite("Description", "ProjectManager");
    }

    public static bool CanAddObject()
    {
      return System.Threading.Thread.CurrentPrincipal.IsInRole("ProjectManager");
    }

    public static bool CanGetObject()
    {
      return true;
    }

    public static bool CanDeleteObject()
    {
      bool result = false;
      if (System.Threading.Thread.CurrentPrincipal.IsInRole("ProjectManager"))
        result = true;
      if (System.Threading.Thread.CurrentPrincipal.IsInRole("Administrator"))
        result = true;
      return result;
    }

    public static bool CanSaveObject()
    {
      return System.Threading.Thread.CurrentPrincipal.IsInRole("ProjectManager");
    }

    #endregion

    #region Factory Methods

    public static Project NewProject()
    {
      if (!CanAddObject())
        throw new System.Security.SecurityException("User not authorized to add a project");
      return DataPortal.Create<Project>(null);
    }

    public static Project GetProject(Guid id)
    {
      if (!CanGetObject())
        throw new System.Security.SecurityException("User not authorized to view a project");
      return DataPortal.Fetch<Project>(new Criteria(id));
    }

    public static void DeleteProject(Guid id)
    {
      if (!CanDeleteObject())
        throw new System.Security.SecurityException("User not authorized to remove a project");
      DataPortal.Delete(new Criteria(id));
    }

    public override Project Save()
    {
      if (IsDeleted)
      {
        if (!CanDeleteObject())
          throw new System.Security.SecurityException("User not authorized to remove a project");
      }
      else
      {
        // no deletion - we're adding or updating
        if (!CanSaveObject())
          throw new System.Security.SecurityException("User not authorized to update a project");
      }

      return base.Save();
    }

    #endregion

    #region Constructors

    private Project()
    {
      AddAuthorizationRules();
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

    #region Data Access

    protected override void DataPortal_Create(object criteria)
    {
      _id = Guid.NewGuid();
      Started = DateTime.Today.ToShortDateString();
      Name = string.Empty;
      ValidationRules.CheckRules();
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
          cm.CommandText = "getProject";
          cm.Parameters.AddWithValue("@ID", crit.Id);

          using (SafeDataReader dr = new SafeDataReader(cm.ExecuteReader()))
          {
            dr.Read();
            _id = dr.GetGuid(0);
            _name = dr.GetString(1);
            _started = dr.GetSmartDate(2, _started.EmptyIsMin);
            _ended = dr.GetSmartDate(3, _ended.EmptyIsMin);
            _description = dr.GetString(4);

            // load child objects
            dr.NextResult();
            _resources = ProjectResources.GetProjectResources(dr);
            dr.Close();
          }
        }
        cn.Close();
      }
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Insert()
    {
      using (SqlConnection cn = new SqlConnection(DataBase.DbConn))
      {
        cn.Open();
        using (SqlCommand cm = cn.CreateCommand())
        {
          cm.CommandType = CommandType.StoredProcedure;
          cm.CommandText = "addProject";
          LoadParameters(cm);
          cm.ExecuteNonQuery();
        }
        cn.Close();
      }
      // update child objects
      _resources.Update(this);
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Update()
    {
      using (SqlConnection cn = new SqlConnection(DataBase.DbConn))
      {
        cn.Open();
        using (SqlCommand cm = cn.CreateCommand())
        {
          cm.CommandType = CommandType.StoredProcedure;
          cm.CommandText = "updateProject";
          LoadParameters(cm);
          cm.ExecuteNonQuery();
        }
        cn.Close();
      }
      // update child objects
      _resources.Update(this);
    }

    private void LoadParameters(SqlCommand cm)
    {
      cm.Parameters.AddWithValue("@ID", _id.ToString());
      cm.Parameters.AddWithValue("@Name", _name);
      cm.Parameters.AddWithValue("@Started", _started.DBValue);
      cm.Parameters.AddWithValue("@Ended", _ended.DBValue);
      cm.Parameters.AddWithValue("@Description", _description);
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_DeleteSelf()
    {
      DataPortal_Delete(new Criteria(_id));
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Delete(object criteria)
    {
      Criteria crit = (Criteria)criteria;

      using (SqlConnection cn = new SqlConnection(DataBase.DbConn))
      {
        cn.Open();
        using (SqlCommand cm = cn.CreateCommand())
        {
          cm.CommandType = CommandType.StoredProcedure;
          cm.CommandText = "deleteProject";
          cm.Parameters.AddWithValue("@ID", crit.Id);
          cm.ExecuteNonQuery();
        }
        cn.Close();
      }
    }

    #endregion

    #region Exists

    public static bool Exists(Guid id)
    {
      ExistsCommand result;
      result = DataPortal.Execute<ExistsCommand>(new ExistsCommand(id));
      return result.Exists;
    }

    [Serializable()]
    private class ExistsCommand : CommandBase
    {
      private Guid _id;
      private bool _exists;

      public bool Exists
      {
        get { return _exists; }
      }

      public ExistsCommand(Guid id)
      {
        _id = id;
      }

      protected override void DataPortal_Execute()
      {
        using (SqlConnection cn = new SqlConnection(DataBase.DbConn))
        {
          cn.Open();
          using (SqlCommand cm = cn.CreateCommand())
          {
            cm.CommandType = CommandType.Text;
            cm.CommandText = "SELECT id FROM Projects WHERE id=@id";
            cm.Parameters.AddWithValue("@ID", _id);
            using (SqlDataReader dr = cm.ExecuteReader())
            {
              if (dr.Read())
                _exists = true;
              else
                _exists = false;
              dr.Close();
            }
          }
          cn.Close();
        }
      }
    }

    #endregion

  }
}
