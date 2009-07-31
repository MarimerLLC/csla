Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Runtime.Serialization
Imports Csla.Core
Imports Csla.Core.FieldManager
Imports Csla.Core.LoadManager
Imports Csla.Reflection
Imports Csla.Server
Imports Csla.Security

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
  Inherits BindableBase

  Implements ICloneable
  Implements Core.IReadOnlyObject
  Implements Csla.Security.IAuthorizeReadWrite
  Implements Server.IDataPortalTarget
  Implements Core.IManageProperties
  Implements INotifyBusy


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
  Private _readResultCache As Dictionary(Of String, Boolean)
  <NotUndoable()> _
  <NonSerialized()> _
  Private _executeResultCache As Dictionary(Of String, Boolean)
  <NotUndoable()> _
  <NonSerialized()> _
  Private _lastPrincipal As System.Security.Principal.IPrincipal

  <NotUndoable()> _
  <NonSerialized()> _
  Private _authorizationRules As Security.AuthorizationRules

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
      If _authorizationRules Is Nothing Then
        _authorizationRules = New Security.AuthorizationRules(Me.GetType)
      End If
      Return _authorizationRules
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

    If Not _readResultCache.TryGetValue(propertyName, result) Then
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
      _readResultCache(propertyName) = result
    End If
    Return result

  End Function

  Private Function CanWriteProperty(ByVal propertyName As String) As Boolean _
    Implements Security.IAuthorizeReadWrite.CanWriteProperty

    Return False

  End Function

  Private Sub VerifyAuthorizationCache()

    If _readResultCache Is Nothing Then
      _readResultCache = New Dictionary(Of String, Boolean)
    End If
    If _executeResultCache Is Nothing Then
      _executeResultCache = New Dictionary(Of String, Boolean)
    End If
    If Not ReferenceEquals(Csla.ApplicationContext.User, _lastPrincipal) Then
      ' the principal has changed - reset the cache
      _readResultCache.Clear()
      _executeResultCache.Clear()
      _lastPrincipal = Csla.ApplicationContext.User
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

    Dim methodName As String = New System.Diagnostics.StackTrace().GetFrame(1).GetMethod.Name
    Dim result As Boolean = CanExecuteMethod(methodName)
    If throwOnFalse AndAlso result = False Then
      Dim ex As New System.Security.SecurityException(String.Format("{0} ({1})", My.Resources.MethodExecuteNotAllowed, methodName))
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
      Dim ex As New System.Security.SecurityException(String.Format("{0} ({1})", My.Resources.MethodExecuteNotAllowed, methodName))
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

    If Not _executeResultCache.TryGetValue(methodName, result) Then
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
      _executeResultCache(methodName) = result
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
  ''' <returns>A new object containing the exact data of the original object.</returns>
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

  ''' <summary>
  ''' Called by the server-side DataPortal prior to calling the 
  ''' requested DataPortal_XYZ method.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member"), EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub Child_OnDataPortalInvoke(ByVal e As DataPortalEventArgs)

  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal after calling the 
  ''' requested DataPortal_XYZ method.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member"), EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub Child_OnDataPortalInvokeComplete(ByVal e As DataPortalEventArgs)

  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal if an exception
  ''' occurs during data access.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  ''' <param name="ex">The Exception thrown during data access.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member"), EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub Child_OnDataPortalException(ByVal e As DataPortalEventArgs, ByVal ex As Exception)

  End Sub

#End Region

#Region " Serialization Notification "

  <OnDeserialized()> _
  Private Sub OnDeserializedHandler(ByVal context As StreamingContext)

    FieldManager.SetPropertyList(Me.GetType)
    InitializeAuthorizationRules()
    OnDeserialized(context)

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
  Protected Overloads Shared Function RegisterProperty(Of P)(ByVal objectType As Type, ByVal info As PropertyInfo(Of P)) As PropertyInfo(Of P)

    Return Core.FieldManager.PropertyInfoManager.RegisterProperty(Of P)(objectType, info)

  End Function

  ''' <summary>
  ''' Indicates that the specified property belongs
  ''' to the business object type.
  ''' </summary>
  ''' <typeparam name="P">
  ''' Type of property.
  ''' </typeparam>
  ''' <param name="info">
  ''' PropertyInfo object for the property.
  ''' </param>
  ''' <returns>
  ''' The provided IPropertyInfo object.
  ''' </returns>
  Protected Overloads Shared Function RegisterProperty(Of P)(ByVal info As PropertyInfo(Of P)) As PropertyInfo(Of P)

    Return Core.FieldManager.PropertyInfoManager.RegisterProperty(Of P)(GetType(T), info)

  End Function

  ''' <summary>
  ''' Indicates that the specified property belongs
  ''' to the business object type.
  ''' </summary>
  ''' <typeparam name="P">Type of property</typeparam>
  ''' <param name="propertyLambdaExpression">Property Expression</param>
  ''' <returns></returns>
  Protected Overloads Shared Function RegisterProperty(Of P)(ByVal propertyLambdaExpression As Expression(Of Func(Of T, Object))) As PropertyInfo(Of P)
    Dim reflectedPropertyInfo As PropertyInfo = Reflect(Of T).GetProperty(propertyLambdaExpression)
    Return RegisterProperty(New PropertyInfo(Of P)(reflectedPropertyInfo.Name))
  End Function

  ''' <summary>
  ''' Indicates that the specified property belongs
  ''' to the business object type.
  ''' </summary>
  ''' <typeparam name="P">Type of property</typeparam>
  ''' <param name="propertyLambdaExpression">Property Expression</param>
  ''' <param name="friendlyName">Friendly description for a property to be used in databinding</param>
  ''' <returns></returns>
  Protected Overloads Shared Function RegisterProperty(Of P)(ByVal propertyLambdaExpression As Expression(Of Func(Of T, P)), ByVal friendlyName As String) As PropertyInfo(Of P)
    Dim reflectedPropertyInfo As PropertyInfo = Reflect(Of T).GetProperty(propertyLambdaExpression)
    Return RegisterProperty(New PropertyInfo(Of P)(reflectedPropertyInfo.Name, friendlyName))
  End Function

  ''' <summary>
  ''' Indicates that the specified property belongs
  ''' to the business object type.
  ''' </summary>
  ''' <typeparam name="P">Type of property</typeparam>
  ''' <param name="propertyLambdaExpression">Property Expression</param>
  ''' <param name="friendlyName">Friendly description for a property to be used in databinding</param>
  ''' <param name="defaultValue">Default Value for the property</param>
  ''' <returns></returns>
  Protected Overloads Shared Function RegisterProperty(Of P)(ByVal propertyLambdaExpression As Expression(Of Func(Of T, Object)), ByVal friendlyName As String, ByVal defaultValue As P) As PropertyInfo(Of P)
    Dim reflectedPropertyInfo As PropertyInfo = Reflect(Of T).GetProperty(propertyLambdaExpression)
    Return RegisterProperty(New PropertyInfo(Of P)(reflectedPropertyInfo.Name, friendlyName, defaultValue))
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

    Return GetProperty(Of P)(propertyName, field, defaultValue, Security.NoAccessBehavior.SuppressException)

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
  ''' <param name="noAccess">
  ''' True if an exception should be thrown when the
  ''' user is not authorized to read this property.</param>
  Protected Function GetProperty(Of P)(ByVal propertyName As String, ByVal field As P, ByVal defaultValue As P, ByVal noAccess As Security.NoAccessBehavior) As P

    If CanReadProperty(propertyName, noAccess = Security.NoAccessBehavior.ThrowException) Then
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

    Return GetProperty(Of P)(propertyInfo.Name, field, propertyInfo.DefaultValue, Security.NoAccessBehavior.SuppressException)

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
  ''' <param name="defaultValue">
  ''' Value to be returned if the user is not
  ''' authorized to read the property.</param>
  ''' <param name="noAccess">
  ''' True if an exception should be thrown when the
  ''' user is not authorized to read this property.</param>
  Protected Function GetProperty(Of P)(ByVal propertyInfo As PropertyInfo(Of P), ByVal field As P, ByVal defaultValue As P, ByVal noAccess As Security.NoAccessBehavior) As P

    Return GetProperty(Of P)(propertyInfo.Name, field, defaultValue, noAccess)

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
  Protected Function GetPropertyConvert(Of F, P)(ByVal propertyInfo As PropertyInfo(Of F), ByVal field As F) As P

    Return CoerceValue(Of P)(GetType(F), Nothing, GetProperty(Of F)(propertyInfo.Name, field, propertyInfo.DefaultValue, Security.NoAccessBehavior.SuppressException))

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
  ''' <param name="noAccess">
  ''' True if an exception should be thrown when the
  ''' user is not authorized to read this property.</param>
  ''' <remarks>
  ''' If the user is not authorized to read the property
  ''' value, the defaultValue value is returned as a
  ''' result.
  ''' </remarks>
  Protected Function GetPropertyConvert(Of F, P)( _
    ByVal propertyInfo As PropertyInfo(Of F), ByVal field As F, ByVal noAccess As Security.NoAccessBehavior) As P

    Return CoerceValue(Of P)(GetType(F), Nothing, GetProperty(Of F)(propertyInfo.Name, field, propertyInfo.DefaultValue, noAccess))

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

    Return GetProperty(Of P)(propertyInfo, Security.NoAccessBehavior.SuppressException)

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
  Protected Function GetPropertyConvert(Of F, P)(ByVal propertyInfo As PropertyInfo(Of F)) As P

    Return CoerceValue(Of P)(GetType(F), Nothing, GetProperty(Of F)(propertyInfo, Security.NoAccessBehavior.SuppressException))

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
  ''' <param name="noAccess">
  ''' True if an exception should be thrown when the
  ''' user is not authorized to read this property.</param>
  ''' <remarks>
  ''' If the user is not authorized to read the property
  ''' value, the defaultValue value is returned as a
  ''' result.
  ''' </remarks>
  Protected Function GetPropertyConvert(Of F, P)(ByVal propertyInfo As PropertyInfo(Of F), ByVal noAccess As Security.NoAccessBehavior) As P

    Return CoerceValue(Of P)(GetType(F), Nothing, GetProperty(Of F)(propertyInfo, noAccess))

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
  ''' <param name="noAccess">
  ''' True if an exception should be thrown when the
  ''' user is not authorized to read this property.</param>
  ''' <remarks>
  ''' If the user is not authorized to read the property
  ''' value, the defaultValue value is returned as a
  ''' result.
  ''' </remarks>
  Protected Function GetProperty(Of P)( _
    ByVal propertyInfo As PropertyInfo(Of P), ByVal noAccess As Security.NoAccessBehavior) As P

    Dim result As P = Nothing
    If CanReadProperty(propertyInfo.Name, noAccess = Security.NoAccessBehavior.ThrowException) Then
      result = ReadProperty(Of P)(propertyInfo)

    Else
      result = propertyInfo.DefaultValue
    End If
    Return result

  End Function

  ''' <summary>
  ''' Gets a property's value as a specified type.
  ''' </summary>
  ''' <param name="propertyInfo">
  ''' PropertyInfo object containing property metadata.</param>
  ''' <remarks>
  ''' If the user is not authorized to read the property
  ''' value, the defaultValue value is returned as a
  ''' result.
  ''' </remarks>
  Protected Function GetProperty(ByVal propertyInfo As IPropertyInfo) As Object
    Dim result As Object = Nothing
    If CanReadProperty(propertyInfo.Name, False) Then
      Dim info = FieldManager.GetFieldData(propertyInfo)
      If info IsNot Nothing Then
        result = info.Value
      End If
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
  Protected Function ReadPropertyConvert(Of F, P)(ByVal propertyInfo As PropertyInfo(Of F)) As P

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

  ''' <summary>
  ''' Gets a property's value as a specified type.
  ''' </summary>
  ''' <param name="propertyInfo">
  ''' PropertyInfo object containing property metadata.</param>
  Protected Function ReadProperty(ByVal propertyInfo As IPropertyInfo) As Object
    Dim info = FieldManager.GetFieldData(propertyInfo)
    If info IsNot Nothing Then
      Return info.Value
    Else
      Return Nothing
    End If
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
  Protected Sub LoadPropertyConvert(Of P, F)(ByVal propertyInfo As PropertyInfo(Of P), ByVal newValue As F)

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

  Private Sub IManageProperties_LoadProperty(Of P)(ByVal propertyInfo As PropertyInfo(Of P), ByVal newValue As P) Implements IManageProperties.LoadProperty
    LoadProperty(Of P)(propertyInfo, newValue)
  End Sub

  Private Function FieldExists(ByVal [property] As IPropertyInfo) As Boolean Implements IManageProperties.FieldExists
    Return FieldManager.FieldExists([property])
  End Function

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

  ''' <summary>
  ''' Loads a property's managed field with the 
  ''' supplied value calling PropertyHasChanged 
  ''' if the value does change.
  ''' </summary>
  ''' <param name="propertyInfo">
  ''' PropertyInfo object containing property metadata.</param>
  ''' <param name="newValue">
  ''' The new value for the property.</param>
  ''' <remarks>
  ''' No authorization checks occur when this method is called,
  ''' and no PropertyChanging or PropertyChanged events are raised.
  ''' Loading values does not cause validation rules to be
  ''' invoked.
  ''' </remarks>
  Protected Sub LoadProperty(ByVal propertyInfo As IPropertyInfo, ByVal newValue As Object)
    FieldManager.LoadFieldData(propertyInfo, newValue)
  End Sub

  'Private Function IManageProperties_FieldExists(ByVal propertyInfo As Core.IPropertyInfo) As Boolean Implements Core.IManageProperties.FieldExists
  '  Return FieldManager.FieldExists(propertyInfo)
  'End Function

  'private AsyncLoadManager
  <NonSerialized()> _
  Private _loadManager As AsyncLoadManager
  Friend ReadOnly Property LoadManager() As AsyncLoadManager
    Get
      If _loadManager Is Nothing Then
        _loadManager = New AsyncLoadManager()
        AddHandler _loadManager.BusyChanged, AddressOf loadManager_BusyChanged
        AddHandler _loadManager.UnhandledAsyncException, AddressOf loadManager_UnhandledAsyncException
      End If
      Return _loadManager
    End Get
  End Property

  Private Sub loadManager_UnhandledAsyncException(ByVal sender As Object, ByVal e As ErrorEventArgs)
    OnUnhandledAsyncException(e)
  End Sub

  Private Sub loadManager_BusyChanged(ByVal sender As Object, ByVal e As BusyChangedEventArgs)
    OnBusyChanged(e)
  End Sub

  ''' <summary>
  ''' Loads a property value asynchronously.
  ''' </summary>
  ''' <typeparam name="R">Type of the property</typeparam>
  ''' <typeparam name="P">Type of the parameter.</typeparam>
  ''' <param name="property">Property to load.</param>
  ''' <param name="factory">AsyncFactory delegate.</param>
  ''' <param name="parameter">Parameter value.</param>
  <EditorBrowsable(EditorBrowsableState.Never)> _
  Protected Sub LoadPropertyAsync(Of R, P)(ByVal [property] As PropertyInfo(Of R), ByVal factory As AsyncFactoryDelegate(Of R, P), ByVal parameter As P)
    Dim actionLoadProperty As Action(Of IPropertyInfo, Object) = AddressOf LoadProperty
    Dim actionOnPropertyChanged As Action(Of String) = AddressOf OnPropertyChanged
    Dim loader As AsyncLoader = New AsyncLoader([property], factory, actionLoadProperty, actionOnPropertyChanged, parameter)
    Dim actionLoadComplete As Action(Of Object, DataPortalResult(Of R)) = AddressOf loader.LoadComplete(Of R)
    LoadManager.BeginLoad(loader, actionLoadComplete)
    'TODO: No idea if this is a good fix or not but I think it works. If this is a good fix then it needs to be used in other methods like this one.
  End Sub

  ''' <summary>
  ''' Loads a property value asynchronously.
  ''' </summary>
  ''' <typeparam name="R">Type of the property</typeparam>
  ''' <typeparam name="P1">Type of the parameter.</typeparam>
  ''' <typeparam name="P2">Type of the parameter.</typeparam>
  ''' <param name="property">Property to load.</param>
  ''' <param name="factory">AsyncFactory delegate.</param>
  ''' <param name="pp1">Parameter value.</param>
  ''' <param name="pp2">Parameter value.</param>
  <EditorBrowsable(EditorBrowsableState.Never)> _
  Protected Sub LoadPropertyAsync(Of R, P1, P2)(ByVal [property] As PropertyInfo(Of R), ByVal factory As AsyncFactoryDelegate(Of R, P1, P2), ByVal pp1 As P1, ByVal pp2 As P2)
    Dim actionLoadProperty As Action(Of IPropertyInfo, Object) = AddressOf LoadProperty
    Dim actionOnPropertyChanged As Action(Of String) = AddressOf OnPropertyChanged
    Dim loader As AsyncLoader = New AsyncLoader([property], factory, actionLoadProperty, actionOnPropertyChanged, pp1, pp2)
    Dim actionLoadComplete As Action(Of Object, DataPortalResult(Of R)) = AddressOf loader.LoadComplete(Of R)
    LoadManager.BeginLoad(loader, actionLoadComplete)
  End Sub

  ''' <summary>
  ''' Loads a property value asynchronously.
  ''' </summary>
  ''' <typeparam name="R">Type of the property</typeparam>
  ''' <typeparam name="P1">Type of the parameter.</typeparam>
  ''' <typeparam name="P2">Type of the parameter.</typeparam>
  ''' <typeparam name="P3">Type of the parameter.</typeparam>
  ''' <param name="property">Property to load.</param>
  ''' <param name="factory">AsyncFactory delegate.</param>
  ''' <param name="pp1">Parameter value.</param>
  ''' <param name="pp2">Parameter value.</param>
  ''' <param name="pp3">Parameter value.</param>
  <EditorBrowsable(EditorBrowsableState.Never)> _
  Protected Sub LoadPropertyAsync(Of R, P1, P2, P3)(ByVal [property] As PropertyInfo(Of R), ByVal factory As AsyncFactoryDelegate(Of R, P1, P2, P3), ByVal pp1 As P1, ByVal pp2 As P2, ByVal pp3 As P3)
    Dim actionLoadProperty As Action(Of IPropertyInfo, Object) = AddressOf LoadProperty
    Dim actionOnPropertyChanged As Action(Of String) = AddressOf OnPropertyChanged
    Dim loader As AsyncLoader = New AsyncLoader([property], factory, actionLoadProperty, actionOnPropertyChanged, pp1, pp2, pp3)
    Dim actionLoadComplete As Action(Of Object, DataPortalResult(Of R)) = AddressOf loader.LoadComplete(Of R)
    LoadManager.BeginLoad(loader, actionLoadComplete)
  End Sub

  ''' <summary>
  ''' Loads a property value asynchronously.
  ''' </summary>
  ''' <typeparam name="R">Type of the property</typeparam>
  ''' <typeparam name="P1">Type of the parameter.</typeparam>
  ''' <typeparam name="P2">Type of the parameter.</typeparam>
  ''' <typeparam name="P3">Type of the parameter.</typeparam>
  ''' <typeparam name="P4">Type of the parameter.</typeparam>
  ''' <param name="property">Property to load.</param>
  ''' <param name="factory">AsyncFactory delegate.</param>
  ''' <param name="pp1">Parameter value.</param>
  ''' <param name="pp2">Parameter value.</param>
  ''' <param name="pp3">Parameter value.</param>
  ''' <param name="pp4">Parameter value.</param>
  <EditorBrowsable(EditorBrowsableState.Never)> _
  Protected Sub LoadPropertyAsync(Of R, P1, P2, P3, P4)(ByVal [property] As PropertyInfo(Of R), ByVal factory As AsyncFactoryDelegate(Of R, P1, P2, P3, P4), ByVal pp1 As P1, ByVal pp2 As P2, ByVal pp3 As P3, ByVal pp4 As P4)
    Dim actionLoadProperty As Action(Of IPropertyInfo, Object) = AddressOf LoadProperty
    Dim actionOnPropertyChanged As Action(Of String) = AddressOf OnPropertyChanged
    Dim loader As AsyncLoader = New AsyncLoader([property], factory, actionLoadProperty, actionOnPropertyChanged, pp1, pp2, pp3, pp4)
    Dim actionLoadComplete As Action(Of Object, DataPortalResult(Of R)) = AddressOf loader.LoadComplete(Of R)
    LoadManager.BeginLoad(loader, actionLoadComplete)
  End Sub

  ''' <summary>
  ''' Loads a property value asynchronously.
  ''' </summary>
  ''' <typeparam name="R">Type of the property</typeparam>
  ''' <typeparam name="P1">Type of the parameter.</typeparam>
  ''' <typeparam name="P2">Type of the parameter.</typeparam>
  ''' <typeparam name="P3">Type of the parameter.</typeparam>
  ''' <typeparam name="P4">Type of the parameter.</typeparam>
  ''' <typeparam name="P5">Type of the parameter.</typeparam>
  ''' <param name="property">Property to load.</param>
  ''' <param name="factory">AsyncFactory delegate.</param>
  ''' <param name="pp1">Parameter value.</param>
  ''' <param name="pp2">Parameter value.</param>
  ''' <param name="pp3">Parameter value.</param>
  ''' <param name="pp4">Parameter value.</param>
  ''' <param name="pp5">Parameter value.</param>
  <EditorBrowsable(EditorBrowsableState.Never)> _
  Protected Sub LoadPropertyAsync(Of R, P1, P2, P3, P4, P5)(ByVal [property] As PropertyInfo(Of R), ByVal factory As AsyncFactoryDelegate(Of R, P1, P2, P3, P4, P5), ByVal pp1 As P1, ByVal pp2 As P2, ByVal pp3 As P3, ByVal pp4 As P4, ByVal pp5 As P5)
    Dim actionLoadProperty As Action(Of IPropertyInfo, Object) = AddressOf LoadProperty
    Dim actionOnPropertyChanged As Action(Of String) = AddressOf OnPropertyChanged
    Dim loader As AsyncLoader = New AsyncLoader([property], factory, actionLoadProperty, actionOnPropertyChanged, pp1, pp2, pp3, pp4, pp5)
    Dim actionLoadComplete As Action(Of Object, DataPortalResult(Of R)) = AddressOf loader.LoadComplete(Of R)
    LoadManager.BeginLoad(loader, actionLoadComplete)
  End Sub

#End Region

#Region " Field Manager "

  <NotUndoable()> _
  Private _fieldManager As FieldManager.FieldDataManager

  ''' <summary>
  ''' Gets the PropertyManager object for this
  ''' business object.
  ''' </summary>
  Protected ReadOnly Property FieldManager() As FieldManager.FieldDataManager
    Get
      If _fieldManager Is Nothing Then
        _fieldManager = New FieldManager.FieldDataManager(Me.GetType)
      End If
      Return _fieldManager
    End Get
  End Property

#End Region

#Region " IsBusy / IsIdle "

  <NonSerialized()> _
  <NotUndoable()> _
  Private _isBusy As Boolean

  ''' <summary>
  ''' Marks the object as being busy (it is
  ''' running an async operation).
  ''' </summary>
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Sub MarkBusy()
    If _isBusy Then
      Throw New InvalidOperationException(My.Resources.BusyObjectsMayNotBeMarkedBusy)
    End If

    _isBusy = True
    OnBusyChanged(New BusyChangedEventArgs("", True))
  End Sub

  ''' <summary>
  ''' Marks the object as being not busy
  ''' (it is not running an async operation).
  ''' </summary>
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Sub MarkIdle()
    _isBusy = False
    OnBusyChanged(New BusyChangedEventArgs("", True))
  End Sub

  ''' <summary>
  ''' Gets a value indicating whether this
  ''' object or any of its child objects are
  ''' running an async operation.
  ''' </summary>
  <Browsable(False)> _
  Public ReadOnly Property IsBusy() As Boolean Implements Core.INotifyBusy.IsBusy
    Get
      Return IsSelfBusy Or (_fieldManager IsNot Nothing And FieldManager.IsBusy())
    End Get
  End Property

  ''' <summary>
  ''' Gets a value indicating whether this
  ''' object is
  ''' running an async operation.
  ''' </summary>
  <Browsable(False)> _
  Public ReadOnly Property IsSelfBusy() As Boolean Implements Core.INotifyBusy.IsSelfBusy
    Get
      Return _isBusy Or LoadManager.IsLoading
    End Get
  End Property

  Private Sub Child_PropertyBusy(ByVal sender As Object, ByVal e As BusyChangedEventArgs)
    OnBusyChanged(e)
  End Sub

  <NotUndoable()> _
  <NonSerialized()> _
  Private _propertyBusy As BusyChangedEventHandler

  ''' <summary>
  ''' Event raised when the IsBusy property value
  ''' has changed.
  ''' </summary>
  Public Custom Event BusyChanged As BusyChangedEventHandler Implements Core.INotifyBusy.BusyChanged
    AddHandler(ByVal value As BusyChangedEventHandler)
      _propertyBusy = CType(System.Delegate.Combine(_propertyBusy, value), BusyChangedEventHandler)
    End AddHandler

    RemoveHandler(ByVal value As BusyChangedEventHandler)
      _propertyBusy = CType(System.Delegate.Remove(_propertyBusy, value), BusyChangedEventHandler)
    End RemoveHandler

    RaiseEvent(ByVal sender As Object, ByVal e As Core.BusyChangedEventArgs)
      If _propertyBusy IsNot Nothing Then
        _propertyBusy.Invoke(sender, e)
      End If
    End RaiseEvent
  End Event

  ''' <summary>
  ''' Raises the BusyChanged event.
  ''' </summary>
  ''' <param name="propertyName">Name of the property
  ''' that has changed.</param>
  ''' <param name="busy">New busy value.</param>
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Sub OnBusyChanged(ByVal propertyName As String, ByVal busy As Boolean)
    OnBusyChanged(New BusyChangedEventArgs(propertyName, busy))
  End Sub

  ''' <summary>
  ''' Raises the BusyChanged event.
  ''' </summary>
  ''' <param name="args">Event arguments.</param>
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub OnBusyChanged(ByVal args As BusyChangedEventArgs)
    If _propertyBusy IsNot Nothing Then
      _propertyBusy(Me, args)
    End If
  End Sub

#End Region

#Region " IDataPortalTarget Members "

  Private Sub CheckRules() Implements Server.IDataPortalTarget.CheckRules

  End Sub

  Private Sub MarkAsChild() Implements Server.IDataPortalTarget.MarkAsChild

  End Sub

  Private Sub MarkNew() Implements Server.IDataPortalTarget.MarkNew

  End Sub

  Private Sub MarkOld() Implements Server.IDataPortalTarget.MarkOld

  End Sub

  Private Sub IDataPortalTarget_DataPortal_OnDataPortalInvoke(ByVal e As DataPortalEventArgs) Implements Server.IDataPortalTarget.DataPortal_OnDataPortalInvoke
    Me.DataPortal_OnDataPortalInvoke(e)
  End Sub

  Private Sub IDataPortalTarget_DataPortal_OnDataPortalInvokeComplete(ByVal e As DataPortalEventArgs) Implements Server.IDataPortalTarget.DataPortal_OnDataPortalInvokeComplete
    Me.DataPortal_OnDataPortalInvokeComplete(e)
  End Sub

  Private Sub IDataPortalTarget_DataPortal_OnDataPortalException(ByVal e As DataPortalEventArgs, ByVal ex As Exception) Implements Server.IDataPortalTarget.DataPortal_OnDataPortalException
    Me.DataPortal_OnDataPortalException(e, ex)
  End Sub

  Private Sub IDataPortalTarget_Child_OnDataPortalInvoke(ByVal e As DataPortalEventArgs) Implements Server.IDataPortalTarget.Child_OnDataPortalInvoke
    Me.Child_OnDataPortalInvoke(e)
  End Sub

  Private Sub IDataPortalTarget_Child_OnDataPortalInvokeComplete(ByVal e As DataPortalEventArgs) Implements Server.IDataPortalTarget.Child_OnDataPortalInvokeComplete
    Me.Child_OnDataPortalInvokeComplete(e)
  End Sub

  Private Sub IDataPortalTarget_Child_OnDataPortalException(ByVal e As DataPortalEventArgs, ByVal ex As Exception) Implements Server.IDataPortalTarget.Child_OnDataPortalException
    Me.Child_OnDataPortalException(e, ex)
  End Sub

#End Region

#Region " IManagedProperties Members "

  Private ReadOnly Property HasManagedProperties() As Boolean Implements IManageProperties.HasManagedProperties
    Get
      Return (_fieldManager IsNot Nothing AndAlso _fieldManager.HasFields)
    End Get
  End Property

  Private Function GetManagedProperties() As System.Collections.Generic.List(Of IPropertyInfo) Implements IManageProperties.GetManagedProperties
    Return FieldManager.GetRegisteredProperties
  End Function

  Private Function IManageProperties_GetProperty(ByVal propertyInfo As IPropertyInfo) As Object Implements IManageProperties.GetProperty
    Return GetProperty(propertyInfo)
  End Function

  Private Function IManageProperties_ReadProperty(ByVal propertyInfo As IPropertyInfo) As Object Implements IManageProperties.ReadProperty
    Return ReadProperty(propertyInfo)
  End Function

  Private Function IManageProperties_ReadProperty(Of P)(ByVal propertyInfo As PropertyInfo(Of P)) As P
    Return ReadProperty(Of P)(propertyInfo)
  End Function

  Private Sub SetProperty(ByVal propertyInfo As Core.IPropertyInfo, ByVal newValue As Object) Implements Core.IManageProperties.SetProperty
    Throw New NotImplementedException("IManageProperties.SetProperty")
  End Sub

  Private Sub IManageProperties_LoadProperty(ByVal propertyInfo As Core.IPropertyInfo, ByVal newValue As Object) Implements IManageProperties.LoadProperty
    LoadProperty(propertyInfo, newValue)
  End Sub

#End Region

#Region " MobileFormatter "

  ''' <summary>
  ''' Override this method to insert your child object
  ''' references into the MobileFormatter serialzation stream.
  ''' </summary>
  ''' <param name="info">
  ''' Object containing the data to serialize.
  ''' </param>
  ''' <param name="formatter">
  ''' Reference to MobileFormatter instance. Use this to
  ''' convert child references to/from reference id values.
  ''' </param>
  Protected Overrides Sub OnGetChildren(ByVal info As Serialization.Mobile.SerializationInfo, ByVal formatter As Serialization.Mobile.MobileFormatter)
    MyBase.OnGetChildren(info, formatter)
    If _fieldManager IsNot Nothing Then
      Dim fieldManagerInfo = formatter.SerializeObject(_fieldManager)
      info.AddChild("_fieldManager", fieldManagerInfo.ReferenceId)
    End If
  End Sub

  ''' <summary>
  ''' Override this method to retrieve your child object
  ''' references from the MobileFormatter serialzation stream.
  ''' </summary>
  ''' <param name="info">
  ''' Object containing the data to serialize.
  ''' </param>
  ''' <param name="formatter">
  ''' Reference to MobileFormatter instance. Use this to
  ''' convert child references to/from reference id values.
  ''' </param>
  Protected Overrides Sub OnSetChildren(ByVal info As Serialization.Mobile.SerializationInfo, ByVal formatter As Serialization.Mobile.MobileFormatter)
    If info.Children.ContainsKey("_fieldManager") Then
      Dim childData = info.Children("_fieldManager")
      _fieldManager = CType(formatter.GetObject(childData.ReferenceId), FieldDataManager)
    End If
    MyBase.OnSetChildren(info, formatter)
  End Sub

#End Region

#Region " INotifyUnhandledAsyncException Members "

  <NotUndoable()> _
  <NonSerialized()> _
  Private _unhandledAsyncException As EventHandler(Of ErrorEventArgs)

  ''' <summary>
  ''' Event raised when an exception occurs on a background
  ''' thread during an asynchronous operation.
  ''' </summary>
  Public Custom Event UnhandledAsyncException As EventHandler(Of ErrorEventArgs) Implements Core.INotifyUnhandledAsyncException.UnhandledAsyncException
    AddHandler(ByVal value As EventHandler(Of ErrorEventArgs))
      _unhandledAsyncException = CType(System.Delegate.Combine(_unhandledAsyncException, value), EventHandler(Of ErrorEventArgs))
    End AddHandler

    RemoveHandler(ByVal value As EventHandler(Of ErrorEventArgs))
      _unhandledAsyncException = CType(System.Delegate.Remove(_unhandledAsyncException, value), EventHandler(Of ErrorEventArgs))
    End RemoveHandler

    RaiseEvent(ByVal sender As Object, ByVal e As Core.ErrorEventArgs)
      If _unhandledAsyncException IsNot Nothing Then
        _unhandledAsyncException.Invoke(sender, e)
      End If
    End RaiseEvent
  End Event

  ''' <summary>
  ''' Raises the UnhandledAsyncException event.
  ''' </summary>
  ''' <param name="e">Error arguments.</param>
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub OnUnhandledAsyncException(ByVal e As ErrorEventArgs)
    If _unhandledAsyncException IsNot Nothing Then
      _unhandledAsyncException(Me, e)
    End If
  End Sub

  ''' <summary>
  ''' Raises the UnhandledAsyncException event.
  ''' </summary>
  ''' <param name="originalSender">Original sender of the
  ''' event.</param>
  ''' <param name="e">Execption that occurred.</param>
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Sub OnUnhandledAsyncException(ByVal originalSender As Object, ByVal e As Exception)
    OnUnhandledAsyncException(New ErrorEventArgs(originalSender, e))
  End Sub

#End Region

End Class
