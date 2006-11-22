Imports System.ComponentModel

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

  End Class

End Namespace
