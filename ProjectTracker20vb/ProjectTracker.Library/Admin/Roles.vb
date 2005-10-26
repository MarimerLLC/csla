Imports System.Data.SqlClient

Namespace Admin

  ''' <summary>
  ''' Used to maintain the list of roles
  ''' in the system.
  ''' </summary>
  <Serializable()> _
  Public Class Roles
    Inherits BusinessListBase(Of Roles, Role)

#Region " Constructors "

    Private Sub New()

    End Sub

#End Region

#Region " Criteria "

    <Serializable()> _
    Private Class Criteria
      ' no criteria
    End Class

#End Region

#Region " Factory Methods "

    Public Shared Function GetRoles() As Roles

      Return DataPortal.Fetch(Of Roles)(New Criteria)

    End Function

#End Region

#Region " Data Access "

    Protected Overrides Sub DataPortal_Fetch(ByVal criteria As Object)

      Dim crit As Criteria = CType(criteria, Criteria)
      Using cn As New SqlConnection(DataBase.DbConn)
        cn.Open()
        Using cm As SqlCommand = cn.CreateCommand
          cm.CommandType = CommandType.StoredProcedure
          cm.CommandText = "getRoles"

          Using dr As New SafeDataReader(cm.ExecuteReader)
            With dr
              While .Read()
                Me.Add(Role.GetRole(dr))
              End While
              .Close()
            End With
          End Using
        End Using
        cn.Close()
      End Using

    End Sub

    <Transactional(TransactionalTypes.Manual)> _
    Protected Overrides Sub DataPortal_Update()

      Using cn As New SqlConnection(DataBase.DbConn)
        cn.Open()
        For Each item As Role In DeletedList
          item.DeleteSelf(cn)
        Next
        DeletedList.Clear()

        For Each item As Role In Me
          If item.IsNew Then
            item.Insert(cn)

          Else
            item.Update(cn)
          End If
        Next
        cn.Close()
      End Using

    End Sub

#End Region

  End Class

End Namespace
