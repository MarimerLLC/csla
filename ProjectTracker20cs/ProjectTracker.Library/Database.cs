using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectTracker.Library
{
    public static class DataBase
    {
      public static string DbConn
      {
        get { return System.Configuration.ConfigurationManager.ConnectionStrings["PTracker"].ConnectionString; }
      }

      public static string SecurityConn
      {
        get { return System.Configuration.ConfigurationManager.ConnectionStrings["Security"].ConnectionString; }
      }
    }
}
