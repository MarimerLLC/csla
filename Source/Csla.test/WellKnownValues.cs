using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Csla.Test
{
  public class WellKnownValues
  {
    private static System.Configuration.Configuration AppConfig = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);

    static WellKnownValues()
    {
      ConnectionStringSettingsCollection conStrings = AppConfig.ConnectionStrings.ConnectionStrings;
      DataPortalTestDatabase = conStrings["DataPortalTestDatabase"].ConnectionString;
      DataPortalTestDatabaseWithInvalidDBValue = conStrings["DataPortalTestDatabaseWithInvalidDBValue"].ConnectionString;
      DataPortalTestDatabaseEntities = conStrings["DataPortalTestDatabaseEntities"].ConnectionString;
      EntityConnectionWithMissingDB = conStrings["DataPortalTestDatabaseEntitiesWithInvalidDBValue"].ConnectionString;
    }

    public static string EntityConnectionWithMissingDBConnectionStringName = "DataPortalTestDatabaseEntitiesWithInvalidDBValue";
    public static string EntityConnectionWithMissingDB {get;} 
    public static string DataPortalTestDatabaseEntities { get; } 
    public static string DataPortalTestDatabaseWithInvalidDBValue { get; } 

    public static string DataPortalTestDatabase { get; }
    public static string TestLinqToSqlContextDataContext { get; }
  }
}
