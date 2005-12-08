using System;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Data;

namespace ProjectTracker.Library
{

  [Serializable()]
  public class ResourceAssignments : BusinessListBase<ResourceAssignments, ResourceAssignment>
  {

    #region Business Methods

    public ResourceAssignment this[Guid projectId]
    {
      get
      {
        foreach (ResourceAssignment res in this)
          if (res.ProjectId.Equals(projectId))
            return res;
        return null;
      }
    }

    public void AssignTo(Guid projectId)
    {
      DoAssignment(ResourceAssignment.NewResourceAssignment(projectId));
    }

    private void DoAssignment(ResourceAssignment project)
    {
      if (!Contains(project))
        this.Add(project);
      else
        throw new InvalidOperationException("Resource already assigned to project");
    }

    public void Remove(Guid projectId)
    {
      foreach (ResourceAssignment res in this)
      {
        if (res.ProjectId.Equals(projectId))
        {
          Remove(res);
          break;
        }
      }
    }

    #endregion

    #region Factory Methods

    internal static ResourceAssignments NewResourceAssignments()
    {
      return new ResourceAssignments();
    }

    internal static ResourceAssignments GetResourceAssignments(SafeDataReader dr)
    {
      return new ResourceAssignments(dr);
    }

    #endregion

    #region Constructors

    private ResourceAssignments()
    {
      MarkAsChild();
    }

    #endregion

    #region Data Access

    private ResourceAssignments(SafeDataReader dr)
    {
      while (dr.Read())
        this.Add(ResourceAssignment.GetResourceAssignment(dr));
    }

    internal void Update(SqlTransaction tr, Resource resource)
    {
      // update (thus deleting) any deleted child objects
      foreach (ResourceAssignment item in DeletedList)
        item.DeleteSelf(tr, resource);
      // now that they are deleted, remove them from memory too
      DeletedList.Clear();

      // add/update any current child objects
      foreach (ResourceAssignment item in this)
      {
        if (item.IsNew)
          item.Insert(tr, resource);
        else
          item.Update(tr, resource);
      }
    }

    #endregion

  }
}