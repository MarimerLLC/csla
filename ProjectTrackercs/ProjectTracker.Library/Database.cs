using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace ProjectTracker.Library
{
    public static class Database
    {
      public static string PTrackerConnection
      {
        get 
        { 
          return ConfigurationManager.ConnectionStrings
            ["PTracker"].ConnectionString; 
        }
      }

      public static string SecurityConnection
      {
        get { return System.Configuration.ConfigurationManager.ConnectionStrings["Security"].ConnectionString; }
      }
    }
}
