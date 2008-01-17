Imports System.Reflection

Namespace Security

  ''' <summary>
  ''' Maintains a list of all object level
  ''' authorization roles.
  ''' </summary>
  Friend Class ObjectAuthorizationRules

    Private Shared _managers As Dictionary(Of Type, RolesForType) = New Dictionary(Of Type, RolesForType)()

    Friend Shared Function GetRoles(ByVal objectType As Type) As RolesForType
      Dim result As RolesForType = Nothing
      If (Not _managers.TryGetValue(objectType, result)) Then
        SyncLock _managers
          If (Not _managers.TryGetValue(objectType, result)) Then
            result = New RolesForType()
            _managers.Add(objectType, result)
            ' invoke method to add auth roles
            Dim flags = BindingFlags.Static Or BindingFlags.Public Or BindingFlags.NonPublic
            Dim method As MethodInfo = objectType.GetMethod("AddObjectAuthorizationRules", flags)
            If method IsNot Nothing Then
              method.Invoke(Nothing, Nothing)
            End If
          End If
        End SyncLock
      End If
      Return result
    End Function

    Public Shared Function RulesExistFor(ByVal objectType As Type) As Boolean
      Return _managers.ContainsKey(objectType)
    End Function
  End Class

End Namespace