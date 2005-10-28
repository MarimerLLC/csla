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

    Public Overrides Function Save() As Roles

      Dim result As Roles
      result = MyBase.Save()
      ' this runs on the client and invalidates
      ' the RoleList cache
      RoleList.InvalidateCache()
      Return result

    End Function

    Protected Overrides Sub DataPortal_OnDataPortalInvokeComplete( _
      ByVal e As Csla.DataPortalEventArgs)

      If ApplicationContext.ExecutionLocation = ApplicationContext.ExecutionLocations.Server Then
        ' this runs on the server and invalidates
        ' the RoleList cache
        RoleList.InvalidateCache()
      End If

    End Sub

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

#Region " Atomic Operations "

    Public Shared Sub InsertRole(ByVal role As Role)

      role.WebSave(False)

    End Sub

    Public Shared Sub UpdateRole(ByVal role As Role)

      role.WebSave(True)

    End Sub

    Public Shared Sub DeleteRole(ByVal id As Integer)

      DataPortal.Execute(New RoleDeleter(id))

    End Sub

    Public Shared Sub DeleteRole(ByVal role As Role)

      DataPortal.Execute(New RoleDeleter(role.Id))

    End Sub

    <Serializable()> _
    Private Class RoleDeleter
      Inherits CommandBase

      Private mId As Integer
      Public ReadOnly Property Id() As Integer
        Get
          Return mId
        End Get
      End Property

      Public Sub New(ByVal id As Integer)
        mId = id
      End Sub

      Protected Overrides Sub DataPortal_Execute()

        Using cn As New SqlConnection(DataBase.DbConn)
          cn.Open()
          Role.DeleteRole(cn, mId)
          cn.Close()
        End Using

      End Sub

    End Class

#End Region

  End Class

End Namespace
