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

    private ResourceAssignments _assignments = ResourceAssignments.NewResourceAssignments();

    public int Id
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

    public static Resource NewResource()
    {
      if (!CanAddObject())
        throw new System.Security.SecurityException("User not authorized to add a resource");
      return DataPortal.Create<Resource>();
    }

    public static void DeleteResource(int id)
    {
      if (!CanDeleteObject())
        throw new System.Security.SecurityException("User not authorized to remove a resource");
      DataPortal.Delete(new Criteria(id));
    }

    public static Resource GetResource(int id)
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

    #region Criteria

    [Serializable()]
    private class Criteria
    {
      private int _id;
      public int Id
      {
        get { return _id; }
      }

      public Criteria(int id)
      {
        _id = id;
      }
    }

    #endregion

    #region Data Access

    [RunLocal()]
    private void DataPortal_Create(Criteria criteria)
    {
      // nothing to initialize
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
          cm.Parameters.AddWithValue("@id", criteria.Id);

          using (SafeDataReader dr = new SafeDataReader(cm.ExecuteReader()))
          {
            dr.Read();
            _id = dr.GetInt32("Id");
            _lastName = dr.GetString("LastName");
            _firstName = dr.GetString("FirstName");
            dr.GetBytes("LastChanged", 0, _timestamp, 0, 8);

            // load child objects
            dr.NextResult();
            _assignments = ResourceAssignments.GetResourceAssignments(dr);
          }
        }
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
          cm.CommandText = "addResource";
          cm.Parameters.AddWithValue("@lastName", _lastName);
          cm.Parameters.AddWithValue("@firstName", _firstName);

          using (SqlDataReader dr = cm.ExecuteReader())
          {
            dr.Read();
            _id = dr.GetInt32(0);
            dr.GetBytes(1, 0, _timestamp, 0, 8);
          }

          cm.ExecuteNonQuery();

          // update child objects
          _assignments.Update(cn, this);
        }
      }
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Update()
    {
      using (SqlConnection cn = new SqlConnection(DataBase.DbConn))
      {
        cn.Open();
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

            using (SqlDataReader dr = cm.ExecuteReader())
            {
              dr.Read();
              dr.GetBytes(0, 0, _timestamp, 0, 8);
            }
          }
        }

        // update child objects
        _assignments.Update(cn, this);
      }
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_DeleteSelf()
    {
      if (!this.IsNew)
      {
        // we're not new, so get rid of our data
        DataPortal_Delete(new Criteria(_id));
      }
    }

    [Transactional(TransactionalTypes.TransactionScope)]
    private void DataPortal_Delete(Criteria criteria)
    {
      using (SqlConnection cn = new SqlConnection(DataBase.DbConn))
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
            }
          }
        }
      }
    }

    #endregion

  }
}
