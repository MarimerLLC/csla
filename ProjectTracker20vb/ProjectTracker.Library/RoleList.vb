Imports System.Data.SqlClient

<Serializable()> _
Public Class RoleList
  Inherits NameValueListBase(Of Integer, String)

#Region " Business Methods "

  Public Shared Function DefaultRole() As Integer

    Return GetList.Items(0).Key

  End Function

#End Region

#Region " Constructors "

  Private Sub New()

  End Sub

#End Region

#Region " Factory Methods "

  Private Shared mList As RoleList

  Public Shared Function GetList() As RoleList

    If mList Is Nothing Then
      mList = DataPortal.Fetch(Of RoleList)(New Criteria(GetType(RoleList)))
    End If
    Return mList

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
          IsReadOnly = False
          With dr
            While .Read()
              Me.Add(New NameValuePair(.GetInt32("id"), .GetString("name")))
            End While
            .Close()
          End With
          IsReadOnly = True
        End Using
      End Using
      cn.Close()
    End Using

  End Sub

#End Region

End Class
