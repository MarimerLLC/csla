using System;
using CSLA.Resources;

namespace CSLA
{
  /// <summary>
  /// This is the base class from which command 
  /// objects will be derived.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Command objects allow the execution of arbitrary server-side
  /// functionality. Most often, this involves the invocation of
  /// a stored procedure in the database, but can involve any other
  /// type of stateless, atomic call to the server instead.
  /// </para><para>
  /// To implement a command object, inherit from CommandBase and
  /// override the DataPortal_Update method. In this method you can
  /// implement any server-side code as required.
  /// </para><para>
  /// To pass data to/from the server, use instance variables within
  /// the command object itself. The command object is instantiated on
  /// the client, and is passed by value to the server where the 
  /// DataPortal_Update method is invoked. The command object is then
  /// returned to the client by value.
  /// </para>
  /// </remarks>
  [Serializable()]
  public class CommandBase
	{
#region Data access

    private void DataPortal_Create(object criteria)
    {
      throw new NotSupportedException(Strings.GetResourceString("CreateNotSupportedException"));
    }

    private void DataPortal_Fetch(object criteria)
    {
      throw new NotSupportedException(Strings.GetResourceString("FetchNotSupportedException"));
    }

    /// <summary>
    /// Override this method to allow insert, update or deletion of a business
    /// object.
    /// </summary>
    protected virtual void DataPortal_Update()
    {
      throw new NotSupportedException(Strings.GetResourceString("UpdateNotSupportedException"));
    }

    private void DataPortal_Delete(object criteria)
    {
      throw new NotSupportedException(Strings.GetResourceString("DeleteNotSupportedException"));
    }

    /// <summary>
    /// Called by the server-side DataPortal prior to calling the 
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    protected virtual void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {
    }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    protected virtual void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
    }

    /// <summary>
    /// Returns the specified database connection string from the application
    /// configuration file.
    /// </summary>
    /// <remarks>
    /// The database connection string must be in the <c>appSettings</c> section
    /// of the application configuration file. The database name should be
    /// prefixed with 'DB:'. For instance, <c>DB:mydatabase</c>.
    /// </remarks>
    /// <param name="databaseName">Name of the database.</param>
    /// <returns>A database connection string.</returns>
    protected string DB(string databaseName)
    {
      return ConfigurationSettings.AppSettings["DB:" + databaseName];
    }

#endregion
      
	}
}
