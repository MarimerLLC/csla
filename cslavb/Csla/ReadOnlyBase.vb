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
  Implements Server.IDataPortalTarget

#Region " Object ID Value "

  ''' <summary>
  ''' Override this method to return a unique identifying
  ''' value for this object.
  ''' </summary>
  Protected Overridable Function GetIdValue() As Object
    Return Nothing
  End Function

#End Region

#Region " System.Object Overrides "

  ''' <summary>
  ''' Returns a text representation of this object by
  ''' returning the <see cref="GetIdValue"/> value
  ''' in text form.
  ''' </summary>
  Public Overrides Function ToString() As String

    Dim id As Object = GetIdValue()
    If id Is Nothing Then
      Return MyBase.ToString
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
    InitializeAuthorizationRules()

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

  Private Sub InitializeAuthorizationRules()

    AddInstanceAuthorizationRules()
    If Not Csla.Security.SharedAuthorizationRules.RulesExistFor(Me.GetType) Then
      SyncLock Me.GetType
        If Not Csla.Security.SharedAuthorizationRules.RulesExistFor(Me.GetType) Then
          Csla.Security.SharedAuthorizationRules.GetManager(Me.GetType, True)
          AddAuthorizationRules()
        End If
      End SyncLock
    End If

  End Sub


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
  <Obsolete("Use overload requiring explicit property name")> _
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
  <Obsolete("Use overload requiring explicit property name")> _
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

    If Not mReadResultCache.TryGetValue(propertyName, result) Then
      result = True
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
  <Obsolete("Use overload requiring explicit method name")> _
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
  <Obsolete("Use overload requiring explicit method name")> _
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

    If Not mExecuteResultCache.TryGetValue(methodName, result) Then
      result = True
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
  Protected Overridable Sub DataPortal_OnDataPortalInvoke(ByVal e As DataPortalEventArgs) Implements Server.IDataPortalTarget.DataPortal_OnDataPortalInvoke

  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal after calling the 
  ''' requested DataPortal_XYZ method.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub DataPortal_OnDataPortalInvokeComplete(ByVal e As DataPortalEventArgs) Implements Server.IDataPortalTarget.DataPortal_OnDataPortalInvokeComplete

  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal if an exception
  ''' occurs during data access.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  ''' <param name="ex">The Exception thrown during data access.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub DataPortal_OnDataPortalException(ByVal e As DataPortalEventArgs, ByVal ex As Exception) Implements Server.IDataPortalTarget.DataPortal_OnDataPortalException

  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal prior to calling the 
  ''' requested DataPortal_XYZ method.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member"), EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub Child_OnDataPortalInvoke(ByVal e As DataPortalEventArgs) Implements Server.IDataPortalTarget.Child_OnDataPortalInvoke

  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal after calling the 
  ''' requested DataPortal_XYZ method.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member"), EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub Child_OnDataPortalInvokeComplete(ByVal e As DataPortalEventArgs) Implements Server.IDataPortalTarget.Child_OnDataPortalInvokeComplete

  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal if an exception
  ''' occurs during data access.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  ''' <param name="ex">The Exception thrown during data access.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member"), EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub Child_OnDataPortalException(ByVal e As DataPortalEventArgs, ByVal ex As Exception) Implements Server.IDataPortalTarget.Child_OnDataPortalException

  End Sub

#End Region

#Region " Serialization Notification "

  <OnDeserialized()> _
  Private Sub OnDeserializedHandler(ByVal context As StreamingContext)

    OnDeserialized(context)
    FieldManager.SetPropertyList(Me.GetType)
    InitializeAuthorizationRules()

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

#Region " Register Properties "

  ''' <summary>
  ''' Indicates that the specified property belongs
  ''' to the type.
  ''' </summary>
  ''' <typeparam name="P">
  ''' Type of property.
  ''' </typeparam>
  ''' <param name="objectType">
  ''' Type of object to which the property belongs.
  ''' </param>
  ''' <param name="info">
  ''' PropertyInfo object for the property.
  ''' </param>
  ''' <returns>
  ''' The provided IPropertyInfo object.
  ''' </returns>
  Protected Shared Function RegisterProperty(Of P)(ByVal objectType As Type, ByVal info As PropertyInfo(Of P)) As PropertyInfo(Of P)

    Return Core.FieldManager.PropertyInfoManager.RegisterProperty(Of P)(objectType, info)

  End Function

#End Region

#Region " Get Properties "

  ''' <summary>
  ''' Gets a property's value, first checking authorization.
  ''' </summary>
  ''' <typeparam name="P">
  ''' Type of the property.
  ''' </typeparam>
  ''' <param name="field">
  ''' The backing field for the property.</param>
  ''' <param name="propertyName">
  ''' The name of the property.</param>
  ''' <param name="defaultValue">
  ''' Value to be returned if the user is not
  ''' authorized to read the property.</param>
  ''' <remarks>
  ''' If the user is not authorized to read the property
  ''' value, the defaultValue value is returned as a
  ''' result.
  ''' </remarks>
  Protected Function GetProperty(Of P)(ByVal propertyName As String, ByVal field As P, ByVal defaultValue As P) As P

    Return GetProperty(Of P)(propertyName, field, defaultValue, False)

  End Function

  ''' <summary>
  ''' Gets a property's value, first checking authorization.
  ''' </summary>
  ''' <typeparam name="P">
  ''' Type of the property.
  ''' </typeparam>
  ''' <param name="field">
  ''' The backing field for the property.</param>
  ''' <param name="propertyName">
  ''' The name of the property.</param>
  ''' <param name="defaultValue">
  ''' Value to be returned if the user is not
  ''' authorized to read the property.</param>
  ''' <param name="throwOnNoAccess">
  ''' True if an exception should be thrown when the
  ''' user is not authorized to read this property.</param>
  Protected Function GetProperty(Of P)(ByVal propertyName As String, ByVal field As P, ByVal defaultValue As P, ByVal throwOnNoAccess As Boolean) As P

    If CanReadProperty(propertyName, throwOnNoAccess) Then
      Return field

    Else
      Return defaultValue
    End If

  End Function

  ''' <summary>
  ''' Gets a property's value, first checking authorization.
  ''' </summary>
  ''' <typeparam name="P">
  ''' Type of the property.
  ''' </typeparam>
  ''' <param name="field">
  ''' The backing field for the property.</param>
  ''' <param name="propertyInfo">
  ''' <see cref="PropertyInfo" /> object containing property metadata.</param>
  ''' <remarks>
  ''' If the user is not authorized to read the property
  ''' value, the defaultValue value is returned as a
  ''' result.
  ''' </remarks>
  Protected Function GetProperty(Of P)(ByVal propertyInfo As PropertyInfo(Of P), ByVal field As P) As P

    Return GetProperty(Of P)(propertyInfo.Name, field, propertyInfo.DefaultValue, False)

  End Function

  ''' <summary>
  ''' Gets a property's value as 
  ''' a specified type, first checking authorization.
  ''' </summary>
  ''' <typeparam name="F">
  ''' Type of the field.
  ''' </typeparam>
  ''' <typeparam name="P">
  ''' Type of the property.
  ''' </typeparam>
  ''' <param name="field">
  ''' The backing field for the property.</param>
  ''' <param name="propertyInfo">
  ''' <see cref="PropertyInfo" /> object containing property metadata.</param>
  ''' <remarks>
  ''' If the user is not authorized to read the property
  ''' value, the defaultValue value is returned as a
  ''' result.
  ''' </remarks>
  Protected Function GetProperty(Of F, P)(ByVal propertyInfo As PropertyInfo(Of F), ByVal field As F) As P

    Return CoerceValue(Of P)(GetType(F), Nothing, GetProperty(Of F)(propertyInfo.Name, field, propertyInfo.DefaultValue, False))

  End Function

  ''' <summary>
  ''' Gets a property's value as a specified type, 
  ''' first checking authorization.
  ''' </summary>
  ''' <typeparam name="F">
  ''' Type of the field.
  ''' </typeparam>
  ''' <typeparam name="P">
  ''' Type of the property.
  ''' </typeparam>
  ''' <param name="field">
  ''' The backing field for the property.</param>
  ''' <param name="propertyInfo">
  ''' <see cref="PropertyInfo" /> object containing property metadata.</param>
  ''' <param name="throwOnNoAccess">
  ''' True if an exception should be thrown when the
  ''' user is not authorized to read this property.</param>
  ''' <remarks>
  ''' If the user is not authorized to read the property
  ''' value, the defaultValue value is returned as a
  ''' result.
  ''' </remarks>
  Protected Function GetProperty(Of F, P)( _
    ByVal propertyInfo As PropertyInfo(Of F), ByVal field As F, ByVal throwOnNoAccess As Boolean) As P

    Return CoerceValue(Of P)(GetType(F), Nothing, GetProperty(Of F)(propertyInfo.Name, field, propertyInfo.DefaultValue, throwOnNoAccess))

  End Function

  ''' <summary>
  ''' Gets a property's managed field value, 
  ''' first checking authorization.
  ''' </summary>
  ''' <typeparam name="P">
  ''' Type of the property.
  ''' </typeparam>
  ''' <param name="propertyInfo">
  ''' <see cref="PropertyInfo" /> object containing property metadata.</param>
  ''' <remarks>
  ''' If the user is not authorized to read the property
  ''' value, the defaultValue value is returned as a
  ''' result.
  ''' </remarks>
  Protected Function GetProperty(Of P)( _
    ByVal propertyInfo As PropertyInfo(Of P)) As P

    Return GetProperty(Of P)(propertyInfo, False)

  End Function

  ''' <summary>
  ''' Gets a property's value from the list of 
  ''' managed field values, first checking authorization,
  ''' and converting the value to an appropriate type.
  ''' </summary>
  ''' <typeparam name="F">
  ''' Type of the field.
  ''' </typeparam>
  ''' <typeparam name="P">
  ''' Type of the property.
  ''' </typeparam>
  ''' <param name="propertyInfo">
  ''' <see cref="PropertyInfo" /> object containing property metadata.</param>
  ''' <remarks>
  ''' If the user is not authorized to read the property
  ''' value, the defaultValue value is returned as a
  ''' result.
  ''' </remarks>
  Protected Function GetProperty(Of F, P)( _
    ByVal propertyInfo As PropertyInfo(Of F)) As P

    Return CoerceValue(Of P)(GetType(F), Nothing, GetProperty(Of F)(propertyInfo, False))

  End Function

  ''' <summary>
  ''' Gets a property's value from the list of 
  ''' managed field values, first checking authorization,
  ''' and converting the value to an appropriate type.
  ''' </summary>
  ''' <typeparam name="F">
  ''' Type of the field.
  ''' </typeparam>
  ''' <typeparam name="P">
  ''' Type of the property.
  ''' </typeparam>
  ''' <param name="propertyInfo">
  ''' <see cref="PropertyInfo" /> object containing property metadata.</param>
  ''' <param name="throwOnNoAccess">
  ''' True if an exception should be thrown when the
  ''' user is not authorized to read this property.</param>
  ''' <remarks>
  ''' If the user is not authorized to read the property
  ''' value, the defaultValue value is returned as a
  ''' result.
  ''' </remarks>
  Protected Function GetProperty(Of F, P)( _
    ByVal propertyInfo As PropertyInfo(Of F), ByVal throwOnNoAccess As Boolean) As P

    Return CoerceValue(Of P)(GetType(F), Nothing, GetProperty(Of F)(propertyInfo, throwOnNoAccess))

  End Function

  ''' <summary>
  ''' Gets a property's value as a specified type, 
  ''' first checking authorization.
  ''' </summary>
  ''' <typeparam name="P">
  ''' Type of the property.
  ''' </typeparam>
  ''' <param name="propertyInfo">
  ''' <see cref="PropertyInfo" /> object containing property metadata.</param>
  ''' <param name="throwOnNoAccess">
  ''' True if an exception should be thrown when the
  ''' user is not authorized to read this property.</param>
  ''' <remarks>
  ''' If the user is not authorized to read the property
  ''' value, the defaultValue value is returned as a
  ''' result.
  ''' </remarks>
  Protected Function GetProperty(Of P)( _
    ByVal propertyInfo As PropertyInfo(Of P), ByVal throwOnNoAccess As Boolean) As P

    Dim result As P = Nothing
    If CanReadProperty(propertyInfo.Name, throwOnNoAccess) Then
      result = ReadProperty(Of P)(propertyInfo)

    Else
      result = propertyInfo.DefaultValue
    End If
    Return result

  End Function

#End Region

#Region " Read Properties"

  ''' <summary>
  ''' Gets a property's value from the list of 
  ''' managed field values, converting the 
  ''' value to an appropriate type.
  ''' </summary>
  ''' <typeparam name="F">
  ''' Type of the field.
  ''' </typeparam>
  ''' <typeparam name="P">
  ''' Type of the property.
  ''' </typeparam>
  ''' <param name="propertyInfo">
  ''' <see cref="PropertyInfo" /> object containing property metadata.</param>
  Protected Function ReadProperty(Of F, P)(ByVal propertyInfo As PropertyInfo(Of F)) As P

    Return Utilities.CoerceValue(Of P)(GetType(F), Nothing, ReadProperty(Of F)(propertyInfo))

  End Function

  ''' <summary>
  ''' Gets a property's value as a specified type.
  ''' </summary>
  ''' <typeparam name="P">
  ''' Type of the property.
  ''' </typeparam>
  ''' <param name="propertyInfo">
  ''' <see cref="PropertyInfo" /> object containing property metadata.</param>
  Protected Function ReadProperty(Of P)(ByVal propertyInfo As PropertyInfo(Of P)) As P

    Dim result As P = Nothing
    Dim data As FieldManager.IFieldData = FieldManager.GetFieldData(propertyInfo)
    If data IsNot Nothing Then
      Dim fd As FieldManager.IFieldData(Of P) = TryCast(data, FieldManager.IFieldData(Of P))
      If fd IsNot Nothing Then
        result = fd.Value

      Else
        result = CType(data.Value, P)
      End If

    Else
      result = propertyInfo.DefaultValue
      FieldManager.LoadFieldData(Of P)(propertyInfo, result)
    End If
    Return result

  End Function

#End Region

#Region " Load Properties "

  ''' <summary>
  ''' Loads a property's managed field with the 
  ''' supplied value calling PropertyHasChanged 
  ''' if the value does change.
  ''' </summary>
  ''' <param name="propertyInfo">
  ''' <see cref="PropertyInfo" /> object containing property metadata.</param>
  ''' <param name="newValue">
  ''' The new value for the property.</param>
  ''' <remarks>
  ''' No authorization checks occur when this method is called,
  ''' and no PropertyChanging or PropertyChanged events are raised.
  ''' Loading values does not cause validation rules to be
  ''' invoked.
  ''' </remarks>
  Protected Sub LoadProperty(Of P, F)( _
    ByVal propertyInfo As PropertyInfo(Of P), ByVal newValue As F)

    Try
      Dim oldValue As P = Nothing
      Dim fieldData = FieldManager.GetFieldData(propertyInfo)
      If fieldData Is Nothing Then
        oldValue = propertyInfo.DefaultValue
        fieldData = FieldManager.LoadFieldData(propertyInfo, oldValue)

      Else
        oldValue = DirectCast(fieldData.Value, P)
      End If

      If oldValue Is Nothing Then
        If Not newValue Is Nothing Then
          FieldManager.LoadFieldData(propertyInfo, CoerceValue(Of P)(GetType(F), oldValue, newValue))
        End If

      ElseIf Not oldValue.Equals(newValue) Then
        FieldManager.LoadFieldData(propertyInfo, CoerceValue(Of P)(GetType(F), oldValue, newValue))
      End If

    Catch ex As Exception
      Throw New PropertyLoadException(String.Format(My.Resources.PropertyLoadException, propertyInfo.Name, ex.Message))
    End Try

  End Sub

  ''' <summary>
  ''' Loads a property's managed field with the 
  ''' supplied value calling PropertyHasChanged 
  ''' if the value does change.
  ''' </summary>
  ''' <typeparam name="P">
  ''' Type of the property.
  ''' </typeparam>
  ''' <param name="propertyInfo">
  ''' <see cref="PropertyInfo" /> object containing property metadata.</param>
  ''' <param name="newValue">
  ''' The new value for the property.</param>
  ''' <remarks>
  ''' No authorization checks occur when this method is called,
  ''' and no PropertyChanging or PropertyChanged events are raised.
  ''' Loading values does not cause validation rules to be
  ''' invoked.
  ''' </remarks>
  Protected Sub LoadProperty(Of P)( _
    ByVal propertyInfo As PropertyInfo(Of P), ByVal newValue As P)

    Try
      Dim oldValue As P = Nothing
      Dim fieldData = FieldManager.GetFieldData(propertyInfo)
      If fieldData Is Nothing Then
        oldValue = propertyInfo.DefaultValue
        fieldData = FieldManager.LoadFieldData(propertyInfo, oldValue)

      Else
        oldValue = DirectCast(fieldData.Value, P)
      End If

      If oldValue Is Nothing Then
        If Not newValue Is Nothing Then
          FieldManager.LoadFieldData(propertyInfo, newValue)
        End If

      ElseIf Not oldValue.Equals(newValue) Then
        FieldManager.LoadFieldData(propertyInfo, newValue)
      End If

    Catch ex As Exception
      Throw New PropertyLoadException(String.Format(My.Resources.PropertyLoadException, propertyInfo.Name, ex.Message))
    End Try

  End Sub

#End Region

#Region " Field Manager "

  <NotUndoable()> _
  Private mFieldManager As FieldManager.FieldDataManager

  ''' <summary>
  ''' Gets the PropertyManager object for this
  ''' business object.
  ''' </summary>
  Protected ReadOnly Property FieldManager() As FieldManager.FieldDataManager
    Get
      If mFieldManager Is Nothing Then
        mFieldManager = New FieldManager.FieldDataManager(Me.GetType)
      End If
      Return mFieldManager
    End Get
  End Property

#End Region

#Region " IDataPortalTarget implementation "

  Private Sub MarkAsChild() Implements Server.IDataPortalTarget.MarkAsChild

  End Sub

  Private Sub MarkNew() Implements Server.IDataPortalTarget.MarkNew

  End Sub

  Private Sub MarkOld() Implements Server.IDataPortalTarget.MarkOld

  End Sub

#End Region

End Class
