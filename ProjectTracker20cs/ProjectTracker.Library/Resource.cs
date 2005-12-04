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

    private string _id = string.Empty;
    private string _lastName = string.Empty;
    private string _firstName = string.Empty;

    private ResourceAssignments _assignments = ResourceAssignments.NewResourceAssignments();

    public string Id
    {
      get
      {
        CanReadProperty(true);
        return _id;
      }
    }

    public string LastName
    {
      get
      {
        CanReadProperty(true);
        return _lastName;
      }
      set
      {
        CanWriteProperty(true);
        if (_lastName != value)
        {
          _lastName = value;
          PropertyHasChanged();
        }
      }
    }

    public string FirstName
    {
      get
      {
        CanReadProperty(true);
        return _firstName;
      }
      set
      {
        CanWriteProperty();
        if (_firstName != value)
        {
          _firstName = value;
          PropertyHasChanged();
        }
      }
    }
    public string FullName
    {
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

    public static Resource NewResource(string id)
    {
      if (!CanAddObject())
        throw new System.Security.SecurityException("User not authorized to add a resource");
      return DataPortal.Create<Resource>(new Criteria(id));
    }

    public static void DeleteResource(string id)
    {
      if (!CanDeleteObject())
        throw new System.Security.SecurityException("User not authorized to remove a resource");
      DataPortal.Delete(new Criteria(id));
    }

    public static Resource GetResource(string id)
    {
      if (!CanGetObject())
        throw new System.Security.SecurityException("User not authorized to view a resource");
      return DataPortal.Fetch<Resource>(new Criteria(id));
    }

    public override Resource Save()
    {
      if (IsDeleted)
        if (!CanDeleteObject())
          throw new System.Security.SecurityException("User not authorized to remove a resource");
        else
          if (!CanSaveObject())
            throw new System.Security.SecurityException("User not authorized to update a resource");
      return base.Save();
    }

    #endregion

    #region Constructors

    private Resource()
    {
      // prevent direct instantiation
    }

    #endregion

    #region

    [Serializable()]
    private class Criteria
    {
      private string _id;
      public string Id
      {
        get { return _id; }
      }

      public Criteria(string id)
      {
        _id = id;
      }
    }

    #endregion

    #region Data Access

    [RunLocal()]
    private void DataPortal_Create(Criteria criteria)
    {
      _id = criteria.Id;
    }

    private void DataPortal_Fetch(Criteria criteria)
    {
      using (SqlConnection cn = new SqlConnection(DataBase.DbConn))
      {
        cn.Open();
        using (SqlCommand cm = cn.CreateCommand())
        {
          cm.CommandType = CommandType.StoredProcedure;
          cm.CommandText = "getResource";
          cm.Parameters.AddWithValue("@ID", criteria.Id);

          using (SafeDataReader dr = new SafeDataReader(cm.ExecuteReader()))
          {
            dr.Read();
            _id = dr.GetString(0);
            _lastName = dr.GetString(1);
            _firstName = dr.GetString(2);

            // load child objects
            dr.NextResult();
            _assignments = ResourceAssignments.GetResourceAssignments(dr);
            dr.Close();
          }
        }
        cn.Close();
      }
    }

    [Transactional(TransactionalTypes.Manual)]
    protected override void DataPortal_Insert()
    {
      using (SqlConnection cn = new SqlConnection(DataBase.DbConn))
      {
        cn.Open();
        using (SqlTransaction tr = cn.BeginTransaction())
        {
          using (SqlCommand cm = cn.CreateCommand())
          {
            cm.Transaction = tr;
            cm.CommandType = CommandType.StoredProcedure;
            cm.CommandText = "addResource";
            LoadParameters(cm);

            cm.ExecuteNonQuery();

            // update child objects
            _assignments.Update(tr, this);
          }
          tr.Commit();
        }
        cn.Close();
      }
    }

    [Transactional(TransactionalTypes.Manual)]
    protected override void DataPortal_Update()
    {
      using (SqlConnection cn = new SqlConnection(DataBase.DbConn))
      {
        cn.Open();
        using (SqlTransaction tr = cn.BeginTransaction())
        {
          using (SqlCommand cm = cn.CreateCommand())
          {
            cm.Transaction = tr;
            cm.CommandType = CommandType.StoredProcedure;
            cm.CommandText = "updateResource";
            LoadParameters(cm);

            cm.ExecuteNonQuery();

            // update child objects
            _assignments.Update(tr, this);
          }
          tr.Commit();
        }
        cn.Close();
      }
    }

    private void LoadParameters(SqlCommand cm)
    {
      cm.Parameters.AddWithValue("@ID", _id);
      cm.Parameters.AddWithValue("@LastName", _lastName);
      cm.Parameters.AddWithValue("@FirstName", _firstName);
    }

    [Transactional(TransactionalTypes.Manual)]
    protected override void DataPortal_DeleteSelf()
    {
      if (!this.IsNew)
      {
        // we're not new, so get rid of our data
        DataPortal_Delete(new Criteria(_id));
      }
    }

    [Transactional(TransactionalTypes.Manual)]
    private void DataPortal_Delete(Criteria criteria)
    {
      using (SqlConnection cn = new SqlConnection(DataBase.DbConn))
      {
        cn.Open();
        using (SqlTransaction tr = cn.BeginTransaction())
        {
          using (SqlCommand cm = cn.CreateCommand())
          {
            cm.Transaction = tr;
            cm.CommandType = CommandType.StoredProcedure;
            cm.CommandText = "deleteResource";
            cm.Parameters.AddWithValue("@ID", criteria.Id);
            cm.ExecuteNonQuery();
          }
          tr.Commit();
        }
        cn.Close();
      }
    }

    #endregion

    #region Exists

    public static bool Exists(string id)
    {
      ExistsCommand result;
      result = DataPortal.Execute<ExistsCommand>(new ExistsCommand(id));
      return result.Exists;
    }

    [Serializable()]
    private class ExistsCommand : CommandBase
    {

      private string _id;
      private bool _exists;

      public bool Exists
      {
        get { return _exists; }
      }

      public ExistsCommand(string id)
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
            cm.CommandText = "SELECT id FROM Resources WHERE id=@id";
            cm.Parameters.AddWithValue("@id", _id);

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
