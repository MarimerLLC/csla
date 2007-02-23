using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ProjectResources : 
    BusinessListBase<ProjectResources, ProjectResource>
  {
    #region Business Methods

    public ProjectResource GetItem(int resourceId)
    {
      foreach (ProjectResource res in this)
        if (res.ResourceId == resourceId)
          return res;
      return null;
    }

    public void Assign(int resourceId)
    {
      if (!Contains(resourceId))
      {
        ProjectResource resource =
          ProjectResource.NewProjectResource(resourceId);
        this.Add(resource);
      }
      else
        throw new InvalidOperationException(
          "Resource already assigned to project");
    }

    public void Remove(int resourceId)
    {
      foreach (ProjectResource res in this)
      {
        if (res.ResourceId == resourceId)
        {
          Remove(res);
          break;
        }
      }
    }

    public bool Contains(int resourceId)
    {
      foreach (ProjectResource res in this)
        if (res.ResourceId == resourceId)
          return true;
      return false;
    }

    public bool ContainsDeleted(int resourceId)
    {
      foreach (ProjectResource res in DeletedList)
        if (res.ResourceId == resourceId)
          return true;
      return false;
    }

    #endregion

    #region Factory Methods

    internal static ProjectResources NewProjectResources()
    {
      return new ProjectResources();
    }

    internal static ProjectResources GetProjectResources(SafeDataReader dr)
    {
      return new ProjectResources(dr);
    }

    private ProjectResources()
    {
      MarkAsChild();
    }

    private ProjectResources(SafeDataReader dr)
    {
      MarkAsChild();
      Fetch(dr);
    }

    #endregion

    #region Data Access

    // called to load data from the database
    private void Fetch(SafeDataReader dr)
    {
      this.RaiseListChangedEvents = false;
      while (dr.Read())
        this.Add(ProjectResource.GetResource(dr));
      this.RaiseListChangedEvents = true;
    }

    internal void Update(Project project)
    {
      this.RaiseListChangedEvents = false;
      // update (thus deleting) any deleted child objects
      foreach (ProjectResource obj in DeletedList)
        obj.DeleteSelf(project);
      // now that they are deleted, remove them from memory too
      DeletedList.Clear();

      // add/update any current child objects
      foreach (ProjectResource obj in this)
      {
        if (obj.IsNew)
          obj.Insert(project);
        else
          obj.Update(project);
      }
      this.RaiseListChangedEvents = true;
    }

    #endregion

  }
}
