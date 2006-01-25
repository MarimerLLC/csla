Imports System.Data.SqlClient

<Serializable()> _
Public Class RoleList
  Inherits NameValueListBase(Of Integer, String)

#Region " Business Methods "

  Public Shared Function DefaultRole() As Integer

    Return GetList.Items(0).Key

  End Function

#End Region

#Region " Factory Methods "

  Private Shared mList As RoleList

  Public Shared Function GetList() As RoleList

    If mList Is Nothing Then
      mList = DataPortal.Fetch(Of RoleList)(New Criteria(GetType(RoleList)))
    End If
    Return mList

  End Function

  ''' <summary>
  ''' Clears the in-memory RoleList cache
  ''' so the list of roles is reloaded on
  ''' next request.
  ''' </summary>
  Public Shared Sub InvalidateCache()

    mList = Nothing

  End Sub

  Private Sub New()
    ' require use of factory methods
  End Sub

#End Region

#Region " Data Access "

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

    Using cn As New SqlConnection(Database.PTrackerConnection)
      cn.Open()
      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = CommandType.StoredProcedure
        cm.CommandText = "getRoles"

        Using dr As New SafeDataReader(cm.ExecuteReader)
          IsReadOnly = False
          With dr
            While .Read()
              Me.Add(New NameValuePair( _
                .GetInt32("id"), .GetString("name")))
            End While
          End With
          IsReadOnly = True
        End Using
      End Using
    End Using

  End Sub

#End Region

End Class
