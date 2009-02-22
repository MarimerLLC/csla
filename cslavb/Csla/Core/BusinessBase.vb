Imports System
Imports System.Linq
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Runtime.Serialization
Imports System.Collections.Specialized
Imports Csla.Validation
Imports System.Collections.ObjectModel
Imports Csla.Core.LoadManager
Imports Csla.Server
Imports Csla.Security
Imports Csla.Serialization.Mobile

Namespace Core

  ''' <summary>
  ''' This is the non-generic base class from which most
  ''' business objects will be derived.
  ''' </summary>
  ''' <remarks>
  ''' See Chapter 3 for details.
  ''' </remarks>
  <Serializable()> _
  Public MustInherit Class BusinessBase
    Inherits UndoableBase
    Implements IEditableBusinessObject
    Implements System.ComponentModel.IEditableObject
    Implements IDataErrorInfo
    Implements ICloneable
    Implements Security.IAuthorizeReadWrite
    Implements IParent
    Implements Server.IDataPortalTarget
    Implements IManageProperties
    Implements INotifyBusy
    Implements INotifyChildChanged
    Implements Serialization.Mobile.ISerializationNotification

#Region " Constructors "

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub New()

      Initialize()
      InitializeBusinessRules()
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

#Region " IsNew, IsDeleted, IsDirty, IsSavable "

    ' keep track of whether we are new, deleted or dirty
    Private _isNew As Boolean = True
    Private _isDeleted As Boolean
    Private _isDirty As Boolean = True

    ''' <summary>
    ''' Returns <see langword="true" /> if this is a new object, 
    ''' <see langword="false" /> if it is a pre-existing object.
    ''' </summary>
    ''' <remarks>
    ''' An object is considered to be new if its primary identifying (key) value 
    ''' doesn't correspond to data in the database. In other words, 
    ''' if the data values in this particular
    ''' object have not yet been saved to the database the object is considered to
    ''' be new. Likewise, if the object's data has been deleted from the database
    ''' then the object is considered to be new.
    ''' </remarks>
    ''' <returns>A value indicating if this object is new.</returns>
    <Browsable(False)> _
    Public ReadOnly Property IsNew() As Boolean Implements IEditableBusinessObject.IsNew
      Get
        Return _isNew
      End Get
    End Property

    ''' <summary>
    ''' Returns <see langword="true" /> if this object is marked for deletion.
    ''' </summary>
    ''' <remarks>
    ''' CSLA .NET supports both immediate and deferred deletion of objects. This
    ''' property is part of the support for deferred deletion, where an object
    ''' can be marked for deletion, but isn't actually deleted until the object
    ''' is saved to the database. This property indicates whether or not the
    ''' current object has been marked for deletion. If it is <see langword="true" />
    ''' , the object will
    ''' be deleted when it is saved to the database, otherwise it will be inserted
    ''' or updated by the save operation.
    ''' </remarks>
    ''' <returns>A value indicating if this object is marked for deletion.</returns>
    <Browsable(False)> _
    Public ReadOnly Property IsDeleted() As Boolean Implements IEditableBusinessObject.IsDeleted
      Get
        Return _isDeleted
      End Get
    End Property

    ''' <summary>
    ''' Returns <see langword="true" /> if this object's 
    ''' data, or any of its fields or child objects data, 
    ''' has been changed.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' When an object's data is changed, CSLA .NET makes note of that change
    ''' and considers the object to be 'dirty' or changed. This value is used to
    ''' optimize data updates, since an unchanged object does not need to be
    ''' updated into the database. All new objects are considered dirty. All objects
    ''' marked for deletion are considered dirty.
    ''' </para><para>
    ''' Once an object's data has been saved to the database (inserted or updated)
    ''' the dirty flag is cleared and the object is considered unchanged. Objects
    ''' newly loaded from the database are also considered unchanged.
    ''' </para>
    ''' </remarks>
    ''' <returns>A value indicating if this object's data has been changed.</returns>
    <Browsable(False)> _
    Public Overridable ReadOnly Property IsDirty() As Boolean Implements IEditableBusinessObject.IsDirty
      Get
        Return IsSelfDirty OrElse (_fieldManager IsNot Nothing AndAlso FieldManager.IsDirty())
      End Get
    End Property

    ''' <summary>
    ''' Returns <see langword="true" /> if this object's data has been 
    ''' changed, ignoring whether any child objects have been changed.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' When an object's data is changed, CSLA .NET makes note of that change
    ''' and considers the object to be 'dirty' or changed. This value is used to
    ''' optimize data updates, since an unchanged object does not need to be
    ''' updated into the database. All new objects are considered dirty. All objects
    ''' marked for deletion are considered dirty.
    ''' </para><para>
    ''' Once an object's data has been saved to the database (inserted or updated)
    ''' the dirty flag is cleared and the object is considered unchanged. Objects
    ''' newly loaded from the database are also considered unchanged.
    ''' </para>
    ''' </remarks>
    ''' <returns>A value indicating if this object's data has been changed.</returns>
    <Browsable(False)> _
    Public Overridable ReadOnly Property IsSelfDirty() As Boolean Implements IEditableBusinessObject.IsSelfDirty
      Get
        Return _isDirty
      End Get
    End Property

    ''' <summary>
    ''' Marks the object as being a new object. This also marks the object
    ''' as being dirty and ensures that it is not marked for deletion.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Newly created objects are marked new by default. You should call
    ''' this method in the implementation of DataPortal_Update when the
    ''' object is deleted (due to being marked for deletion) to indicate
    ''' that the object no longer reflects data in the database.
    ''' </para><para>
    ''' If you override this method, make sure to call the base
    ''' implementation after executing your new code.
    ''' </para>
    ''' </remarks>
    Protected Overridable Sub MarkNew()
      _isNew = True
      _isDeleted = False
      MarkDirty()
    End Sub

    ''' <summary>
    ''' Marks the object as being an old (not new) object. This also
    ''' marks the object as being unchanged (not dirty).
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' You should call this method in the implementation of
    ''' DataPortal_Fetch to indicate that an existing object has been
    ''' successfully retrieved from the database.
    ''' </para><para>
    ''' You should call this method in the implementation of 
    ''' DataPortal_Update to indicate that a new object has been successfully
    ''' inserted into the database.
    ''' </para><para>
    ''' If you override this method, make sure to call the base
    ''' implementation after executing your new code.
    ''' </para>
    ''' </remarks>
    Protected Overridable Sub MarkOld()
      _isNew = False
      MarkClean()
    End Sub

    ''' <summary>
    ''' Marks an object for deletion. This also marks the object
    ''' as being dirty.
    ''' </summary>
    ''' <remarks>
    ''' You should call this method in your business logic in the
    ''' case that you want to have the object deleted when it is
    ''' saved to the database.
    ''' </remarks>
    Protected Sub MarkDeleted()
      _isDeleted = True
      MarkDirty()
    End Sub

    ''' <summary>
    ''' Marks an object as being dirty, or changed.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' You should call this method in your business logic any time
    ''' the object's internal data changes. Any time any instance
    ''' variable changes within the object, this method should be called
    ''' to tell CSLA .NET that the object's data has been changed.
    ''' </para><para>
    ''' Marking an object as dirty does two things. First it ensures
    ''' that CSLA .NET will properly save the object as appropriate. Second,
    ''' it causes CSLA .NET to tell Windows Forms data binding that the
    ''' object's data has changed so any bound controls will update to
    ''' reflect the new values.
    ''' </para>
    ''' </remarks>
    Protected Sub MarkDirty()
      MarkDirty(False)
    End Sub

    ''' <summary>
    ''' Marks an object as being dirty, or changed.
    ''' </summary>
    ''' <param name="suppressEvent">
    ''' <see langword="true" /> to supress the PropertyChanged event that is otherwise
    ''' raised to indicate that the object's state has changed.
    ''' </param>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Sub MarkDirty(ByVal suppressEvent As Boolean)
      _isDirty = True
      If Not suppressEvent Then
        OnUnknownPropertyChanged()
      End If
    End Sub

    ''' <summary>
    ''' Performs processing required when the current
    ''' property has changed.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' This method calls CheckRules(propertyName), MarkDirty and
    ''' OnPropertyChanged(propertyName). MarkDirty is called such
    ''' that no event is raised for IsDirty, so only the specific
    ''' property changed event for the current property is raised.
    ''' </para><para>
    ''' This implementation uses System.Diagnostics.StackTrace to
    ''' determine the name of the current property, and so must be called
    ''' directly from the property to be checked.
    ''' </para>
    ''' </remarks>
    <Obsolete("Use overload requiring explicit property name")> _
    <System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)> _
    <System.Runtime.CompilerServices.MethodImpl( _
      System.Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Protected Sub PropertyHasChanged()

      Dim propertyName As String = _
        New System.Diagnostics.StackTrace(). _
        GetFrame(1).GetMethod.Name.Substring(4)
      PropertyHasChanged(propertyName)

    End Sub

    ''' <summary>
    ''' Performs processing required when a property
    ''' has changed.
    ''' </summary>
    ''' <param name="propertyName">Name of the property that
    ''' has changed.</param>
    ''' <remarks>
    ''' This method calls CheckRules(propertyName), MarkDirty and
    ''' OnPropertyChanged(propertyName). MarkDirty is called such
    ''' that no event is raised for IsDirty, so only the specific
    ''' property changed event for the current property is raised.
    ''' </remarks>
    Protected Overridable Sub PropertyHasChanged(ByVal propertyName As String)

      MarkDirty(True)
      Dim propertyNames As String() = ValidationRules.CheckRules(propertyName)

      If ApplicationContext.PropertyChangedMode = ApplicationContext.PropertyChangedModes.Windows Then
        OnPropertyChanged(propertyName)
      Else
        For Each name As String In propertyNames
          OnPropertyChanged(name)
        Next
      End If
    End Sub

    ''' <summary>
    ''' Forces the object's IsDirty flag to <see langword="false" />.
    ''' </summary>
    ''' <remarks>
    ''' This method is normally called automatically and is
    ''' not intended to be called manually.
    ''' </remarks>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Sub MarkClean()

      _isDirty = False
      If _fieldManager IsNot Nothing Then
        FieldManager.MarkClean()
      End If
      OnUnknownPropertyChanged()

    End Sub

    ''' <summary>
    ''' Returns <see langword="true" /> if this object is both dirty and valid.
    ''' </summary>
    ''' <remarks>
    ''' An object is considered dirty (changed) if 
    ''' <see cref="P:Csla.BusinessBase.IsDirty" /> returns <see langword="true" />. It is
    ''' considered valid if IsValid
    ''' returns <see langword="true" />. The IsSavable property is
    ''' a combination of these two properties. 
    ''' </remarks>
    ''' <returns>A value indicating if this object is both dirty and valid.</returns>
    <Browsable(False)> _
    Public Overridable ReadOnly Property IsSavable() As Boolean Implements IEditableBusinessObject.IsSavable
      Get
        Dim auth As Boolean
        If IsDeleted Then
          auth = Security.AuthorizationRules.CanDeleteObject(Me.GetType())
        ElseIf IsNew Then
          auth = Security.AuthorizationRules.CanCreateObject(Me.GetType())
        Else
          auth = Security.AuthorizationRules.CanEditObject(Me.GetType())
        End If
        Return (auth AndAlso IsDirty AndAlso IsValid AndAlso Not IsBusy)
      End Get
    End Property

#End Region

#Region " Authorization "

    <NotUndoable()> _
    <NonSerialized()> _
    Private _readResultCache As Dictionary(Of String, Boolean)
    <NotUndoable()> _
    <NonSerialized()> _
    Private _writeResultCache As Dictionary(Of String, Boolean)
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
      If Not Security.SharedAuthorizationRules.RulesExistFor(Me.GetType) Then
        SyncLock Me.GetType
          If Not Security.SharedAuthorizationRules.RulesExistFor(Me.GetType) Then
            Security.SharedAuthorizationRules.GetManager(Me.GetType, True)
            AddAuthorizationRules()
          End If
        End SyncLock
      End If

    End Sub

    ''' <summary>
    ''' Override this method to add authorization
    ''' rules for your object's properties.
    ''' </summary>
    ''' <remarks>
    ''' AddInstanceAuthorizationRules is automatically called by CSLA .NET
    ''' when your object should associate per-instance authorization roles
    ''' with its properties.
    ''' </remarks>
    Protected Overridable Sub AddInstanceAuthorizationRules()

    End Sub

    ''' <summary>
    ''' Override this method to add per-type
    ''' authorization rules for your type's properties.
    ''' </summary>
    ''' <remarks>
    ''' AddAuthorizationRules is automatically called by CSLA .NET
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
    <System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)> _
    <System.Runtime.CompilerServices.MethodImpl( _
      System.Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Public Function CanReadProperty(ByVal throwOnFalse As Boolean) As Boolean

      Dim propertyName As String = _
        New System.Diagnostics.StackTrace(). _
        GetFrame(1).GetMethod.Name.Substring(4)
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
    <System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)> _
    <System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
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
      ByVal propertyName As String) As Boolean Implements Security.IAuthorizeReadWrite.CanReadProperty

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

    ''' <summary>
    ''' Returns <see langword="true" /> if the user is allowed to write the
    ''' calling property.
    ''' </summary>
    ''' <returns><see langword="true" /> if write is allowed.</returns>
    ''' <param name="throwOnFalse">Indicates whether a negative
    ''' result should cause an exception.</param>
    <Obsolete("Use overload requiring explicit property name")> _
    <System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)> _
    <System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Public Function CanWriteProperty(ByVal throwOnFalse As Boolean) As Boolean

      Dim propertyName As String = _
        New System.Diagnostics.StackTrace().GetFrame(1).GetMethod.Name.Substring(4)
      Dim result As Boolean = CanWriteProperty(propertyName)
      If throwOnFalse AndAlso result = False Then
        Dim ex As New System.Security.SecurityException( _
          String.Format("{0} ({1})", My.Resources.PropertySetNotAllowed, propertyName))
        ex.Action = System.Security.Permissions.SecurityAction.Deny
        Throw ex
      End If
      Return result

    End Function

    ''' <summary>
    ''' Returns <see langword="true" /> if the user is allowed to write the
    ''' calling property.
    ''' </summary>
    ''' <returns><see langword="true" /> if write is allowed.</returns>
    ''' <param name="propertyName">Name of the property to write.</param>
    ''' <param name="throwOnFalse">Indicates whether a negative
    ''' result should cause an exception.</param>
    Public Function CanWriteProperty(ByVal propertyName As String, ByVal throwOnFalse As Boolean) As Boolean

      Dim result As Boolean = CanWriteProperty(propertyName)
      If throwOnFalse AndAlso result = False Then
        Dim ex As New System.Security.SecurityException( _
          String.Format("{0} ({1})", My.Resources.PropertySetNotAllowed, propertyName))
        ex.Action = System.Security.Permissions.SecurityAction.Deny
        Throw ex
      End If
      Return result

    End Function

    ''' <summary>
    ''' Returns <see langword="true" /> if the user is allowed to write the
    ''' calling property.
    ''' </summary>
    ''' <returns><see langword="true" /> if write is allowed.</returns>
    <Obsolete("Use overload requiring explicit property name")> _
    <System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)> _
    <System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Public Function CanWriteProperty() As Boolean

      Dim propertyName As String = _
        New System.Diagnostics.StackTrace().GetFrame(1).GetMethod.Name.Substring(4)
      Return CanWriteProperty(propertyName)

    End Function

    ''' <summary>
    ''' Returns <see langword="true" /> if the user is allowed to write the
    ''' specified property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property to write.</param>
    ''' <returns><see langword="true" /> if write is allowed.</returns>
    ''' <remarks>
    ''' <para>
    ''' If a list of allowed roles is provided then only users in those
    ''' roles can write. If no list of allowed roles is provided then
    ''' the list of denied roles is checked.
    ''' </para><para>
    ''' If a list of denied roles is provided then users in the denied
    ''' roles are denied write access. All other users are allowed.
    ''' </para><para>
    ''' If neither a list of allowed nor denied roles is provided then
    ''' all users will have write access.
    ''' </para>
    ''' </remarks>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Public Overridable Function CanWriteProperty( _
      ByVal propertyName As String) As Boolean Implements Security.IAuthorizeReadWrite.CanWriteProperty

      Dim result As Boolean = True

      VerifyAuthorizationCache()

      If Not _writeResultCache.TryGetValue(propertyName, result) Then
        result = True
        If AuthorizationRules.HasWriteAllowedRoles(propertyName) Then
          ' some users are explicitly granted write access
          ' in which case all other users are denied
          If Not AuthorizationRules.IsWriteAllowed(propertyName) Then
            result = False
          End If

        ElseIf AuthorizationRules.HasWriteDeniedRoles(propertyName) Then
          ' some users are explicitly denied write access
          If AuthorizationRules.IsWriteDenied(propertyName) Then
            result = False
          End If
        End If
        _writeResultCache(propertyName) = result
      End If
      Return result

    End Function

    Private Sub VerifyAuthorizationCache()

      If _readResultCache Is Nothing Then
        _readResultCache = New Dictionary(Of String, Boolean)
      End If
      If _writeResultCache Is Nothing Then
        _writeResultCache = New Dictionary(Of String, Boolean)
      End If
      If _executeResultCache Is Nothing Then
        _executeResultCache = New Dictionary(Of String, Boolean)
      End If
      If Not ReferenceEquals(ApplicationContext.User, _lastPrincipal) Then
        ' the principal has changed - reset the cache
        _readResultCache.Clear()
        _writeResultCache.Clear()
        _executeResultCache.Clear()
        _lastPrincipal = ApplicationContext.User
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
    <System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)> _
    <System.Runtime.CompilerServices.MethodImpl( _
      System.Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Public Function CanExecuteMethod(ByVal throwOnFalse As Boolean) As Boolean

      Dim methodName As String = New System.Diagnostics.StackTrace().GetFrame(1).GetMethod.Name
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
    <System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)> _
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
    ''' roles can execute the method. If no list of allowed roles is 
    ''' provided then the list of denied roles is checked.
    ''' </para><para>
    ''' If a list of denied roles is provided then users in the denied
    ''' roles are not allowed to execute the method. 
    ''' All other users are allowed.
    ''' </para><para>
    ''' If neither a list of allowed nor denied roles is provided then
    ''' all users will be allowed to execute the method..
    ''' </para>
    ''' </remarks>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Public Overridable Function CanExecuteMethod( _
      ByVal methodName As String) As Boolean Implements Security.IAuthorizeReadWrite.CanExecuteMethod

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

#Region " Parent/Child link "

    <NotUndoable()> _
    <NonSerialized()> _
    Private _parent As Core.IParent

    ''' <summary>
    ''' Provide access to the parent reference for use
    ''' in child object code.
    ''' </summary>
    ''' <remarks>
    ''' This value will be Nothing for root objects.
    ''' </remarks>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Friend ReadOnly Property Parent() As Core.IParent
      Get
        Return _parent
      End Get
    End Property

    ''' <summary>
    ''' Used by BusinessListBase as a child object is 
    ''' created to tell the child object about its
    ''' parent.
    ''' </summary>
    ''' <param name="parent">A reference to the parent collection object.</param>
    Friend Sub SetParent(ByVal parent As Core.IParent)

      _parent = parent

    End Sub

#End Region

#Region " IEditableObject "

    Private _neverCommitted As Boolean = True
    <NotUndoable()> _
    Private _disableIEditableObject As Boolean

    ''' <summary>
    ''' Gets or sets a value indicating whether the
    ''' IEditableObject interface methods should
    ''' be disabled for this object.
    ''' </summary>
    ''' <value>Defaults to False, indicating that
    ''' the IEditableObject methods will behave
    ''' normally.</value>
    ''' <remarks>
    ''' If you disable the IEditableObject methods
    ''' then Windows Forms data binding will no longer
    ''' automatically call BeginEdit, CancelEdit or
    ''' ApplyEdit on your object, and you will have
    ''' to call these methods manually to get proper
    ''' n-level undo behavior.
    ''' </remarks>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Property DisableIEditableObject() As Boolean
      Get
        Return _disableIEditableObject
      End Get
      Set(ByVal value As Boolean)
        _disableIEditableObject = value
      End Set
    End Property

    ''' <summary>
    ''' Allow data binding to start a nested edit on the object.
    ''' </summary>
    ''' <remarks>
    ''' Data binding may call this method many times. Only the first
    ''' call should be honored, so we have extra code to detect this
    ''' and do nothing for subsquent calls.
    ''' </remarks>
    Private Sub IEditableObject_BeginEdit() _
      Implements System.ComponentModel.IEditableObject.BeginEdit

      If Not _disableIEditableObject AndAlso Not BindingEdit Then
        BindingEdit = True
        BeginEdit()
      End If

    End Sub

    ''' <summary>
    ''' Allow data binding to cancel the current edit.
    ''' </summary>
    ''' <remarks>
    ''' Data binding may call this method many times. Only the first
    ''' call to either IEditableObject.CancelEdit or 
    ''' IEditableObject.EndEdit
    ''' should be honored. We include extra code to detect this and do
    ''' nothing for subsequent calls.
    ''' </remarks>
    Private Sub IEditableObject_CancelEdit() _
      Implements System.ComponentModel.IEditableObject.CancelEdit

      If Not _disableIEditableObject AndAlso BindingEdit Then
        CancelEdit()
        If IsNew AndAlso _neverCommitted AndAlso _
          EditLevel <= EditLevelAdded Then
          ' we're new and no EndEdit or ApplyEdit has ever been
          ' called on us, and now we've been canceled back to 
          ' where we were added so we should have ourselves  
          ' removed from the parent collection
          If Not Parent Is Nothing Then
            Parent.RemoveChild(Me)
          End If
        End If
      End If

    End Sub

    ''' <summary>
    ''' Allow data binding to apply the current edit.
    ''' </summary>
    ''' <remarks>
    ''' Data binding may call this method many times. Only the first
    ''' call to either IEditableObject.EndEdit or 
    ''' IEditableObject.CancelEdit
    ''' should be honored. We include extra code to detect this and do
    ''' nothing for subsequent calls.
    ''' </remarks>
    Private Sub IEditableObject_EndEdit() _
      Implements System.ComponentModel.IEditableObject.EndEdit

      If Not _disableIEditableObject AndAlso BindingEdit Then
        ApplyEdit()
      End If

    End Sub

#End Region

#Region " Begin/Cancel/ApplyEdit "

    ''' <summary>
    ''' Starts a nested edit on the object.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' When this method is called the object takes a snapshot of
    ''' its current state (the values of its variables). This snapshot
    ''' can be restored by calling CancelEdit
    ''' or committed by calling ApplyEdit.
    ''' </para><para>
    ''' This is a nested operation. Each call to BeginEdit adds a new
    ''' snapshot of the object's state to a stack. You should ensure that 
    ''' for each call to BeginEdit there is a corresponding call to either 
    ''' CancelEdit or ApplyEdit to remove that snapshot from the stack.
    ''' </para><para>
    ''' See Chapters 2 and 3 for details on n-level undo and state stacking.
    ''' </para>
    ''' </remarks>
    Public Sub BeginEdit() Implements IEditableBusinessObject.BeginEdit
      CopyState(Me.EditLevel + 1)
    End Sub

    ''' <summary>
    ''' Cancels the current edit process, restoring the object's state to
    ''' its previous values.
    ''' </summary>
    ''' <remarks>
    ''' Calling this method causes the most recently taken snapshot of the 
    ''' object's state to be restored. This resets the object's values
    ''' to the point of the last BeginEdit call.
    ''' </remarks>
    Public Sub CancelEdit() Implements IEditableBusinessObject.CancelEdit
      UndoChanges(Me.EditLevel - 1)
    End Sub

    ''' <summary>
    ''' Called when an undo operation has completed.
    ''' </summary>
    ''' <remarks>
    ''' This method resets the object as a result of
    ''' deserialization and raises PropertyChanged events
    ''' to notify data binding that the object has changed.
    ''' </remarks>
    Protected Overrides Sub UndoChangesComplete()

      BindingEdit = False
      ValidationRules.SetTarget(Me)
      InitializeBusinessRules()
      OnUnknownPropertyChanged()
      MyBase.UndoChangesComplete()

    End Sub

    ''' <summary>
    ''' Commits the current edit process.
    ''' </summary>
    ''' <remarks>
    ''' Calling this method causes the most recently taken snapshot of the 
    ''' object's state to be discarded, thus committing any changes made
    ''' to the object's state since the last BeginEdit call.
    ''' </remarks>
    Public Sub ApplyEdit() Implements IEditableBusinessObject.ApplyEdit
      _neverCommitted = False
      AcceptChanges(Me.EditLevel - 1)
      BindingEdit = False
    End Sub

    ''' <summary>
    ''' Notifies the parent object (if any) that this
    ''' child object's edits have been accepted.
    ''' </summary>
    Protected Overrides Sub AcceptChangesComplete()

      If Parent IsNot Nothing Then
        Parent.ApplyEditChild(Me)
      End If
      MyBase.AcceptChangesComplete()

    End Sub

#End Region

#Region " IsChild "

    <NotUndoable()> _
    Private _isChild As Boolean

    ''' <summary>
    ''' Returns <see langword="true" /> if this is a child (non-root) object.
    ''' </summary>
    Protected Friend ReadOnly Property IsChild() As Boolean
      Get
        Return _isChild
      End Get
    End Property

    ''' <summary>
    ''' Marks the object as being a child object.
    ''' </summary>
    Protected Sub MarkAsChild()
      _isChild = True
    End Sub

#End Region

#Region " Delete "

    ''' <summary>
    ''' Marks the object for deletion. The object will be deleted as part of the
    ''' next save operation.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' CSLA .NET supports both immediate and deferred deletion of objects. This
    ''' method is part of the support for deferred deletion, where an object
    ''' can be marked for deletion, but isn't actually deleted until the object
    ''' is saved to the database. This method is called by the UI developer to
    ''' mark the object for deletion.
    ''' </para><para>
    ''' To 'undelete' an object, use n-level undo as discussed in Chapters 2 and 3.
    ''' </para>
    ''' </remarks>
    Public Overridable Sub Delete() Implements IEditableBusinessObject.Delete
      If Me.IsChild Then
        Throw New NotSupportedException(My.Resources.ChildDeleteException)
      End If

      MarkDeleted()

    End Sub

    ''' <summary>
    ''' Called by a parent object to mark the child
    ''' for deferred deletion.
    ''' </summary>
    Friend Sub DeleteChild()
      If Not Me.IsChild Then
        Throw New NotSupportedException(My.Resources.NoDeleteRootException)
      End If

      BindingEdit = False
      MarkDeleted()

    End Sub

#End Region

#Region " Edit Level Tracking (child only) "

    ' we need to keep track of the edit
    ' level when we were added so if the user
    ' cancels below that level we can be destroyed
    <NotUndoable()> _
    Private _editLevelAdded As Integer

    ''' <summary>
    ''' Gets or sets the current edit level of the
    ''' object.
    ''' </summary>
    ''' <remarks>
    ''' Allow the collection object to use the
    ''' edit level as needed.
    ''' </remarks>
    Friend Property EditLevelAdded() As Integer
      Get
        Return _editLevelAdded
      End Get
      Set(ByVal Value As Integer)
        _editLevelAdded = Value
      End Set
    End Property

    Public ReadOnly Property IUndoableObject_EditLevel() As Integer Implements Core.IUndoableObject.EditLevel
      Get
        Return Me.EditLevel
      End Get
    End Property

#End Region

#Region " ICloneable "

    Private Function Clone() As Object Implements ICloneable.Clone

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

#End Region

#Region " ValidationRules, IsValid "

    Private _validationRules As Validation.ValidationRules

    <NonSerialized()> _
    Private _validationCompleteHandlers As EventHandler

    ''' <summary>
    ''' Event raised when validation is complete.
    ''' </summary>
    Public Custom Event ValidationComplete As EventHandler
      AddHandler(ByVal value As EventHandler)
        _validationCompleteHandlers = CType(System.Delegate.Combine(_validationCompleteHandlers, value), EventHandler)
      End AddHandler

      RemoveHandler(ByVal value As EventHandler)
        _validationCompleteHandlers = CType(System.Delegate.Remove(_validationCompleteHandlers, value), EventHandler)
      End RemoveHandler

      RaiseEvent(ByVal sender As Object, ByVal e As System.EventArgs)
        If _validationCompleteHandlers IsNot Nothing Then
          _validationCompleteHandlers.Invoke(Me, e)
        End If
      End RaiseEvent
    End Event

    ''' <summary>
    ''' Raises the ValidationComplete event
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)> _
    Protected Overridable Sub OnValidationComplete()
      RaiseEvent ValidationComplete(Me, EventArgs.Empty)
    End Sub

    Private Sub InitializeBusinessRules()

      AddInstanceBusinessRules()
      If Not Validation.SharedValidationRules.RulesExistFor(Me.GetType) Then
        SyncLock Me.GetType
          If Not Validation.SharedValidationRules.RulesExistFor(Me.GetType) Then
            Validation.SharedValidationRules.GetManager(Me.GetType, True)
            AddBusinessRules()
          End If
        End SyncLock
      End If

    End Sub

    ''' <summary>
    ''' Provides access to the broken rules functionality.
    ''' </summary>
    ''' <remarks>
    ''' This property is used within your business logic so you can
    ''' easily call the AddRule() method to associate validation
    ''' rules with your object's properties.
    ''' </remarks>
    Protected ReadOnly Property ValidationRules() _
      As Validation.ValidationRules
      Get
        If _validationRules Is Nothing Then
          _validationRules = New Validation.ValidationRules(Me)
          AddHandler _validationRules.ValidatingRules.CollectionChanged, AddressOf ValidatingRules_CollectionChanged
        ElseIf _validationRules.Target Is Nothing Then
          _validationRules.SetTarget(Me)
        End If
        Return _validationRules
      End Get
    End Property

    Private Sub ValidatingRules_CollectionChanged(ByVal sender As Object, ByVal e As NotifyCollectionChangedEventArgs)
      If e.Action = NotifyCollectionChangedAction.Remove Then
        For Each rule As IAsyncRuleMethod In e.OldItems
          SyncLock _validationRules.ValidatingRules
            'This rule could be validating multiple times simultaneously, we only want to call
            'OnPropertyIdle if the rule is completely removed from the list.
            If Not _validationRules.ValidatingRules.Contains(rule) Then
              For Each [property] As IPropertyInfo In rule.AsyncRuleArgs.Properties
                OnPropertyChanged([property].Name)
                OnBusyChanged(New BusyChangedEventArgs([property].Name, False))
              Next
            End If
          End SyncLock
        Next

        If Not ValidationRules.IsValidating Then
          OnValidationComplete()
        End If
      ElseIf e.Action = NotifyCollectionChangedAction.Add Then
        For Each rule As IAsyncRuleMethod In e.NewItems
          For Each [property] As IPropertyInfo In rule.AsyncRuleArgs.Properties
            OnBusyChanged(New BusyChangedEventArgs([property].Name, False))
          Next
        Next
      End If

    End Sub

    ''' <summary>
    ''' Override this method in your business class to
    ''' be notified when you need to set up business
    ''' rules.
    ''' </summary>
    ''' <remarks>
    ''' This method is automatically called by CSLA .NET
    ''' when your object should associate per-instance
    ''' validation rules with its properties.
    ''' </remarks>
    Protected Overridable Sub AddInstanceBusinessRules()

    End Sub

    ''' <summary>
    ''' Override this method in your business class to
    ''' be notified when you need to set up shared 
    ''' business rules.
    ''' </summary>
    ''' <remarks>
    ''' This method is automatically called by CSLA .NET
    ''' when your object should associate per-type 
    ''' validation rules with its properties.
    ''' </remarks>
    Protected Overridable Sub AddBusinessRules()

    End Sub

    ''' <summary>
    ''' Returns <see langword="true" /> if the object 
    ''' and its child objects are currently valid, 
    ''' <see langword="false" /> if the
    ''' object or any of its child objects have broken 
    ''' rules or are otherwise invalid.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' By default this property relies on the underling ValidationRules
    ''' object to track whether any business rules are currently broken for this object.
    ''' </para><para>
    ''' You can override this property to provide more sophisticated
    ''' implementations of the behavior. For instance, you should always override
    ''' this method if your object has child objects, since the validity of this object
    ''' is affected by the validity of all child objects.
    ''' </para>
    ''' </remarks>
    ''' <returns>A value indicating if the object is currently valid.</returns>
    <Browsable(False)> _
    Public Overridable ReadOnly Property IsValid() As Boolean Implements IEditableBusinessObject.IsValid
      Get
        Return IsSelfValid AndAlso (_fieldManager Is Nothing OrElse FieldManager.IsValid())
      End Get
    End Property

    ''' <summary>
    ''' Returns <see langword="true" /> if the object is currently 
    ''' valid, <see langword="false" /> if the
    ''' object has broken rules or is otherwise invalid.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' By default this property relies on the underling ValidationRules
    ''' object to track whether any business rules are currently broken for this object.
    ''' </para><para>
    ''' You can override this property to provide more sophisticated
    ''' implementations of the behavior. 
    ''' </para>
    ''' </remarks>
    ''' <returns>A value indicating if the object is currently valid.</returns>
    <Browsable(False)> _
    Public Overridable ReadOnly Property IsSelfValid() As Boolean Implements IEditableBusinessObject.IsSelfValid
      Get
        Return ValidationRules.IsValid
      End Get
    End Property

    ''' <summary>
    ''' Provides access to the readonly collection of broken business rules
    ''' for this object.
    ''' </summary>
    ''' <returns>A Csla.Validation.RulesCollection object.</returns>
    <Browsable(False)> _
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Public Overridable ReadOnly Property BrokenRulesCollection() _
      As Validation.BrokenRulesCollection
      Get
        Return ValidationRules.GetBrokenRules
      End Get
    End Property

#End Region

#Region " Data Access "

    ''' <summary>
    ''' Override this method to load a new business object with default
    ''' values from the database.
    ''' </summary>
    ''' <remarks>
    ''' Normally you will overload this method to accept a strongly-typed
    ''' criteria parameter, rather than overriding the method with a
    ''' loosely-typed criteria parameter.
    ''' </remarks>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
    <RunLocal()> _
    Protected Overridable Sub DataPortal_Create()
      ValidationRules.CheckRules()
    End Sub

    ''' <summary>
    ''' Override this method to allow retrieval of an existing business
    ''' object based on data in the database.
    ''' </summary>
    ''' <remarks>
    ''' Normally you will overload this method to accept a strongly-typed
    ''' criteria parameter, rather than overriding the method with a
    ''' loosely-typed criteria parameter.
    ''' </remarks>
    ''' <param name="criteria">An object containing criteria values to identify the object.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
    Protected Overridable Sub DataPortal_Fetch(ByVal criteria As Object)
      Throw New NotSupportedException(My.Resources.FetchNotSupportedException)
    End Sub

    ''' <summary>
    ''' Override this method to allow insertion of a business
    ''' object.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
    Protected Overridable Sub DataPortal_Insert()
      Throw New NotSupportedException(My.Resources.InsertNotSupportedException)
    End Sub

    ''' <summary>
    ''' Override this method to allow update of a business
    ''' object.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
    Protected Overridable Sub DataPortal_Update()
      Throw New NotSupportedException(My.Resources.UpdateNotSupportedException)
    End Sub

    ''' <summary>
    ''' Override this method to allow deferred deletion of a business object.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
    Protected Overridable Sub DataPortal_DeleteSelf()
      Throw New NotSupportedException(My.Resources.DeleteNotSupportedException)
    End Sub

    ''' <summary>
    ''' Override this method to allow immediate deletion of a business object.
    ''' </summary>
    ''' <param name="criteria">An object containing criteria values to identify the object.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
    Protected Overridable Sub DataPortal_Delete(ByVal criteria As Object)
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
    ''' Override this method to load a new business object with default
    ''' values from the database.
    ''' </summary>
    ''' <remarks>
    ''' Normally you will overload this method to accept a strongly-typed
    ''' criteria parameter, rather than overriding the method with a
    ''' loosely-typed criteria parameter.
    ''' </remarks>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
    Protected Overridable Sub Child_Create()
      ValidationRules.CheckRules()
    End Sub

    ''' <summary>
    ''' Called by the server-side DataPortal prior to calling the 
    ''' requested DataPortal_XYZ method.
    ''' </summary>
    ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub Child_OnDataPortalInvoke(ByVal e As DataPortalEventArgs)

    End Sub

    ''' <summary>
    ''' Called by the server-side DataPortal after calling the 
    ''' requested DataPortal_XYZ method.
    ''' </summary>
    ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub Child_OnDataPortalInvokeComplete(ByVal e As DataPortalEventArgs)

    End Sub

    ''' <summary>
    ''' Called by the server-side DataPortal if an exception
    ''' occurs during data access.
    ''' </summary>
    ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    ''' <param name="ex">The Exception thrown during data access.</param>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub Child_OnDataPortalException(ByVal e As DataPortalEventArgs, ByVal ex As Exception)

    End Sub

#End Region

#Region " IDataErrorInfo "

    Private ReadOnly Property [Error]() As String _
      Implements System.ComponentModel.IDataErrorInfo.Error
      Get
        If Not IsSelfValid Then
          Return ValidationRules.GetBrokenRules().ToString(Validation.RuleSeverity.Error)

        Else
          Return String.Empty
        End If
      End Get
    End Property

    Private ReadOnly Property Item(ByVal columnName As String) As String _
      Implements System.ComponentModel.IDataErrorInfo.Item
      Get
        Dim result As String = String.Empty
        If Not IsSelfValid Then
          Dim rule As Validation.BrokenRule = _
            ValidationRules.GetBrokenRules.GetFirstBrokenRule(columnName)
          If rule IsNot Nothing Then
            result = rule.Description()
          End If
        End If
        Return result
      End Get
    End Property

#End Region

#Region " Serialization Notification "

    Sub ISerializationNotification_Deserialized() Implements Serialization.Mobile.ISerializationNotification.Deserialized
      OnDeserializedHandler(New StreamingContext())
    End Sub

    <OnDeserialized()> _
    Private Sub OnDeserializedHandler(ByVal context As StreamingContext)
      ValidationRules.SetTarget(Me)
      If _fieldManager IsNot Nothing Then
        FieldManager.SetPropertyList(Me.GetType)
      End If
      InitializeBusinessRules()
      InitializeAuthorizationRules()
      FieldDataDeserialized()
      OnDeserialized(context)
    End Sub

    ''' <summary>
    ''' This method is called on a newly deserialized object
    ''' after deserialization is complete.
    ''' </summary>
    ''' <param name="context">Serialization context object.</param>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub OnDeserialized(ByVal context As StreamingContext)
    End Sub

#End Region

#Region "Bubbling event Hooks"

    ''' <summary>
    ''' For internal use.
    ''' </summary>
    ''' <param name="child">Child object.</param>
    <EditorBrowsable(EditorBrowsableState.Never)> _
    Protected Sub AddEventHooks(ByVal child As IBusinessObject)
      OnAddEventHooks(child)
    End Sub

    ''' <summary>
    ''' Hook child object events.
    ''' </summary>
    ''' <param name="child">Child object.</param>
    <EditorBrowsable(EditorBrowsableState.Never)> _
    Protected Overridable Sub OnAddEventHooks(ByVal child As IBusinessObject)
      Dim busy As INotifyBusy = TryCast(child, INotifyBusy)
      If busy IsNot Nothing Then
        AddHandler busy.BusyChanged, AddressOf Child_BusyChanged
      End If

      Dim unhandled As INotifyUnhandledAsyncException = TryCast(child, INotifyUnhandledAsyncException)
      If unhandled IsNot Nothing Then
        AddHandler unhandled.UnhandledAsyncException, AddressOf Child_UnhandledAsyncException
      End If

      Dim pc As INotifyPropertyChanged = TryCast(child, INotifyPropertyChanged)
      If pc IsNot Nothing Then
        AddHandler pc.PropertyChanged, AddressOf Child_PropertyChanged
      End If

      Dim bl As IBindingList = TryCast(child, IBindingList)
      If bl IsNot Nothing Then
        AddHandler bl.ListChanged, AddressOf Child_ListChanged
      End If

      Dim cc As INotifyChildChanged = TryCast(child, INotifyChildChanged)
      If cc IsNot Nothing Then
        AddHandler cc.ChildChanged, AddressOf Child_Changed
      End If

    End Sub

    ''' <summary>
    ''' For internal use only.
    ''' </summary>
    ''' <param name="child">Child object.</param>
    <EditorBrowsable(EditorBrowsableState.Never)> _
    Protected Sub RemoveEventHooks(ByVal child As IBusinessObject)
      OnRemoveEventHooks(child)
    End Sub

    ''' <summary>
    ''' Unhook child object events.
    ''' </summary>
    ''' <param name="child">Child object.</param>
    <EditorBrowsable(EditorBrowsableState.Never)> _
    Protected Overridable Sub OnRemoveEventHooks(ByVal child As IBusinessObject)
      Dim busy As INotifyBusy = DirectCast(child, INotifyBusy)
      If busy IsNot Nothing Then
        RemoveHandler busy.BusyChanged, AddressOf Child_BusyChanged
      End If

      Dim unhandled As INotifyUnhandledAsyncException = DirectCast(child, INotifyUnhandledAsyncException)
      If unhandled IsNot Nothing Then
        RemoveHandler unhandled.UnhandledAsyncException, AddressOf Child_UnhandledAsyncException
      End If

      Dim pc As INotifyPropertyChanged = DirectCast(child, INotifyPropertyChanged)
      If pc IsNot Nothing Then
        RemoveHandler pc.PropertyChanged, AddressOf Child_PropertyChanged
      End If

      Dim bl As IBindingList = DirectCast(child, IBindingList)
      If bl IsNot Nothing Then
        RemoveHandler bl.ListChanged, AddressOf Child_ListChanged
      End If

      Dim cc As INotifyChildChanged = DirectCast(child, INotifyChildChanged)
      If cc IsNot Nothing Then
        RemoveHandler cc.ChildChanged, AddressOf Child_Changed
      End If
    End Sub

#End Region

#Region "Busy / Unhandled exception bubbling"

    Private Sub Child_UnhandledAsyncException(ByVal sender As Object, ByVal e As ErrorEventArgs)
      OnUnhandledAsyncException(e)
    End Sub

    Private Sub Child_BusyChanged(ByVal sender As Object, ByVal e As BusyChangedEventArgs)
      OnBusyChanged(e)
    End Sub

#End Region

#Region "IEditableBusinessObject Members"

    Property IEditableBusinessObject_EditLevelAdded() As Integer Implements IEditableBusinessObject.EditLevelAdded
      Get
        Return Me.EditLevelAdded
      End Get
      Set(ByVal value As Integer)
        Me.EditLevelAdded = value
      End Set
    End Property

    Private Sub IEditableBusinessObject_DeleteChild() Implements IEditableBusinessObject.DeleteChild
      Me.DeleteChild()
    End Sub

    Private Sub IEditableBusinessObject_SetParent(ByVal parent As IParent) Implements IEditableBusinessObject.SetParent
      Me.SetParent(parent)
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

      If _bypassPropertyChecks OrElse CanReadProperty(propertyName, noAccess = Security.NoAccessBehavior.ThrowException) Then
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
    Protected Function GetProperty(Of P)(ByVal propertyInfo As PropertyInfo(Of P)) As P

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
    Protected Function GetProperty(Of P)(ByVal propertyInfo As PropertyInfo(Of P), ByVal noAccess As Security.NoAccessBehavior) As P

      Dim result As P = Nothing
      If _bypassPropertyChecks OrElse CanReadProperty(propertyInfo.Name, noAccess = Security.NoAccessBehavior.ThrowException) Then
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
      If _bypassPropertyChecks OrElse CanReadProperty(propertyInfo.Name, False) Then
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

      If (((propertyInfo.RelationshipType And RelationshipTypes.LazyLoad) <> 0) AndAlso Not FieldManager.FieldExists(propertyInfo)) Then
        Throw New InvalidOperationException(My.Resources.PropertyGetNotAllowed)
      End If

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
    Protected Function ReadProperty(ByVal propertyInfo As IPropertyInfo) As Object '' Implements IManageProperties.ReadProperty

      If (((propertyInfo.RelationshipType And RelationshipTypes.LazyLoad) <> 0) AndAlso Not FieldManager.FieldExists(propertyInfo)) Then
        Throw New InvalidOperationException(My.Resources.PropertyGetNotAllowed)
      End If

      Dim info = FieldManager.GetFieldData(propertyInfo)
      If info IsNot Nothing Then
        Return info.Value
      Else
        Return Nothing
      End If
    End Function

#End Region

#Region " Set Properties "

    ''' <summary>
    ''' Sets a property's backing field with the supplied
    ''' value, first checking authorization, and then
    ''' calling PropertyHasChanged if the value does change.
    ''' </summary>
    ''' <param name="field">
    ''' A reference to the backing field for the property.</param>
    ''' <param name="newValue">
    ''' The new value for the property.</param>
    ''' <param name="propertyInfo">
    ''' <see cref="PropertyInfo" /> object containing property metadata.</param>
    ''' <remarks>
    ''' If the user is not authorized to change the property, this
    ''' overload throws a SecurityException.
    ''' </remarks>
    Protected Sub SetProperty(Of P)(ByVal propertyInfo As PropertyInfo(Of P), ByRef field As P, ByVal newValue As P)

      SetProperty(Of P)(propertyInfo.Name, field, newValue, Security.NoAccessBehavior.ThrowException)

    End Sub

    ''' <summary>
    ''' Sets a property's backing field with the supplied
    ''' value, first checking authorization, and then
    ''' calling PropertyHasChanged if the value does change.
    ''' </summary>
    ''' <param name="field">
    ''' A reference to the backing field for the property.</param>
    ''' <param name="newValue">
    ''' The new value for the property.</param>
    ''' <param name="propertyName">
    ''' The name of the property.</param>
    ''' <remarks>
    ''' If the user is not authorized to change the property, this
    ''' overload throws a SecurityException.
    ''' </remarks>
    Protected Sub SetProperty(Of P)(ByVal propertyName As String, ByRef field As P, ByVal newValue As P)

      SetProperty(Of P)(propertyName, field, newValue, Security.NoAccessBehavior.ThrowException)

    End Sub

    ''' <summary>
    ''' Sets a property's backing field with the 
    ''' supplied value, first checking authorization, and then
    ''' calling PropertyHasChanged if the value does change.
    ''' </summary>
    ''' <typeparam name="P">
    ''' Type of the field being set.
    ''' </typeparam>
    ''' <typeparam name="V">
    ''' Type of the value provided to the field.
    ''' </typeparam>
    ''' <param name="field">
    ''' A reference to the backing field for the property.</param>
    ''' <param name="newValue">
    ''' The new value for the property.</param>
    ''' <param name="propertyInfo">
    ''' <see cref="PropertyInfo" /> object containing property metadata.</param>
    ''' <remarks>
    ''' If the user is not authorized to change the property, this
    ''' overload throws a SecurityException.
    ''' </remarks>
    Protected Sub SetPropertyConvert(Of P, V)(ByVal propertyInfo As PropertyInfo(Of P), ByRef field As P, ByVal newValue As V)

      SetPropertyConvert(Of P, V)(propertyInfo, field, newValue, Security.NoAccessBehavior.ThrowException)

    End Sub

    ''' <summary>
    ''' Sets a property's backing field with the 
    ''' supplied value, first checking authorization, and then
    ''' calling PropertyHasChanged if the value does change.
    ''' </summary>
    ''' <typeparam name="P">
    ''' Type of the field being set.
    ''' </typeparam>
    ''' <typeparam name="V">
    ''' Type of the value provided to the field.
    ''' </typeparam>
    ''' <param name="field">
    ''' A reference to the backing field for the property.</param>
    ''' <param name="newValue">
    ''' The new value for the property.</param>
    ''' <param name="propertyInfo">
    ''' <see cref="PropertyInfo" /> object containing property metadata.</param>
    ''' <param name="noAccess">
    ''' True if an exception should be thrown when the
    ''' user is not authorized to change this property.</param>
    ''' <remarks>
    ''' If the field value is of type string, any incoming
    ''' null values are converted to string.Empty.
    ''' </remarks>
    Protected Sub SetPropertyConvert(Of P, V)(ByVal propertyInfo As PropertyInfo(Of P), ByRef field As P, ByVal newValue As V, ByVal noAccess As Security.NoAccessBehavior)

      SetPropertyConvert(Of P, V)(propertyInfo.Name, field, newValue, noAccess)

    End Sub

    ''' <summary>
    ''' Sets a property's backing field with the supplied
    ''' value, first checking authorization, and then
    ''' calling PropertyHasChanged if the value does change.
    ''' </summary>
    ''' <param name="field">
    ''' A reference to the backing field for the property.</param>
    ''' <param name="newValue">
    ''' The new value for the property.</param>
    ''' <param name="propertyName">
    ''' The name of the property.</param>
    ''' <param name="noAccess">
    ''' True if an exception should be thrown when the
    ''' user is not authorized to change this property.</param>
    Protected Sub SetProperty(Of P)(ByVal propertyName As String, ByRef field As P, ByVal newValue As P, ByVal noAccess As Security.NoAccessBehavior)
      Try
        If _bypassPropertyChecks OrElse CanWriteProperty(propertyName, noAccess = Security.NoAccessBehavior.ThrowException) Then
          Dim doChange As Boolean = False

          If field Is Nothing Then
            If newValue IsNot Nothing Then
              doChange = True
            End If

          Else
            If GetType(P) Is GetType(String) AndAlso newValue Is Nothing Then _
              newValue = CoerceValue(Of P)(GetType(String), field, String.Empty)

            If Not field.Equals(newValue) Then _
              doChange = True
          End If

          If doChange Then
            If Not _bypassPropertyChecks Then OnPropertyChanging(propertyName)
            field = newValue
            If Not _bypassPropertyChecks Then PropertyHasChanged(propertyName)
          End If
        End If
      Catch sec As System.Security.SecurityException
        Throw
      Catch ex As Exception
        Throw New PropertyLoadException(String.Format(My.Resources.PropertyLoadException, propertyName, ex.Message, ex.Message))
      End Try

    End Sub

    ''' <summary>
    ''' Sets a property's backing field with the 
    ''' supplied value, first checking authorization, and then
    ''' calling PropertyHasChanged if the value does change.
    ''' </summary>
    ''' <typeparam name="P">
    ''' Type of the field being set.
    ''' </typeparam>
    ''' <typeparam name="V">
    ''' Type of the value provided to the field.
    ''' </typeparam>
    ''' <param name="field">
    ''' A reference to the backing field for the property.</param>
    ''' <param name="newValue">
    ''' The new value for the property.</param>
    ''' <param name="propertyName">
    ''' The name of the property.</param>
    ''' <param name="noAccess">
    ''' True if an exception should be thrown when the
    ''' user is not authorized to change this property.</param>
    ''' <remarks>
    ''' If the field value is of type string, any incoming
    ''' null values are converted to string.Empty.
    ''' </remarks>
    Protected Sub SetPropertyConvert(Of P, V)(ByVal propertyName As String, ByRef field As P, ByVal newValue As V, ByVal noAccess As Security.NoAccessBehavior)
      Try
        If _bypassPropertyChecks OrElse CanWriteProperty(propertyName, noAccess = Security.NoAccessBehavior.ThrowException) Then
          Dim doChange As Boolean = False

          If field Is Nothing Then
            If newValue IsNot Nothing Then doChange = True

          Else
            If GetType(V) Is GetType(String) AndAlso newValue Is Nothing Then
              newValue = Utilities.CoerceValue(Of V)(GetType(String), Nothing, String.Empty)
            End If
            If Not field.Equals(newValue) Then
              doChange = True
            End If
          End If

          If doChange Then
            If Not _bypassPropertyChecks Then OnPropertyChanging(propertyName)
            field = Utilities.CoerceValue(Of P)(GetType(V), field, newValue)
            If Not _bypassPropertyChecks Then PropertyHasChanged(propertyName)
          End If
        End If
      Catch sec As System.Security.SecurityException
        Throw
      Catch ex As Exception
        Throw New PropertyLoadException(String.Format(My.Resources.PropertyLoadException, propertyName, ex.Message))
      End Try

    End Sub

    ''' <summary>
    ''' Sets a property's managed field with the 
    ''' supplied value, first checking authorization, and then
    ''' calling PropertyHasChanged if the value does change.
    ''' </summary>
    ''' <typeparam name="P">Property type.</typeparam>
    ''' <param name="propertyInfo">
    ''' <see cref="PropertyInfo" /> object containing property metadata.</param>
    ''' <param name="newValue">
    ''' The new value for the property.</param>
    ''' <remarks>
    ''' If the user is not authorized to change the property, this
    ''' overload throws a SecurityException.
    ''' </remarks>
    Protected Sub SetProperty(Of P)(ByVal propertyInfo As PropertyInfo(Of P), ByVal newValue As P)

      SetProperty(Of P)(propertyInfo, newValue, Security.NoAccessBehavior.ThrowException)

    End Sub

    ''' <summary>
    ''' Sets a property's managed field with the 
    ''' supplied value, first checking authorization, and then
    ''' calling PropertyHasChanged if the value does change.
    ''' </summary>
    ''' <param name="propertyInfo">
    ''' <see cref="PropertyInfo" /> object containing property metadata.</param>
    ''' <param name="newValue">
    ''' The new value for the property.</param>
    ''' <remarks>
    ''' If the user is not authorized to change the property, this
    ''' overload throws a SecurityException.
    ''' </remarks>
    Protected Sub SetPropertyConvert(Of P, F)(ByVal propertyInfo As PropertyInfo(Of P), ByVal newValue As F)

      SetPropertyConvert(Of P, F)(propertyInfo, newValue, Security.NoAccessBehavior.ThrowException)

    End Sub

    ''' <summary>
    ''' Sets a property's managed field with the 
    ''' supplied value, first checking authorization, and then
    ''' calling PropertyHasChanged if the value does change.
    ''' </summary>
    ''' <param name="propertyInfo">
    ''' <see cref="PropertyInfo" /> object containing property metadata.</param>
    ''' <param name="newValue">
    ''' The new value for the property.</param>
    ''' <param name="noAccess">
    ''' True if an exception should be thrown when the
    ''' user is not authorized to change this property.</param>
    Protected Sub SetPropertyConvert(Of P, F)(ByVal propertyInfo As PropertyInfo(Of P), ByVal newValue As F, ByVal noAccess As Security.NoAccessBehavior)
      Try
        If _bypassPropertyChecks OrElse CanWriteProperty(propertyInfo.Name, noAccess = Security.NoAccessBehavior.ThrowException) Then
          Dim oldValue As P = Nothing
          Dim fieldData = FieldManager.GetFieldData(propertyInfo)
          If fieldData Is Nothing Then
            oldValue = propertyInfo.DefaultValue
            fieldData = FieldManager.LoadFieldData(Of P)(propertyInfo, oldValue)

          Else
            Dim fd = TryCast(fieldData, FieldManager.IFieldData(Of P))
            If fd IsNot Nothing Then
              oldValue = fd.Value

            Else
              oldValue = DirectCast(fieldData.Value, P)
            End If
          End If

          If GetType(F) Is GetType(String) AndAlso newValue Is Nothing Then
            newValue = CoerceValue(Of F)(GetType(String), Nothing, String.Empty)
          End If
          LoadPropertyValue(Of P)(propertyInfo, oldValue, CoerceValue(Of P)(GetType(F), oldValue, newValue), Not _bypassPropertyChecks)

        End If
      Catch sec As System.Security.SecurityException
        Throw
      Catch ex As Exception
        Throw New PropertyLoadException(String.Format(My.Resources.PropertyLoadException, propertyInfo.Name, ex.Message))
      End Try

    End Sub

    ''' <summary>
    ''' Sets a property's managed field with the 
    ''' supplied value, first checking authorization, and then
    ''' calling PropertyHasChanged if the value does change.
    ''' </summary>
    ''' <typeparam name="P">
    ''' Type of the property.
    ''' </typeparam>
    ''' <param name="propertyInfo">
    ''' <see cref="PropertyInfo" /> object containing property metadata.</param>
    ''' <param name="newValue">
    ''' The new value for the property.</param>
    ''' <param name="noAccess">
    ''' True if an exception should be thrown when the
    ''' user is not authorized to change this property.</param>
    Protected Sub SetProperty(Of P)( _
      ByVal propertyInfo As PropertyInfo(Of P), ByVal newValue As P, ByVal noAccess As Security.NoAccessBehavior)

      If _bypassPropertyChecks OrElse CanWriteProperty(propertyInfo.Name, noAccess = Security.NoAccessBehavior.ThrowException) Then
        Try
          Dim oldValue As P = Nothing
          Dim fieldData = FieldManager.GetFieldData(propertyInfo)
          If fieldData Is Nothing Then
            oldValue = propertyInfo.DefaultValue
            fieldData = FieldManager.LoadFieldData(Of P)(propertyInfo, oldValue)

          Else
            Dim fd = TryCast(fieldData, FieldManager.IFieldData(Of P))
            If fd IsNot Nothing Then
              oldValue = fd.Value

            Else
              oldValue = DirectCast(fieldData.Value, P)
            End If
          End If

          If GetType(P) Is GetType(String) AndAlso newValue Is Nothing Then
            newValue = CoerceValue(Of P)(GetType(String), Nothing, String.Empty)
          End If

          LoadPropertyValue(Of P)(propertyInfo, oldValue, newValue, Not _bypassPropertyChecks)

        Catch ex As Exception
          Throw New PropertyLoadException(String.Format(My.Resources.PropertyLoadException, propertyInfo.Name, ex.Message))
        End Try
      End If
    End Sub

    ''' <summary>
    ''' Sets a property's managed field with the 
    ''' supplied value, and then
    ''' calls PropertyHasChanged if the value does change.
    ''' </summary>
    ''' <param name="propertyInfo">
    ''' PropertyInfo object containing property metadata.</param>
    ''' <param name="newValue">
    ''' The new value for the property.</param>
    ''' <remarks>
    ''' If the user is not authorized to change the 
    ''' property a SecurityException is thrown.
    ''' </remarks>
    Protected Sub SetProperty(ByVal propertyInfo As IPropertyInfo, ByVal newValue As Object)

      Try
        If _bypassPropertyChecks OrElse CanWriteProperty(propertyInfo.Name, True) Then
          If Not _bypassPropertyChecks Then
            OnPropertyChanging(propertyInfo.Name)
          End If
          FieldManager.SetFieldData(propertyInfo, newValue)
          If Not _bypassPropertyChecks Then
            PropertyHasChanged(propertyInfo.Name)
          End If
        End If
      Catch sec As System.Security.SecurityException
        Throw
      Catch ex As Exception
        Throw New PropertyLoadException(String.Format(My.Resources.PropertyLoadException, propertyInfo.Name, ex.Message), ex)
      End Try


    End Sub

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
          fieldData = FieldManager.LoadFieldData(Of P)(propertyInfo, oldValue)

        Else
          Dim fd = TryCast(fieldData, FieldManager.IFieldData(Of P))
          If fd IsNot Nothing Then
            oldValue = fd.Value

          Else
            oldValue = DirectCast(fieldData.Value, P)
          End If
        End If
        LoadPropertyValue(Of P)(propertyInfo, oldValue, CoerceValue(Of P)(GetType(F), oldValue, newValue), False)

      Catch ex As Exception
        Throw New PropertyLoadException(String.Format(My.Resources.PropertyLoadException, propertyInfo.Name, ex.Message))
      End Try

    End Sub

    Private Sub IManageProperties_LoadProperty(Of P)(ByVal propertyInfo As PropertyInfo(Of P), ByVal newValue As P) Implements Core.IManageProperties.LoadProperty
      LoadProperty(Of P)(propertyInfo, newValue)
    End Sub

    Private Function FieldExists(ByVal [property] As Core.IPropertyInfo) As Boolean Implements Core.IManageProperties.FieldExists
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
    Protected Sub LoadProperty(Of P)(ByVal propertyInfo As PropertyInfo(Of P), ByVal newValue As P)

      Try
        Dim oldValue As P = Nothing
        Dim fieldData = FieldManager.GetFieldData(propertyInfo)
        If fieldData Is Nothing Then
          oldValue = propertyInfo.DefaultValue
          fieldData = FieldManager.LoadFieldData(Of P)(propertyInfo, oldValue)

        Else
          Dim fd = TryCast(fieldData, FieldManager.IFieldData(Of P))
          If fd IsNot Nothing Then
            oldValue = fd.Value

          Else
            oldValue = DirectCast(fieldData.Value, P)
          End If
        End If

        LoadPropertyValue(Of P)(propertyInfo, oldValue, newValue, False)

      Catch ex As Exception
        Throw New PropertyLoadException(String.Format(My.Resources.PropertyLoadException, propertyInfo.Name, ex.Message))
      End Try

    End Sub

    Private Sub LoadPropertyValue(Of P)( _
      ByVal propertyInfo As PropertyInfo(Of P), _
      ByVal oldValue As P, _
      ByVal newValue As P, _
      ByVal markDirty As Boolean)

      Dim valuesDiffer As Boolean = False
      If oldValue Is Nothing Then
        valuesDiffer = newValue IsNot Nothing

      Else
        valuesDiffer = Not (oldValue.Equals(newValue))
      End If

      If valuesDiffer Then
        'von: VB cannot explicitly convert Value Types to IBusinessObject.
        'So let us check the Type of P first then do the conversion later.
        If GetType(P) Is GetType(IBusinessObject) Then
          Dim old As IBusinessObject = DirectCast(oldValue, IBusinessObject)
          If old IsNot Nothing Then
            RemoveEventHooks(old)
          End If
        End If

        'von: VB cannot explicitly convert Value Types to IBusinessObject.
        'So let us check the Type of P first then do the conversion later.
        If GetType(P) Is GetType(IBusinessObject) Then
          Dim [new] As IBusinessObject = DirectCast(newValue, IBusinessObject)
          If [new] IsNot Nothing Then
            AddEventHooks([new])
          End If
        End If

        If GetType(IEditableBusinessObject).IsAssignableFrom(propertyInfo.Type) Then
          '' remove old event hook
          'If oldValue IsNot Nothing Then
          '  Dim pc As INotifyPropertyChanged = DirectCast(oldValue, INotifyPropertyChanged)
          '  RemoveHandler pc.PropertyChanged, AddressOf Child_PropertyChanged
          'End If

          If markDirty Then
            OnPropertyChanging(propertyInfo.Name)
            FieldManager.SetFieldData(Of P)(propertyInfo, newValue)
            PropertyHasChanged(propertyInfo.Name)
          Else
            FieldManager.LoadFieldData(Of P)(propertyInfo, newValue)
          End If
          Dim child As IEditableBusinessObject = DirectCast(newValue, IEditableBusinessObject)
          If child IsNot Nothing Then
            child.SetParent(Me)
            ' set child edit level
            UndoableBase.ResetChildEditLevel(child, Me.EditLevel, Me.BindingEdit)
            ' reset EditLevelAdded 
            child.EditLevelAdded = Me.EditLevel
            '' hook child event
            'Dim pc As INotifyPropertyChanged = DirectCast(newValue, INotifyPropertyChanged)
            'AddHandler pc.PropertyChanged, AddressOf Child_PropertyChanged
          End If

        ElseIf GetType(IEditableCollection).IsAssignableFrom(propertyInfo.Type) Then
          '' remove old event hooks
          'If oldValue IsNot Nothing Then
          '  Dim pc As IBindingList = DirectCast(oldValue, IBindingList)
          '  RemoveHandler pc.ListChanged, AddressOf Child_ListChanged
          'End If
          If markDirty Then
            OnPropertyChanging(propertyInfo.Name)
            FieldManager.SetFieldData(Of P)(propertyInfo, newValue)
            PropertyHasChanged(propertyInfo.Name)
          Else
            FieldManager.LoadFieldData(Of P)(propertyInfo, newValue)
          End If
          Dim child As IEditableCollection = DirectCast(newValue, IEditableCollection)
          If child IsNot Nothing Then
            child.SetParent(Me)
            Dim undoChild As IUndoableObject = TryCast(child, IUndoableObject)
            If undoChild IsNot Nothing Then
              ' set child edit level
              UndoableBase.ResetChildEditLevel(undoChild, Me.EditLevel, Me.BindingEdit)
            End If
            'Dim pc As IBindingList = DirectCast(newValue, IBindingList)
            'AddHandler pc.ListChanged, AddressOf Child_ListChanged
          End If

        Else
          If markDirty Then
            OnPropertyChanging(propertyInfo.Name)
            FieldManager.SetFieldData(Of P)(propertyInfo, newValue)
            PropertyHasChanged(propertyInfo.Name)
          Else
            FieldManager.LoadFieldData(Of P)(propertyInfo, newValue)
          End If
        End If
      End If

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

    <NonSerialized()> _
    <NotUndoable()> _
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
    Protected Sub LoadPropertyAsync(Of R, P)(ByVal [property] As PropertyInfo(Of R), ByVal factory As AsyncFactoryDelegate(Of R, P), ByVal [parameter] As P)
      Dim actionLoadProperty As Action(Of IPropertyInfo, Object) = AddressOf LoadProperty
      Dim actionOnPropertyChanged As Action(Of String) = AddressOf OnPropertyChanged
      Dim loader As AsyncLoader = New AsyncLoader([property], factory, actionLoadProperty, actionOnPropertyChanged, parameter)
      Dim actionLoadComplete As Action(Of Object, DataPortalResult(Of R)) = AddressOf loader.LoadComplete(Of R)
      LoadManager.BeginLoad(loader, actionLoadComplete)
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

#Region "IsBusy / IsIdle"

    <NonSerialized()> _
    <NotUndoable()> _
    Private _isBusy As Boolean

    ''' <summary>
    ''' Mark the object as busy (it is
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
    ''' Mark the object as not busy (it is
    ''' not running an async operation).
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Sub MarkIdle()
      _isBusy = False
      OnBusyChanged(New BusyChangedEventArgs("", False))
    End Sub

    ''' <summary>
    ''' Gets a value indicating if this
    ''' object or its child objects are
    ''' busy.
    ''' </summary>
    <Browsable(False)> _
    Public ReadOnly Property IsBusy() As Boolean Implements INotifyBusy.IsBusy
      Get
        Return (IsSelfBusy OrElse (_fieldManager IsNot Nothing And FieldManager.IsBusy()))
      End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating if this
    ''' object is busy.
    ''' </summary>
    <Browsable(False)> _
    Public ReadOnly Property IsSelfBusy() As Boolean Implements INotifyBusy.IsSelfBusy
      Get
        Return (_isBusy OrElse ValidationRules.IsValidating OrElse LoadManager.IsLoading)
      End Get
    End Property

    <NonSerialized()> _
    <NotUndoable()> _
    Private _busyChanged As BusyChangedEventHandler

    ''' <summary>
    ''' Event indicating that the IsBusy property has changed.
    ''' </summary>
    Public Custom Event BusyChanged As BusyChangedEventHandler Implements INotifyBusy.BusyChanged
      AddHandler(ByVal value As BusyChangedEventHandler)
        _busyChanged = CType(System.Delegate.Combine(_busyChanged, value), BusyChangedEventHandler)
      End AddHandler

      RemoveHandler(ByVal value As BusyChangedEventHandler)
        _busyChanged = CType(System.Delegate.Remove(_busyChanged, value), BusyChangedEventHandler)
      End RemoveHandler

      RaiseEvent(ByVal sender As Object, ByVal e As BusyChangedEventArgs)
        If _busyChanged IsNot Nothing Then
          _busyChanged.Invoke(Me, e)
        End If
      End RaiseEvent

    End Event

    ''' <summary>
    ''' Raise the BusyChanged event.
    ''' </summary>
    ''' <param name="args">Event args.</param>
    Protected Sub OnBusyChanged(ByVal args As BusyChangedEventArgs)
      RaiseEvent BusyChanged(Me, args)
    End Sub

    ''' <summary>
    ''' Gets a value indicating whether a
    ''' specific property is busy (has a
    ''' currently executing async rule).
    ''' </summary>
    ''' <param name="propertyName">
    ''' Name of the property.
    ''' </param>
    Public Function IsPropertyBusy(ByVal propertyName As String) As Boolean
      Dim isbusy As Boolean = False
      If _validationRules IsNot Nothing Then
        SyncLock _validationRules.ValidatingRules
          isbusy = (From rules In _validationRules.ValidatingRules _
                    From [property] In rules.AsyncRuleArgs.Properties _
                    Where [property].Name = propertyName _
                   Select rules).Count() > 0
        End SyncLock
      End If

      Return isbusy
    End Function

#End Region

#Region " INotifyUnhandledAsyncException Members "

    <NonSerialized()> _
    <NotUndoable()> _
    Private _unhandledAsyncException As EventHandler(Of ErrorEventArgs)

    ''' <summary>
    ''' Event indicating that an exception occurred during
    ''' the processing of an async operation.
    ''' </summary>
    Public Custom Event UnhandledAsyncException As EventHandler(Of ErrorEventArgs) Implements INotifyUnhandledAsyncException.UnhandledAsyncException
      AddHandler(ByVal value As EventHandler(Of ErrorEventArgs))
        _unhandledAsyncException = CType(System.Delegate.Combine(_unhandledAsyncException, value), EventHandler(Of ErrorEventArgs))
      End AddHandler

      RemoveHandler(ByVal value As EventHandler(Of ErrorEventArgs))
        _unhandledAsyncException = CType(System.Delegate.Remove(_unhandledAsyncException, value), EventHandler(Of ErrorEventArgs))
      End RemoveHandler

      RaiseEvent(ByVal sender As Object, ByVal e As ErrorEventArgs)
        If _unhandledAsyncException IsNot Nothing Then
          _unhandledAsyncException.Invoke(Me, e)
        End If
      End RaiseEvent
    End Event

    ''' <summary>
    ''' Raises the UnhandledAsyncException event.
    ''' </summary>
    ''' <param name="error">Args parameter.</param>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub OnUnhandledAsyncException(ByVal [error] As ErrorEventArgs)
      RaiseEvent UnhandledAsyncException(Me, [error])
    End Sub

    ''' <summary>
    ''' Raises the UnhandledAsyncException event.
    ''' </summary>
    ''' <param name="originalSender">Original sender of
    ''' the event.</param>
    ''' <param name="error">Exception object.</param>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Sub OnUnhandledAsyncException(ByVal originalSender As Object, ByVal [error] As Exception)
      OnUnhandledAsyncException(New ErrorEventArgs(originalSender, [error]))
    End Sub

#End Region

#Region " Child Change Notification "

    <NonSerialized(), NotUndoable()> _
    Private _childChangedHandlers As EventHandler(Of Core.ChildChangedEventArgs)

    ''' <summary>
    ''' Event raised when a child object has been changed.
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
    Public Custom Event ChildChanged As EventHandler(Of Core.ChildChangedEventArgs) Implements INotifyChildChanged.ChildChanged
      AddHandler(ByVal value As EventHandler(Of Core.ChildChangedEventArgs))
        _childChangedHandlers = CType(System.Delegate.Combine(_childChangedHandlers, value), EventHandler(Of Csla.Core.ChildChangedEventArgs))
      End AddHandler

      RemoveHandler(ByVal value As EventHandler(Of Core.ChildChangedEventArgs))
        _childChangedHandlers = CType(System.Delegate.Remove(_childChangedHandlers, value), EventHandler(Of Csla.Core.ChildChangedEventArgs))
      End RemoveHandler

      RaiseEvent(ByVal sender As System.Object, ByVal e As Core.ChildChangedEventArgs)
        If _childChangedHandlers IsNot Nothing Then
          _childChangedHandlers.Invoke(sender, e)
        End If
      End RaiseEvent
    End Event

    ''' <summary>
    ''' Raises the ChildChanged event, indicating that a child
    ''' object has been changed.
    ''' </summary>
    ''' <param name="e">
    ''' ChildChangedEventArgs object.
    ''' </param>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub OnChildChanged(ByVal e As ChildChangedEventArgs)
      RaiseEvent ChildChanged(Me, e)
    End Sub

    ''' <summary>
    ''' Creates a ChildChangedEventArgs and raises the event.
    ''' </summary>
    Private Sub RaiseChildChanged(ByVal childObject As Object, ByVal propertyArgs As PropertyChangedEventArgs, ByVal listArgs As ListChangedEventArgs)
      Dim args As ChildChangedEventArgs = New ChildChangedEventArgs(childObject, propertyArgs, listArgs)
      OnChildChanged(args)
    End Sub

    ''' <summary>
    ''' Handles any PropertyChanged event from 
    ''' a child object and echoes it up as
    ''' a ChildChanged event.
    ''' </summary>
    Private Sub Child_PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs)
      RaiseChildChanged(sender, e, Nothing)
    End Sub

    ''' <summary>
    ''' Handles any ListChanged event from 
    ''' a child list and echoes it up as
    ''' a ChildChanged event.
    ''' </summary>
    Private Sub Child_ListChanged(ByVal sender As Object, ByVal e As ListChangedEventArgs)
      If e.ListChangedType <> ListChangedType.ItemChanged Then
        RaiseChildChanged(sender, Nothing, e)
      End If
    End Sub

    ''' <summary>
    ''' Handles any ChildChanged event from
    ''' a child object and echoes it up as
    ''' a ChildChanged event.
    ''' </summary>
    Private Sub Child_Changed(ByVal sender As Object, ByVal e As ChildChangedEventArgs)
      RaiseChildChanged(e.ChildObject, e.PropertyChangedArgs, e.ListChangedArgs)
    End Sub

#End Region

#Region " Field Manager "

    Private _fieldManager As FieldManager.FieldDataManager

    ''' <summary>
    ''' Gets the PropertyManager object for this
    ''' business object.
    ''' </summary>
    Protected ReadOnly Property FieldManager() As FieldManager.FieldDataManager
      Get
        If _fieldManager Is Nothing Then
          _fieldManager = New FieldManager.FieldDataManager(Me.GetType)
          UndoableBase.ResetChildEditLevel(_fieldManager, Me.EditLevel, Me.BindingEdit)
        End If
        Return _fieldManager
      End Get
    End Property

    Private Sub FieldDataDeserialized()

      For Each item As Object In FieldManager.GetChildren()
        Dim business As IBusinessObject = TryCast(item, IBusinessObject)
        If business IsNot Nothing Then
          OnAddEventHooks(business)
        End If

        Dim child As IEditableBusinessObject = TryCast(item, IEditableBusinessObject)
        If child IsNot Nothing Then
          child.SetParent(Me)
        End If

        Dim childCollection As IEditableCollection = TryCast(item, IEditableCollection)
        If childCollection IsNot Nothing Then
          childCollection.SetParent(Me)
        End If
      Next

    End Sub

#End Region

#Region " IParent "

    ''' <summary>
    ''' Override this method to be notified when a child object's
    ''' <see cref="Core.BusinessBase.ApplyEdit" /> method has
    ''' completed.
    ''' </summary>
    ''' <param name="child">The child object that was edited.</param>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub EditChildComplete(ByVal child As Core.IEditableBusinessObject)
      ' do nothing, we don't really care
      ' when a child has its edits applied
    End Sub

    Private Sub ApplyEditChild(ByVal child As IEditableBusinessObject) Implements IParent.ApplyEditChild
      Me.EditChildComplete(child)
    End Sub

    Private Sub RemoveChild(ByVal child As IEditableBusinessObject) Implements IParent.RemoveChild
      Dim info = FieldManager.FindProperty(child)
      FieldManager.RemoveField(info)
    End Sub

#End Region

#Region " IDataPortalTarget Members "

    Private Sub CheckRules() Implements Server.IDataPortalTarget.CheckRules
      ValidationRules.CheckRules()
    End Sub

    Private Sub IDataPortalTarget_MarkAsChild() Implements Server.IDataPortalTarget.MarkAsChild
      Me.MarkAsChild()
    End Sub

    Private Sub IDataPortalTarget_MarkNew() Implements Server.IDataPortalTarget.MarkNew
      Me.MarkNew()
    End Sub

    Private Sub IDataPortalTarget_MarkOld() Implements Server.IDataPortalTarget.MarkOld
      Me.MarkOld()
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

#Region " IManageProperties Members "

    Private ReadOnly Property HasManagedProperties() As Boolean Implements IManageProperties.HasManagedProperties
      Get
        Return (_fieldManager IsNot Nothing AndAlso _fieldManager.HasFields)
      End Get
    End Property

    Private Function GetManagedProperties() As System.Collections.Generic.List(Of IPropertyInfo) Implements IManageProperties.GetManagedProperties
      Return FieldManager.GetRegisteredProperties()
    End Function

    Private Function IManageProperties_GetProperty(ByVal propertyInfo As IPropertyInfo) As Object Implements IManageProperties.GetProperty
      Return GetProperty(propertyInfo)
    End Function

    Private Function IManageProperties_ReadProperty(ByVal propertyInfo As IPropertyInfo) As Object Implements IManageProperties.ReadProperty
      Return ReadProperty(propertyInfo)
    End Function

    Private Sub IManageProperties_SetProperty(ByVal propertyInfo As IPropertyInfo, ByVal newValue As Object) Implements IManageProperties.SetProperty
      SetProperty(propertyInfo, newValue)
    End Sub

    Private Sub IManageProperties_LoadProperty(ByVal propertyInfo As IPropertyInfo, ByVal newValue As Object) Implements IManageProperties.LoadProperty
      LoadProperty(propertyInfo, newValue)
    End Sub

#End Region

#Region " MobileFormatter "

    ''' <summary>
    ''' Override this method to insert your field values
    ''' into the MobileFormatter serialzation stream.
    ''' </summary>
    ''' <param name="info">
    ''' Object containing the data to serialize.
    ''' </param>
    ''' <param name="mode">
    ''' The StateMode indicating why this method was invoked.
    ''' </param>
    Protected Overrides Sub OnGetState(ByVal info As Serialization.Mobile.SerializationInfo, ByVal mode As StateMode)
      MyBase.OnGetState(info, mode)
      info.AddValue("Csla.Core.BusinessBase._isNew", _isNew)
      info.AddValue("Csla.Core.BusinessBase._isDeleted", _isDeleted)
      info.AddValue("Csla.Core.BusinessBase._isDirty", _isDirty)
      info.AddValue("Csla.Core.BusinessBase._neverCommitted", _neverCommitted)
      info.AddValue("Csla.Core.BusinessBase._disableIEditableObject", _disableIEditableObject)
      info.AddValue("Csla.Core.BusinessBase._isChild", _isChild)
      info.AddValue("Csla.Core.BusinessBase._editLevelAdded", _editLevelAdded)
    End Sub

    ''' <summary>
    ''' Override this method to retrieve your field values
    ''' from the MobileFormatter serialzation stream.
    ''' </summary>
    ''' <param name="info">
    ''' Object containing the data to serialize.
    ''' </param>
    ''' <param name="mode">
    ''' The StateMode indicating why this method was invoked.
    ''' </param>
    Protected Overrides Sub OnSetState(ByVal info As Serialization.Mobile.SerializationInfo, ByVal mode As StateMode)
      MyBase.OnSetState(info, mode)
      _isNew = info.GetValue(Of Boolean)("Csla.Core.BusinessBase._isNew")
      _isDeleted = info.GetValue(Of Boolean)("Csla.Core.BusinessBase._isDeleted")
      _isDirty = info.GetValue(Of Boolean)("Csla.Core.BusinessBase._isDirty")
      _neverCommitted = info.GetValue(Of Boolean)("Csla.Core.BusinessBase._neverCommitted")
      _disableIEditableObject = info.GetValue(Of Boolean)("Csla.Core.BusinessBase._disableIEditableObject")
      _isChild = info.GetValue(Of Boolean)("Csla.Core.BusinessBase._isChild")
      _editLevelAdded = info.GetValue(Of Integer)("Csla.Core.BusinessBase._editLevelAdded")
    End Sub

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

      If _validationRules IsNot Nothing Then
        Dim vrInfo = formatter.SerializeObject(_validationRules)
        info.AddChild("_validationRules", vrInfo.ReferenceId)
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
        _fieldManager = CType(formatter.GetObject(childData.ReferenceId), Core.FieldManager.FieldDataManager)
      End If

      If info.Children.ContainsKey("_validationRules") Then
        Dim refId As Integer = info.Children("_validationRules").ReferenceId
        _validationRules = CType(formatter.GetObject(refId), Csla.Validation.ValidationRules)
      End If

      MyBase.OnSetChildren(info, formatter)
    End Sub

#End Region

#Region " Property Checks ByPass "

    <NonSerialized()> _
    <NotUndoable()> _
    Private _bypassPropertyChecks As Boolean = False

    ''' <summary>
    ''' By wrapping this property inside Using block
    ''' you can set property values on current business object
    ''' without raising PropertyChanged events
    ''' and checking user rights.
    ''' </summary>
    Protected Friend ReadOnly Property BypassPropertyChecks() As BypassPropertyChecksObject
      Get
        Return New BypassPropertyChecksObject(Me)
      End Get
    End Property

    ''' <summary>
    ''' Class that allows setting of property values on 
    ''' current business object
    ''' without raising PropertyChanged events
    ''' and checking user rights.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)> _
    Protected Friend Class BypassPropertyChecksObject
      Implements IDisposable

      Private _businessObject As BusinessBase

      Private Sub New()

      End Sub

      Friend Sub New(ByVal businessObject As BusinessBase)
        _businessObject = businessObject
        _businessObject._bypassPropertyChecks = True
      End Sub

#Region " IDisposable Members "

      ''' <summary>
      ''' Disposes the object.
      ''' </summary>
      Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
      End Sub

      ''' <summary>
      ''' Disposes the object.
      ''' </summary>
      ''' <param name="dispose">Dispose flag.</param>
      Public Sub Dispose(ByVal dispose As Boolean)
        _businessObject._bypassPropertyChecks = False
        _businessObject = Nothing
      End Sub

#End Region

    End Class
#End Region

  End Class

End Namespace
