using System;
using CSLA;
using CSLA.Data;

namespace ProjectTracker.Library
{
  [Serializable()]
	public class ProjectResources : BusinessCollectionBase
	{
    #region Business Properties and Methods

    public ProjectResource this [int index]
    {
      get
      {
        return (ProjectResource)List[index];
      }
    }

    public ProjectResource this [string resourceID]
    {
      get
      {
        foreach(ProjectResource r in List)
        {
          if(r.ResourceID == resourceID)
            return r;
        }
        return null;
      }
    }

    public void Assign(Resource resource, string role)
    {
      DoAssignment(ProjectResource.NewProjectResource(resource, role));
    }

    public void Assign(string resourceID, string role)
    {
      DoAssignment(ProjectResource.NewProjectResource(resourceID, role));
    }

    public void Assign(string resourceID)
    {
      DoAssignment(ProjectResource.NewProjectResource(resourceID));
    }

    private void DoAssignment(ProjectResource resource)
    {
      if(!Contains(resource))
        List.Add(resource);
      else
        throw new Exception("Resource already assigned");
    }

    public void Remove(ProjectResource resource)
    {
      List.Remove(resource);
    }

    public void Remove(string resourceID)
    {
      Remove(this[resourceID]);
    }

    #endregion

    #region Contains

    public bool Contains(ProjectResource assignment) 
    {
      foreach(ProjectResource child in List)
        if(child.Equals(assignment))
          return true;
      return false;
    }

    public bool ContainsDeleted(ProjectResource assignment)
    {
      foreach(ProjectResource child in deletedList)
        if(child.Equals(assignment))
          return true;
      return false;
    }

    public bool Contains(string resourceID)
    {
      foreach(ProjectResource r in List)
        if(r.ResourceID == resourceID)
          return true;
      return false;
    }

    public bool ContainsDeleted(string resourceID)
    {
      foreach(ProjectResource r in deletedList)
        if(r.ResourceID == resourceID)
          return true;
      return false;
    }

    #endregion

    #region Static Methods

    internal static ProjectResources NewProjectResources() 
    {
      return new ProjectResources();
    }

    internal static ProjectResources GetProjectResources(SafeDataReader dr) 
    {
      ProjectResources col = new ProjectResources();
      col.Fetch(dr);
      return col;
    }

    #endregion

    #region Constructors

    public ProjectResources()
    {
      MarkAsChild();
    }

    #endregion

    #region Data Access

    // called to load data from the database
    private void Fetch(SafeDataReader dr)
    {
      while(dr.Read())
        List.Add(ProjectResource.GetProjectResource(dr));
    }

    // called by Project to delete/add/update data into the database
    internal void Update(Project project)
    {
      // update (thus deleting) any deleted child objects
      foreach(ProjectResource obj in deletedList)
        obj.Update(project);

      // now that they are deleted, remove them from memory too
      deletedList.Clear();

      // add/update any current child objects
      foreach(ProjectResource obj in List)
        obj.Update(project);
    }

    #endregion
	}
}
