//-----------------------------------------------------------------------
// <copyright file="DalManagerActionExtenderSample.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary></summary>
// <remarks>Generated file.</remarks>
//-----------------------------------------------------------------------

using System;
using System.Configuration;
using System.Data.SqlClient;
using Csla.Data;

namespace ActionExtenderSample.DataAccess.Sql
{
  /// <summary>
  /// Implements <see cref="IDalManagerActionExtenderSample"/> interface.
  /// </summary>
  /// <remarks>
  /// To use this DAL:<br/>
  /// 1) name this assembly ActionExtenderSample.DataAccess.Sql<br/>
  /// 2) add the following line to the <strong>appSettings</strong>
  /// section of the application .config file: <br/>
  /// &lt;add key="ActionExtenderSample.DalManagerType" value="ActionExtenderSample.DataAccess.Sql.DalManagerActionExtenderSample, ActionExtenderSample.DataAccess.Sql" /&gt;
  /// </remarks>
  public class DalManagerActionExtenderSample : IDalManagerActionExtenderSample
  {
    #region Fields and Properties

    private static readonly string TypeMask =
      typeof(DalManagerActionExtenderSample).FullName?.Replace("DalManagerActionExtenderSample", @"{0}");

    /// <summary>
    /// Gets the ADO ConnectionManager object.
    /// </summary>
    /// <value>The ConnectionManager object</value>
    public ConnectionManager<SqlConnection> ConnectionManager { get; private set; }

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="DalManagerActionExtenderSample"/> class.
    /// </summary>
    public DalManagerActionExtenderSample()
    {
      try
      {
        ConnectionManager = ConnectionManager<SqlConnection>.GetManager("ActionExtenderSample");
      }
      catch (ConfigurationErrorsException)
      {
        if (ConnectionManager == null)
          throw new ArgumentException(string.Format("ConnectionString {0} could not be found",
            "ActionExtenderSample"));
      }
    }

    #endregion

    #region IDalManagerActionExtenderSample Members

    /// <summary>
    /// Gets the ActionExtenderSample DAL provider for a given object Type.
    /// </summary>
    /// <typeparam name="T">Object Type that requires a ActionExtenderSample DAL provider.</typeparam>
    /// <returns>A new ActionExtenderSample DAL instance for the given Type.</returns>
    public T GetProvider<T>() where T : class
    {
      var typeName = string.Format(TypeMask, typeof(T).Name.Substring(1));
      var type = Type.GetType(typeName);
      if (type != null)
        return Activator.CreateInstance(type) as T;

      throw new NotImplementedException(typeName);
    }

    #endregion

    #region IDisposable Members

    /// <summary>
    /// Disposes the ConnectionManager.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing && ConnectionManager != null)
      {
        ConnectionManager.Dispose();
        ConnectionManager = null;
      }
    }

    #endregion
  }
}