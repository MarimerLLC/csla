Imports System.Data.SqlClient

<Serializable()> _
  Public Class ClientList
  Inherits BusinessListBase(Of ClientList, ClientEdit)

#Region " Business Methods "

  Protected Overrides Function AddNewCore() As Object

    Dim child As ClientEdit = ClientEdit.NewClient
    Add(child)
    Return child

  End Function

#End Region

#Region " Factory Methods "

  Public Shared Function NewList() As ClientList

    Return New ClientList

  End Function

  Public Shared Function GetList() As ClientList

    Return DataPortal.Fetch(Of ClientList)()

  End Function

  Private Sub New()

    AllowEdit = True
    AllowNew = True
    AllowRemove = True

  End Sub

#End Region

#Region " Data Access "

  Private Overloads Sub DataPortal_Fetch()

    Using cn As New SqlConnection(Database.SalesConnectionString)
      cn.Open()
      ApplicationContext.LocalContext.Add("cn", cn)
      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = System.Data.CommandType.Text
        cm.CommandText = "SELECT id,name FROM Client ORDER BY name"
        Using dr As New Csla.Data.SafeDataReader(cm.ExecuteReader)
          While dr.Read
            Add(ClientEdit.GetClient(dr))
          End While
        End Using
      End Using
      ApplicationContext.LocalContext.Remove("cn")
    End Using

  End Sub

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overrides Sub DataPortal_Update()

    Using cn As New SqlConnection(Database.SalesConnectionString)
      cn.Open()
      ApplicationContext.LocalContext.Add("cn", cn)
      For Each child As ClientEdit In DeletedList
        If Not child.IsNew Then
          child.DeleteSelf(Me)
        End If
      Next
      DeletedList.Clear()

      For Each child As ClientEdit In Me
        If child.IsDirty Then
          If child.IsNew Then
            If Not child.IsDeleted Then
              child.Insert(Me)
            End If
          Else
            child.Update(Me)
          End If
        End If
      Next
      ApplicationContext.LocalContext.Remove("cn")
    End Using
    If ApplicationContext.ExecutionLocation = ExecutionLocations.Server Then
      ClientNVList.FlushCache()
      ClientProjectNVList.FlushCache()
    End If

  End Sub

  Public Overrides Function Save() As ClientList

    Dim result As ClientList = MyBase.Save
    ClientNVList.FlushCache()
    ClientProjectNVList.FlushCache()
    Return result

  End Function

#End Region

End Class
