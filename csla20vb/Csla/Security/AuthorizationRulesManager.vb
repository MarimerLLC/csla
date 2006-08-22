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
