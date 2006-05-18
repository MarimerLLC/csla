using System;
using System.Data;
using System.Data.SqlClient;
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
    private byte[] _timestamp = new byte[8];

    private ProjectResources _resources =
      ProjectResources.NewProjectResources();

    [System.ComponentModel.DataObjectField(true, true)]
    public Guid Id
    {
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get
      {
        CanReadProperty(true);
        return _id;
      }
    }

    public string Name
    {
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get
      {
        CanReadProperty(true);
        return _name;
      }
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
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
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get
      {
        CanReadProperty(true);
        return _started.Text;
      }
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
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
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get
      {
        CanReadProperty(true);
        return _ended.Text;
      }
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
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
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get
      {
        CanReadProperty(true);
          return _description;
      }
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
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
      ValidationRules.AddRule(
        Csla.Validation.CommonRules.StringRequired, "Name");
      ValidationRules.AddRule(
        Csla.Validation.CommonRules.StringMaxLength, 
        new Csla.Validation.CommonRules.MaxLengthRuleArgs("Name", 50));

      ValidationRules.AddRule(
        StartDateGTEndDate, "Started");
      ValidationRules.AddRule(
        StartDateGTEndDate, "Ended");
    }

    private bool StartDateGTEndDate(
      object target, Csla.Validation.RuleArgs e)
    {
      if (_started > _ended)
      {
        e.Description = 
          "Start date can't be after end date";
        return false;
      }
      else
        return true;
    }

    #endregion

    #region Authorization Rules

    protected override void AddAuthorizationRules()
    {
      AuthorizationRules.AllowWrite(
        "Name", "ProjectManager");
      AuthorizationRules.AllowWrite(
        "Started", "ProjectManager");
      AuthorizationRules.AllowWrite(
        "Ended", "ProjectManager");
      AuthorizationRules.AllowWrite(
        "Description", "ProjectManager");
    }

    public static bool CanAddObject()
    {
      return Csla.ApplicationContext.User.IsInRole(
        "ProjectManager");
    }

    public static bool CanGetObject()
    {
      return true;
    }

    public static bool CanDeleteObject()
    {
      bool result = false;
      if (Csla.ApplicationContext.User.IsInRole(
        "ProjectManager"))
        result = true;
      if (Csla.ApplicationContext.User.IsInRole(
        "Administrator"))
        result = true;
      return result;
    }

    public static bool CanEditObject()
    {
      return Csla.ApplicationContext.User.IsInRole("ProjectManager");
    }

    #endregion

    #region Factory Methods

    public static Project NewProject()
    {
      if (!CanAddObject())
        throw new System.Security.SecurityException(
          "User not authorized to add a project");
      return DataPortal.Create<Project>();
    }

    public static Project GetProject(Guid id)
    {
      if (!CanGetObject())
        throw new System.Security.SecurityException(
          "User not authorized to view a project");
      return DataPortal.Fetch<Project>(new Criteria(id));
    }

    public static void DeleteProject(Guid id)
    {
      if (!CanDeleteObject())
        throw new System.Security.SecurityException(
          "User not authorized to remove a project");
      DataPortal.Delete(new Criteria(id));
    }

    private Project()
    { /* require use of factory methods */ }

    public override Project Save()
    {
      if (IsDeleted && !CanDeleteObject())
        throw new System.Security.SecurityException(
          "User not authorized to remove a project");
      else if (IsNew && !CanAddObject())
        throw new System.Security.SecurityException(
          "User not authorized to add a project");
      else if (!CanEditObject())
        throw new System.Security.SecurityException(
          "User not authorized to update a project");

      return base.Save();
    }

    #endregion

    #region Data Access

    [Serializable()]
    private class Criteria
    {
      private Guid _id;
      public Guid Id
      {
        get { return _id; }
      }

      public Criteria(Guid id)
      { _id = id; }
    }

    [RunLocal()]
    private void DataPortal_Create(Criteria criteria)
    {
      _id = Guid.NewGuid();
      _started.Date = DateTime.Today;
      ValidationRules.CheckRules();
    }

    private void DataPortal_Fetch(Criteria criteria)
    {
      using (SqlConnection cn = new SqlConnection(Database.PTrackerConnection))
      {
        cn.Open();
        using (SqlCommand cm = cn.CreateCommand())
        {
          cm.CommandType = CommandType.StoredProcedure;
          cm.CommandText = "getProject";
          cm.Parameters.AddWithValue("@id", criteria.Id);

          using (SafeDataReader dr = new SafeDataReader(cm.ExecuteReader()))
          {
            dr.Read();
            _id = dr.GetGuid("Id");
            _name = dr.GetString("Name");
            _started = dr.GetSmartDate("Started", _started.EmptyIsMin);
            _ended = dr.GetSmartDate("Ended", _ended.EmptyIsMin);
            _description = dr.GetString("Description");
            dr.GetBytes("LastChanged", 0, _timestamp, 0, 8);

            // load child objects
            dr.NextResult();
            _resources = ProjectResources.GetProjectResources(dr);
          }
        }
      }
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Insert()
    {
      using (SqlConnection cn = new SqlConnection(Database.PTrackerConnection))
      {
        cn.Open();
        using (SqlCommand cm = cn.CreateCommand())
        {
          cm.CommandText = "addProject";
          DoInsertUpdate(cm);
        }
      }
      // update child objects
      _resources.Update(this);
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Update()
    {
      if (base.IsDirty)
      {
        using (SqlConnection cn = new SqlConnection(Database.PTrackerConnection))
        {
          cn.Open();
          using (SqlCommand cm = cn.CreateCommand())
          {
            cm.CommandText = "updateProject";
            cm.Parameters.AddWithValue("@lastChanged", _timestamp);
            DoInsertUpdate(cm);
          }
        }
      }
      // update child objects
      _resources.Update(this);
    }

    private void DoInsertUpdate(SqlCommand cm)
    {
      cm.CommandType = CommandType.StoredProcedure;
      cm.Parameters.AddWithValue("@id", _id);
      cm.Parameters.AddWithValue("@name", _name);
      cm.Parameters.AddWithValue("@started", _started.DBValue);
      cm.Parameters.AddWithValue("@ended", _ended.DBValue);
      cm.Parameters.AddWithValue("@description", _description);
      SqlParameter param =
        new SqlParameter("@newLastChanged", SqlDbType.Timestamp);
      param.Direction = ParameterDirection.Output;
      cm.Parameters.Add(param);

      cm.ExecuteNonQuery();

      _timestamp = (byte[])cm.Parameters["@newLastChanged"].Value;
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_DeleteSelf()
    {
      DataPortal_Delete(new Criteria(_id));
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    private void DataPortal_Delete(Criteria criteria)
    {
      using (SqlConnection cn = new SqlConnection(Database.PTrackerConnection))
      {
        cn.Open();
        using (SqlCommand cm = cn.CreateCommand())
        {
          cm.CommandType = CommandType.StoredProcedure;
          cm.CommandText = "deleteProject";
          cm.Parameters.AddWithValue("@id", criteria.Id);
          cm.ExecuteNonQuery();
        }
      }
    }

    #endregion

    #region Exists

    public static bool Exists(Guid id)
    {
      ExistsCommand result;
      result = DataPortal.Execute<ExistsCommand>
        (new ExistsCommand(id));
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
        using (SqlConnection cn = new SqlConnection(Database.PTrackerConnection))
        {
          cn.Open();
          using (SqlCommand cm = cn.CreateCommand())
          {
            cm.CommandType = CommandType.StoredProcedure;
            cm.CommandText = "existsProject";
            cm.Parameters.AddWithValue("@id", _id);
            int count = (int)cm.ExecuteScalar();
            _exists = (count > 0);
          }
        }
      }
    }

    #endregion

  }
}
