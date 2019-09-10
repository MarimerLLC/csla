using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Rolodex.Business.Data
{
  public static class DataConnection
  {
    public static string EFConnectionName
    {
      get
      {
        return "RolodexEntities";
      }
    }
  }
}
