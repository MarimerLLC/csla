using System;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using CSLA;
using CSLA.Data;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class Resource : BusinessBase
  {
    string _id = string.Empty;
    string _lastName = string.Empty;
    string _firstName = string.Empty;

    ResourceAssignments _assignments = ResourceAssignments.NewResourceAssignments();

    #region Business Properties and Methods

    public string ID
    {
      get
      {
        return _id;
      }
    }

    public string LastName
    {
      get
      {
        return _lastName;
      }
      set
      {
        if(value == null) value = string.Empty;
        if(_lastName != value)
        {
          _lastName = value;
          BrokenRules.Assert("LNameReq", "Value required", value.Length == 0);
          BrokenRules.Assert("LNameLen", "Value too long", value.Length > 50);
          MarkDirty();
        }
      }
    }

    public string FirstName
    {
      get
      {
        return _firstName;
      }
      set
      {
        if(value == null) value = string.Empty;
        if(_firstName != value)
        {
          _firstName = value;
          BrokenRules.Assert("FNameLen", "Value too long", value.Length > 50);
          MarkDirty();
        }
      }
    }

    public ResourceAssignments Assignments
    {
      get
      {
        return _assignments;
      }
    }

    public override bool IsValid
    {
      get
      {
        return base.IsValid && _assignments.IsValid;
      }
    }

    public override bool IsDirty
    {
      get
      {
        return base.IsDirty || _assignments.IsDirty;
      }
    }

    #endregion

    #region System.Object Overrides

    public override string ToString()
    {
      return _id;
    }

    public new static bool Equals(object objA, object objB)
    {
      if(objA is Resource && objB is Resource)
        return ((Resource)objA).Equals((Resource)objB);
      else
        return false;
    }

    public override bool Equals(object resource)
    {
      if(resource is Resource)
        return Equals((Resource)resource);
      else
        return false;
    }

    public bool Equals(Resource resource)
    {
      return _id == resource.ID;
    }

    public override int GetHashCode()
    {
      return _id.GetHashCode();
    }

    #endregion

    #region Shared Methods

    // create new object
    public static Resource NewResource(string id)
    {
      if(!Thread.CurrentPrincipal.IsInRole("Supervisor") && 
        !Thread.CurrentPrincipal.IsInRole("ProjectManager"))
        throw new System.Security.SecurityException(
          "User not authorized to add a resource");
      return new Resource(id);
    }

    // delete object
    public static void DeleteResource(string id)
    {
      if(!Thread.CurrentPrincipal.IsInRole("Supervisor") && 
        !Thread.CurrentPrincipal.IsInRole("ProjectManager") && 
        !Thread.CurrentPrincipal.IsInRole("Administrator"))
        throw new System.Security.SecurityException(
          "User not authorized to remove a resource");
      DataPortal.Delete(new Criteria(id));
    }

    // load existing object
    public static Resource GetResource(string id)
    {
      return (Resource)DataPortal.Fetch(new Criteria(id));
    }

    public override BusinessBase Save()
    {
      if(this.IsDeleted)
      {
        if(!Thread.CurrentPrincipal.IsInRole("Supervisor") &&
          !Thread.CurrentPrincipal.IsInRole("ProjectManager") &&
          !Thread.CurrentPrincipal.IsInRole("Administrator"))
          throw new System.Security.SecurityException(
            "User not authorized to remove a resource");
      }
      else
        if(!Thread.CurrentPrincipal.IsInRole("Supervisor") && 
        !Thread.CurrentPrincipal.IsInRole("ProjectManager"))
        throw new System.Security.SecurityException(
          "User not authorized to update a resource");

      return base.Save();
    }

    #endregion

    #region Constructors

    private Resource(string id)
    {
      _id = id;
    }

    private Resource()
    {
      // prevent direct instantiation
    }

    #endregion

    #region Criteria

    [Serializable()]
      private class Criteria
    {
      public string ID;

      public Criteria(string id)
      {
        ID = id;
      }
    }

    #endregion

    #region Data Access

    // called by DataPortal to load data from the database
    protected override void DataPortal_Fetch(object criteria)
    {
      Criteria crit = (Criteria)criteria;
      using(SqlConnection cn = new SqlConnection(DB("PTracker")))
      {
        cn.Open();
        SqlTransaction tr = 
          cn.BeginTransaction(IsolationLevel.ReadCommitted);
        try
        {
          using(SqlCommand cm = cn.CreateCommand())
          {
            cm.Transaction = tr;
            cm.CommandType = CommandType.StoredProcedure;
            cm.CommandText = "getResource";
            cm.Parameters.Add("@ID", crit.ID);

            using(SafeDataReader dr = new SafeDataReader(cm.ExecuteReader()))
            {
              dr.Read();
              _id = dr.GetString(0);
              _lastName = dr.GetString(1);
              _firstName = dr.GetString(2);

              // load child objects
              dr.NextResult();
              _assignments = ResourceAssignments.GetResourceAssignments(dr);
            }
          }
          MarkOld();
          tr.Commit();
        }
        catch
        {
          tr.Rollback();
          throw;
        }
      }
    }

    // called by DataPortal to delete/add/update data into the database
    protected override void DataPortal_Update()
    {
      // save data into db
      using(SqlConnection cn = new SqlConnection(DB("PTracker")))
      {
        cn.Open();
        SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);
        try
        {
          using(SqlCommand cm = cn.CreateCommand())
          {
            cm.Transaction = tr;
            cm.CommandType = CommandType.StoredProcedure;
            if(this.IsDeleted)
            {
              // we're being deleted
              if(!this.IsNew)
              {
                // we're not new, so get rid of our data
                cm.CommandText = "deleteResource";
                cm.Parameters.Add("@ID", _id);
                cm.ExecuteNonQuery();
              }
              // reset our status to be a new object
              MarkNew();
            }                                                                                              
            else
            {
              // we're not being deleted, so insert or update
              if(this.IsNew)
              {
                // we're new, so insert
                cm.CommandText = "addResource";
              }
              else
              {                                                                                                // we're not new, so update
                cm.CommandText = "updateResource";
              }
              cm.Parameters.Add("@ID", _id);
              cm.Parameters.Add("@LastName", _lastName);
              cm.Parameters.Add("@FirstName", _firstName);
              cm.ExecuteNonQuery();

              // update child objects
              _assignments.Update(tr, this);

              // make sure we're marked as an old object
              MarkOld();
            }
          }
          tr.Commit();
        }
        catch
        {
          tr.Rollback();
          throw;
        }
      }
    }

    protected override void DataPortal_Delete(object criteria)
    {
      Criteria crit = (Criteria)criteria;
      using(SqlConnection cn = new SqlConnection(DB("PTracker")))
      {
        cn.Open();
        SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);
        try
        {
          using(SqlCommand cm = cn.CreateCommand())
          {
            cm.Transaction = tr;
            cm.CommandType = CommandType.StoredProcedure;
            cm.CommandText = "deleteResource";
            cm.Parameters.Add("@ID", crit.ID);
            cm.ExecuteNonQuery();
            tr.Commit();
          }
        }
        catch
        {
          tr.Rollback();
          throw;
        }
      }
    }

    #endregion

  }
}
