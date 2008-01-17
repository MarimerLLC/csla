Imports System.ComponentModel
Imports System.Security.Principal

Namespace Security

  ''' <summary>
  ''' Maintains authorization roles for a business object
  ''' or business object type.
  ''' </summary>
  Friend Class AuthorizationRulesManager

    Private mRules As Dictionary(Of String, RolesForProperty)

    Friend ReadOnly Property RulesList() _
      As Dictionary(Of String, RolesForProperty)
      Get
        If mRules Is Nothing Then
          mRules = New Dictionary(Of String, RolesForProperty)
        End If
        Return mRules
      End Get
    End Property

#Region " Get Roles "

    Friend Function GetRolesForProperty( _
      ByVal propertyName As String) As RolesForProperty

      Dim currentRoles As RolesForProperty = Nothing
      If Not RulesList.ContainsKey(propertyName) Then
        currentRoles = New RolesForProperty
        RulesList.Add(propertyName, currentRoles)

      Else
        currentRoles = RulesList.Item(propertyName)
      End If
      Return currentRoles

    End Function

#End Region

#Region "IsInRole"

    Friend Shared Function PrincipalRoleInList(ByVal principal As IPrincipal, ByVal roleList As List(Of String)) As Boolean
      Dim result As Boolean = False
      For Each role As String In roleList
        If IsInRole(principal, role) Then
          result = True
          Exit For
        End If
      Next role
      Return result
    End Function

    Private Shared mIsInRoleProvider As IsInRoleProvider

    Private Shared Function IsInRole(ByVal principal As IPrincipal, ByVal role As String) As Boolean
      If mIsInRoleProvider Is Nothing Then
        Dim provider As String = ApplicationContext.IsInRoleProvider
        If String.IsNullOrEmpty(provider) Then
          mIsInRoleProvider = AddressOf IsInRoleDefault
        Else
          Dim items() As String = provider.Split(","c)
          Dim containingType As Type = Type.GetType(items(0) & "," & items(1))
          mIsInRoleProvider = CType(System.Delegate.CreateDelegate(GetType(IsInRoleProvider), containingType, items(2)), IsInRoleProvider)
        End If
      End If
      Return mIsInRoleProvider(principal, role)
    End Function

    Private Shared Function IsInRoleDefault(ByVal principal As IPrincipal, ByVal role As String) As Boolean
      Return principal.IsInRole(role)
    End Function

#End Region

  End Class

  ''' <summary>
  ''' Delegate for the method called when the a role
  ''' needs to be checked for the current user.
  ''' </summary>
  ''' <param name="principal">
  ''' The current security principal object.
  ''' </param>
  ''' <param name="role">
  ''' The role to be checked.
  ''' </param>
  ''' <returns>
  ''' True if the current user is in the specified role.
  ''' </returns>
  Public Delegate Function IsInRoleProvider(ByVal principal As IPrincipal, ByVal role As String) As Boolean

End Namespace
