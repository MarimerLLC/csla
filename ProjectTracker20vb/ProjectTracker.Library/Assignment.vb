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
    ByVal cn As SqlConnection, ByVal projectId As Guid, _
    ByVal resourceId As Integer, ByVal assigned As SmartDate, _
    ByVal role As Integer) As Byte()

    Using cm As SqlCommand = cn.CreateCommand()
      cm.CommandText = "addAssignment"
      Return DoAddUpdate( _
        cm, projectId, resourceId, assigned, role)
    End Using

  End Function

  Public Function UpdateAssignment(ByVal cn As SqlConnection, _
    ByVal projectId As Guid, ByVal resourceId As Integer, _
    ByVal assigned As SmartDate, ByVal newRole As Integer, _
    ByVal timestamp() As Byte) As Byte()

    Using cm As SqlCommand = cn.CreateCommand()
      cm.CommandText = "updateAssignment"
      cm.Parameters.AddWithValue("@lastChanged", timestamp)
      Return DoAddUpdate( _
        cm, projectId, resourceId, assigned, newRole)
    End Using

  End Function

  Private Function DoAddUpdate(ByVal cm As SqlCommand, _
    ByVal projectId As Guid, ByVal resourceId As Integer, _
    ByVal assigned As SmartDate, _
    ByVal newRole As Integer) As Byte()

    cm.CommandType = CommandType.StoredProcedure
    cm.Parameters.AddWithValue("@projectId", projectId)
    cm.Parameters.AddWithValue("@resourceId", resourceId)
    cm.Parameters.AddWithValue("@assigned", assigned.DBValue)
    cm.Parameters.AddWithValue("@role", newRole)
    Dim param As New SqlParameter("@newLastChanged", SqlDbType.Timestamp)
    param.Direction = ParameterDirection.Output
    cm.Parameters.Add(param)

    cm.ExecuteNonQuery()

    Return CType(cm.Parameters("@newLastChanged").Value, Byte())

  End Function

  Public Sub RemoveAssignment( _
    ByVal cn As SqlConnection, ByVal projectId As Guid, _
    ByVal resourceId As Integer)

    Using cm As SqlCommand = cn.CreateCommand()
      cm.CommandType = CommandType.StoredProcedure
      cm.CommandText = "deleteAssignment"
      cm.Parameters.AddWithValue("@projectId", projectId)
      cm.Parameters.AddWithValue("@resourceId", resourceId)

      cm.ExecuteNonQuery()
    End Using

  End Sub

#End Region

End Module
