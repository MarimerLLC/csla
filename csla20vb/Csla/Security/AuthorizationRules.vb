Imports System.ComponentModel

Namespace Security

  ''' <summary>
  ''' Maintains a list of allowed and denied user roles
  ''' for each property.
  ''' </summary>
  ''' <remarks></remarks>
  <Serializable()> _
  Public Class AuthorizationRules

    Private mRules As Dictionary(Of String, RolesForProperty)

    Private ReadOnly Property Rules() _
      As Dictionary(Of String, RolesForProperty)
      Get
        If mRules Is Nothing Then
          mRules = New Dictionary(Of String, RolesForProperty)
        End If
        Return mRules
      End Get
    End Property

#Region " Get Roles "

    ''' <summary>
    ''' Returns a list of roles for the property
    ''' and requested access type.
    ''' </summary>
    ''' <param name="propertyName">
    ''' Name of the object property.</param>
    ''' <param name="access">Desired access type.</param>
    ''' <returns>An string array of roles.</returns>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Public Function GetRolesForProperty(ByVal propertyName As String, _
      ByVal access As AccessType) As String()

      Dim currentRoles As RolesForProperty = GetRolesForProperty(propertyName)
      Select Case access
        Case AccessType.ReadAllowed
          Return currentRoles.ReadAllowed.ToArray
        Case AccessType.ReadDenied
          Return currentRoles.ReadDenied.ToArray
        Case AccessType.WriteAllowed
          Return currentRoles.WriteAllowed.ToArray
        Case AccessType.WriteDenied
          Return currentRoles.WriteDenied.ToArray
      End Select
      Return Nothing

    End Function

    Private Function GetRolesForProperty( _
      ByVal propertyName As String) As RolesForProperty

      Dim currentRoles As RolesForProperty = Nothing
      If Not Rules.ContainsKey(propertyName) Then
        currentRoles = New RolesForProperty
        Rules.Add(propertyName, currentRoles)

      Else
        currentRoles = Rules.Item(propertyName)
      End If
      Return currentRoles

    End Function

#End Region

#Region " Add Roles "

    ''' <summary>
    ''' Specify the roles allowed to read a given
    ''' property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    ''' <param name="roles">List of roles granted read access.</param>
    ''' <remarks>
    ''' This method may be called multiple times, with the roles in
    ''' each call being added to the end of the list of allowed roles.
    ''' In other words, each call is cumulative, adding more roles
    ''' to the list.
    ''' </remarks>
    Public Sub AllowRead( _
      ByVal propertyName As String, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = GetRolesForProperty(propertyName)
      For Each item As String In roles
        currentRoles.ReadAllowed.Add(item)
      Next

    End Sub

    ''' <summary>
    ''' Specify the roles denied read access to 
    ''' a given property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    ''' <param name="roles">List of roles denied read access.</param>
    ''' <remarks>
    ''' This method may be called multiple times, with the roles in
    ''' each call being added to the end of the list of denied roles.
    ''' In other words, each call is cumulative, adding more roles
    ''' to the list.
    ''' </remarks>
    Public Sub DenyRead(ByVal propertyName As String, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = GetRolesForProperty(propertyName)
      For Each item As String In roles
        currentRoles.ReadDenied.Add(item)
      Next

    End Sub

    ''' <summary>
    ''' Specify the roles allowed to write a given
    ''' property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    ''' <param name="roles">List of roles granted write access.</param>
    ''' <remarks>
    ''' This method may be called multiple times, with the roles in
    ''' each call being added to the end of the list of allowed roles.
    ''' In other words, each call is cumulative, adding more roles
    ''' to the list.
    ''' </remarks>
    Public Sub AllowWrite(ByVal propertyName As String, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = GetRolesForProperty(propertyName)
      For Each item As String In roles
        currentRoles.WriteAllowed.Add(item)
      Next

    End Sub

    ''' <summary>
    ''' Specify the roles denied write access to 
    ''' a given property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    ''' <param name="roles">List of roles denied write access.</param>
    ''' <remarks>
    ''' This method may be called multiple times, with the roles in
    ''' each call being added to the end of the list of denied roles.
    ''' In other words, each call is cumulative, adding more roles
    ''' to the list.
    ''' </remarks>
    Public Sub DenyWrite(ByVal propertyName As String, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = GetRolesForProperty(propertyName)
      For Each item As String In roles
        currentRoles.WriteDenied.Add(item)
      Next

    End Sub

#End Region

#Region " Check Roles "

    ''' <summary>
    ''' Indicates whether the property has a list
    ''' of roles granted read access.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    Public Function HasReadAllowedRoles( _
      ByVal propertyName As String) As Boolean

      Return (GetRolesForProperty(propertyName).ReadAllowed.Count > 0)

    End Function

    ''' <summary>
    ''' Indicates whether the current user as defined by
    ''' <see cref="Csla.ApplicationContext.User" />
    ''' is explicitly allowed to read the property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    Public Function IsReadAllowed(ByVal propertyName As String) As Boolean

      Return GetRolesForProperty(propertyName). _
        IsReadAllowed(ApplicationContext.User)

    End Function

    ''' <summary>
    ''' Indicates whether the property has a list
    ''' of roles denied read access.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    Public Function HasReadDeniedRoles(ByVal propertyName As String) As Boolean

      Return (GetRolesForProperty(propertyName).ReadDenied.Count > 0)

    End Function

    ''' <summary>
    ''' Indicates whether the current user as defined by
    ''' <see cref="Csla.ApplicationContext.User" />
    ''' is explicitly denied read access to the property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    Public Function IsReadDenied(ByVal propertyName As String) As Boolean

      Return GetRolesForProperty(propertyName).IsReadDenied(ApplicationContext.User)

    End Function

    ''' <summary>
    ''' Indicates whether the property has a list
    ''' of roles granted write access.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    Public Function HasWriteAllowedRoles(ByVal propertyName As String) As Boolean

      Return (GetRolesForProperty(propertyName).WriteAllowed.Count > 0)

    End Function

    ''' <summary>
    ''' Indicates whether the current user as defined by
    ''' <see cref="Csla.ApplicationContext.User" />
    ''' is explicitly allowed to set the property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    Public Function IsWriteAllowed(ByVal propertyName As String) As Boolean

      Return GetRolesForProperty(propertyName).IsWriteAllowed(ApplicationContext.User)

    End Function

    ''' <summary>
    ''' Indicates whether the property has a list
    ''' of roles denied write access.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    Public Function HasWriteDeniedRoles(ByVal propertyName As String) As Boolean

      Return (GetRolesForProperty(propertyName).WriteDenied.Count > 0)

    End Function

    ''' <summary>
    ''' Indicates whether the current user as defined by
    ''' <see cref="Csla.ApplicationContext.User" />
    ''' is explicitly denied write access to the property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    Public Function IsWriteDenied(ByVal propertyName As String) As Boolean

      Return GetRolesForProperty(propertyName).IsWriteDenied(ApplicationContext.User)

    End Function

#End Region

  End Class

End Namespace
