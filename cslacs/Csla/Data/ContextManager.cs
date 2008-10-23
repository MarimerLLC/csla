#if !CLIENTONLY
using System;
using System.Configuration;
using System.Data.Linq;

using Csla.Properties;

namespace Csla.Data
{
  /// <summary>
  /// Provides an automated way to reuse 
  /// LINQ data context objects within the context
  /// of a single data portal operation.
  /// </summary>
  /// <typeparam name="C">
  /// Type of database 
  /// LINQ data context objects object to use.
  /// </typeparam>
  /// <remarks>
  /// This type stores the LINQ data context object 
  /// in <see cref="Csla.ApplicationContext.LocalContext" />
  /// and uses reference counting through
  /// <see cref="IDisposable" /> to keep the data context object
  /// open for reuse by child objects, and to automatically
  /// dispose the object when the last consumer
  /// has called Dispose."
  /// </remarks>
  public class ContextManager<C> : IDisposable where C : DataContext
  {
    private static object _lock = new object();
    private C _context;
    private string _connectionString;

    /// <summary>
    /// Gets the ContextManager object for the 
    /// specified database.
    /// </summary>
    /// <param name="database">
    /// Database name as shown in the config file.
    /// </param>
    public static ContextManager<C> GetManager(string database)
    {
      return GetManager(database, true);
    }

    /// <summary>
    /// Gets the ContextManager object for the 
    /// specified database.
    /// </summary>
    /// <param name="database">
    /// The database name or connection string.
    /// </param>
    /// <param name="isDatabaseName">
    /// True to indicate that the connection string
    /// should be retrieved from the config file. If
    /// False, the database parameter is directly 
    /// used as a connection string.
    /// </param>
    /// <returns>ContextManager object for the name.</returns>
    public static ContextManager<C> GetManager(string database, bool isDatabaseName)
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
        ContextManager<C> mgr = null;
        if (ApplicationContext.LocalContext.Contains("__ctx:" + database))
        {
          mgr = (ContextManager<C>)(ApplicationContext.LocalContext["__ctx:" + database]);

        }
        else
        {
          mgr = new ContextManager<C>(database);
          ApplicationContext.LocalContext["__ctx:" + database] = mgr;
        }
        mgr.AddRef();
        return mgr;
      }
    }

    private ContextManager(string connectionString)
    {

      _connectionString = connectionString;

      _context = (C)(Activator.CreateInstance(typeof(C), connectionString));

    }

    /// <summary>
    /// Gets the LINQ data context object.
    /// </summary>
    public C DataContext
    {
      get
      {
        return _context;
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
          _context.Dispose();
          ApplicationContext.LocalContext.Remove("__ctx:" + _connectionString);
        }
      }

    }

    #endregion

    #region  IDisposable

    /// <summary>
    /// Dispose object, dereferencing or
    /// disposing the context it is
    /// managing.
    /// </summary>
    public void Dispose()
    {
      DeRef();
    }

    #endregion

  }
}
#endif