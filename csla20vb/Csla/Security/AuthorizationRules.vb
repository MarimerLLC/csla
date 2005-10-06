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

    <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Advanced)> _
    Public Function GetRolesForProperty(ByVal propertyName As String) As RolesForProperty

      Dim currentRoles As RolesForProperty = Nothing
      If Not Rules.ContainsKey(propertyName) Then
        currentRoles = New RolesForProperty
        Rules.Add(propertyName, currentRoles)

      Else
        currentRoles = Rules.Item(propertyName)
      End If
      Return currentRoles

    End Function

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

    Public Function IsReadAllowed(ByVal propertyName As String) As Boolean

      Return GetRolesForProperty(propertyName).IsReadAllowed(My.User.CurrentPrincipal)

    End Function

    Public Function IsReadDenied(ByVal propertyName As String) As Boolean

      Return GetRolesForProperty(propertyName).IsReadDenied(My.User.CurrentPrincipal)

    End Function

    Public Function IsWriteAllowed(ByVal propertyName As String) As Boolean

      Return GetRolesForProperty(propertyName).IsWriteAllowed(My.User.CurrentPrincipal)

    End Function

    Public Function IsWriteDenied(ByVal propertyName As String) As Boolean

      Return GetRolesForProperty(propertyName).IsWriteDenied(My.User.CurrentPrincipal)

    End Function

  End Class

End Namespace
