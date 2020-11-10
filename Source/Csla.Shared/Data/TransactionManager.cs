#if !NETFX_CORE && !(ANDROID || IOS)
//-----------------------------------------------------------------------
// <copyright file="TransactionManager.cs" company="Marimer LLC">
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
    private string _label;

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
    /// Database name as shown in the config file.
    /// </param>
    /// <param name="label">Label for this transaction.</param>
    public static TransactionManager<C, T> GetManager(string database, string label)
    {
      return GetManager(database, true, label);
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
      return GetManager(database, isDatabaseName, "default");
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
    /// <param name="label">Label for this transaction.</param>
    /// <returns>TransactionManager object for the name.</returns>
    public static TransactionManager<C, T> GetManager(string database, bool isDatabaseName, string label)
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

      lock (_lock)
      {
        var contextLabel = GetContextName(database, label);
        TransactionManager<C, T> mgr = null;
        if (ApplicationContext.LocalContext.Contains(contextLabel))
        {
          mgr = (TransactionManager<C, T>)(ApplicationContext.LocalContext[contextLabel]);

        }
        else
        {
          mgr = new TransactionManager<C, T>(database, label);
          ApplicationContext.LocalContext[contextLabel] = mgr;
        }
        mgr.AddRef();
        return mgr;
      }
    }

    private TransactionManager(string connectionString, string label)
    {
      _label = label;
      _connectionString = connectionString;

      // create and open connection
      _connection = new C();
      _connection.ConnectionString = connectionString;
      _connection.Open();
      //start transaction
      _transaction = (T)_connection.BeginTransaction();

    }

    private static string GetContextName(string connectionString, string label)
    {
      return "__transaction:" + label + "-" + connectionString;
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

    private bool _commit = false;

    /// <summary>
    /// Indicates that the current transactional
    /// scope has completed successfully. If all
    /// transactional scopes complete successfully
    /// the transaction will commit when the
    /// TransactionManager object is disposed.
    /// </summary>
    public void Commit()
    {
      if (RefCount == 1)
        _commit = true;
    }

#region  Reference counting

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
      lock (_lock)
      {
        _refCount -= 1;
        if (_refCount == 0)
        {
          try
          {
            if (_commit)
              _transaction.Commit();
            else
              _transaction.Rollback();
          }
          finally
          {
            _transaction.Dispose();
            _connection.Dispose();
            ApplicationContext.LocalContext.Remove(GetContextName(_connectionString, _label));
          }
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
#endif