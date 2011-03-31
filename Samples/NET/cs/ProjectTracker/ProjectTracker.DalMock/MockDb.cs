using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectTracker.DalMock.MockDbTypes;

namespace ProjectTracker.DalMock
{
  public static class MockDb
  {
    public static List<ProjectData> Projects { get; private set; }
    public static List<ResourceData> Resources { get; private set; }
    public static List<AssignmentData> Assignments { get; private set; }
    public static List<RoleData> Roles { get; private set; }

    static MockDb()
    {
      Projects = new List<ProjectData>
        {
          new ProjectData { Id = 1, Name = "Update ProjectTracker", Description = "Update ProjectTracker for CSLA 4", Started = new DateTime(2011, 3, 22), LastChanged = GetTimeStamp() }
        };
      Resources = new List<ResourceData>
        {
          new ResourceData { Id = 1, FirstName = "Rocky", LastName = "Lhotka", LastChanged = GetTimeStamp() }
        };
      Assignments = new List<AssignmentData>
        {
          new AssignmentData { ProjectId = 1, ResourceId = 1, Assigned = new DateTime(2011, 3, 23), RoleId = 2, LastChanged = GetTimeStamp() }
        };
      Roles = new List<RoleData>
        {
          new RoleData { Id = 1, Name = "Project manager", LastChanged = GetTimeStamp() },
          new RoleData { Id = 2, Name = "Developer", LastChanged = GetTimeStamp() },
          new RoleData { Id = 3, Name = "QA", LastChanged = GetTimeStamp() },
          new RoleData { Id = 4, Name = "Sponsor", LastChanged = GetTimeStamp() }
        };
    }

    private static long _lastTimeStamp = 1;

    public static byte[] GetTimeStamp()
    {
      var stamp = System.Threading.Interlocked.Add(ref _lastTimeStamp, 1);
      return System.Text.ASCIIEncoding.ASCII.GetBytes(stamp.ToString());
    }

    public static bool TimeStampEquals(byte[] stamp1, byte[] stamp2)
    {
      if (stamp1 != null && stamp2 != null)
        if (stamp1.Length == stamp2.Length)
        {
          for (int i = 0; i < stamp1.Length; i++)
            if (!stamp1[i].Equals(stamp2[i]))
              return false;
          return true;
        }
      return false;
    }
  }
}
