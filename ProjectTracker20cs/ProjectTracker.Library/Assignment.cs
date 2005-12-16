using System;
using System.Collections.Generic;
using System.Text;
using Csla.Validation;

namespace ProjectTracker.Library
{
  static class Assignment
  {
    /// <summary>
    /// Ensure the Role property value exists
    /// in RoleList
    /// </summary>
    internal static bool ValidRole(object target, RuleArgs e)
    {
      int role = -1;
      if (target is ProjectResource)
        role = ((ProjectResource)target).Role;
      else if (target is ResourceAssignment)
        role = ((ResourceAssignment)target).Role;

      if (RoleList.GetList().ContainsKey(role))
        return true;
      else
      {
        e.Description = "Role must be in RoleList";
        return false;
      }
    }
  }
}
