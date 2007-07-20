using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;
using Csla.Validation;

namespace ProjectTracker.Library
{

  [Serializable()]
  public class Resource : BusinessBase<Resource>
  {

    #region Business Methods

    private int _id;
    private string _lastName = string.Empty;
    private string _firstName = string.Empty;
    private byte[] _timestamp = new byte[8];

    private ResourceAssignments _assignments; 

    [System.ComponentModel.DataObjectField(true, true)]
    public int Id
    {
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get
      {
        CanReadProperty(true);
        return _id;
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
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      set
      {
        CanWriteProperty(true);
        if (value == null) value = string.Empty;
        if (_lastName != value)
        {
          _lastName = value;
          PropertyHasChanged();
        }
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
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      set
      {
        CanWriteProperty();
        if (value == null) value = string.Empty;
        if (_firstName != value)
        {
          _firstName = value;
          PropertyHasChanged();
        }
      }
    }
    public string FullName
    {
      [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
      get
      {
        if (CanReadProperty("FirstName") && CanReadProperty("LastName"))
          return string.Format("{0}, {1}", _lastName, _firstName);
        else
          throw new System.Security.SecurityException("Property read not allowed");
      }
    }

    public ResourceAssignments Assignments
    {
      get { return _assignments; }
    }

    public override bool IsValid
    {
      get { return base.IsValid && _assignments.IsValid; }
    }

    public override bool IsDirty
    {
      get { return base.IsDirty || _assignments.IsDirty; }
    }

    protected override object GetIdValue()
    {
      return _id;
    }

    #endregion

    #region Validation Rules

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(new RuleHandler(CommonRules.StringRequired), "FirstName");
      ValidationRules.AddRule(new RuleHandler(CommonRules.StringMaxLength), 
        new CommonRules.MaxLengthRuleArgs("FirstName", 50));

      ValidationRules.AddRule(new RuleHandler(CommonRules.StringMaxLength),
        new CommonRules.MaxLengthRuleArgs("LastName", 50));
    }

    #endregion

    #region Authorization Rules

    protected override void AddAuthorizationRules()
    {
      // add AuthorizationRules here
      AuthorizationRules.AllowWrite("LastName", "ProjectManager");
      AuthorizationRules.AllowWrite("FirstName", "ProjectManager");
    }

    public static bool CanAddObject()
    {
      return Csla.ApplicationContext.User.IsInRole("ProjectManager");
    }

    public static bool CanGetObject()
    {
      return true;
    }

    public static bool CanDeleteObject()
    {
      bool result = false;
      if (Csla.ApplicationContext.User.IsInRole("ProjectManager"))
        result = true;
      if (Csla.ApplicationContext.User.IsInRole("Administrator"))
        result = true;
      return result;
    }

    public static bool CanEditObject()
    {
      return Csla.ApplicationContext.User.IsInRole("ProjectManager");
    }

    #endregion

    #region Factory Methods

    public static Resource NewResource()
    {
      if (!CanAddObject())
        throw new System.Security.SecurityException(
          "User not authorized to add a resource");
      return DataPortal.Create<Resource>();
    }

    public static void DeleteResource(int id)
    {
      if (!CanDeleteObject())
        throw new System.Security.SecurityException(
          "User not authorized to remove a resource");
      DataPortal.Delete(new Criteria(id));
    }

    public static Resource GetResource(int id)
    {
      if (!CanGetObject())
        throw new System.Security.SecurityException(
          "User not authorized to view a resource");
      return DataPortal.Fetch<Resource>(new Criteria(id));
    }

    public override Resource Save()
    {
      if (IsDeleted && !CanDeleteObject())
        throw new System.Security.SecurityException(
          "User not authorized to remove a resource");
      else if (IsNew && !CanAddObject())
        throw new System.Security.SecurityException(
          "User not authorized to add a resource");
      else if (!CanEditObject())
        throw new System.Security.SecurityException(
          "User not authorized to update a resource");
      return base.Save();
    }

    private Resource()
    {
      _assignments = ResourceAssignments.NewResourceAssignments();
      _assignments.ListChanged += new System.ComponentModel.ListChangedEventHandler(_assignments_ListChanged);
    }

    void _assignments_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
    {
      OnUnknownPropertyChanged();
    }

    #endregion

    #region Data Access

    [Serializable()]
    private class Criteria
    {
      private int _id;
      public int Id
      {
        get { return _id; }
      }

      public Criteria(int id)
      { _id = id; }
    }

    [RunLocal()]
    protected override void DataPortal_Create()
    {
      // nothing to initialize
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
          cm.CommandText = "getResource";
          cm.Parameters.AddWithValue("@id", criteria.Id);

          using (SafeDataReader dr = 
            new SafeDataReader(cm.ExecuteReader()))
          {
            dr.Read();
            _id = dr.GetInt32("Id");
            _lastName = dr.GetString("LastName");
            _firstName = dr.GetString("FirstName");
            dr.GetBytes("LastChanged", 0, _timestamp, 0, 8);

            // load child objects
            dr.NextResult();
            _assignments.ListChanged -= new System.ComponentModel.ListChangedEventHandler(_assignments_ListChanged);
            _assignments = 
              ResourceAssignments.GetResourceAssignments(dr);
            _assignments.ListChanged += new System.ComponentModel.ListChangedEventHandler(_assignments_ListChanged);
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
        ApplicationContext.LocalContext["cn"] = cn;
        using (SqlCommand cm = cn.CreateCommand())
        {
          cm.CommandType = CommandType.StoredProcedure;
          cm.CommandText = "addResource";
          cm.Parameters.AddWithValue("@lastName", _lastName);
          cm.Parameters.AddWithValue("@firstName", _firstName);
          SqlParameter param = 
            new SqlParameter("@newId",SqlDbType.Int);
          param.Direction = ParameterDirection.Output;
          cm.Parameters.Add(param);
          param = new SqlParameter("@newLastChanged", SqlDbType.Timestamp);
          param.Direction = ParameterDirection.Output;
          cm.Parameters.Add(param);

          cm.ExecuteNonQuery();

          _id = (int)cm.Parameters["@newId"].Value;
          _timestamp = (byte[])cm.Parameters["@newLastChanged"].Value;
        }
        // update child objects
        _assignments.Update(this);
        // removing of item only needed for local data portal
        if (ApplicationContext.ExecutionLocation==ApplicationContext.ExecutionLocations.Client)
          ApplicationContext.LocalContext.Remove("cn");
      }
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Update()
    {
      using (SqlConnection cn = new SqlConnection(Database.PTrackerConnection))
      {
        cn.Open();
        ApplicationContext.LocalContext["cn"] = cn;
        if (base.IsDirty)
        {
          using (SqlCommand cm = cn.CreateCommand())
          {
            cm.CommandType = CommandType.StoredProcedure;
            cm.CommandText = "updateResource";
            cm.Parameters.AddWithValue("@id", _id);
            cm.Parameters.AddWithValue("@lastName", _lastName);
            cm.Parameters.AddWithValue("@firstName", _firstName);
            cm.Parameters.AddWithValue("@lastChanged", _timestamp);
            SqlParameter param = 
              new SqlParameter("@newLastChanged", SqlDbType.Timestamp);
            param.Direction = ParameterDirection.Output;
            cm.Parameters.Add(param);

            cm.ExecuteNonQuery();

            _timestamp = (byte[])cm.Parameters["@newLastChanged"].Value;
          }
        }
        // update child objects
        _assignments.Update(this);
        // removing of item only needed for local data portal
        if (ApplicationContext.ExecutionLocation == ApplicationContext.ExecutionLocations.Client)
          ApplicationContext.LocalContext.Remove("cn");
      }
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
          cm.CommandText = "deleteResource";
          cm.Parameters.AddWithValue("@id", criteria.Id);
          cm.ExecuteNonQuery();
        }
      }
    }

    protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
    {
      _assignments.ListChanged += new System.ComponentModel.ListChangedEventHandler(_assignments_ListChanged);
    }

    #endregion

    #region Exists

    public static bool Exists(string id)
    {
      return ExistsCommand.Exists(id);
    }

    [Serializable()]
    private class ExistsCommand : CommandBase
    {

      private string _id;
      private bool _exists;

      public bool ResourceExists
      {
        get { return _exists; }
      }

      public static bool Exists(string id)
      {
        ExistsCommand result;
        result = DataPortal.Execute<ExistsCommand>(new ExistsCommand(id));
        return result.ResourceExists;
      }

      private ExistsCommand(string id)
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
            cm.CommandText = "existsResource";
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
