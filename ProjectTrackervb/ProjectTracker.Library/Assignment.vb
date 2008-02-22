Imports Csla.Validation
Imports System.Reflection

Friend Interface IHoldRoles
  Property Role() As Integer
End Interface

Friend Module Assignment

#Region " Business Methods "

  Public Function GetDefaultAssignedDate() As Date

    Return Today

  End Function

#End Region

#Region " Validation Rules "

  ''' <summary>
  ''' Ensure the Role property value exists
  ''' in RoleList
  ''' </summary>
  Public Function ValidRole( _
    ByVal target As Object, ByVal e As RuleArgs) As Boolean

    Dim role As Integer = CType(target, IHoldRoles).Role

    If RoleList.GetList.ContainsKey(role) Then
      Return True

    Else
      e.Description = "Role must be in RoleList"
      Return False
    End If

  End Function

#End Region

#Region " Data Access "

  Public Function AddAssignment( _
    ByVal projectId As Guid, _
    ByVal resourceId As Integer, ByVal assigned As SmartDate, _
    ByVal role As Integer) As Byte()

    Using ctx = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager(ProjectTracker.DalLinq.Database.PTracker, True)
      Dim lastChanged As System.Data.Linq.Binary = Nothing
      ctx.DataContext.addAssignment( _
        projectId, resourceId, assigned, role, lastChanged)
      Return lastChanged.ToArray
    End Using

  End Function

  Public Function UpdateAssignment( _
    ByVal projectId As Guid, ByVal resourceId As Integer, _
    ByVal assigned As SmartDate, ByVal newRole As Integer, _
    ByVal timestamp() As Byte) As Byte()

    Using ctx = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager(ProjectTracker.DalLinq.Database.PTracker, True)
      Dim lastChanged As System.Data.Linq.Binary = Nothing
      ctx.DataContext.UpdateAssignment( _
        projectId, resourceId, assigned, newRole, timestamp, lastChanged)
      Return lastChanged.ToArray
    End Using

  End Function

  Public Sub RemoveAssignment( _
    ByVal projectId As Guid, _
    ByVal resourceId As Integer)

    Using ctx = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager(ProjectTracker.DalLinq.Database.PTracker, True)
      Dim lastChanged As System.Data.Linq.Binary = Nothing
      ctx.DataContext.DeleteAssignment( _
        projectId, resourceId)
    End Using

  End Sub

#End Region

End Module
