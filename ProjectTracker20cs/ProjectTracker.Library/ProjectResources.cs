using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ProjectResources : BusinessListBase<ProjectResources, ProjectResource>
  {

    #region Business Methods

    public ProjectResource this[int resourceId]
    {
      get
      {
        foreach (ProjectResource res in this)
          if (res.ResourceId == resourceId)
            return res;
        return null;
      }
    }

    public void Assign(int resourceId)
    {
      DoAssignment(ProjectResource.NewProjectResource(resourceId, RoleList.DefaultRole()));
    }

    private void DoAssignment(ProjectResource resource)
    {
      if (!Contains(resource))
        this.Add(resource);
      else
        throw new InvalidOperationException("Resource already assigned to project");
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

    #endregion

    #region Contains

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

    #endregion

    #region Constructors

    private ProjectResources()
    {
      MarkAsChild();
    }

    #endregion

    #region Data Access

    // called to load data from the database
    private ProjectResources(SafeDataReader dr)
    {
      MarkAsChild();
      while (dr.Read())
        this.Add(ProjectResource.GetResource(dr));
    }

    internal void Update(Project project)
    {
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
    }

    #endregion

  }
}
