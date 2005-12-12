Imports System.Reflection
Imports System.ComponentModel

''' <summary>
''' This is a base class from which readonly business classes
''' can be derived.
''' </summary>
''' <remarks>
''' This base class only supports data retrieve, not updating or
''' deleting. Any business classes derived from this base class
''' should only implement readonly properties.
''' </remarks>
<Serializable()> _
Public MustInherit Class ReadOnlyBase(Of T As ReadOnlyBase(Of T))

  Implements ICloneable
  Implements Core.IReadOnlyObject

#Region " Constructors "

  Protected Sub New()

    AddAuthorizationRules()

  End Sub

#End Region

#Region " Authorization "

  <NotUndoable()> _
  Private mAuthorizationRules As New Security.AuthorizationRules

  ''' <summary>
  ''' Override this method to add authorization
  ''' rules for your object's properties.
  ''' </summary>
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
  Protected ReadOnly Property AuthorizationRules() As Security.AuthorizationRules
    Get
      Return mAuthorizationRules
    End Get
  End Property

  ''' <summary>
  ''' Returns True if the user is allowed to read the
  ''' calling property.
  ''' </summary>
  ''' <returns>True if read is allowed.</returns>
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
  ''' <param name="throwOnFalse">Indicates whether a negative
  ''' result should cause an exception.</param>
  <System.Runtime.CompilerServices.MethodImpl( _
    System.Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
  Public Function CanReadProperty(ByVal throwOnFalse As Boolean) As Boolean

    Dim propertyName As String = _
      New System.Diagnostics.StackTrace().GetFrame(1).GetMethod.Name.Substring(4)
    Dim result As Boolean = CanReadProperty(propertyName)
    If throwOnFalse AndAlso result = False Then
      Throw New System.Security.SecurityException(My.Resources.PropertyGetNotAllowed)
    End If
    Return result

  End Function

  ''' <summary>
  ''' Returns True if the user is allowed to read the
  ''' calling property.
  ''' </summary>
  ''' <returns>True if read is allowed.</returns>
  ''' <remarks>
  ''' <para>
  ''' If and only if the user is in a role explicitly denied 
  ''' access and NOT in a role that explicitly
  ''' allows access they will be denied read access to the property.
  ''' </para><para>
  ''' This implementation uses System.Diagnostics.StackTrace to
  ''' determine the name of the current property, and so must be called
  ''' directly from the property to be checked.
  ''' </para>
  ''' </remarks>
  Public Function CanReadProperty() As Boolean

    Dim propertyName As String = _
      New System.Diagnostics.StackTrace().GetFrame(1).GetMethod.Name.Substring(4)
    Return CanReadProperty(propertyName)

  End Function

  ''' <summary>
  ''' Returns True if the user is allowed to read the
  ''' specified property.
  ''' </summary>
  ''' <param name="propertyName">Name of the property to read.</param>
  ''' <returns>True if read is allowed.</returns>
  ''' <remarks>
  ''' If and only if the user is in a role explicitly denied 
  ''' access and NOT in a role that explicitly
  ''' allows access they will be denied read access to the property.
  ''' </remarks>
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Public Overridable Function CanReadProperty( _
    ByVal propertyName As String) As Boolean _
    Implements Core.IReadOnlyObject.CanReadProperty

    Dim result As Boolean = True
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
    Return result

  End Function

#End Region

#Region " ICloneable "

  Private Function ICloneable_Clone() As Object Implements ICloneable.Clone

    Return OnClone()

  End Function

  ''' <summary>
  ''' Creates a clone of the object.
  ''' </summary>
  ''' <returns>
  ''' A new object containing the exact data of the original object.
  ''' </returns>
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Function OnClone() As Object

    Return ObjectCloner.Clone(Me)

  End Function

  ''' <summary>
  ''' Creates a clone of the object.
  ''' </summary>
  ''' <returns>
  ''' A new object containing the exact data of the original object.
  ''' </returns>
  Public Overloads Function Clone() As T

    Return DirectCast(OnClone(), T)

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
  ''' <param name="Criteria">An object containing criteria values to identify the object.</param>
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
  ''' requested DataPortal_xyz method.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub DataPortal_OnDataPortalInvoke(ByVal e As DataPortalEventArgs)

  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal after calling the 
  ''' requested DataPortal_xyz method.
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

End Class
