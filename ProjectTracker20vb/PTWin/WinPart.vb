Public Class WinPart
  Inherits System.Windows.Forms.UserControl

  Public Event CloseWinPart As EventHandler
  Public Event StatusChanged(ByVal sender As Object, ByVal e As StatusChangedEventArgs)

  Private mTitle As String

  Public Overridable Property Title() As String
    Get
      Return mTitle
    End Get
    Set(ByVal value As String)
      mTitle = value
    End Set
  End Property

  Protected Sub Close()

    RaiseEvent CloseWinPart(Me, EventArgs.Empty)

  End Sub

  Protected Overridable Sub OnStatusChanged()

    OnStatusChanged("", False)

  End Sub

  Protected Overridable Sub OnStatusChanged(ByVal statusText As String)

    RaiseEvent StatusChanged(Me, New StatusChangedEventArgs(statusText))

  End Sub

  Protected Overridable Sub OnStatusChanged(ByVal statusText As String, ByVal busy As Boolean)

    RaiseEvent StatusChanged(Me, New StatusChangedEventArgs(statusText, busy))

  End Sub

#Region " CurrentPrincipalChanged "

  Protected Event CurrentPrincipalChanged As EventHandler

  Friend Sub PrincipalChanged(ByVal sender As Object, ByVal e As EventArgs)

    OnCurrentPrincipalChanged(sender, e)

  End Sub

  Protected Overridable Sub OnCurrentPrincipalChanged(ByVal sender As Object, ByVal e As EventArgs)

    RaiseEvent CurrentPrincipalChanged(sender, e)

  End Sub

#End Region

End Class
