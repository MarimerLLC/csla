//-----------------------------------------------------------------------
// <copyright file="ReadOnlyBindingListBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is the base class from which readonly collections</summary>
//-----------------------------------------------------------------------
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Csla.Core;
using Csla.Properties;

namespace Csla
{
  /// <summary>
  /// This is the base class from which readonly collections
  /// of readonly objects should be derived.
  /// </summary>
  /// <typeparam name="T">Type of the list class.</typeparam>
  /// <typeparam name="C">Type of child objects contained in the list.</typeparam>
  [SuppressMessage(
    "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  [Serializable]
  public abstract class ReadOnlyBindingListBase<T, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] C> :
    Core.ReadOnlyBindingList<C>, Csla.Core.IReadOnlyCollection,
    ICloneable, Server.IDataPortalTarget, Core.IUseApplicationContext
    where T : ReadOnlyBindingListBase<T, C>
  {
    /// <summary>
    /// Gets the current ApplicationContext
    /// </summary>
    protected ApplicationContext ApplicationContext { get; private set; }

    /// <inheritdoc />
    ApplicationContext IUseApplicationContext.ApplicationContext
    {
      get => ApplicationContext;
      set
      {
        ApplicationContext = value ?? throw new ArgumentNullException(nameof(ApplicationContext));
        Initialize();
      }
    }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable. Necessary for derived classes
    protected ReadOnlyBindingListBase()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    { }

    #region Initialize

    /// <summary>
    /// Override this method to set up event handlers so user
    /// code in a partial class can respond to events raised by
    /// generated code.
    /// </summary>
    protected virtual void Initialize()
    { /* allows subclass to initialize events before any other activity occurs */ }

    #endregion

    #region ICloneable

    object ICloneable.Clone()
    {
      return GetClone();
    }
    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>A new object containing the exact data of the original object.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual object GetClone()
    {
      return ObjectCloner.GetInstance(ApplicationContext).Clone(this)!;
    }

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>
    /// A new object containing the exact data of the original object.
    /// </returns>
    public T Clone()
    {
      return (T)GetClone();
    }

    #endregion

    #region Data Access

    private void DataPortal_Update()
    {
      throw new NotSupportedException(Resources.UpdateNotSupportedException);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "criteria")]
    [Delete]
    private void DataPortal_Delete(object criteria)
    {
      throw new NotSupportedException(Resources.DeleteNotSupportedException);
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
    /// occurs during data access.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    /// <param name="ex">The Exception thrown during data access.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal prior to calling the 
    /// requested DataPortal_XYZ method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void Child_OnDataPortalInvoke(DataPortalEventArgs e)
    {
    }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_XYZ method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void Child_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
    }

    /// <summary>
    /// Called by the server-side DataPortal if an exception
    /// occurs during data access.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    /// <param name="ex">The Exception thrown during data access.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void Child_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
    }

    #endregion

    #region ToArray

    /// <summary>
    /// Get an array containing all items in the list.
    /// </summary>
    public C[] ToArray()
    {
      List<C> result = new List<C>();
      foreach (C item in this)
        result.Add(item);
      return result.ToArray();
    }
    #endregion

    #region IDataPortalTarget Members

    void Server.IDataPortalTarget.CheckRules()
    { }

    Task Server.IDataPortalTarget.CheckRulesAsync() => Task.CompletedTask;

    Task Csla.Server.IDataPortalTarget.WaitForIdle(TimeSpan timeout) => WaitForIdle(timeout);
    Task Csla.Server.IDataPortalTarget.WaitForIdle(CancellationToken ct) => WaitForIdle(ct);

    void Server.IDataPortalTarget.MarkAsChild()
    { }

    void Server.IDataPortalTarget.MarkNew()
    { }

    void Server.IDataPortalTarget.MarkOld()
    { }

    void Server.IDataPortalTarget.DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      DataPortal_OnDataPortalInvoke(e);
    }

    void Server.IDataPortalTarget.DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      DataPortal_OnDataPortalInvokeComplete(e);
    }

    void Server.IDataPortalTarget.DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
      DataPortal_OnDataPortalException(e, ex);
    }

    void Server.IDataPortalTarget.Child_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      Child_OnDataPortalInvoke(e);
    }

    void Server.IDataPortalTarget.Child_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      Child_OnDataPortalInvokeComplete(e);
    }

    void Server.IDataPortalTarget.Child_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
      Child_OnDataPortalException(e, ex);
    }

    #endregion
  }
}