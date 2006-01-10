using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Validation;
using Csla.Data;
using System.Reflection;

namespace ProjectTracker.Library
{
  internal static class Assignment
  {

    #region Business Methods

    public static DateTime GetDefaultAssignedDate()
    {
      return DateTime.Today;
    }

    #endregion

    #region Validation Rules

    /// <summary>
    /// Ensure the Role property value exists
    /// in RoleList
    /// </summary>
    public static bool ValidRole(object target, RuleArgs e)
    {
      int role = -1;
      PropertyInfo p = 
        target.GetType().GetProperty(e.PropertyName);
      role = (int)p.GetValue(target, null);

      //if (target is ProjectResource)
      //  role = ((ProjectResource)target).Role;
      //else if (target is ResourceAssignment)
      //  role = ((ResourceAssignment)target).Role;

      if (RoleList.GetList().ContainsKey(role))
        return true;
      else
      {
        e.Description = "Role must be in RoleList";
        return false;
      }
    }

    #endregion

    #region Data Access

    public static byte[] AddAssignment(
      SqlConnection cn, Guid projectId, int resourceId, 
      SmartDate assigned, int role)
    {
      using (SqlCommand cm = cn.CreateCommand())
      {
        cm.CommandText = "addAssignment";
        return DoAddUpdate(
          cm, projectId, resourceId, assigned, role);
      }
    }

    public static byte[] UpdateAssignment(SqlConnection cn,
      Guid projectId, int resourceId, SmartDate assigned, 
      int newRole, byte[] timestamp)
    {
      using (SqlCommand cm = cn.CreateCommand())
      {
        cm.CommandText = "updateAssignment";
        cm.Parameters.AddWithValue("@lastChanged", timestamp);
        return DoAddUpdate(
          cm, projectId, resourceId, assigned, newRole);
      }
    }

    private static byte[] DoAddUpdate(SqlCommand cm,
      Guid projectId, int resourceId, SmartDate assigned,
      int newRole)
    {
      byte[] result = new byte[8];
      cm.CommandType = CommandType.StoredProcedure;
      cm.Parameters.AddWithValue("@projectId", projectId);
      cm.Parameters.AddWithValue("@resourceId", resourceId);
      cm.Parameters.AddWithValue("@assigned", assigned.DBValue);
      cm.Parameters.AddWithValue("@role", newRole);

      using (SqlDataReader dr = cm.ExecuteReader())
      {
        dr.Read();
        // get the new timestamp for the row
        dr.GetBytes(0, 0, result, 0, 8);
      }
      return result;
    }

    public static void RemoveAssignment(
      SqlConnection cn, Guid projectId, int resourceId)
    {
      using (SqlCommand cm = cn.CreateCommand())
      {
        cm.CommandType = CommandType.StoredProcedure;
        cm.CommandText = "deleteAssignment";
        cm.Parameters.AddWithValue("@projectId", projectId);
        cm.Parameters.AddWithValue("@resourceId", resourceId);

        cm.ExecuteNonQuery();
      }
    }

    #endregion

  }
}
