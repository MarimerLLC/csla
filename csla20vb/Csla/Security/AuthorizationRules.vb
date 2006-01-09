Namespace Security

  ''' <summary>
  ''' Maintains a list of allowed and denied user roles
  ''' for each property.
  ''' </summary>
  ''' <remarks></remarks>
  <Serializable()> _
  Public Class AuthorizationRules

    Private mRules As Dictionary(Of String, RolesForProperty)

    Private ReadOnly Property Rules() As Dictionary(Of String, RolesForProperty)
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
    <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Advanced)> _
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

    Private Function GetRolesForProperty(ByVal propertyName As String) As RolesForProperty

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

    Public Sub AllowRead(ByVal propertyName As String, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = GetRolesForProperty(propertyName)
      For Each item As String In roles
        currentRoles.ReadAllowed.Add(item)
      Next

    End Sub

    Public Sub DenyRead(ByVal propertyName As String, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = GetRolesForProperty(propertyName)
      For Each item As String In roles
        currentRoles.ReadDenied.Add(item)
      Next

    End Sub

    Public Sub AllowWrite(ByVal propertyName As String, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = GetRolesForProperty(propertyName)
      For Each item As String In roles
        currentRoles.WriteAllowed.Add(item)
      Next

    End Sub

    Public Sub DenyWrite(ByVal propertyName As String, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = GetRolesForProperty(propertyName)
      For Each item As String In roles
        currentRoles.WriteDenied.Add(item)
      Next

    End Sub

#End Region

#Region " Check Roles "

    Public Function HasReadAllowedRoles(ByVal propertyName As String) As Boolean

      Return (GetRolesForProperty(propertyName).ReadAllowed.Count > 0)

    End Function

    Public Function IsReadAllowed(ByVal propertyName As String) As Boolean

      Return GetRolesForProperty(propertyName).IsReadAllowed(ApplicationContext.User)

    End Function

    Public Function HasReadDeniedRoles(ByVal propertyName As String) As Boolean

      Return (GetRolesForProperty(propertyName).ReadDenied.Count > 0)

    End Function

    Public Function IsReadDenied(ByVal propertyName As String) As Boolean

      Return GetRolesForProperty(propertyName).IsReadDenied(ApplicationContext.User)

    End Function

    Public Function HasWriteAllowedRoles(ByVal propertyName As String) As Boolean

      Return (GetRolesForProperty(propertyName).WriteAllowed.Count > 0)

    End Function

    Public Function IsWriteAllowed(ByVal propertyName As String) As Boolean

      Return GetRolesForProperty(propertyName).IsWriteAllowed(ApplicationContext.User)

    End Function

    Public Function HasWriteDeniedRoles(ByVal propertyName As String) As Boolean

      Return (GetRolesForProperty(propertyName).WriteDenied.Count > 0)

    End Function

    Public Function IsWriteDenied(ByVal propertyName As String) As Boolean

      Return GetRolesForProperty(propertyName).IsWriteDenied(ApplicationContext.User)

    End Function

#End Region

  End Class

End Namespace
