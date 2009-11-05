using System;
using Csla;
using Csla.Data;
using Csla.Validation;

namespace ProjectTracker.Library
{
  internal interface IHoldRoles
  {
    int Role { get; set; }
  }

  internal static class Assignment
  {
    #region  Business Methods

    public static System.DateTime GetDefaultAssignedDate()
    {
      return System.DateTime.Today;
    }

    #endregion

    #region  Validation Rules

    /// <summary>
    /// Ensure the Role property value exists
    /// in RoleList
    /// </summary>
    public static bool ValidRole<T>(T target, RuleArgs e)
      where T : IHoldRoles
    {
      int role = target.Role;

      if (RoleList.GetList().ContainsKey(role))
      {
        return true;
      }
      else
      {
        e.Description = "Role must be in RoleList";
        return false;
      }
    }

    #endregion

    #region  Data Access

    public static byte[] AddAssignment(
      Guid projectId, 
      int resourceId, 
      SmartDate assigned, 
      int role)
    {
      using (var ctx = 
        ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.
        GetManager(ProjectTracker.DalLinq.Database.PTracker))
      {
        System.Data.Linq.Binary lastChanged = null;
        ctx.DataContext.addAssignment(
          projectId, 
          resourceId, 
          assigned, 
          role, 
          ref lastChanged);
        return lastChanged.ToArray();
      }
    }

    public static byte[] UpdateAssignment(Guid projectId, int resourceId, SmartDate assigned, int newRole, byte[] timestamp)
    {
      using (var ctx = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(ProjectTracker.DalLinq.Database.PTracker))
      {
        System.Data.Linq.Binary lastChanged = null;
        ctx.DataContext.updateAssignment(projectId, resourceId, assigned, newRole, timestamp, ref lastChanged);
        return lastChanged.ToArray();
      }
    }

    public static void RemoveAssignment(Guid projectId, int resourceId)
    {
      using (var ctx = ContextManager<ProjectTracker.DalLinq.PTrackerDataContext>.GetManager(ProjectTracker.DalLinq.Database.PTracker))
      {
        ctx.DataContext.deleteAssignment(projectId, resourceId);
      }
    }

    #endregion

  }
}