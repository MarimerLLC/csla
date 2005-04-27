using System;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using CSLA;
using CSLA.Data;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class Project : BusinessBase 
  {
    Guid _id = Guid.NewGuid();
    string _name = string.Empty;
    SmartDate _started = new SmartDate(false);
    SmartDate _ended = new SmartDate();
    string _description = string.Empty;

    ProjectResources _resources = 
      ProjectResources.NewProjectResources();

    #region Business Properties and Methods

    public Guid ID
    {
      get
      {
        return _id;
      }
    }

    public string Name
    {
      get
      {
        return _name;
      }
      set
      {
        if(value == null) value = string.Empty;
        if(_name != value)
        {
          _name = value;
          BrokenRules.Assert("NameLen", "Name too long", (value.Length > 50));
          BrokenRules.Assert("NameRequired", "Project name required", 
            (value.Length == 0));
          MarkDirty();
        }
      }
    }

    public string Started
    {
      get
      {
        return _started.Text;
      }
      set
      {
        if(value == null) value = string.Empty;
        if(_started.Text != value)
        {
          _started.Text = value;
          if(_ended.IsEmpty)
            BrokenRules.Assert("DateCol", "", false);
          
          else
            BrokenRules.Assert("DateCol", 
              "Start date must be prior to end date", 
              _started.CompareTo(_ended) > 0);
          MarkDirty();
        }
      }
    }

    public string Ended
    {
      get
      {
        return _ended.Text;
      }
      set
      {
        if(value == null) value = string.Empty;
        if(_ended.Text != value)
        {
          _ended.Text = value;
          if(_ended.IsEmpty)
            BrokenRules.Assert("DateCol", "", false);
          else
          {
            if(_started.IsEmpty)
              BrokenRules.Assert("DateCol", 
                "Ended date must be later than started date", true);
            else
              BrokenRules.Assert("DateCol", 
                "Ended date must be later than started date", 
                _ended.CompareTo(_started) < 0);
          }
          MarkDirty();
        }
      }
    }

    public string Description
    {
      get
      {
        return _description;
      }
      set
      {
        if(value == null) value = string.Empty;
        if(_description != value)
        {
          _description = value;
          MarkDirty();
        }
      }
    }

    public ProjectResources Resources
    {
      get
      {
        return _resources;
      }
    }

    public override bool IsValid
    {
      get
      {
        return base.IsValid && _resources.IsValid;
      }
    }

    public override bool IsDirty
    {
      get
      {
        return base.IsDirty || _resources.IsDirty;
      }
    }

    #endregion

    #region System.Object Overrides

    public override string ToString()
    {
      return _id.ToString();
    }

    public override bool Equals(object obj)
    {
      if (obj == null || obj.GetType() != this.GetType())
        return false;
      return _id == ((Project)obj).ID;
    }

    public override int GetHashCode()
    {
      return _id.GetHashCode();
    }

    #endregion

    #region Static Methods

    // create new object
    public static Project NewProject()
    {
      if(!Thread.CurrentPrincipal.IsInRole("ProjectManager"))
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
      if(!Thread.CurrentPrincipal.IsInRole("ProjectManager") && 
        !Thread.CurrentPrincipal.IsInRole("Administrator"))
        throw new System.Security.SecurityException(
          "User not authorized to remove a project");
      DataPortal.Delete(new Criteria(id));
    }

    public override BusinessBase Save()
    {
      if(IsDeleted)
      {
        System.Security.Principal.IIdentity user = 
          Thread.CurrentPrincipal.Identity;
        bool b = user.IsAuthenticated;
        if(!Thread.CurrentPrincipal.IsInRole("ProjectManager") && 
          !Thread.CurrentPrincipal.IsInRole("Administrator"))
          throw new System.Security.SecurityException(
            "User not authorized to remove a project");
      }
      else
      {
        // no deletion - we're adding or updating
        if(!Thread.CurrentPrincipal.IsInRole("ProjectManager"))
          throw new System.Security.SecurityException(
            "User not authorized to update a project");
      }
      return base.Save();
    }

    #endregion

    #region Constructors

    private Project()
    {
      // prevent direct instantiation
    }
    #endregion

    #region Criteria

    // criteria for identifying existing object
    [Serializable()]
      private class Criteria
    {
      public Guid ID;

      public Criteria(Guid id)
      {
        ID = id;
      }
    }

    #endregion

    #region Data Access

    //protected override BusinessBase Save()
    //{}

    // called by DataPortal so we can set defaults as needed
    protected override void DataPortal_Create(object criteria)
    {
      // for this object the criteria can be ignored on creation
      _id = Guid.NewGuid();
      Started = DateTime.Today.ToShortDateString();
      Name = String.Empty;
    }

    // called by DataPortal to load data from the database
    protected override void DataPortal_Fetch(object criteria)
    {
      // retrieve data from db
      Criteria crit = (Criteria)criteria;
      using(SqlConnection cn = new SqlConnection(DB("PTracker")))
      {
        cn.Open();
        using(SqlCommand cm = cn.CreateCommand())
        {
          cm.CommandType = CommandType.StoredProcedure;
          cm.CommandText = "getProject";
          cm.Parameters.Add("@ID", crit.ID);

          using(SafeDataReader dr = new SafeDataReader(cm.ExecuteReader()))
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
          }
          MarkOld();
        }
      }
    }

    // called by DataPortal to delete/add/update data into the database
    [Transactional()]
    protected override void DataPortal_Update()
    {
      // save data into db
      using(SqlConnection cn = new SqlConnection(DB("PTracker")))
      {
        cn.Open();
        using(SqlCommand cm = cn.CreateCommand())
        {
          cm.CommandType = CommandType.StoredProcedure;
          if(this.IsDeleted)
          {
            // we're being deleted
            if(!this.IsNew)
            {
              // we're not new, so get rid of our data
              cm.CommandText = "deleteProject";
              cm.Parameters.Add("@ID", _id.ToString());
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
              cm.CommandText = "addProject";
            }
            else
            {
              // we're not new, so update
              cm.CommandText = "updateProject";
            }

            cm.Parameters.Add("@ID", _id.ToString());
            cm.Parameters.Add("@Name", _name);
            cm.Parameters.Add("@Started", _started.DBValue);
            cm.Parameters.Add("@Ended", _ended.DBValue);
            cm.Parameters.Add("@Description", _description);

            cm.ExecuteNonQuery();

            // make sure we're marked as an old object
            MarkOld();
          }
        }
      }

      // update child objects
      _resources.Update(this);
    }

    [Transactional()]
    protected override void DataPortal_Delete(object criteria)
    {
      Criteria crit = (Criteria)criteria;
      using(SqlConnection cn = new SqlConnection(DB("PTracker")))
      {
        cn.Open();
        using(SqlCommand cm = cn.CreateCommand())
        {
          cm.CommandType = CommandType.StoredProcedure;
          cm.CommandText = "deleteProject";
          cm.Parameters.Add("@ID", crit.ID.ToString());
          cm.ExecuteNonQuery();
        }
      }
    }

    #endregion

  }
}
