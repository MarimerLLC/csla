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
    Guid _ID = Guid.NewGuid();
    string _Name = string.Empty;
    SmartDate _Started = new SmartDate(false);
    SmartDate _Ended = new SmartDate();
    string _Description = string.Empty;

    ProjectResources _Resources = ProjectResources.NewProjectResources();

    #region Business Properties and Methods

    public Guid ID
    {
      get
      {
        return _ID;
      }
    }

    public string Name
    {
      get
      {
        return _Name;
      }
      set
      {
        if(_Name != value)
        {
          _Name = value;
          BrokenRules.Assert("NameLen", "Name too long", (value.Length > 50));
          BrokenRules.Assert("NameRequired", "Project name required", (value.Length == 0));
          MarkDirty();
        }
      }
    }

    public string Started
    {
      get
      {
        return _Started.Text;
      }
      set
      {
        if(_Started.Text != value)
        {
          _Started.Text = value;
          if(_Ended.IsEmpty)
            BrokenRules.Assert("DateCol", "", false);
          
          else
            BrokenRules.Assert("DateCol", 
              "Start date must be prior to end date", 
              _Started.CompareTo(_Ended) > 0);
          MarkDirty();
        }
      }
    }

    public string Ended
    {
      get
      {
        return _Ended.Text;
      }
      set
      {
        if(_Ended.Text != value)
        {
          _Ended.Text = value;
          if(_Ended.IsEmpty)
            BrokenRules.Assert("DateCol", "", false);
          else
          {
            if(_Started.IsEmpty)
              BrokenRules.Assert("DateCol", 
                "Ended date must be later than started date", true);
            else
              BrokenRules.Assert("DateCol", 
                "Ended date must be later than started date", 
                _Ended.CompareTo(_Started) < 0);
          }
          MarkDirty();
        }
      }
    }

    public string Description
    {
      get
      {
        return _Description;
      }
      set
      {
        if(_Description != value)
        {
          _Description = value;
          MarkDirty();
        }
      }
    }

    public ProjectResources Resources
    {
      get
      {
        return _Resources;
      }
    }

    public override bool IsValid
    {
      get
      {
        return base.IsValid && _Resources.IsValid;
      }
    }

    public override bool IsDirty
    {
      get
      {
        return base.IsDirty || _Resources.IsDirty;
      }
    }

    #endregion


    #region System.Object Overrides

    public override string ToString()
    {
      return _ID.ToString();
    }

    public new static bool Equals(object objA, object objB)
    {
      if(objA is Project && objB is Project)
        return ((Project)objA).Equals((Project)objB);
      else
        return false;
    }

    public override bool Equals(object project)
    {
      if(project is Project)
        return Equals((Project)project);
      else
        return false;
    }

    public bool Equals(Project project)
    {
      return _ID.Equals(project.ID);
    }

    public override int GetHashCode()
    {
      return _ID.GetHashCode();
    }

    #endregion

    #region Shared Methods

    // create new object
    public static Project NewProject()
    {
      if(!Thread.CurrentPrincipal.IsInRole("ProjectManager"))
        throw new System.Security.SecurityException("User not authorized to add a project");
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
        throw new System.Security.SecurityException("User not authorized to remove a project");
      DataPortal.Delete(new Criteria(id));
    }

    public override BusinessBase Save()
    {
      if(IsDeleted)
      {
        System.Security.Principal.IIdentity user = Thread.CurrentPrincipal.Identity;
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

    public Project()
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
      Criteria crit = (Criteria)criteria;
      _ID = Guid.NewGuid();
      Started = DateTime.Today.ToShortDateString();
      Name = String.Empty;
    }

    // called by DataPortal to load data from the database
    protected override void DataPortal_Fetch(object criteria)
    {
      // retrieve data from db
      Criteria crit = (Criteria)criteria;
      SqlConnection cn = new SqlConnection(DB("PTracker"));
      SqlCommand cm = new SqlCommand();

      cn.Open();
      try
      {
        cm.Connection = cn;
        cm.CommandType = CommandType.StoredProcedure;
        cm.CommandText = "getProject";
        cm.Parameters.Add("@ID", crit.ID);

        SafeDataReader dr = new SafeDataReader(cm.ExecuteReader());
        try
        {
          dr.Read();
          _ID = dr.GetGuid(0);
          _Name = dr.GetString(1);
          _Started = dr.GetSmartDate(2, _Started.EmptyIsMin);
          _Ended = dr.GetSmartDate(3, _Ended.EmptyIsMin);
          _Description = dr.GetString(4);

          // load child objects
          dr.NextResult();
          _Resources = ProjectResources.GetProjectResources(dr);
        }
        finally
        {
          dr.Close();
        }
        MarkOld();
      }
      finally
      {
        cn.Close();
      }
    }

    // called by DataPortal to delete/add/update data into the database
    [Transactional()]
    protected override void DataPortal_Update()
    {
      // save data into db
      SqlConnection cn = new SqlConnection(DB("PTracker"));
      SqlCommand cm = new SqlCommand();

      cn.Open();
      try
      {
        cm.Connection = cn;
        cm.CommandType = CommandType.StoredProcedure;
        if(this.IsDeleted)
        {
          // we're being deleted
          if(!this.IsNew)
          {
            // we're not new, so get rid of our data
            cm.CommandText = "deleteProject";
            cm.Parameters.Add("@ID", _ID.ToString());
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

          cm.Parameters.Add("@ID", _ID.ToString());
          cm.Parameters.Add("@Name", _Name);
          cm.Parameters.Add("@Started", _Started.DBValue);
          cm.Parameters.Add("@Ended", _Ended.DBValue);
          cm.Parameters.Add("@Description", _Description);

          cm.ExecuteNonQuery();

          // make sure we're marked as an old object
          MarkOld();
        }

      }
      finally
      {
        cn.Close();
      }

      // update child objects
      _Resources.Update(this);
    }

    [Transactional()]
    protected override void DataPortal_Delete(object criteria)
    {
      Criteria crit = (Criteria)criteria;
      SqlConnection cn = new SqlConnection(DB("PTracker"));
      SqlCommand cm = new SqlCommand();

      cn.Open();

      try
      {
        cm.Connection = cn;
        cm.CommandType = CommandType.StoredProcedure;
        cm.CommandText = "deleteProject";
        cm.Parameters.Add("@ID", crit.ID.ToString());
        cm.ExecuteNonQuery();
      }
      finally
      {
        cn.Close();
      }
    }

    #endregion

  }
}
