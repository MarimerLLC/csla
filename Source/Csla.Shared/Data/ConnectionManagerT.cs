//-----------------------------------------------------------------------
// <copyright file="ConnectionManagerT.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides an automated way to reuse open</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Configuration;
using System.Data;
using Csla.Properties;

namespace Csla.Data
{
  /// <summary>
  /// Provides an automated way to reuse open
  /// database connections within the context
  /// of a single data portal operation.
  /// </summary>
  /// <typeparam name="C">
  /// Type of database connection object to use.
  /// </typeparam>
  /// <remarks>
  /// This type stores the open database connection
  /// in <see cref="Csla.ApplicationContext.LocalContext" />
  /// and uses reference counting through
  /// <see cref="IDisposable" /> to keep the connection
  /// open for reuse by child objects, and to automatically
  /// dispose the connection when the last consumer
  /// has called Dispose."
  /// </remarks>
  public class ConnectionManager<C> : IDisposable where C : IDbConnection, new()
  {
    private static object _lock = new object();
    private C _connection;
    private string _connectionString;
    private string _label;

    /// <summary>
    /// Gets the ConnectionManager object for the 
    /// specified database.
    /// </summary>
    /// <param name="database">
    /// Database name as shown in the config file.
    /// </param>
    public static ConnectionManager<C> GetManager(string database)
    {
      return GetManager(database, true);
    }

    /// <summary>
    /// Gets the ConnectionManager object for the 
    /// specified database.
    /// </summary>
    /// <param name="database">
    /// Database name as shown in the config file.
    /// </param>
    /// <param name="label">Label for this connection.</param>
    public static ConnectionManager<C> GetManager(string database, string label)
    {
      return GetManager(database, true, label);
    }

    /// <summary>
    /// Gets the ConnectionManager object for the 
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
    /// <returns>ConnectionManager object for the name.</returns>
    public static ConnectionManager<C> GetManager(string database, bool isDatabaseName)
    {
      return GetManager(database, isDatabaseName, "default");
    }

    /// <summary>
    /// Gets the ConnectionManager object for the 
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
    /// <param name="label">Label for this connection.</param>
    /// <returns>ConnectionManager object for the name.</returns>
    public static ConnectionManager<C> GetManager(string database, bool isDatabaseName, string label)
    {
      if (isDatabaseName)
      {
#if NETSTANDARD2_0 || NET5_0
        throw new NotSupportedException("isDatabaseName==true");
#else
        var connection = ConfigurationManager.ConnectionStrings[database];
        if (connection == null)
          throw new System.Configuration.ConfigurationErrorsException(String.Format(Resources.DatabaseNameNotFound, database));

        var conn = ConfigurationManager.ConnectionStrings[database].ConnectionString;
        if (string.IsNullOrEmpty(conn))
          throw new System.Configuration.ConfigurationErrorsException(String.Format(Resources.DatabaseNameNotFound, database));
        database = conn;
#endif
      }

      ConnectionManager<C> mgr = null;
      var ctxName = GetContextName(database, label);
      lock (_lock)
      {
        var cached = ApplicationContext.LocalContext.GetValueOrNull(ctxName);
        if (cached != null)
        {
          mgr = (ConnectionManager<C>)cached;
          mgr.AddRef();
        }
      }

      if (mgr == null)
      {
        mgr = new ConnectionManager<C>(database, label);
        lock (_lock)
        {
          ApplicationContext.LocalContext[ctxName] = mgr;
          mgr.AddRef();
        }
      }
      return mgr;
    }

    private ConnectionManager(string connectionString, string label)
    {
      _label = label;
      _connectionString = connectionString;

      // open connection
      _connection = new C { ConnectionString = connectionString };
      _connection.Open();
    }

    private static string GetContextName(string connectionString, string label)
    {
      return "__db:" + label + "-" + connectionString;
    }

    /// <summary>
    /// Gets the open database connection object.
    /// </summary>
    public C Connection
    {
      get
      {
        return _connection;
      }
    }

    private int _refCount;

    /// <summary>
    /// Gets the current reference count for this
    /// object.
    /// </summary>
    public int RefCount
    {
      get { return _refCount; }
    }

    private void AddRef()
    {
      _refCount += 1;
    }

    private void DeRef()
    {
      _refCount -= 1;
      if (_refCount == 0)
      {
        _connection.Close();
        _connection.Dispose();
        lock (_lock)
          ApplicationContext.LocalContext.Remove(GetContextName(_connectionString, _label));
      }
    }

    /// <summary>
    /// Dispose object, dereferencing or
    /// disposing the connection it is
    /// managing.
    /// </summary>
    public void Dispose()
    {
      DeRef();
    }
  }
}
