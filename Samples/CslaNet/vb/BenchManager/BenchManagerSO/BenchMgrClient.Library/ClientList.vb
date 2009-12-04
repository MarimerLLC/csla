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

  Public Shared Function Test() As String

    Dim svc As New SalesService.SalesServiceClient
    Return svc.Test

  End Function
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

  <RunLocal()> _
  Private Overloads Sub DataPortal_Fetch()

    Dim svc As New SalesService.SalesServiceClient
    Dim clientList() As SalesService.ClientData = Nothing
    clientList = svc.GetClientList

    ' load business objects from DTOs
    For Each item As SalesService.ClientData In clientList
      Add(ClientEdit.GetClient(item))
    Next

  End Sub

  <RunLocal()> _
  Protected Overrides Sub DataPortal_Update()

    ' copy changed objects to DTOs
    Dim clientList As New List(Of SalesService.ClientDataUpdate)
    For Each child As ClientEdit In DeletedList
      Dim data As New SalesService.ClientDataUpdate
      data.Id = child.Id
      data.Delete = True
      clientList.Add(data)
    Next
    For Each child As ClientEdit In Me
      If child.IsDirty Then
        Dim data As New SalesService.ClientDataUpdate
        data.Id = child.Id
        data.Name = child.Name
        data.IsNew = child.IsNew
        clientList.Add(data)
      End If
    Next

    ' update data on server
    Dim svc As New SalesService.SalesServiceClient
    svc.UpdateClients(clientList.ToArray)

    ' clear all data
    Me.Clear()
    DeletedList.Clear()

    ' reload list
    DataPortal_Fetch()

    ' raise reset event
    OnListChanged(New System.ComponentModel.ListChangedEventArgs(ComponentModel.ListChangedType.Reset, -1))

  End Sub

  Public Overrides Function Save() As ClientList

    Dim result As ClientList = MyBase.Save
    ClientNVList.FlushCache()
    ClientProjectNVList.FlushCache()
    Return result

  End Function

#End Region

End Class
