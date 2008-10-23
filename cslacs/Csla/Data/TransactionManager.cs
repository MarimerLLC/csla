using System;
using System.Configuration;
using System.Data;
using Csla.Properties;

namespace Csla.Data
{
  /// <summary>
  /// Provides an automated way to reuse open
  /// database connections and associated
  /// ADO.NET transactions within the context
  /// of a single data portal operation.
  /// </summary>
  /// <typeparam name="C">
  /// Type of database connection object to use.
  /// </typeparam>
  /// <typeparam name="T">
  /// Type of ADO.NET transaction object to use.
  /// </typeparam>
  /// <remarks>
  /// This type stores the open ADO.NET transaction
  /// in <see cref="Csla.ApplicationContext.LocalContext" />
  /// and uses reference counting through
  /// <see cref="IDisposable" /> to keep the transaction
  /// open for reuse by child objects, and to automatically
  /// dispose the transaction when the last consumer
  /// has called Dispose."
  /// </remarks>
  public class TransactionManager<C, T> : IDisposable
    where C : IDbConnection, new()
    where T : IDbTransaction
  {
    private static object _lock = new object();
    private C _connection;
    private T _transaction;
    private string _connectionString;

    /// <summary>
    /// Gets the TransactionManager object for the 
    /// specified database.
    /// </summary>
    /// <param name="database">
    /// Database name as shown in the config file.
    /// </param>
    public static TransactionManager<C, T> GetManager(string database)
    {
      return GetManager(database, true);
    }

    /// <summary>
    /// Gets the TransactionManager object for the 
    /// specified database.
    /// </summary>
    /// <param name="database">
    /// The database name or connection string.
    /// </param>
    /// <param name="isDatabaseName">
    /// True to indicate that the Transaction string
    /// should be retrieved from the config file. If
    /// False, the database parameter is directly 
    /// used as a Transaction string.
    /// </param>
    /// <returns>TransactionManager object for the name.</returns>
    public static TransactionManager<C, T> GetManager(string database, bool isDatabaseName)
    {
      if (isDatabaseName)
      {
        var connection = ConfigurationManager.ConnectionStrings[database];
        if (connection == null)
          throw new ConfigurationErrorsException(String.Format(Resources.DatabaseNameNotFound, database));

        var conn = ConfigurationManager.ConnectionStrings[database].ConnectionString;
        if (string.IsNullOrEmpty(conn))
          throw new ConfigurationErrorsException(String.Format(Resources.DatabaseNameNotFound, database));
        database = conn;
      }

      lock (_lock)
      {
        TransactionManager<C, T> mgr = null;
        if (ApplicationContext.LocalContext.Contains("__transaction:" + database))
        {
          mgr = (TransactionManager<C, T>)(ApplicationContext.LocalContext["__transaction:" + database]);

        }
        else
        {
          mgr = new TransactionManager<C, T>(database);
          ApplicationContext.LocalContext["__transaction:" + database] = mgr;
        }
        mgr.AddRef();
        return mgr;
      }
    }

    private TransactionManager(string connectionString)
    {

      _connectionString = connectionString;

      // create and open connection
      _connection = new C();
      _connection.ConnectionString = connectionString;
      _connection.Open();
      //start transaction
      _transaction = (T)_connection.BeginTransaction();

    }

    /// <summary>
    /// Gets a reference to the current ADO.NET
    /// transaction object.
    /// </summary>
    public T Transaction
    {
      get
      {
        return _transaction;
      }
    }

    /// <summary>
    /// Gets a reference to the current ADO.NET
    /// connection object that is associated with 
    /// current trasnaction.
    /// </summary>
    public C Connection
    {
      get
      {
        return _connection;
      }
    }

    #region  Reference counting

    private int mRefCount;

    private void AddRef()
    {
      mRefCount += 1;
    }

    private void DeRef()
    {

      lock (_lock)
      {
        mRefCount -= 1;
        if (mRefCount == 0)
        {
          _transaction.Dispose();
          _connection.Dispose();
          ApplicationContext.LocalContext.Remove("__transaction:" + _connectionString);
        }
      }

    }

    #endregion

    #region  IDisposable

    /// <summary>
    /// Dispose object, dereferencing or
    /// disposing the connection it is
    /// managing.
    /// </summary>
    public void Dispose()
    {
      DeRef();
    }

    #endregion
  }
}
