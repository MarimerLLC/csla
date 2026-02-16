namespace Csla.Test
{
  public class WellKnownValues
  {
    public static string DataPortalTestDatabase
    {
      get
      {
        if (SqliteTestDb.ConnectionString == null)
          throw new InvalidOperationException("SqliteTestDb has not been initialized. Ensure AssemblyInitialize has run.");
        return SqliteTestDb.ConnectionString;
      }
    }
  }
}
