Imports System.ComponentModel

Namespace Security

  ''' <summary>
  ''' Maintains a list of all the per-type
  ''' <see cref="AuthorizationRulesManager"/> objects
  ''' loaded in memory.
  ''' </summary>
  Friend Module SharedAuthorizationRules

    Private mManagers As New Dictionary(Of Type, AuthorizationRulesManager)

    ''' <summary>
    ''' Gets the <see cref="AuthorizationRulesManager"/> for the 
    ''' specified object type, optionally creating a new instance 
    ''' of the object if necessary.
    ''' </summary>
    ''' <param name="objectType">
    ''' Type of business object for which the rules apply.
    ''' </param>
    ''' <param name="create">Indicates whether to create
    ''' a new instance of the object if one doesn't exist.</param>
    Friend Function GetManager(ByVal objectType As Type, ByVal create As Boolean) As AuthorizationRulesManager

      Dim result As AuthorizationRulesManager = Nothing
      If mManagers.ContainsKey(objectType) Then
        result = mManagers.Item(objectType)

      ElseIf create Then
        result = New AuthorizationRulesManager
        mManagers.Add(objectType, result)
      End If
      Return result

    End Function

    ''' <summary>
    ''' Gets a value indicating whether a set of rules
    ''' have been created for a given <see cref="Type" />.
    ''' </summary>
    ''' <param name="objectType">
    ''' Type of business object for which the rules apply.
    ''' </param>
    ''' <returns><see langword="true" /> if rules exist for the type.</returns>
    Public Function RulesExistFor(ByVal objectType As Type) As Boolean

      Return mManagers.ContainsKey(objectType)

    End Function

#Region " Get Roles "

    ''' <summary>
    ''' Returns a list of roles for the property,
    ''' object type and requested access type.
    ''' </summary>
    ''' <param name="propertyName">
    ''' Name of the object property.</param>
    ''' <param name="objectType">
    ''' Type of the business object.
    ''' </param>
    ''' <param name="access">Desired access type.</param>
    ''' <returns>An string array of roles.</returns>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Public Function GetRolesForProperty(ByVal propertyName As String, _
      ByVal objectType As Type, ByVal access As AccessType) As String()

      Dim currentRoles As RolesForProperty = GetRolesForProperty(propertyName, objectType)
      If currentRoles IsNot Nothing Then
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
      End If
      Return Nothing

    End Function

    Private Function GetRolesForProperty( _
      ByVal propertyName As String, ByVal objectType As Type) As RolesForProperty

      Dim manager As AuthorizationRulesManager = GetManager(objectType, True)
      Dim currentRoles As RolesForProperty = Nothing
      If Not manager.RulesList.ContainsKey(propertyName) Then
        currentRoles = New RolesForProperty
        manager.RulesList.Add(propertyName, currentRoles)

      Else
        currentRoles = manager.RulesList.Item(propertyName)
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
      ByVal propertyName As String, ByVal objectType As Type, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = GetRolesForProperty(propertyName, objectType)
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
    Public Sub DenyRead(ByVal propertyName As String, ByVal objectType As Type, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = GetRolesForProperty(propertyName, objectType)
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
    Public Sub AllowWrite(ByVal propertyName As String, ByVal objectType As Type, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = GetRolesForProperty(propertyName, objectType)
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
    Public Sub DenyWrite(ByVal propertyName As String, ByVal objectType As Type, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = GetRolesForProperty(propertyName, objectType)
      For Each item As String In roles
        currentRoles.WriteDenied.Add(item)
      Next

    End Sub

#End Region

  End Module

End Namespace
