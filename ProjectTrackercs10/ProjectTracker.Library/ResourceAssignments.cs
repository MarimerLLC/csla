using System;
using System.Data;
using System.Data.SqlClient;
using CSLA;
using CSLA.Data;

namespace ProjectTracker.Library
{
  [Serializable()]
	public class ResourceAssignments : BusinessCollectionBase
	{
    #region Business Properties and Methods

    public ResourceAssignment this [int index]
    {
      get
      {
        return (ResourceAssignment)List[index];
      }
    }

    public ResourceAssignment this [Guid projectID]
    {
      get
      {
        foreach(ResourceAssignment r in List)
          if(r.ProjectID.Equals(projectID))
            return r;
        return null;
      }
    }

    public void AssignTo(Project project, string role)
    {
      DoAssignment(ResourceAssignment.NewResourceAssignment(project, role));
    }

    public void AssignTo(Guid projectID, string role)
    {
      DoAssignment(ResourceAssignment.NewResourceAssignment(projectID, role));
    }

    public void AssignTo(Guid projectID)
    {
      DoAssignment(ResourceAssignment.NewResourceAssignment(projectID));
    }

    private void DoAssignment(ResourceAssignment project)
    {
      if(!Contains(project))
        List.Add(project);
      else
        throw new Exception("Resource already assigned to project");
    }

    public void Remove(ResourceAssignment project)
    {
      List.Remove(project);
    }

    public void Remove(Guid projectID)
    {
      foreach(ResourceAssignment obj in List)
        if(obj.ProjectID.Equals(projectID))
        {
          Remove(obj);
          break;
        }
    }

    #endregion

    #region Contains

    public bool Contains(ResourceAssignment assignment)
    {
      foreach(ResourceAssignment child in List)
        if(child.Equals(assignment))
          return true;
      return false;
    }

    public bool ContainsDeleted(ResourceAssignment assignment)
    {
      foreach(ResourceAssignment child in deletedList)
        if(child.Equals(assignment))
          return true;
      return false;
    }

    public bool Contains(Guid projectID)
    {
      foreach(ResourceAssignment r in List)
        if(r.ProjectID.Equals(projectID))
          return true;
      return false;
    }

    public bool ContainsDeleted(Guid projectID)
    {
      foreach(ResourceAssignment r in deletedList)
        if(r.ProjectID.Equals(projectID))
          return true;
      return false;
    }

    #endregion

    #region Shared Methods

    internal static ResourceAssignments NewResourceAssignments()
    {
      return new ResourceAssignments();
    }

    internal static ResourceAssignments GetResourceAssignments(SafeDataReader dr)
    {
      ResourceAssignments col = new ResourceAssignments();
      col.Fetch(dr);
      return col;
    }

    #endregion

    #region Constructors

		public ResourceAssignments()
		{
      MarkAsChild();
		}

    #endregion

    #region Data Access

    // called by Resource to load data from the database
    private void Fetch(SafeDataReader dr)
    {
      while(dr.Read())
        List.Add(ResourceAssignment.GetResourceAssignment(dr));
    }

    // called by Resource to delete/add/update data into the database
    internal void Update(SqlTransaction tr, Resource resource)
    {
      // update (thus deleting) any deleted child objects
      foreach(ResourceAssignment obj in deletedList)
        obj.Update(tr, resource);
      // now that they are deleted, remove them from memory too
      deletedList.Clear();

      // add/update any current child objects
      foreach(ResourceAssignment obj in List)
        obj.Update(tr, resource);
    }

    #endregion

  }
}
