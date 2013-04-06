using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTracker.Dal
{
  public static class TimeStampExtensions
  {
    public static bool Matches(this byte[] stamp1, byte[] stamp2)
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
