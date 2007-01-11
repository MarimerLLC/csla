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
    Friend Function GetManager(ByVal objectType As Type, ByVal create As Boolean) _
      As AuthorizationRulesManager

      SyncLock mManagers
        Dim result As AuthorizationRulesManager = Nothing
        If mManagers.ContainsKey(objectType) Then
          result = mManagers.Item(objectType)

        ElseIf create Then
          result = New AuthorizationRulesManager
          mManagers.Add(objectType, result)
        End If
        Return result
      End SyncLock

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

      SyncLock mManagers
        Return mManagers.ContainsKey(objectType)
      End SyncLock

    End Function

  End Module

End Namespace
