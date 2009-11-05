<Serializable()> _
Public Class ClientUpdater
  Inherits CommandBase

  Private _clientList As List(Of ClientEdit)

#Region " Factory Methods "

  Public Shared Sub UpdateClients(ByVal clientList As ClientEdit())

    Dim cmd As New ClientUpdater(New List(Of ClientEdit)(clientList))
    cmd = DataPortal.Execute(cmd)

  End Sub

  Private Sub New()

  End Sub

  Private Sub New(ByVal clientList As List(Of ClientEdit))

    _clientList = clientList

  End Sub

#End Region

#Region " Server-side Code "

  <Transactional(TransactionalTypes.TransactionScope)> _
  Protected Overrides Sub DataPortal_Execute()

    For Each item As ClientEdit In _clientList
      item.Save()
    Next
    ' clear list
    _clientList = Nothing

  End Sub

#End Region

End Class
