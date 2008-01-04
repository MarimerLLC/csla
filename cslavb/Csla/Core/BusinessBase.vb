Imports System.Reflection
Imports System.ComponentModel

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
    Implements ICloneable
    Implements IDataErrorInfo
    Implements Csla.Security.IAuthorizeReadWrite
    Implements IParent

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

#Region " IsNew, IsDeleted, IsDirty "

    ' keep track of whether we are new, deleted or dirty
    Private mIsNew As Boolean = True
    Private mIsDeleted As Boolean
    Private mIsDirty As Boolean = True

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
        Return mIsNew
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
        Return mIsDeleted
      End Get
    End Property

    ''' <summary>
    ''' Returns <see langword="true" /> if this object's data has been changed.
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
        Return mIsDirty OrElse (mFieldManager IsNot Nothing AndAlso FieldManager.IsDirty())
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
      mIsNew = True
      mIsDeleted = False
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
      mIsNew = False
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
      mIsDeleted = True
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
      mIsDirty = True
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
    <Obsolete("Use overload the requires property name")> _
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

      OnPropertyChanging(propertyName)
      ValidationRules.CheckRules(propertyName)
      MarkDirty(True)
      OnPropertyChanged(propertyName)

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

      mIsDirty = False
      If mFieldManager IsNot Nothing Then
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
        Return IsDirty AndAlso IsValid
      End Get
    End Property

#End Region

#Region " Authorization "

    <NotUndoable()> _
    <NonSerialized()> _
    Private mReadResultCache As Dictionary(Of String, Boolean)
    <NotUndoable()> _
    <NonSerialized()> _
    Private mWriteResultCache As Dictionary(Of String, Boolean)
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
    ''' <remarks>
    ''' AddAuthorizationRules is automatically called by CSLA .NET
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
    ''' specified property.
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
      ByVal propertyName As String) As Boolean Implements Csla.Security.IAuthorizeReadWrite.CanReadProperty

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

    ''' <summary>
    ''' Returns <see langword="true" /> if the user is allowed to write the
    ''' calling property.
    ''' </summary>
    ''' <returns><see langword="true" /> if write is allowed.</returns>
    ''' <param name="throwOnFalse">Indicates whether a negative
    ''' result should cause an exception.</param>
    <Obsolete("Use overload requiring explicit property name")> _
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
      ByVal propertyName As String) As Boolean Implements Csla.Security.IAuthorizeReadWrite.CanWriteProperty

      Dim result As Boolean = True

      VerifyAuthorizationCache()

      If mWriteResultCache.ContainsKey(propertyName) Then
        ' cache contains value - get cached value
        result = mWriteResultCache(propertyName)

      Else
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
        mWriteResultCache(propertyName) = result
      End If
      Return result

    End Function

    Private Sub VerifyAuthorizationCache()

      If mReadResultCache Is Nothing Then
        mReadResultCache = New Dictionary(Of String, Boolean)
      End If
      If mWriteResultCache Is Nothing Then
        mWriteResultCache = New Dictionary(Of String, Boolean)
      End If
      If mExecuteResultCache Is Nothing Then
        mExecuteResultCache = New Dictionary(Of String, Boolean)
      End If
      If Not ReferenceEquals(Csla.ApplicationContext.User, mLastPrincipal) Then
        ' the principal has changed - reset the cache
        mReadResultCache.Clear()
        mWriteResultCache.Clear()
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

#Region " Parent/Child link "

    <NotUndoable()> _
    <NonSerialized()> _
    Private mParent As Core.IParent

    ''' <summary>
    ''' Provide access to the parent reference for use
    ''' in child object code.
    ''' </summary>
    ''' <remarks>
    ''' This value will be Nothing for root objects.
    ''' </remarks>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected ReadOnly Property Parent() As Core.IParent
      Get
        Return mParent
      End Get
    End Property

    ''' <summary>
    ''' Used by BusinessListBase as a child object is 
    ''' created to tell the child object about its
    ''' parent.
    ''' </summary>
    ''' <param name="parent">A reference to the parent collection object.</param>
    Friend Sub SetParent(ByVal parent As Core.IParent) Implements IEditableBusinessObject.SetParent

      mParent = parent

    End Sub

#End Region

#Region " IEditableObject "

    Private mNeverCommitted As Boolean = True
    <NotUndoable()> _
    Private mDisableIEditableObject As Boolean

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
        Return mDisableIEditableObject
      End Get
      Set(ByVal value As Boolean)
        mDisableIEditableObject = value
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

      If Not mDisableIEditableObject AndAlso Not BindingEdit Then
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

      If Not mDisableIEditableObject AndAlso BindingEdit Then
        CancelEdit()
        If IsNew AndAlso mNeverCommitted AndAlso _
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
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")> _
    Private Sub IEditableObject_EndEdit() _
      Implements System.ComponentModel.IEditableObject.EndEdit

      If Not mDisableIEditableObject AndAlso BindingEdit Then
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
      mNeverCommitted = False
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
    Private mIsChild As Boolean

    ''' <summary>
    ''' Returns <see langword="true" /> if this is a child (non-root) object.
    ''' </summary>
    Protected Friend ReadOnly Property IsChild() As Boolean
      Get
        Return mIsChild
      End Get
    End Property

    ''' <summary>
    ''' Marks the object as being a child object.
    ''' </summary>
    Protected Sub MarkAsChild()
      mIsChild = True
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
    Friend Sub DeleteChild() Implements IEditableBusinessObject.DeleteChild
      If Not Me.IsChild Then
        Throw New NotSupportedException(My.Resources.NoDeleteRootException)
      End If

      MarkDeleted()

    End Sub

#End Region

#Region " Edit Level Tracking (child only) "

    ' we need to keep track of the edit
    ' level when we were added so if the user
    ' cancels below that level we can be destroyed
    <NotUndoable()> _
    Private mEditLevelAdded As Integer

    ''' <summary>
    ''' Gets or sets the current edit level of the
    ''' object.
    ''' </summary>
    ''' <remarks>
    ''' Allow the collection object to use the
    ''' edit level as needed.
    ''' </remarks>
    Friend Property EditLevelAdded() As Integer Implements IEditableBusinessObject.EditLevelAdded
      Get
        Return mEditLevelAdded
      End Get
      Set(ByVal Value As Integer)
        mEditLevelAdded = Value
      End Set
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

    Private mValidationRules As Validation.ValidationRules

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
        If mValidationRules Is Nothing Then
          mValidationRules = New Validation.ValidationRules(Me)
        End If
        Return mValidationRules
      End Get
    End Property

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
    ''' Returns <see langword="true" /> if the object is currently valid, <see langword="false" /> if the
    ''' object has broken rules or is otherwise invalid.
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
        Return ValidationRules.IsValid AndAlso (mFieldManager Is Nothing OrElse FieldManager.IsValid())
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
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
    Protected Overridable Sub DataPortal_Create()
      Throw New NotSupportedException( _
        My.Resources.CreateNotSupportedException)
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
    ''' <remarks>
    ''' Normally you will overload this method to accept a strongly-typed
    ''' criteria parameter, rather than overriding the method with a
    ''' loosely-typed criteria parameter.
    ''' </remarks>
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

#End Region

#Region " IDataErrorInfo "

    Private ReadOnly Property [Error]() As String _
      Implements System.ComponentModel.IDataErrorInfo.Error
      Get
        If Not IsValid Then
          Return ValidationRules.GetBrokenRules.ToString(Validation.RuleSeverity.Error)

        Else
          Return ""
        End If
      End Get
    End Property

    Private ReadOnly Property Item(ByVal columnName As String) As String _
      Implements System.ComponentModel.IDataErrorInfo.Item
      Get
        Dim result As String = ""
        If Not IsValid Then
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

    <OnDeserialized()> _
    Private Sub OnDeserializedHandler(ByVal context As StreamingContext)

      OnDeserialized(context)
      ValidationRules.SetTarget(Me)
      InitializeBusinessRules()
      InitializeAuthorizationRules()
      FieldDataDeserialized()

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

      Return CoerceValue(Of P)(GetType(F), GetProperty(Of F)(propertyInfo.Name, field, propertyInfo.DefaultValue, False))

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

      Return CoerceValue(Of P)(GetType(F), GetProperty(Of F)(propertyInfo.Name, field, propertyInfo.DefaultValue, throwOnNoAccess))

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

      Return CoerceValue(Of P)(GetType(F), GetProperty(Of F)(propertyInfo, False))

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

      Return CoerceValue(Of P)(GetType(F), GetProperty(Of F)(propertyInfo, throwOnNoAccess))

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

      Dim result As P
      If CanReadProperty(propertyInfo.Name, throwOnNoAccess) Then
        Dim data As FieldManager.IFieldData = FieldManager.GetFieldData(propertyInfo)
        If data IsNot Nothing Then
          Dim fd As FieldManager.FieldData(Of P) = TryCast(data, FieldManager.FieldData(Of P))
          If fd IsNot Nothing Then
            result = fd.Value

          Else
            result = DirectCast(data.Value, P)
          End If

        Else
          result = propertyInfo.DefaultValue
          FieldManager.LoadFieldData(propertyInfo, result)
        End If

      Else
        result = propertyInfo.DefaultValue
      End If
      Return result

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

      SetProperty(Of P)(propertyInfo.Name, field, newValue, True)

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

      SetProperty(Of P)(propertyName, field, newValue, True)

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
    Protected Sub SetProperty(Of P, V)(ByVal propertyInfo As PropertyInfo(Of P), ByRef field As P, ByVal newValue As V)

      SetProperty(Of P, V)(propertyInfo, field, newValue, True)

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
    ''' <param name="throwOnNoAccess">
    ''' True if an exception should be thrown when the
    ''' user is not authorized to change this property.</param>
    ''' <remarks>
    ''' If the field value is of type string, any incoming
    ''' null values are converted to string.Empty.
    ''' </remarks>
    Protected Sub SetProperty(Of P, V)(ByVal propertyInfo As PropertyInfo(Of P), ByRef field As P, ByVal newValue As V, ByVal throwOnNoAccess As Boolean)

      SetProperty(Of P, V)(propertyInfo.Name, field, newValue, throwOnNoAccess)

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
    ''' <param name="throwOnNoAccess">
    ''' True if an exception should be thrown when the
    ''' user is not authorized to change this property.</param>
    Protected Sub SetProperty(Of P)(ByVal propertyName As String, ByRef field As P, ByVal newValue As P, ByVal throwOnNoAccess As Boolean)

      If CanWriteProperty(propertyName, throwOnNoAccess) Then
        If field Is Nothing Then
          If newValue IsNot Nothing Then
            OnPropertyChanging(propertyName)
            field = newValue
            PropertyHasChanged(propertyName)
          End If

        ElseIf Not field.Equals(newValue) Then
          If TypeOf newValue Is String AndAlso newValue Is Nothing Then
            newValue = CoerceValue(Of P)(GetType(String), String.Empty)
          End If
          OnPropertyChanging(propertyName)
          field = newValue
          PropertyHasChanged(propertyName)
        End If
      End If

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
    ''' <param name="throwOnNoAccess">
    ''' True if an exception should be thrown when the
    ''' user is not authorized to change this property.</param>
    ''' <remarks>
    ''' If the field value is of type string, any incoming
    ''' null values are converted to string.Empty.
    ''' </remarks>
    Protected Sub SetProperty(Of P, V)(ByVal propertyName As String, ByRef field As P, ByVal newValue As V, ByVal throwOnNoAccess As Boolean)

      If CanWriteProperty(propertyName, throwOnNoAccess) Then
        If field Is Nothing Then
          If newValue IsNot Nothing Then
            OnPropertyChanging(propertyName)
            field = CoerceValue(Of P)(GetType(V), newValue)
            PropertyHasChanged(propertyName)
          End If

        ElseIf Not field.Equals(newValue) Then
          If TypeOf newValue Is String AndAlso newValue Is Nothing Then
            newValue = CoerceValue(Of V)(GetType(String), String.Empty)
          End If
          OnPropertyChanging(propertyName)
          field = CoerceValue(Of P)(GetType(V), newValue)
          PropertyHasChanged(propertyName)
        End If
      End If

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
    Protected Sub SetProperty(Of P)( _
      ByVal propertyInfo As PropertyInfo(Of P), ByVal newValue As P)

      SetProperty(Of P)(propertyInfo, newValue, True)

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
    Protected Sub SetProperty(Of P, F)( _
      ByVal propertyInfo As PropertyInfo(Of P), ByVal newValue As F)

      SetProperty(Of P, F)(propertyInfo, newValue, True)

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
    ''' <param name="throwOnNoAccess">
    ''' True if an exception should be thrown when the
    ''' user is not authorized to change this property.</param>
    Protected Sub SetProperty(Of P, F)( _
      ByVal propertyInfo As PropertyInfo(Of P), ByVal newValue As F, ByVal throwOnNoAccess As Boolean)

      If CanWriteProperty(propertyInfo.Name, throwOnNoAccess) Then
        Dim fieldData = FieldManager.GetFieldData(propertyInfo)
        Dim oldValue As P = Nothing
        If fieldData IsNot Nothing Then
          oldValue = DirectCast(fieldData.Value, P)
        End If

        If fieldData Is Nothing OrElse oldValue Is Nothing Then
          If Not newValue Is Nothing Then
            SetPropertyValue(Of P)(propertyInfo, oldValue, CoerceValue(Of P)(GetType(F), newValue))
          End If

        ElseIf Not newValue.Equals(oldValue) Then
          SetPropertyValue(Of P)(propertyInfo, oldValue, CoerceValue(Of P)(GetType(F), newValue))
        End If
      End If

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
    ''' <param name="throwOnNoAccess">
    ''' True if an exception should be thrown when the
    ''' user is not authorized to change this property.</param>
    Protected Sub SetProperty(Of P)( _
      ByVal propertyInfo As PropertyInfo(Of P), ByVal newValue As P, ByVal throwOnNoAccess As Boolean)

      If CanWriteProperty(propertyInfo.Name, throwOnNoAccess) Then
        Dim fieldData = FieldManager.GetFieldData(propertyInfo)
        Dim oldValue As P = Nothing
        If fieldData IsNot Nothing Then
          oldValue = DirectCast(fieldData.Value, P)
        End If

        If fieldData Is Nothing OrElse oldValue Is Nothing Then
          If Not newValue Is Nothing Then
            SetPropertyValue(Of P)(propertyInfo, oldValue, newValue)
          End If

        ElseIf Not newValue.Equals(oldValue) Then
          SetPropertyValue(Of P)(propertyInfo, oldValue, newValue)
        End If
      End If

    End Sub

    Private Sub SetPropertyValue(Of P)( _
      ByVal propertyInfo As PropertyInfo(Of P), ByVal oldValue As P, ByVal newValue As P)

      OnPropertyChanging(propertyInfo.Name)
      If GetType(IEditableBusinessObject).IsAssignableFrom(propertyInfo.Type) Then
        ' remove old event hook
        If oldValue IsNot Nothing Then
          Dim pc As INotifyPropertyChanged = DirectCast(oldValue, INotifyPropertyChanged)
          RemoveHandler pc.PropertyChanged, AddressOf Child_PropertyChanged
        End If
        FieldManager.SetFieldData(propertyInfo, newValue)
        Dim child As IEditableBusinessObject = DirectCast(newValue, IEditableBusinessObject)
        If child IsNot Nothing Then
          child.SetParent(Me)
          ' set child edit level
          UndoableBase.ResetChildEditLevel(child, Me.EditLevel)
          ' reset EditLevelAdded 
          child.EditLevelAdded = Me.EditLevel
          ' hook child event
          Dim pc As INotifyPropertyChanged = DirectCast(newValue, INotifyPropertyChanged)
          AddHandler pc.PropertyChanged, AddressOf Child_PropertyChanged
        End If

      ElseIf GetType(IEditableCollection).IsAssignableFrom(propertyInfo.Type) Then
        ' remove old event hooks
        If oldValue IsNot Nothing Then
          Dim pc As IBindingList = DirectCast(oldValue, IBindingList)
          RemoveHandler pc.ListChanged, AddressOf Child_ListChanged
        End If
        FieldManager.SetFieldData(propertyInfo, newValue)
        Dim child As IEditableCollection = DirectCast(newValue, IEditableCollection)
        If child IsNot Nothing Then
          child.SetParent(Me)
          Dim undoChild As IUndoableObject = TryCast(child, IUndoableObject)
          If undoChild IsNot Nothing Then
            ' set child edit level
            UndoableBase.ResetChildEditLevel(undoChild, Me.EditLevel)
          End If
          Dim pc As IBindingList = DirectCast(newValue, IBindingList)
          AddHandler pc.ListChanged, AddressOf Child_ListChanged
        End If

      Else
        FieldManager.SetFieldData(propertyInfo, newValue)
      End If
      OnPropertyChanged(propertyInfo.Name)

    End Sub

    Private Sub Child_PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs)
      Dim data = FieldManager.FindPropertyName(sender)
      OnPropertyChanged(data)
    End Sub

    Private Sub Child_ListChanged(ByVal sender As Object, ByVal e As ListChangedEventArgs)
      Dim data = FieldManager.FindPropertyName(sender)
      OnPropertyChanged(data)
    End Sub

#End Region

#Region " Field Manager "

    Private mFieldManager As FieldManager.FieldDataManager

    ''' <summary>
    ''' Gets the PropertyManager object for this
    ''' business object.
    ''' </summary>
    Protected ReadOnly Property FieldManager() As FieldManager.FieldDataManager
      Get
        If mFieldManager Is Nothing Then
          mFieldManager = New FieldManager.FieldDataManager
          UndoableBase.ResetChildEditLevel(mFieldManager, Me.EditLevel)
        End If
        Return mFieldManager
      End Get
    End Property

    Private Sub FieldDataDeserialized()

      For Each item As Object In FieldManager.GetChildren
        Dim eo As IEditableBusinessObject = DirectCast(item, IEditableBusinessObject)
        If eo IsNot Nothing Then
          eo.SetParent(Me)
          Dim pc As INotifyPropertyChanged = DirectCast(item, INotifyPropertyChanged)
          AddHandler pc.PropertyChanged, AddressOf Child_PropertyChanged

        Else
          Dim el As IEditableCollection = DirectCast(item, IEditableCollection)
          If el IsNot Nothing Then
            el.SetParent(Me)
            Dim bl As IBindingList = DirectCast(item, IBindingList)
            AddHandler bl.ListChanged, AddressOf Child_ListChanged
          End If
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
    Protected Overridable Sub EditChildComplete( _
      ByVal child As Core.IEditableBusinessObject) _
      Implements Core.IParent.ApplyEditChild

      ' do nothing, we don't really care
      ' when a child has its edits applied
    End Sub

    Private Sub RemoveChild(ByVal child As IEditableBusinessObject) _
      Implements IParent.RemoveChild

      Dim name = FieldManager.FindPropertyName(child)
      FieldManager.RemoveField(name)

    End Sub

#End Region

  End Class

End Namespace
