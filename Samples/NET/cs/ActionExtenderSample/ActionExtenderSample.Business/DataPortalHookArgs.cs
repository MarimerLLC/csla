//-----------------------------------------------------------------------
// <copyright file="DataPortalHookArgs.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary></summary>
// <remarks>Generated file.</remarks>
//-----------------------------------------------------------------------

using System.Data;
using System.Data.Common;
using Csla.Data;

namespace ActionExtenderSample.Business
{
  /// <summary>
  /// Event arguments for the DataPortalHook.<br/>
  /// This class holds the arguments for events that happen during DataPortal operations.
  /// </summary>
  public class DataPortalHookArgs
  {

    #region Properties

    /// <summary>
    /// Gets or sets the criteria argument.
    /// </summary>
    /// <value>The criteria object.</value>
    public object CriteriaArg { get; private set; }

    /// <summary>
    /// Gets or sets the connection argument.
    /// </summary>
    /// <value>The connection.</value>
    public DbConnection ConnectionArg { get; private set; }

    /// <summary>
    /// Gets or sets the command argument.
    /// </summary>
    /// <value>The command.</value>
    public DbCommand CommandArg { get; private set; }

    /// <summary>
    /// Gets or sets the ADO transaction argument.
    /// </summary>
    /// <value>The ADO transaction.</value>
    public DbTransaction TransactionArg { get; private set; }

    /// <summary>
    /// Gets or sets the data reader argument.
    /// </summary>
    /// <value>The data reader.</value>
    public SafeDataReader DataReaderArg { get; private set; }

    /// <summary>
    /// Gets or sets the data row argument.
    /// </summary>
    /// <value>The data row.</value>
    public DataRow DataRowArg { get; private set; }

    /// <summary>
    /// Gets or sets the data set argument.
    /// </summary>
    /// <value>The data set.</value>
    public DataSet DataSetArg { get; private set; }

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new empty instance of the <see cref="DataPortalHookArgs"/> class.
    /// </summary>
    public DataPortalHookArgs()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataPortalHookArgs"/> class.
    /// </summary>
    /// <param name="crit">The criteria object argument.</param>
    public DataPortalHookArgs(object crit)
    {
      CriteriaArg = crit;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataPortalHookArgs"/> class.
    /// </summary>
    /// <param name="cmd">The command argument.</param>
    /// <remarks>The connection and ADO transaction arguments are set automatically, based on the command argument.</remarks>
    public DataPortalHookArgs(DbCommand cmd)
    {
      CommandArg = cmd;
      ConnectionArg = cmd.Connection;
      TransactionArg = cmd.Transaction;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataPortalHookArgs"/> class.
    /// </summary>
    /// <param name="cmd">The command argument.</param>
    /// <param name="crit">The criteria argument.</param>
    /// <remarks>The connection and ADO transaction arguments are set automatically, based on the command argument.</remarks>
    public DataPortalHookArgs(DbCommand cmd, object crit)
      : this(cmd)
    {
      CriteriaArg = crit;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataPortalHookArgs"/> class.
    /// </summary>
    /// <param name="dr">The SafeDataReader argument.</param>
    public DataPortalHookArgs(SafeDataReader dr)
    {
      DataReaderArg = dr;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataPortalHookArgs"/> class.
    /// </summary>
    /// <param name="cmd">The command argument.</param>
    /// <param name="dr">The SafeDataReader argument.</param>
    /// <remarks>The connection and ADO transaction arguments are set automatically, based on the command argument.</remarks>
    public DataPortalHookArgs(DbCommand cmd, SafeDataReader dr)
      : this(cmd)
    {
      DataReaderArg = dr;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataPortalHookArgs"/> class.
    /// </summary>
    /// <param name="cmd">The command argument.</param>
    /// <param name="ds">The DataSet argument.</param>
    /// <remarks>The connection and ADO transaction arguments are set automatically, based on the command argument.</remarks>
    public DataPortalHookArgs(DbCommand cmd, DataSet ds)
      : this(cmd)
    {
      DataSetArg = ds;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataPortalHookArgs"/> class.
    /// </summary>
    /// <param name="dr">The data row argument.</param>
    public DataPortalHookArgs(DataRow dr)
    {
      DataRowArg = dr;
    }

    #endregion

  }
}
