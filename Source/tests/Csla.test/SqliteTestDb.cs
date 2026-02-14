using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace Csla.Test
{
  /// <summary>
  /// Creates and manages a temporary SQLite database for integration tests
  /// that previously required SQL Server LocalDB.
  /// </summary>
  public static class SqliteTestDb
  {
    private static string _dbPath;

    public static string ConnectionString { get; private set; }

    public static void Initialize()
    {
      _dbPath = Path.Combine(Path.GetTempPath(), $"CslaTest_{Guid.NewGuid():N}.db");
      ConnectionString = $"Data Source={_dbPath}";

      using var connection = new SqliteConnection(ConnectionString);
      connection.Open();

      // Table1: used by SafeDataReaderTests.TestSafeDataReader
      ExecuteNonQuery(connection,
        "CREATE TABLE Table1 (Name VARCHAR(50), Date TEXT, Age INTEGER)");
      ExecuteNonQuery(connection,
        "INSERT INTO Table1 (Name, Date, Age) VALUES ('Bill', '2004-12-23', 56)");
      ExecuteNonQuery(connection,
        "INSERT INTO Table1 (Name, Date, Age) VALUES ('Jim', '2003-01-14', 33)");

      // Table2: used by DataPortalTests.TestTransactionScopeUpdate and SafeDataReaderTests
      // CHECK constraint replicates SQL Server's VARCHAR(5) column length enforcement
      ExecuteNonQuery(connection,
        "CREATE TABLE Table2 (" +
        "FirstName TEXT NOT NULL, " +
        "LastName TEXT NOT NULL, " +
        "SmallColumn TEXT NOT NULL CHECK(LENGTH(SmallColumn) <= 5))");

      // MultiDataTypes: used by SafeDataReaderTests (GetSchemaTable, IsDBNull, GetDataTypes)
      ExecuteNonQuery(connection,
        "CREATE TABLE MultiDataTypes (" +
        "BIGINTFIELD INTEGER, " +
        "IMAGEFIELD BLOB, " +
        "BITFIELD INTEGER, " +
        "CHARFIELD TEXT, " +
        "DATETIMEFIELD TEXT, " +
        "UNIQUEIDENTIFIERFIELD TEXT, " +
        "SMALLINTFIELD INTEGER, " +
        "INTFIELD INTEGER, " +
        "TEXT TEXT)");
      ExecuteNonQuery(connection,
        "INSERT INTO MultiDataTypes " +
        "(BIGINTFIELD, IMAGEFIELD, BITFIELD, CHARFIELD, DATETIMEFIELD, " +
        "UNIQUEIDENTIFIERFIELD, SMALLINTFIELD, INTFIELD, TEXT) " +
        "VALUES (92233720368547111, NULL, 0, 'z', '2005-12-13 00:00:00', " +
        "'c0f92820-61b5-11da-8cd6-0800200c9a66', 32767, 2147483647, " +
        "'a bunch of text...a bunch of text...a bunch of text...a bunch of text...')");
    }

    public static void Cleanup()
    {
      ConnectionString = null;
      if (_dbPath != null && File.Exists(_dbPath))
      {
        try
        {
          File.Delete(_dbPath);
        }
        catch
        {
          // best effort cleanup
        }
        _dbPath = null;
      }
    }

    private static void ExecuteNonQuery(SqliteConnection connection, string sql)
    {
      using var cmd = connection.CreateCommand();
      cmd.CommandText = sql;
      cmd.ExecuteNonQuery();
    }
  }
}
