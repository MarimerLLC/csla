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
      DataDirectory = BuildDataDirectory();
      DataPortalTestDatabase = GetConnectionString("DataPortalTestDatabase");
      DataPortalTestDatabaseWithInvalidDBValue = GetConnectionString("DataPortalTestDatabaseWithInvalidDBValue");
      DataPortalTestDatabaseEntities = GetConnectionString("DataPortalTestDatabaseEntities");
      EntityConnectionWithMissingDB = GetConnectionString("DataPortalTestDatabaseEntitiesWithInvalidDBValue");
    }

    public static string DataDirectory { get; }

    public static string EntityConnectionWithMissingDBConnectionStringName = "DataPortalTestDatabaseEntitiesWithInvalidDBValue";
    public static string EntityConnectionWithMissingDB {get;} 
    public static string DataPortalTestDatabaseEntities { get; } 
    public static string DataPortalTestDatabaseWithInvalidDBValue { get; } 

    public static string DataPortalTestDatabase { get; }
    public static string TestLinqToSqlContextDataContext { get; }

    #region Private Helper Methods

    private static string BuildDataDirectory()
    {
      string dataDirectory = AppDomain.CurrentDomain.GetData("DataDirectory")?.ToString() ?? AppDomain.CurrentDomain.BaseDirectory;
      if (dataDirectory.EndsWith(@"\"))
      {
        dataDirectory = dataDirectory.Substring(0, dataDirectory.Length - 1);
      }
      return dataDirectory;
    }

    private static string GetConnectionString(string connectionName)
    {
      string connectionString;
      ConnectionStringSettingsCollection conStrings = AppConfig.ConnectionStrings.ConnectionStrings;

      connectionString = conStrings[connectionName].ConnectionString;
      connectionString = connectionString.Replace("|DataDirectory|", DataDirectory);
      return connectionString;
    }

    #endregion
  }
}
