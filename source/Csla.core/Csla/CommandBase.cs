using System;
using System.ComponentModel;
using Csla.Server;
using Csla.Properties;
using Csla.Core;
using Csla.Serialization.Mobile;
using Csla.Serialization;
using Csla.DataPortalClient;


namespace Csla
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
  /// override the DataPortal_Execute method. In this method you can
  /// implement any server-side code as required.
  /// </para><para>
  /// To pass data to/from the server, use instance variables within
  /// the command object itself. The command object is instantiated on
  /// the client, and is passed by value to the server where the 
  /// DataPortal_Execute method is invoked. The command object is then
  /// returned to the client by value.
  /// </para>
  /// </remarks>
  [Serializable]
  public abstract class CommandBase : ManagedObjectBase, 
      ICommandObject,
      Csla.Server.IDataPortalTarget
  {
    #region Constructors

#if SILVERLIGHT
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public CommandBase()
    {
      Initialize();
    }
#else
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    protected CommandBase()
    {
      Initialize();
    }
#endif

    #endregion

    #region Initialize

    /// <summary>
    /// Override this method to set up event handlers so user
    /// code in a partial class can respond to events raised by
    /// generated code.
    /// </summary>
    protected virtual void Initialize()
    { /* allows subclass to initialize events before any other activity occurs */ }

    #endregion

    #region Data Access

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "criteria")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    private void DataPortal_Create(object criteria)
    {
      throw new NotSupportedException(Resources.CreateNotSupportedException);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "criteria")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    private void DataPortal_Fetch(object criteria)
    {
      throw new NotSupportedException(Resources.FetchNotSupportedException);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    private void DataPortal_Update()
    {
      throw new NotSupportedException(Resources.UpdateNotSupportedException);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "criteria")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    private void DataPortal_Delete(object criteria)
    {
      throw new NotSupportedException(Resources.DeleteNotSupportedException);
    }

    /// <summary>
    /// Override this method to implement any server-side code
    /// that is to be run when the command is executed.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_Execute()
    {
      throw new NotSupportedException(Resources.ExecuteNotSupportedException);
    }

    /// <summary>
    /// Called by the server-side DataPortal prior to calling the 
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal if an exception
    /// occurs during server-side processing.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    /// <param name="ex">The Exception thrown during processing.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {

    }

    #endregion

    #region IDataPortalTarget Members

    void IDataPortalTarget.CheckRules()
    { }

    void IDataPortalTarget.MarkAsChild()
    { }

    void IDataPortalTarget.MarkNew()
    { }

    void IDataPortalTarget.MarkOld()
    { }

    void IDataPortalTarget.DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      this.DataPortal_OnDataPortalInvoke(e);
    }

    void IDataPortalTarget.DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      this.DataPortal_OnDataPortalInvokeComplete(e);
    }

    void IDataPortalTarget.DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
      this.DataPortal_OnDataPortalException(e, ex);
    }

    void IDataPortalTarget.Child_OnDataPortalInvoke(DataPortalEventArgs e)
    { }

    void IDataPortalTarget.Child_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    { }

    void IDataPortalTarget.Child_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    { }

    #endregion
  }
}
