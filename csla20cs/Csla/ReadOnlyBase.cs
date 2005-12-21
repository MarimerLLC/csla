using System;
using System.Reflection;
using System.ComponentModel;
using Csla.Properties;

namespace Csla
{

  /// <summary>
  /// This is a base class from which readonly business classes
  /// can be derived.
  /// </summary>
  /// <remarks>
  /// This base class only supports data retrieve, not updating or
  /// deleting. Any business classes derived from this base class
  /// should only implement readonly properties.
  /// </remarks>
  [Serializable()]
  public abstract class ReadOnlyBase<T> : ICloneable, Core.IReadOnlyObject
    where T : ReadOnlyBase<T>
  {
    #region Object ID Value

    protected abstract object GetIdValue();

    #endregion

    #region System.Object Overrides

    public override bool Equals(object obj)
    {
      if (obj is T)
      {
        object id = GetIdValue();
        if (id == null)
          throw new ArgumentException(Resources.GetIdValueCantBeNull);
        return ((T)obj).GetIdValue().Equals(id);
      }
      else
        return false;
    }

    public override int GetHashCode()
    {
      object id = GetIdValue();
      if (id == null)
        throw new ArgumentException(Resources.GetIdValueCantBeNull);
      return id.GetHashCode();
    }

    public override string ToString()
    {
      object id = GetIdValue();
      if (id == null)
        throw new ArgumentException(Resources.GetIdValueCantBeNull);
      return id.ToString();
    }

    #endregion

    #region Constructors

    protected ReadOnlyBase()
    {
      AddAuthorizationRules();
    }

    #endregion

    #region Authorization

    [NotUndoable()]
    private Security.AuthorizationRules _authorizationRules = 
      new Security.AuthorizationRules();

    /// <summary>
    /// Override this method to add authorization
    /// rules for your object's properties.
    /// </summary>
    protected virtual void AddAuthorizationRules()
    {

    }

    /// <summary>
    /// Provides access to the AuthorizationRules object for this
    /// object.
    /// </summary>
    /// <remarks>
    /// Use this object to add a list of allowed and denied roles for
    /// reading and writing properties of the object. Typically these
    /// values are added once when the business object is instantiated.
    /// </remarks>
    protected Security.AuthorizationRules AuthorizationRules
    {
      get { return _authorizationRules; }
    }

    /// <summary>
    /// Returns True if the user is allowed to read the
    /// calling property.
    /// </summary>
    /// <returns>True if read is allowed.</returns>
    /// <remarks>
    /// <para>
    /// If a list of allowed roles is provided then only users in those
    /// roles can read. If no list of allowed roles is provided then
    /// the list of denied roles is checked.
    /// </para><para>
    /// If a list of denied roles is provided then users in the denied
    /// roles are denied read access. All other users are allowed.
    /// </para><para>
    /// If neither a list of allowed nor denied roles is provided then
    /// all users will have read access.
    /// </para>
    /// </remarks>
    /// <param name="throwOnFalse">Indicates whether a negative
    /// result should cause an exception.</param>    
    [System.Runtime.CompilerServices.MethodImpl(
      System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    public bool CanReadProperty(bool throwOnFalse)
    {
      string propertyName =
        new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name.Substring(4);
      bool result = CanReadProperty(propertyName);
      if (throwOnFalse && result == false)
        throw new System.Security.SecurityException(Resources.PropertyGetNotAllowed);
      return result;
    }

    /// <summary>
    /// Returns True if the user is allowed to read the
    /// calling property.
    /// </summary>
    /// <returns>True if read is allowed.</returns>
    /// <remarks>
    /// <para>
    /// If and only if the user is in a role explicitly denied 
    /// access and NOT in a role that explicitly
    /// allows access they will be denied read access to the property.
    /// </para><para>
    /// This implementation uses System.Diagnostics.StackTrace to
    /// determine the name of the current property, and so must be called
    /// directly from the property to be checked.
    /// </para>
    /// </remarks>
    public bool CanReadProperty()
    {
      string propertyName = 
        new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name.Substring(4);
      return CanReadProperty(propertyName);
    }

    /// <summary>
    /// Returns True if the user is allowed to read the
    /// specified property.
    /// </summary>
    /// <param name="propertyName">Name of the property to read.</param>
    /// <returns>True if read is allowed.</returns>
    /// <remarks>
    /// If and only if the user is in a role explicitly denied 
    /// access and NOT in a role that explicitly
    /// allows access they will be denied read access to the property.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual bool CanReadProperty(string propertyName)
    {
      bool result = true;
      if (AuthorizationRules.HasReadAllowedRoles(propertyName))
      {
        // some users are explicitly granted read access
        // in which case all other users are denied.
        if (!AuthorizationRules.IsReadAllowed(propertyName))
          result = false;
      }
      else if (AuthorizationRules.HasReadDeniedRoles(propertyName))
      {
        // some users are explicitly denied read access.
        if (AuthorizationRules.IsReadDenied(propertyName))
          result = false;
      }
      return result;
    }

    #endregion

    #region IClonable

    object ICloneable.Clone()
    {
      return OnClone();
    }

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>A new object containing the exact data of the original object.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual object OnClone()
    {
      return Core.ObjectCloner.Clone(this);
    }

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>
    /// A new object containing the exact data of the original object.
    /// </returns>
    public T Clone()
    {
      return (T)OnClone();
    }
    #endregion

    #region Data Access

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "criteria")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    private void DataPortal_Create(object criteria)
    {
      throw new NotSupportedException(Resources.CreateNotSupportedException);
    }

    /// <summary>
    /// Override this method to allow retrieval of an existing business
    /// object based on data in the database.
    /// </summary>
    /// <param name="Criteria">An object containing criteria values to identify the object.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_Fetch(object criteria)
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

    #endregion

  }
}