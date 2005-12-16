using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Csla;
using Csla.Validation;
using Csla.Data;

namespace ProjectTracker.Library
{
  internal static class Assignment
  {
    /// <summary>
    /// Ensure the Role property value exists
    /// in RoleList
    /// </summary>
    public static bool ValidRole(object target, RuleArgs e)
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

    #region Data Access

    public static DateTime GetDefaultAssignedDate()
    {
      return DateTime.Today;
    }

    public static byte[] AddAssignment(SqlConnection cn, Guid projectId, int resourceId, SmartDate assigned, int role)
    {
      byte[] result = new byte[8];
      using (SqlCommand cm = cn.CreateCommand())
      {
        cm.CommandType = CommandType.StoredProcedure;
        cm.CommandText = "addAssignment";
        cm.Parameters.AddWithValue("@projectId", projectId);
        cm.Parameters.AddWithValue("@resourceId", resourceId);
        cm.Parameters.AddWithValue("@assigned", assigned.DBValue);
        cm.Parameters.AddWithValue("@role", role);

        using (SqlDataReader dr = cm.ExecuteReader())
        {
          dr.Read();
          // get the timestamp for the new row
          dr.GetBytes(0, 0, result, 0, 8);
        }
      }
      return result;
    }

    public static byte[] UpdateAssignment(SqlConnection cn,
      Guid projectId, int resourceId, SmartDate assigned, int newRole, byte[] timestamp)
    {
      byte[] result = new byte[8];
      using (SqlCommand cm = cn.CreateCommand())
      {
        cm.CommandType = CommandType.StoredProcedure;
        cm.CommandText = "updateAssignment";
        cm.Parameters.AddWithValue("@projectId", projectId);
        cm.Parameters.AddWithValue("@resourceId", resourceId);
        cm.Parameters.AddWithValue("@assigned", assigned.DBValue);
        cm.Parameters.AddWithValue("@role", newRole);
        cm.Parameters.AddWithValue("@lastChanged", timestamp);

        using (SqlDataReader dr = cm.ExecuteReader())
        {
          dr.Read();
          // get the new timestamp for the row
          dr.GetBytes(0, 0, result, 0, 8);
        }
      }
      return result;
    }

    public static void RemoveAssignment(SqlConnection cn, Guid projectId, int resourceId)
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
