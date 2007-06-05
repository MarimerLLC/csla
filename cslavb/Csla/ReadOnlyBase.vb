Imports System.Reflection
Imports System.ComponentModel
Imports Csla.Core

''' <summary>
''' This is a base class from which readonly business classes
''' can be derived.
''' </summary>
''' <remarks>
''' This base class only supports data retrieve, not updating or
''' deleting. Any business classes derived from this base class
''' should only implement readonly properties.
''' </remarks>
''' <typeparam name="T">Type of the business object.</typeparam>
<Serializable()> _
Public MustInherit Class ReadOnlyBase(Of T As ReadOnlyBase(Of T))

  Implements ICloneable
  Implements Core.IReadOnlyObject
  Implements Csla.Security.IAuthorizeReadWrite

#Region " Object ID Value "

  ''' <summary>
  ''' Override this method to return a unique identifying
  ''' vlaue for this object.
  ''' </summary>
  ''' <remarks>
  ''' If you can not provide a unique identifying value, it
  ''' is best if you can generate such a unique value (even
  ''' temporarily). If you can not do that, then return 
  ''' <see langword="Nothing"/> and then manually override the
  ''' <see cref="Equals"/>, <see cref="GetHashCode"/> and
  ''' <see cref="ToString"/> methods in your business object.
  ''' </remarks>
  Protected MustOverride Function GetIdValue() As Object

#End Region

#Region " System.Object Overrides "

  ''' <summary>
  ''' Compares this object for equality with another object, using
  ''' the results of <see cref="GetIdValue"/> to determine
  ''' equality.
  ''' </summary>
  ''' <param name="obj">The object to be compared.</param>
  Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean

    If TypeOf obj Is T Then
      Dim id As Object = GetIdValue()
      If id Is Nothing Then
        Throw New ArgumentException(My.Resources.GetIdValueCantBeNull)
      End If
      Return id.Equals(DirectCast(obj, T).GetIdValue)

    Else
      Return False
    End If

  End Function

  ''' <summary>
  ''' Returns a hash code value for this object, based on
  ''' the results of <see cref="GetIdValue"/>.
  ''' </summary>
  Public Overrides Function GetHashCode() As Integer

    Dim id As Object = GetIdValue()
    If id Is Nothing Then
      Throw New ArgumentException(My.Resources.GetIdValueCantBeNull)
    End If
    Return id.GetHashCode

  End Function

  ''' <summary>
  ''' Returns a text representation of this object by
  ''' returning the <see cref="GetIdValue"/> value
  ''' in text form.
  ''' </summary>
  Public Overrides Function ToString() As String

    Dim id As Object = GetIdValue()
    If id Is Nothing Then
      Throw New ArgumentException(My.Resources.GetIdValueCantBeNull)
    End If
    Return id.ToString

  End Function

#End Region

#Region " Constructors "

  ''' <summary>
  ''' Creates an instance of the object.
  ''' </summary>
  Protected Sub New()

    Initialize()
    AddInstanceAuthorizationRules()
    If Not Csla.Security.SharedAuthorizationRules.RulesExistFor(Me.GetType) Then
      SyncLock Me.GetType
        If Not Csla.Security.SharedAuthorizationRules.RulesExistFor(Me.GetType) Then
          AddAuthorizationRules()
        End If
      End SyncLock
    End If

  End Sub

#End Region

#Region " Initialize "

  ''' <summary>
  ''' Override this method to set up event handlers so user
  ''' code in a partial class can respond to events raised by
  ''' generated code.
  ''' </summary>
  Protected Overridable Sub Initialize()
    ' allows a generated class to set up events to be
    ' handled by a partial class containing user code
  End Sub

#End Region

#Region " Authorization "

  <NotUndoable()> _
  <NonSerialized()> _
  Private mReadResultCache As Dictionary(Of String, Boolean)
  <NotUndoable()> _
  <NonSerialized()> _
  Private mExecuteResultCache As Dictionary(Of String, Boolean)
  <NotUndoable()> _
  <NonSerialized()> _
  Private mLastPrincipal As System.Security.Principal.IPrincipal

  <NotUndoable()> _
  <NonSerialized()> _
  Private mAuthorizationRules As Security.AuthorizationRules

  ''' <summary>
  ''' Override this method to add authorization
  ''' rules for your object's properties.
  ''' </summary>
  Protected Overridable Sub AddInstanceAuthorizationRules()

  End Sub

  ''' <summary>
  ''' Override this method to add per-type
  ''' authorization rules for your type's properties.
  ''' </summary>
  ''' <remarks>
  ''' AddSharedAuthorizationRules is automatically called by CSLA .NET
  ''' when your object should associate per-type authorization roles
  ''' with its properties.
  ''' </remarks>
  Protected Overridable Sub AddAuthorizationRules()

  End Sub

  ''' <summary>
  ''' Provides access to the AuthorizationRules object for this
  ''' object.
  ''' </summary>
  ''' <remarks>
  ''' Use this object to add a list of allowed and denied roles for
  ''' reading and writing properties of the object. Typically these
  ''' values are added once when the business object is instantiated.
  ''' </remarks>
  Protected ReadOnly Property AuthorizationRules() _
    As Security.AuthorizationRules
    Get
      If mAuthorizationRules Is Nothing Then
        mAuthorizationRules = New Security.AuthorizationRules(Me.GetType)
      End If
      Return mAuthorizationRules
    End Get
  End Property

  ''' <summary>
  ''' Returns <see langword="true" /> if the user is allowed to read the
  ''' calling property.
  ''' </summary>
  ''' <returns><see langword="true" /> if read is allowed.</returns>
  ''' <param name="throwOnFalse">Indicates whether a negative
  ''' result should cause an exception.</param>
  <System.Runtime.CompilerServices.MethodImpl( _
    System.Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
  Public Function CanReadProperty(ByVal throwOnFalse As Boolean) As Boolean

    Dim propertyName As String = _
      New System.Diagnostics.StackTrace().GetFrame(1).GetMethod.Name.Substring(4)
    Dim result As Boolean = CanReadProperty(propertyName)
    If throwOnFalse AndAlso result = False Then
      Dim ex As New System.Security.SecurityException( _
        String.Format("{0} ({1})", _
        My.Resources.PropertyGetNotAllowed, propertyName))
      ex.Action = System.Security.Permissions.SecurityAction.Deny
      Throw ex
    End If
    Return result

  End Function

  ''' <summary>
  ''' Returns <see langword="true" /> if the user is allowed to read the
  ''' calling property.
  ''' </summary>
  ''' <returns><see langword="true" /> if read is allowed.</returns>
  ''' <param name="propertyName">Name of the property to read.</param>
  ''' <param name="throwOnFalse">Indicates whether a negative
  ''' result should cause an exception.</param>
  Public Function CanReadProperty(ByVal propertyName As String, ByVal throwOnFalse As Boolean) As Boolean

    Dim result As Boolean = CanReadProperty(propertyName)
    If throwOnFalse AndAlso result = False Then
      Dim ex As New System.Security.SecurityException( _
        String.Format("{0} ({1})", _
        My.Resources.PropertyGetNotAllowed, propertyName))
      ex.Action = System.Security.Permissions.SecurityAction.Deny
      Throw ex
    End If
    Return result

  End Function

  ''' <summary>
  ''' Returns <see langword="true" /> if the user is allowed to read the
  ''' calling property.
  ''' </summary>
  ''' <returns><see langword="true" /> if read is allowed.</returns>
  Public Function CanReadProperty() As Boolean

    Dim propertyName As String = _
      New System.Diagnostics.StackTrace().GetFrame(1).GetMethod.Name.Substring(4)
    Return CanReadProperty(propertyName)

  End Function

  ''' <summary>
  ''' Returns <see langword="true" /> if the user is allowed to read the
  ''' specified property.
  ''' </summary>
  ''' <param name="propertyName">Name of the property to read.</param>
  ''' <returns><see langword="true" /> if read is allowed.</returns>
  ''' <remarks>
  ''' <para>
  ''' If a list of allowed roles is provided then only users in those
  ''' roles can read. If no list of allowed roles is provided then
  ''' the list of denied roles is checked.
  ''' </para><para>
  ''' If a list of denied roles is provided then users in the denied
  ''' roles are denied read access. All other users are allowed.
  ''' </para><para>
  ''' If neither a list of allowed nor denied roles is provided then
  ''' all users will have read access.
  ''' </para>
  ''' </remarks>
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Public Overridable Function CanReadProperty( _
    ByVal propertyName As String) As Boolean _
    Implements Core.IReadOnlyObject.CanReadProperty, Csla.Security.IAuthorizeReadWrite.CanReadProperty

    Dim result As Boolean = True

    VerifyAuthorizationCache()

    If mReadResultCache.ContainsKey(propertyName) Then
      ' cache contains value - get cached value
      result = mReadResultCache(propertyName)

    Else
      If AuthorizationRules.HasReadAllowedRoles(propertyName) Then
        ' some users are explicitly granted read access
        ' in which case all other users are denied
        If Not AuthorizationRules.IsReadAllowed(propertyName) Then
          result = False
        End If

      ElseIf AuthorizationRules.HasReadDeniedRoles(propertyName) Then
        ' some users are explicitly denied read access
        If AuthorizationRules.IsReadDenied(propertyName) Then
          result = False
        End If
      End If
      ' store value in cache
      mReadResultCache(propertyName) = result
    End If
    Return result

  End Function

  Private Function CanWriteProperty(ByVal propertyName As String) As Boolean _
    Implements Security.IAuthorizeReadWrite.CanWriteProperty

    Return False

  End Function

  Private Sub VerifyAuthorizationCache()

    If mReadResultCache Is Nothing Then
      mReadResultCache = New Dictionary(Of String, Boolean)
    End If
    If mExecuteResultCache Is Nothing Then
      mExecuteResultCache = New Dictionary(Of String, Boolean)
    End If
    If Not ReferenceEquals(Csla.ApplicationContext.User, mLastPrincipal) Then
      ' the principal has changed - reset the cache
      mReadResultCache.Clear()
      mExecuteResultCache.Clear()
      mLastPrincipal = Csla.ApplicationContext.User
    End If

  End Sub

  ''' <summary>
  ''' Returns <see langword="true" /> if the user is allowed to execute
  ''' the calling method.
  ''' </summary>
  ''' <returns><see langword="true" /> if execute is allowed.</returns>
  ''' <param name="throwOnFalse">Indicates whether a negative
  ''' result should cause an exception.</param>
  <System.Runtime.CompilerServices.MethodImpl( _
    System.Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
  Public Function CanExecuteMethod(ByVal throwOnFalse As Boolean) As Boolean

    Dim methodName As String = _
      New System.Diagnostics.StackTrace(). _
      GetFrame(1).GetMethod.Name
    Dim result As Boolean = CanExecuteMethod(methodName)
    If throwOnFalse AndAlso result = False Then
      Dim ex As New System.Security.SecurityException( _
        String.Format("{0} ({1})", _
        My.Resources.MethodExecuteNotAllowed, methodName))
      ex.Action = System.Security.Permissions.SecurityAction.Deny
      Throw ex
    End If
    Return result

  End Function

  ''' <summary>
  ''' Returns <see langword="true" /> if the user is allowed to execute
  ''' the specified method.
  ''' </summary>
  ''' <returns><see langword="true" /> if execute is allowed.</returns>
  ''' <param name="methodName">Name of the method to execute.</param>
  ''' <param name="throwOnFalse">Indicates whether a negative
  ''' result should cause an exception.</param>
  Public Function CanExecuteMethod(ByVal methodName As String, ByVal throwOnFalse As Boolean) As Boolean

    Dim result As Boolean = CanExecuteMethod(methodName)
    If throwOnFalse AndAlso result = False Then
      Dim ex As New System.Security.SecurityException( _
        String.Format("{0} ({1})", _
        My.Resources.MethodExecuteNotAllowed, methodName))
      ex.Action = System.Security.Permissions.SecurityAction.Deny
      Throw ex
    End If
    Return result

  End Function

  ''' <summary>
  ''' Returns <see langword="true" /> if the user is allowed to execute
  ''' the calling method.
  ''' </summary>
  ''' <returns><see langword="true" /> if execute is allowed.</returns>
  <System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
  Public Function CanExecuteMethod() As Boolean

    Dim methodName As String = _
      New System.Diagnostics.StackTrace().GetFrame(1).GetMethod.Name
    Return CanExecuteMethod(methodName)

  End Function

  ''' <summary>
  ''' Returns <see langword="true" /> if the user is allowed to execute
  ''' the specified method.
  ''' </summary>
  ''' <param name="methodName">Name of the method to execute.</param>
  ''' <returns><see langword="true" /> if execute is allowed.</returns>
  ''' <remarks>
  ''' <para>
  ''' If a list of allowed roles is provided then only users in those
  ''' roles can read. If no list of allowed roles is provided then
  ''' the list of denied roles is checked.
  ''' </para><para>
  ''' If a list of denied roles is provided then users in the denied
  ''' roles are denied read access. All other users are allowed.
  ''' </para><para>
  ''' If neither a list of allowed nor denied roles is provided then
  ''' all users will have read access.
  ''' </para>
  ''' </remarks>
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Public Overridable Function CanExecuteMethod( _
    ByVal methodName As String) As Boolean Implements Csla.Security.IAuthorizeReadWrite.CanExecuteMethod

    Dim result As Boolean = True

    VerifyAuthorizationCache()

    If mExecuteResultCache.ContainsKey(methodName) Then
      ' cache contains value - get cached value
      result = mExecuteResultCache(methodName)

    Else
      If AuthorizationRules.HasExecuteAllowedRoles(methodName) Then
        ' some users are explicitly granted read access
        ' in which case all other users are denied
        If Not AuthorizationRules.IsExecuteAllowed(methodName) Then
          result = False
        End If

      ElseIf AuthorizationRules.HasExecuteDeniedRoles(methodName) Then
        ' some users are explicitly denied read access
        If AuthorizationRules.IsExecuteDenied(methodName) Then
          result = False
        End If
      End If
      ' store value in cache
      mExecuteResultCache(methodName) = result
    End If
    Return result

  End Function

#End Region

#Region " ICloneable "

  Private Function ICloneable_Clone() As Object Implements ICloneable.Clone

    Return GetClone()

  End Function

  ''' <summary>
  ''' Creates a clone of the object.
  ''' </summary>
  ''' <returns>
  ''' A new object containing the exact data of the original object.
  ''' </returns>
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Function GetClone() As Object

    Return ObjectCloner.Clone(Me)

  End Function

  ''' <summary>
  ''' Creates a clone of the object.
  ''' </summary>
  ''' <returns>
  ''' A new object containing the exact data of the original object.
  ''' </returns>
  Public Overloads Function Clone() As T

    Return DirectCast(GetClone(), T)

  End Function

#End Region

#Region " Data Access "

  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId:="criteria")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")> _
  Private Sub DataPortal_Create(ByVal criteria As Object)
    Throw New NotSupportedException(My.Resources.CreateNotSupportedException)
  End Sub

  ''' <summary>
  ''' Override this method to allow retrieval of an existing business
  ''' object based on data in the database.
  ''' </summary>
  ''' <param name="criteria">An object containing criteria values to identify the object.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  Protected Overridable Sub DataPortal_Fetch(ByVal criteria As Object)
    Throw New NotSupportedException(My.Resources.FetchNotSupportedException)
  End Sub

  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")> _
  Private Sub DataPortal_Update()
    Throw New NotSupportedException(My.Resources.UpdateNotSupportedException)
  End Sub

  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId:="criteria")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")> _
  Private Sub DataPortal_Delete(ByVal criteria As Object)
    Throw New NotSupportedException(My.Resources.DeleteNotSupportedException)
  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal prior to calling the 
  ''' requested DataPortal_XYZ method.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub DataPortal_OnDataPortalInvoke(ByVal e As DataPortalEventArgs)

  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal after calling the 
  ''' requested DataPortal_XYZ method.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub DataPortal_OnDataPortalInvokeComplete(ByVal e As DataPortalEventArgs)

  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal if an exception
  ''' occurs during data access.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  ''' <param name="ex">The Exception thrown during data access.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub DataPortal_OnDataPortalException(ByVal e As DataPortalEventArgs, ByVal ex As Exception)

  End Sub

#End Region

#Region " Serialization Notification "

  <OnDeserialized()> _
  Private Sub OnDeserializedHandler(ByVal context As StreamingContext)

    OnDeserialized(context)
    AddInstanceAuthorizationRules()
    If Not Csla.Security.SharedAuthorizationRules.RulesExistFor(Me.GetType) Then
      SyncLock Me.GetType
        If Not Csla.Security.SharedAuthorizationRules.RulesExistFor(Me.GetType) Then
          AddAuthorizationRules()
        End If
      End SyncLock
    End If

  End Sub

  ''' <summary>
  ''' This method is called on a newly deserialized object
  ''' after deserialization is complete.
  ''' </summary>
  ''' <param name="context">Serialization context object.</param>
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub OnDeserialized( _
    ByVal context As StreamingContext)

    ' do nothing - this is here so a subclass
    ' could override if needed

  End Sub

#End Region

End Class
